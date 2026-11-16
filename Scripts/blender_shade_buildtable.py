"""Blender 5.2: shade a RainbowToolkit buildtable export (materials only).

Import the mesh_*.cast files first, then run this in the Text Editor (Alt+P)
and pick the buildtable_* folder. Each material_<UID> already in the scene
is rebuilt from materials/material_<UID>/.
"""
import bpy, json, math, os
from bpy.props import StringProperty, BoolProperty

EYE_TEMPLATE = 366951634061  # CHR_EYE; eye atlas lives in u >= 0.5
HAIR_COLOR = (0.02, 0.015, 0.01, 1)  # lash/hair tint isn't exported (HairShaderParams floats unparsed)
CHANNELS = ('Red', 'Green', 'Blue')


def _sock(coll, key):
    if isinstance(key, int):
        return coll[key]
    return next((s for s in coll if s.identifier == key), None) or coll[key]


class G:
    """Node-tree builder; places nodes left to right by dependency depth."""

    def __init__(self, nt):
        self.nt, self.cols = nt, {}

    def node(self, kind, ins=None, **props):
        n = self.nt.nodes.new(kind)
        for k, v in props.items():
            setattr(n, k, v)
        x = 0
        for k, v in (ins or {}).items():
            if v is None:
                continue
            s = _sock(n.inputs, k)
            if isinstance(v, bpy.types.NodeSocket):
                self.nt.links.new(v, s)
                x = max(x, v.node.location.x + 240)
            else:
                if isinstance(v, (int, float)) and s.type in ('VECTOR', 'RGBA'):
                    v = (v,) * len(s.default_value)
                s.default_value = v
        n.location = (x, self.cols.get(x, 0))
        self.cols[x] = n.location.y - 200
        return n

    def math(self, op, a, b=None, c=None, clamp=False):
        return self.node('ShaderNodeMath', {0: a, 1: b, 2: c}, operation=op, use_clamp=clamp).outputs[0]

    def vec(self, op, a, b=None, c=None):  # SCALE: b is the scalar
        ins = {0: a, 3: b} if op == 'SCALE' else {0: a, 1: b, 2: c}
        n = self.node('ShaderNodeVectorMath', ins, operation=op)
        return n.outputs['Value' if op in ('DOT_PRODUCT', 'LENGTH') else 'Vector']

    def mix(self, t, a, b, kind='VECTOR'):  # lerp(a, b, t)
        k = {'VECTOR': 'Vector', 'RGBA': 'Color', 'FLOAT': 'Float'}[kind]
        n = self.node('ShaderNodeMix', {'Factor_Float': t, f'A_{k}': a, f'B_{k}': b}, data_type=kind)
        return _sock(n.outputs, f'Result_{k}')

    def comb(self, x, y, z):
        return self.node('ShaderNodeCombineXYZ', {0: x, 1: y, 2: z}).outputs[0]

    def sepxyz(self, v):
        return self.node('ShaderNodeSeparateXYZ', {0: v}).outputs

    def sepcol(self, c):
        return self.node('ShaderNodeSeparateColor', {0: c}).outputs

    def gloss_to_rough(self, gloss):  # EKSpecularCommon: saturate(0.9 - gloss / 1.15)
        return self.math('MULTIPLY_ADD', gloss, -1 / 1.15, 0.9, clamp=True)

    def nrm(self, x, y):  # z = sqrt(1 - saturate(x*x + y*y))
        xy = self.comb(x, y, 0)
        return self.comb(x, y, self.math('SQRT', self.math('SUBTRACT', 1, self.vec('DOT_PRODUCT', xy, xy), clamp=True)))


def new_group(name, ins, outs):
    ng = bpy.data.node_groups.new(name, 'ShaderNodeTree')
    for n, t, d in ins:
        ng.interface.new_socket(n, in_out='INPUT', socket_type='NodeSocket' + t).default_value = d
    for n, t in outs:
        ng.interface.new_socket(n, in_out='OUTPUT', socket_type='NodeSocket' + t)
    gi = ng.nodes.new('NodeGroupInput')
    gi.location = (-300, 0)
    return ng, G(ng), gi.outputs


FLAT, GREY = (0.5, 0.5, 1, 1), (0.5, 0.5, 0.5, 1)


def customization_group():
    """CHR_Customization_ShaderCodeModule_vert.hlsl."""
    name = 'RTK CHR_Customization 2'
    if name in bpy.data.node_groups:
        return bpy.data.node_groups[name]
    ins = [('Diffuse', 'Color', GREY), ('Diffuse Alpha', 'Float', 1.0),
           ('Color Mask', 'Color', (0, 0, 0, 1)), ('HasColorMask', 'Float', 0.0),
           ('Normal Map', 'Color', FLAT), ('Specular Map', 'Color', (0, 0.5, 1, 1)), ('Normal Strength', 'Float', 1.0)]
    for c in CHANNELS:  # names match the template's ShaderParams
        ins += [(f'Mask{c}_Color', 'Color', GREY), (f'Mask{c}_Detail', 'Color', FLAT),
                (f'Mask{c}_Camo', 'Color', GREY), (f'Mask{c}_Camo_Alpha', 'Float', 0.0),
                (f'Mask{c}_Camo_Gloss', 'Float', 0.0), (f'Mask{c}_Camo_Metal', 'Float', 0.0)]
    ng, g, i = new_group(name, ins, [('Base Color', 'Color'), ('Metallic', 'Float'), ('Roughness', 'Float'),
                                     ('Specular', 'Float'), ('Normal', 'Vector')])

    # Mask: round(alpha*2) -> 2 red, 1 green; or colormask / max(r+g+b, 1)
    a = i['Diffuse Alpha']
    red = g.math('GREATER_THAN', a, 0.75)
    alpha_mask = g.comb(red, g.math('SUBTRACT', g.math('GREATER_THAN', a, 0.25), red), 0)
    cm = i['Color Mask']
    cm_mask = g.vec('SCALE', cm, g.math('DIVIDE', 1, g.math('MAXIMUM', g.vec('DOT_PRODUCT', cm, 1), 1)))
    mask = g.mix(i['HasColorMask'], alpha_mask, cm_mask)
    m = dict(zip(CHANNELS, g.sepxyz(mask)))
    mask_factor = g.math('MINIMUM', g.vec('LENGTH', mask), 1)

    def masked_sum(fmt):  # sum(input_c * mask_c)
        s = g.vec('SCALE', i[fmt.format('Red')], m['Red'])
        for c in CHANNELS[1:]:
            s = g.vec('MULTIPLY_ADD', i[fmt.format(c)], m[c], s)
        return s

    def camo_add(value, kind):  # saturate(value + sum(camo.a * mask_c * Camo_<kind>_c))
        for c in CHANNELS:
            value = g.math('MULTIPLY_ADD', g.math('MULTIPLY', i[f'Mask{c}_Camo_Alpha'], m[c]),
                           i[f'Mask{c}_Camo_{kind}'], value, clamp=c == CHANNELS[-1])
        return value

    # Base: lerp(diffuse, overlay(camo, diffuse), maskFactor) * lerp(0.5, tint, maskFactor) * 2
    overlay = _sock(g.node('ShaderNodeMix', {'Factor_Float': 1, 'A_Color': masked_sum('Mask{}_Camo'),
                                             'B_Color': i['Diffuse']}, data_type='RGBA', blend_type='OVERLAY').outputs,
                    'Result_Color')
    tint = g.mix(mask_factor, 0.5, masked_sum('Mask{}_Color'))
    base = g.vec('SCALE', g.vec('MULTIPLY', g.mix(mask_factor, i['Diffuse'], overlay), tint), 2)

    # Specular: R metal, G gloss, B cavity
    metal, gloss, cavity = g.sepcol(i['Specular Map'])

    # Normal: BC5 base + masked detail, BlendDetailMap (RNM)
    nx, ny, _ = g.sepcol(i['Normal Map'])
    t = g.vec('ADD', g.nrm(g.math('MULTIPLY_ADD', nx, 2, -1), g.math('MULTIPLY_ADD', ny, 2, -1)), (0, 0, 1))
    d = (0.5, 0.5, 0)
    for c in CHANNELS:
        d = g.mix(m[c], d, i[f'Mask{c}_Detail'])
    dx, dy, _ = g.sepxyz(d)
    u = g.nrm(g.math('MULTIPLY_ADD', dx, -2, 1), g.math('MULTIPLY_ADD', dy, -2, 1))
    n = g.vec('NORMALIZE', g.vec('SUBTRACT', g.vec('SCALE', t, g.vec('DOT_PRODUCT', t, u)),
                                 g.vec('SCALE', u, g.sepxyz(t)[2])))
    n = g.node('ShaderNodeNormalMap', {'Color': g.vec('MULTIPLY_ADD', n, 0.5, 0.5),
                                       'Strength': i['Normal Strength']}).outputs[0]

    g.node('NodeGroupOutput', {'Base Color': base, 'Metallic': camo_add(metal, 'Metal'),
                               'Roughness': g.gloss_to_rough(camo_add(gloss, 'Gloss')),
                               'Specular': g.math('MULTIPLY', cavity, 0.5), 'Normal': n})
    return ng


def eye_group():
    """CHR_EYE_HERE.hlsl minus refraction (needs eye-bone direction) and matcap."""
    name = 'RTK CHR_Eye 2'
    if name in bpy.data.node_groups:
        return bpy.data.node_groups[name]
    # names match the template's ShaderParams; defaults are Tachanka's
    ng, g, i = new_group(name, [
        ('Diffuse', 'Color', GREY), ('Diffuse Alpha', 'Float', 1.0), ('Normal Map', 'Color', FLAT),
        ('IrisColorBlack', 'Color', (0.612, 0.463, 0.533, 1)), ('IrisColorWhite', 'Color', (0.322, 0.208, 0.078, 1)),
        ('ScleraColorBlack', 'Color', (0.275, 0.078, 0.078, 1)), ('ScleraColorWhite', 'Color', (0.471, 0.412, 0.392, 1)),
        ('IrisUVRadius', 'Float', 0.19), ('LimbusDarkScale', 'Float', 2.15), ('LimbusPow', 'Float', 9.2),
        ('IrisGlossiness', 'Float', 0.93), ('ScleraGlossiness', 'Float', 0.64),
    ], [('Base Color', 'Color'), ('Roughness', 'Float'), ('Specular', 'Float'), ('Normal', 'Vector')])

    uv = g.node('ShaderNodeTexCoord').outputs['UV']
    u, v, _ = g.sepxyz(uv)
    radius = i['IrisUVRadius']

    # irisMask = smoothstep(saturate((radius - dist) / LimbusUVWidthColor))
    dist = g.vec('LENGTH', g.comb(g.math('MULTIPLY_ADD', u, 2, -1.5), g.math('SUBTRACT', v, 0.5), 0))
    iris = g.node('ShaderNodeMapRange', {'Value': g.math('DIVIDE', g.math('SUBTRACT', radius, dist), 0.035)},
                  interpolation_type='SMOOTHSTEP').outputs[0]

    # limbus = 1 - saturate(length(irisUVScaled) * LimbusDarkScale) ^ LimbusPow
    c = g.vec('SCALE', g.vec('SUBTRACT', uv, (0.75, 0.5, 0)), g.math('DIVIDE', 0.5, radius))
    shrink = g.math('MULTIPLY', g.math('MULTIPLY_ADD', g.vec('LENGTH', c), -2, 1), 0.725, clamp=True)  # PupilScale
    edge = g.vec('LENGTH', g.vec('MULTIPLY', g.vec('NORMALIZE', c), (0.75, 0.5, 0)))
    edge = g.math('MULTIPLY', g.math('MULTIPLY', edge, g.math('SUBTRACT', 1, shrink)), i['LimbusDarkScale'], clamp=True)
    limbus = g.math('SUBTRACT', 1, g.math('POWER', edge, i['LimbusPow']))

    # Diffuse: R iris/sclera ramp, B cavity, A iris shading
    ramp, _, cavity = g.sepcol(i['Diffuse'])
    a = i['Diffuse Alpha']
    iris_col = g.vec('SCALE', g.mix(ramp, i['IrisColorBlack'], i['IrisColorWhite'], 'RGBA'), g.math('MULTIPLY', a, limbus))
    sclera = g.vec('SCALE', g.mix(ramp, i['ScleraColorBlack'], i['ScleraColorWhite'], 'RGBA'), g.mix(iris, 1, a, 'FLOAT'))
    n = g.node('ShaderNodeNormalMap', {'Color': g.mix(iris, FLAT, i['Normal Map'], 'RGBA')}).outputs[0]
    g.node('NodeGroupOutput', {
        'Base Color': g.mix(iris, sclera, iris_col), 'Normal': n,
        'Roughness': g.gloss_to_rough(g.mix(iris, i['ScleraGlossiness'], i['IrisGlossiness'], 'FLOAT')),
        'Specular': g.math('MULTIPLY', cavity, 0.5)})
    return ng


def camo_uv(g, uv, c, params):
    """Red rotates about 0.5 then scales; Green/Blue scale then rotate (as in the HLSL)."""
    scale = (params[f'Mask{c}_Camo_UV_Factor'], params[f'Mask{c}_Camo_UV_Factor_V'], 1)
    rot = lambda v: g.node('ShaderNodeVectorRotate', {'Vector': v, 'Center': (0.5, 0.5, 0),
                                                       'Angle': math.radians(params[f'Mask{c}_Camo_UV_Rot'])},
                           rotation_type='Z_AXIS').outputs[0]
    return g.vec('MULTIPLY', rot(uv), scale) if c == 'Red' else rot(g.vec('MULTIPLY', uv, scale))


def tex(g, path, uv, label, color=False):
    img = bpy.data.images.load(path, check_existing=True)
    img.colorspace_settings.name = 'sRGB' if color else 'Non-Color'
    img.alpha_mode = 'CHANNEL_PACKED'  # alpha is data (mask index / opacity), keep RGB intact
    return g.node('ShaderNodeTexImage', {'Vector': uv}, image=img, label=label)


def shade(mat, folder):
    with open(os.path.join(folder, 'material_config.json')) as f:
        cfg = json.load(f)
    has = {os.path.splitext(n)[0] for n in os.listdir(folder)}
    p = lambda n: os.path.join(folder, n + '.dds')

    nt = mat.node_tree
    nt.nodes.clear()
    g = G(nt)
    uv = g.node('ShaderNodeUVMap').outputs[0]
    diff = tex(g, p('diffuse'), uv, 'Diffuse', color=True)
    ins = {'Diffuse': diff.outputs['Color'], 'Diffuse Alpha': diff.outputs['Alpha']}
    if 'normal' in has:
        ins['Normal Map'] = tex(g, p('normal'), uv, 'Normal').outputs['Color']
    params = cfg.get('ShaderParams', {})

    def group(ng):  # ShaderParams fill same-named inputs; numbers on colour inputs are texture uids
        kinds = {s.name: s.socket_type for s in ng.interface.items_tree if getattr(s, 'in_out', '') == 'INPUT'}
        for k, v in params.items():
            if kinds.get(k) == 'NodeSocketColor' and isinstance(v, dict):
                ins.setdefault(k, (v['X'], v['Y'], v['Z'], 1))
            elif kinds.get(k) == 'NodeSocketFloat' and not isinstance(v, dict):
                ins.setdefault(k, float(v))
        return g.node('ShaderNodeGroup', ins, node_tree=ng)

    wire = lambda grp, *keys: {k: grp.outputs[k] for k in keys} | {'Specular IOR Level': grp.outputs['Specular']}

    if cfg['ShaderTemplateUid'] == EYE_TEMPLATE:
        bsdf = wire(group(eye_group()), 'Base Color', 'Roughness', 'Normal')
    elif 'HairShaderParams' in cfg:  # lashes/brows: opacity in diffuse alpha
        bsdf = {'Base Color': HAIR_COLOR, 'Roughness': 0.5, 'Alpha': ins['Diffuse Alpha'],
                'Normal': g.node('ShaderNodeNormalMap', {'Color': ins.get('Normal Map')}).outputs[0]}
    else:
        skin = 'CharacterSkinShaderParams' in cfg
        if 'specular' in has:
            ins['Specular Map'] = tex(g, p('specular'), uv, 'Specular').outputs['Color']
        if skin:  # full red mask: DetailMap everywhere, neutral tint
            ins['Color Mask'], ins['HasColorMask'] = (1, 0, 0, 1), 1.0
        elif 'colormask' in has:
            ins['Color Mask'] = tex(g, p('colormask'), uv, 'ColorMask').outputs['Color']
            ins.setdefault('HasColorMask', float(params.get('HasColorMask', 1)))
        for c in CHANNELS:  # template textures (CHR_Customization ShaderParams)
            det, camo = f'Mask{c}_Detail', f'Mask{c}_Camo_Texture'
            if det in has:
                ins[det] = tex(g, p(det), g.vec('MULTIPLY', uv, params[f'{det}_UV_Factor']), det).outputs['Color']
            if camo in has:
                # camo is linear: the engine's blank camo (0x119CC47A7) is flat 128 grey = neutral overlay
                t = tex(g, p(camo), camo_uv(g, uv, c, params), camo)
                ins[f'Mask{c}_Camo'], ins[f'Mask{c}_Camo_Alpha'] = t.outputs['Color'], t.outputs['Alpha']
        if not any(f'Mask{c}_Detail' in ins for c in CHANNELS):  # skin / no ShaderParams: material DetailMaps
            for file, key, c in (('detail_1', 'DetailMap', 'Red'), ('detail_2', 'DetailMap2', 'Green')):
                if file in has:
                    s = cfg[key]['Scale']
                    ins[f'Mask{c}_Detail'] = tex(g, p(file), g.vec('MULTIPLY', uv, (s['X'], s['Y'], 1)), file).outputs['Color']
        bsdf = wire(group(customization_group()), 'Base Color', 'Metallic', 'Roughness', 'Normal')
        if skin:
            bsdf.update({'Subsurface Weight': 1.0, 'Subsurface Radius': (1, 0.32, 0.19), 'Subsurface Scale': 0.01})

    out = g.node('ShaderNodeBsdfPrincipled', bsdf)
    g.node('ShaderNodeOutputMaterial', {'Surface': out.outputs[0]})


class RTK_OT_shade_buildtable(bpy.types.Operator):
    """Rebuild scene materials from a RainbowToolkit buildtable folder"""
    bl_idname = 'rtk.shade_buildtable'
    bl_label = 'Select Buildtable Folder'

    directory: StringProperty(subtype='DIR_PATH')
    filter_folder: BoolProperty(default=True, options={'HIDDEN'})

    def invoke(self, context, event):
        context.window_manager.fileselect_add(self)
        return {'RUNNING_MODAL'}

    def execute(self, context):
        root = os.path.join(self.directory, 'materials')
        if not os.path.isdir(root):
            self.report({'ERROR'}, f'No materials folder in {self.directory}')
            return {'CANCELLED'}
        done = 0
        for name in sorted(os.listdir(root)):
            mats = [m for m in bpy.data.materials if m.name == name or m.name.startswith(name + '.')]
            for m in mats:
                shade(m, os.path.join(root, name))
            done += len(mats)
            if not mats:
                print(f'[RTK] {name}: not in scene, skipped')
        self.report({'INFO'}, f'Shaded {done} material(s)')
        return {'FINISHED'}


if hasattr(bpy.types, RTK_OT_shade_buildtable.__name__):
    bpy.utils.unregister_class(getattr(bpy.types, RTK_OT_shade_buildtable.__name__))
bpy.utils.register_class(RTK_OT_shade_buildtable)
if not bpy.app.background:
    bpy.ops.rtk.shade_buildtable('INVOKE_DEFAULT')
