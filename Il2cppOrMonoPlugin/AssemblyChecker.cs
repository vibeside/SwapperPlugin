using MelonLoader;
using Mono.Cecil;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SwapperPlugin
{
    enum Backend
    {
        Unknown,
        Il2cpp,
        Mono
    }
    internal class AssemblyChecker
    {
        public static string[] monoRefs = ["mscorlib","netstandard","scheduleone","system"];
        public static string[] il2cppRefs = ["il2cppmscorlib","il2cppscheduleone","il2cppsystem"];
        public static Backend CheckAtPath(string path)
        {
            string workingDir = Path.GetDirectoryName(path);
            if (!File.Exists(path)) return Backend.Unknown;
            if (!path.Contains(".dll")) return Backend.Unknown;
            AssemblyDefinition def = AssemblyDefinition.ReadAssembly(path);
            if (Path.GetFileName(path).ToLower().Contains("mono")) return Backend.Mono;
            if(Path.GetFileName(path).ToLower().Contains("il2cpp")) return Backend.Il2cpp;
            foreach (ModuleDefinition m in def.Modules)
            {
                foreach (TypeReference s in m.GetTypeReferences())
                {
                    if (!string.IsNullOrEmpty(s.Namespace))
                    {
                        if (il2cppRefs.Contains(s.Namespace.ToLower())) return Backend.Il2cpp;
                        if (monoRefs.Contains(s.Namespace.ToLower()) && !s.Namespace.ToLower().Contains("il2cpp"))
                            return Backend.Mono;
                    
                    }
                }
            }
            return Backend.Unknown;
        }
    }
}
