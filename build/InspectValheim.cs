using System;
using System.IO;
using System.Linq;
using System.Reflection;

internal static class InspectValheim
{
    private static string ManagedPath;

    private static void Main()
    {
        ManagedPath = @"D:\SteamLibrary\steamapps\common\Valheim\valheim_Data\Managed";
        AppDomain.CurrentDomain.AssemblyResolve += ResolveManagedAssembly;
        Assembly asm = Assembly.LoadFrom(Path.Combine(ManagedPath, "assembly_valheim.dll"));

        Type attack = asm.GetType("Attack");
        Type projectile = asm.GetType("Projectile");
        Type seStats = asm.GetType("SE_Stats");
        Type player = asm.GetType("Player");
        Type character = asm.GetType("Character");

        Console.WriteLine("Attack.Update targets:");
        foreach (MethodInfo method in attack.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                     .Where(m => m.Name == "Update"))
        {
            Console.WriteLine(Signature(method));
        }

        Console.WriteLine("Attack.GetAttackStamina:");
        MethodInfo stamina = attack.GetMethod("GetAttackStamina", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        Console.WriteLine(stamina == null ? "missing" : Signature(stamina));

        Console.WriteLine("Projectile.Setup targets:");
        foreach (MethodInfo method in projectile.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                     .Where(m => m.Name == "Setup"))
        {
            Console.WriteLine(Signature(method));
        }

        Console.WriteLine("SE_Stats fields:");
        foreach (string fieldName in new[] { "m_noiseModifier", "m_stealthModifier", "m_speedModifier" })
        {
            FieldInfo field = seStats.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            Console.WriteLine(fieldName + ": " + (field == null ? "missing" : field.FieldType.FullName));
        }

        Console.WriteLine("Sneak/Stamina members:");
        foreach (string methodName in new[] { "IsCrouching", "IsSneaking", "GetStamina", "GetMaxStamina", "AddStamina" })
        {
            MethodInfo method = player.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                                ?? character.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            Console.WriteLine(methodName + ": " + (method == null ? "missing" : Signature(method)));
        }
    }

    private static Assembly ResolveManagedAssembly(object sender, ResolveEventArgs args)
    {
        string name = new AssemblyName(args.Name).Name + ".dll";
        string path = Path.Combine(ManagedPath, name);
        return File.Exists(path) ? Assembly.LoadFrom(path) : null;
    }

    private static string Signature(MethodInfo method)
    {
        return method.ReturnType.Name + " " + method.Name + "(" +
               string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name).ToArray()) +
               ")";
    }
}
