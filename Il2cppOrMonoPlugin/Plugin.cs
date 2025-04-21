using MelonLoader;
using MelonLoader.Utils;
using System;
using System.IO;
using System.Linq;
using System.Net;
[assembly: MelonInfo(typeof(SwapperPlugin.Plugin), "SwapperPlugin", "1.0.0", "coolpaca")]
[assembly:MelonColor(255,149,82,255)]
namespace SwapperPlugin
{
    public class Plugin : MelonPlugin
    {
        static Backend backend = Backend.Unknown;
        public override void OnPreInitialization()
        {
            base.OnPreInitialization();
            string mods = Path.GetDirectoryName(MelonEnvironment.ModsDirectory) + @"\Mods";
            //MelonLogger.Msg(mods);
            backend = MelonUtils.IsGameIl2Cpp() ? Backend.Il2cpp : Backend.Mono;
            if (Directory.Exists(mods))
            {
                string[] files = Directory.GetFiles(mods, "*", SearchOption.AllDirectories);
                Backend fBackend;
                for(int i = 0; i < files.Length; i++) {
                    if (files[i].Contains(".dll"))
                    {
                        File.Move(files[i], files[i].Replace(".di", ""));
                        files[i] = files[i].Replace(".di", "");
                    }
                }
                foreach (string s in files)
                {
                    if (!s.Contains(".dll")) continue;
                    if (s.Contains(".old")) continue;
                    if (Path.GetFileName(s).Contains("S1API")) continue;
                    fBackend = AssemblyChecker.CheckAtPath(s);
                    //MelonLogger.Msg(fBackend.ToString());
                    if (fBackend == Backend.Unknown) MelonLogger.Error(
                        "Found a DLL, but couldn't determine backend!" +
                        "\nTell coolpaca in the S1 modding discord and" +
                        $"\nprovide the mod name! File: {Path.GetFileName(s)}");
                    if (fBackend == backend) continue;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    File.Move(s, s + ".di");
                }
            }
        }
    }
}
