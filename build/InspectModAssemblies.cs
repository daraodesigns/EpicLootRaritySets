using System;
using System.IO;
using System.Linq;
using System.Reflection;

internal static class InspectModAssemblies
{
    private static readonly string BepInExPath = @"C:\Users\Fran\AppData\Roaming\Thunderstore Mod Manager\DataFolder\Valheim\profiles\Valheim\BepInEx";
    private static readonly string ManagedPath = @"D:\SteamLibrary\steamapps\common\Valheim\valheim_Data\Managed";

    private static void Main()
    {
        AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

        Assembly norse = Assembly.LoadFrom(Path.Combine(BepInExPath, @"plugins\Alpus-NorseDemigods\NorseDemigods.dll"));
        Assembly betterArchery = Assembly.LoadFrom(Path.Combine(BepInExPath, @"plugins\ishid4-BetterArchery\BetterArchery.dll"));
        Assembly epicLoot = Assembly.LoadFrom(Path.Combine(BepInExPath, @"plugins\RandyKnapp-EpicLoot\EpicLoot.dll"));

        DumpTypes(norse, "Norse Tornado/Sneaky/Visual", "Tornado", "Sneaky", "Ghost", "Spirit");
        DumpCacheFields(norse, "Tornado", "Water", "Wave", "Storm", "Wind", "Mist", "Ghost", "Spirit", "Sneak", "Wolf", "Njord");
        DumpTypeMembers(norse, "NorseDemigods.Abilities.AbilityTornado");
        DumpTypeMembers(norse, "NorseDemigods.Attributes.AttributeSneaky");
        DumpTypeMembers(norse, "NorseDemigods.DemigodAbility");
        DumpTypeMembers(norse, "NorseDemigods.DemigodAbility+AbilityParametersStruct");
        DumpTypeMembers(norse, "NorseDemigods.Demigod");
        DumpTypes(betterArchery, "BetterArchery Quiver", "Quiver", "Slot", "Prefab", "Leather");
        DumpTypes(epicLoot, "EpicLoot Commands/Adventure", "Adventure", "Console", "Command", "CreateItem");
        DumpTypeMembers(epicLoot, "EpicLoot.Adventure.Feature.AdventureFeature");
    }

    private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
    {
        string dll = new AssemblyName(args.Name).Name + ".dll";
        foreach (string path in new[]
        {
            Path.Combine(ManagedPath, dll),
            Path.Combine(BepInExPath, "core", dll),
            Path.Combine(BepInExPath, @"plugins\RandyKnapp-EpicLoot", dll),
            Path.Combine(BepInExPath, @"plugins\Alpus-NorseDemigods", dll),
            Path.Combine(BepInExPath, @"plugins\ishid4-BetterArchery", dll),
            Path.Combine(BepInExPath, @"plugins\ValheimModding-Jotunn", dll),
            Path.Combine(BepInExPath, @"plugins\ValheimModding-JsonDotNET", dll)
        })
        {
            if (File.Exists(path))
            {
                return Assembly.LoadFrom(path);
            }
        }

        return null;
    }

    private static void DumpTypes(Assembly assembly, string title, params string[] needles)
    {
        Console.WriteLine();
        Console.WriteLine("== " + title + " ==");
        foreach (Type type in SafeTypes(assembly)
                     .Where(type => needles.Any(needle => type.FullName.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0))
                     .OrderBy(type => type.FullName))
        {
            Console.WriteLine(type.FullName);
        }
    }

    private static void DumpCacheFields(Assembly assembly, params string[] needles)
    {
        Console.WriteLine();
        Console.WriteLine("== Norse Cache fields ==");
        Type cache = assembly.GetType("NorseDemigods.Cache");
        if (cache == null)
        {
            Console.WriteLine("missing cache");
            return;
        }

        foreach (FieldInfo field in cache.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                     .Where(field =>
                         needles.Any(needle => field.Name.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0) ||
                         field.FieldType.FullName == "UnityEngine.GameObject" ||
                         field.FieldType.FullName == "Attack" ||
                         field.FieldType.FullName.IndexOf("GameObject", StringComparison.OrdinalIgnoreCase) >= 0)
                     .OrderBy(field => field.Name))
        {
            Console.WriteLine(field.FieldType.FullName + " " + field.Name + " = " + SafeValueName(field));
        }
    }

    private static void DumpTypeMembers(Assembly assembly, string fullName)
    {
        Console.WriteLine();
        Console.WriteLine("== " + fullName + " ==");
        Type type = assembly.GetType(fullName);
        if (type == null)
        {
            Console.WriteLine("missing");
            return;
        }

        Console.WriteLine("Base: " + (type.BaseType == null ? "" : type.BaseType.FullName));
        Console.WriteLine("-- ctors");
        foreach (ConstructorInfo ctor in type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            Console.WriteLine(SafeSignature(ctor));
        }

        Console.WriteLine("-- fields");
        foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                     .OrderBy(field => field.Name))
        {
            Console.WriteLine(SafeFieldType(field) + " " + field.Name);
        }

        Console.WriteLine("-- methods");
        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                     .Where(method => !method.IsSpecialName)
                     .OrderBy(method => method.Name))
        {
            Console.WriteLine(SafeSignature(method));
        }
    }

    private static Type[] SafeTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(type => type != null).ToArray();
        }
    }

    private static string Signature(MethodBase method)
    {
        MethodInfo info = method as MethodInfo;
        string returnType = info != null ? info.ReturnType.Name + " " : "";
        return returnType + method.Name + "(" +
               string.Join(", ", method.GetParameters().Select(p => p.ParameterType.FullName + " " + p.Name).ToArray()) +
               ")";
    }

    private static string SafeSignature(MethodBase method)
    {
        try
        {
            return Signature(method);
        }
        catch (Exception ex)
        {
            return method.Name + "(<signature failed: " + ex.GetType().Name + ">)";
        }
    }

    private static string SafeValueName(FieldInfo field)
    {
        try
        {
            object value = field.GetValue(null);
            if (value == null)
            {
                return "null";
            }

            PropertyInfo nameProperty = value.GetType().GetProperty("name");
            if (nameProperty != null)
            {
                object name = nameProperty.GetValue(value, null);
                return Convert.ToString(name);
            }

            return value.GetType().FullName;
        }
        catch (Exception ex)
        {
            return "<value failed: " + ex.GetType().Name + ">";
        }
    }

    private static string SafeFieldType(FieldInfo field)
    {
        try
        {
            return field.FieldType.FullName;
        }
        catch (Exception ex)
        {
            return "<field type failed: " + ex.GetType().Name + ">";
        }
    }
}
