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
        Mono,
        S1API
    }
    internal class AssemblyChecker
    {
        public static string[] monoRefs = ["scheduleone","system"];
        public static string[] il2cppRefs = ["il2cppscheduleone","il2cppsystem"];
        public static Backend CheckAtPath(string path)
        {
            string workingDir = Path.GetDirectoryName(path);
            if (!File.Exists(path)) return Backend.Unknown;
            if (!path.Contains(".dll")) return Backend.Unknown;
            if (Path.GetFileName(path).ToLower().Contains("mono")) return Backend.Mono;
            if(Path.GetFileName(path).ToLower().Contains("il2cpp")) return Backend.Il2cpp;
            AssemblyDefinition def = AssemblyDefinition.ReadAssembly(path);
            foreach (ModuleDefinition m in def.Modules)
            {
                var typeReferences = m.GetTypeReferences();
                if (typeReferences.Any(x => x.Namespace.ToLower().Contains("s1api"))) return Backend.S1API;
                if (typeReferences.Any(x => il2cppRefs.Contains(x.Namespace.ToLower()))) return Backend.Il2cpp;
                if (typeReferences.Any(x => monoRefs.Contains(x.Namespace.ToLower()))) return Backend.Mono;
            }
            return Backend.Unknown;
        }
    }
}
