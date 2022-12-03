using System;
using System.IO;
using Microsoft.Extensions.Logging;
using SoG;
using System.Linq;
using HarmonyLib;

namespace Grindless
{
    internal static class Program
    {
        public static Harmony HarmonyInstance { get; } = new Harmony("Grindless");

        public static DateTime LaunchTime { get; private set; }

        public static ILoggerFactory LogFactory { get; } = LoggerFactory.Create(config =>
        {
            config.AddSimpleConsole(consoleConfig =>
            {
                consoleConfig.SingleLine = true;
            });
        });

        public static ILogger Logger { get; } = LogFactory.CreateLogger("Grindless");

        public static void Main(string[] args)
        {
            LaunchTime = DateTime.Now;

            try
            {
                SetupGrindless();
                InvokeSoGMain(args);
            }
            catch (Exception e)
            {
                Logger.LogCritical("Grindless crashed!");
                Logger.LogCritical("{Exception}", e);

                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
            }
        }

        private static void SetupGrindless()
        {
            // Should force Secrets Of Grindea.exe assembly to be loaded
            _ = typeof(Game1);

            Directory.CreateDirectory("Mods");
            Directory.CreateDirectory("Content/ModContent");

            Logger.LogInformation("Applying Patches...");

            HarmonyInstance.PatchAll(typeof(ModManager).Assembly);

            Logger.LogInformation("Patched {Count} methods!", HarmonyInstance.GetPatchedMethods().Count());
        }

        private static void InvokeSoGMain(string[] args)
        {
            typeof(Game1).Assembly
                .GetType("SoG.Program")
                .GetMethod("Main", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .Invoke(null, new object[] { args });
        }
    }
}
