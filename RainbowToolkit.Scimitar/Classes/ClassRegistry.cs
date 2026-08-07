using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace RainbowToolkit.Scimitar.Classes;

public static class ClassRegistry {
    private static readonly Dictionary<uint, Type> Classes = new();
    private static readonly Dictionary<uint, Func<BaseObject>> Factories = new();

    static ClassRegistry() {
        foreach (var type in typeof(BaseObject).Assembly.GetTypes()) {
            if (type.IsAbstract || !type.IsAssignableTo(typeof(BaseObject))) {
                continue;
            }
            var field = type.GetField("MAGIC", BindingFlags.Public | BindingFlags.Static);
            if (field?.FieldType == typeof(uint)) {
                Register((uint)field.GetValue(null)!, type);
            }
        }
    }

    public static IReadOnlyDictionary<uint, Type> RegisteredClasses => Classes;

    public static void Register<T>(uint magic) where T : BaseObject => Register(magic, typeof(T));

    public static void Register(uint magic, Type type) {
        if (!type.IsAssignableTo(typeof(BaseObject))) {
            throw new ArgumentException($"{type.Name} does not derive from {nameof(BaseObject)}.", nameof(type));
        }
        Classes[magic] = type;
        Factories[magic] = Expression.Lambda<Func<BaseObject>>(Expression.New(type)).Compile();
    }

    public static BaseObject Create(uint magic) => Factories.TryGetValue(magic, out var factory)
        ? factory()
        : throw new Exception($"No class registered for magic {magic:X8}");

    public static Type GetClass(uint magic) => TryGetClass(magic, out var type)
        ? type
        : throw new Exception($"No class registered for magic {magic:X8}");

    public static bool TryGetClass(uint magic, [NotNullWhen(true)] out Type? type) => Classes.TryGetValue(magic, out type);

    public static string? GetClassName(uint magic) => TryGetClass(magic, out var type) ? type.Name : null;
}
