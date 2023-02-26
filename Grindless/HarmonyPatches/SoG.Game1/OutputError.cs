using HarmonyLib;
using SoG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindless.HarmonyPatches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1.OutputError), typeof(string), typeof(string))]
    static class OutputError
    {
        static bool Prefix(string p_sLocation, string e)
        {
            if (CAS.IsDebugFlagSet_Release("silentsend"))
            {
                // Ignore silent sends for now
                return false;
            }

            if (e.Contains("OutOfMemoryException") && e.Contains("VertexBuffer"))
            {
                Globals.Game.xOptions.bLoneBats = true;
                Globals.Game.xOptions.SaveText();
            }

            e = e.Replace("C:\\Dropbox\\Eget jox\\!DugTrio\\Legend Of Grindia\\Legend Of Grindia\\Legend Of Grindia", "(path)");
            e = e.Replace("F:\\Stable Branch\\Legend Of Grindia\\Legend Of Grindia", "(path)");

            StringBuilder msg = new(2048);

            msg.AppendLine("An error happened while running a modded game instance!");
            msg.AppendLine("=== Exception message ===");
            msg.AppendLine(e);
            msg.AppendLine("=== Game Settings ===");
            msg.AppendLine("Game Version = " + Globals.Game.sVersionNumberOnly);
            msg.AppendLine("Fullscreen = " + Globals.Game.xOptions.enFullScreen);
            msg.AppendLine("Network role = " + Globals.Game.xNetworkInfo.enCurrentRole);
            msg.AppendLine("Extra Error Info => " + DebugKing.dssExtraErrorInfo.Count + " pairs");

            foreach (KeyValuePair<string, string> kvp in DebugKing.dssExtraErrorInfo)
            {
                msg.AppendLine("  " + kvp.Key + " = " + kvp.Value);
            }

            msg.AppendLine("=== GrindScript Info ===");
            msg.AppendLine("Active Mods => " + ModManager.Mods.Count + " mods");

            foreach (Mod mod in ModManager.Mods)
            {
                msg.AppendLine("  " + mod.ToString());
            }

            var time = DateTime.Now;

            string logName = $"CrashLog_{time.Year}.{time.Month}.{time.Day}_{time.Hour}.{time.Minute}.{time.Second}.txt";

            StreamWriter writer = null;
            try
            {
                Directory.CreateDirectory(Path.Combine(Globals.AppDataPath, "Logs"));
                writer = new StreamWriter(new FileStream(Path.Combine(Globals.AppDataPath, "Logs", logName), FileMode.Create, FileAccess.Write));
                writer.Write(msg.ToString());
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                Console.ReadLine();
            }
            finally
            {
                writer?.Close();
            }

            return false;
        }
    }
}
