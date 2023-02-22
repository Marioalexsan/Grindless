using System;
using System.IO;
using Microsoft.Extensions.Logging;
using SoG;
using System.Linq;
using HarmonyLib;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading;

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

        private static IConfigurationBuilder _configBuilder;
        private static string GetConfigPath() => Path.Combine(Globals.AppDataPath, "GrindlessConfig.json");

        public static IConfiguration ReadConfig()
        {
            if (!File.Exists(GetConfigPath()))
            {
                const string BaseConfig = """
                {
                    "IgnoredMods": [
                        
                    ]
                }
                """;

                File.WriteAllText(GetConfigPath(), BaseConfig);
                Thread.Sleep(10);
            }

            try
            {
                return _configBuilder.Build();
            }
            catch
            {
                Logger.LogError("Failed to read configuration file! Please check if GrindlessConfig.json is valid.");
                return null;
            }
        }

        public static ILogger Logger { get; } = LogFactory.CreateLogger("Grindless");

        public static void Main(string[] args)
        {
            LaunchTime = DateTime.Now;

            try
            {
                HarmonyMetaPatch();
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
            _configBuilder = new ConfigurationBuilder().AddJsonFile(GetConfigPath());

            Logger.LogInformation("Applying Patches...");

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            HarmonyInstance.PatchAll(typeof(ModManager).Assembly);
            stopwatch.Stop();

            Logger.LogInformation("Patched {Count} methods in {Time:F2} seconds!", HarmonyInstance.GetPatchedMethods().Count(), stopwatch.Elapsed.TotalSeconds);
        }

        private static void InvokeSoGMain(string[] args)
        {
            typeof(Game1).Assembly
                .GetType("SoG.Program")
                .GetMethod("Main", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .Invoke(null, new object[] { args });
        }

        private static void HarmonyMetaPatch()
        {
            HarmonyInstance.Patch(
                AccessTools.Method("HarmonyLib.PatchFunctions:UpdateWrapper"),
                prefix: new HarmonyMethod(typeof(Program), nameof(PrefixStopwatchStart)),
                postfix: new HarmonyMethod(typeof(Program), nameof(PostfixStopwatchStop))
            );
        }

        private static void PrefixStopwatchStart(out Stopwatch __state)
        {
            __state = new Stopwatch();
            __state.Start();
        }

        private static void PostfixStopwatchStop(Stopwatch __state, MethodBase original)
        {
            if (__state != null)
            {
                __state.Stop();

                if (__state.Elapsed > TimeSpan.FromSeconds(0.25))
                    Logger.LogWarning($"Patch is taking a long time! ({__state.Elapsed.TotalSeconds:F2}s) ({original.Name})");
            }
        }
    }
}
