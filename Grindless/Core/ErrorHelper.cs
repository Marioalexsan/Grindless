using Microsoft.Extensions.Logging;
using System.Text;

namespace Grindless
{
    /// <summary>
    /// Helper class for error handling.
    /// </summary>
    internal static class ErrorHelper
    {
        public static string UseThisDuringLoad => $"This method should be called only during {nameof(Mod.Load)}.";

        public static string UseThisAfterLoad => $"This method cannot be called during {nameof(Mod.Load)}. Try calling it in {nameof(Mod.PostLoad)} instead.";

        public static string DuplicateModID => $"A game object with the given ModID already exists. ModIDs must be distinct between game objects of the same type.";

        public static string AudioEntryAlreadyCreated => "An audio entry for this mod has already been created. You can only call this method once.";

        public static string NoWhiteSpaceInCommand => "Commands must not have any whitespace.";

        public static string ObjectCreationDisabled => "Creating game objects is now allowed for mods with disabled object creation.";

        public static string OutOfIdentifiers => "Ran out of identifiers for the given object type. Smells of big modding energy!";

        public static string BadIDType => "The IP type requested does not support allocating.";

        public static string InternalError => "Something bad happened inside the modding tool. Report this to the developer, please!";

        public static string UnknownEntry => "Failed to retrieve a valid mod entry in a method that doesn't use default entries.";

        public static void ThrowIfNotLoading(Mod mod)
        {
            if (!mod.InLoad)
            {
                throw new InvalidOperationException(UseThisDuringLoad);
            }
        }

        public static void ThrowIfLoading(Mod mod)
        {
            if (mod.InLoad)
            {
                throw new InvalidOperationException(UseThisAfterLoad);
            }
        }

        public static void AssertLoading(Mod mod, Action statement)
        {
            if (!mod.InLoad)
            {
                throw new InvalidOperationException(UseThisDuringLoad);
            }
            else
            {
                statement();
            }
        }

        public static void Assert(bool trueCondition, string exceptionMessage = null)
        {
            if (!trueCondition)
            {
                throw new InvalidOperationException(exceptionMessage ?? InternalError);
            }
        }

        internal static void ForceExit(Exception exception, bool skipLogging = false)
        {
            Program.Logger.LogCritical("Grindless crashed!");
            Program.Logger.LogCritical("{Exception}", exception);

            if (!skipLogging)
                LogException(exception);

            Globals.Game?.Exit();
            Program.HasCrashed = true;
        }

        internal static void LogException(Exception exception)
        {
            string e = exception.Message;

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
            msg.AppendLine("=== Stack Trace ===");
            msg.AppendLine(exception.StackTrace);
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
                Program.Logger.LogCritical("Exception information written to %appdata%\\Grindless\\Logs!");
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
        }
    }
}
