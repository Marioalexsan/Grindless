using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;

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
                        
                    ],
                    "HarmonyDebug": false
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

        internal static bool HasCrashed { get; set; } = false;

        public static void Main(string[] args)
        {
            LaunchTime = DateTime.Now;

            try
            {
                CheckFirstTimeBoot();
                HarmonyMetaPatch();
                SetupGrindless();
                InvokeSoGMain(args);
            }
            catch (Exception e)
            {
                ErrorHelper.ForceExit(e, skipLogging: true);
            }

            if (HasCrashed)
            {
                Thread.Sleep(1000);

                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
            }
        }

        private static void CheckFirstTimeBoot()
        {
            static void SetColors(ConsoleColor fore, ConsoleColor back)
            {
                Console.ForegroundColor = fore;
                Console.BackgroundColor = back;
            }

            if (!Directory.Exists(Globals.AppDataPath))
            {
                var lastFore = Console.ForegroundColor;
                var lastBack = Console.BackgroundColor;

                SetColors(ConsoleColor.Yellow, ConsoleColor.DarkBlue);
                Console.WriteLine("""

                ┌────────────────────────────────────────────────────────────────────────┐
                │  ________      ___           _______       ________      _________     │
                │ |\   __  \    |\  \         |\  ___ \     |\   __  \    |\___   ___\   │
                │ \ \  \|\  \   \ \  \        \ \   __/|    \ \  \|\  \   \|___ \  \_|   │
                │  \ \   __  \   \ \  \        \ \  \_|/__   \ \   _  _\       \ \  \    │
                │   \ \  \ \  \   \ \  \____    \ \  \_|\ \   \ \  \\  \|       \ \  \   │
                │    \ \__\ \__\   \ \_______\   \ \_______\   \ \__\\ _\        \ \__\  │
                │     \|__|\|__|    \|_______|    \|_______|    \|__|\|__|        \|__|  │
                │                                                                        │
                └────────────────────────────────────────────────────────────────────────┘

                """);

                SetColors(ConsoleColor.DarkGreen, ConsoleColor.White);
                Console.WriteLine("""

                ┌──────────────────────────────────────────────────────────────┐
                │                                                              │
                │ Seems like this is the first time you're using Grindless!    │
                │ The mod tool uses a separate save location from vanilla SoG. │
                │ Would you like to copy over your saves from the base game?   │
                │                                                              │
                │ SoG savepath:       %appdata%\Secrets of Grindea\            │
                │ Grindless savepath: %appdata%\Grindless\                     │
                │                                                              │
                │                                                              │
                │             [Y] Hell yeah!    [N] Nah, don't.                │
                └──────────────────────────────────────────────────────────────┘

                """);

                ConsoleKeyInfo c;
                do
                {
                    c = Console.ReadKey(true);
                }
                while (c.KeyChar != 'Y' && c.KeyChar != 'y' && c.KeyChar != 'N' && c.KeyChar != 'n');

                Directory.CreateDirectory(Globals.AppDataPath);
                Directory.CreateDirectory(Path.Combine(Globals.AppDataPath, "Characters"));
                Directory.CreateDirectory(Path.Combine(Globals.AppDataPath, "Worlds"));

                if (c.KeyChar == 'Y' || c.KeyChar == 'y')
                {
                    var vanilla = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Secrets of Grindea");

                    for (int i = 0; i < 9; i++)
                    {
                        if (File.Exists(Path.Combine(vanilla, "Characters", i + ".cha")))
                            File.Copy(Path.Combine(vanilla, "Characters", i + ".cha"), Path.Combine(Globals.AppDataPath, "Characters", i + ".cha"), true);

                        if (File.Exists(Path.Combine(vanilla, "Worlds", i + ".wld")))
                            File.Copy(Path.Combine(vanilla, "Worlds", i + ".wld"), Path.Combine(Globals.AppDataPath, "Worlds", i + ".wld"), true);
                    }

                    if (File.Exists(Path.Combine(vanilla, "arcademode.sav")))
                        File.Copy(Path.Combine(vanilla, "arcademode.sav"), Path.Combine(Globals.AppDataPath, "arcademode.sav"), true);

                    SetColors(ConsoleColor.DarkGreen, ConsoleColor.White);
                    Console.WriteLine("""

                    ┌───────────────┐
                    │               │
                    │ Saves copied! │
                    │               │
                    └───────────────┘

                    """);
                }

                SetColors(lastFore, lastBack);
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

            var lastDebugMode = Harmony.DEBUG;
            Harmony.DEBUG = ReadConfig().GetValue("HarmonyDebug", false);

            if (Harmony.DEBUG)
                Logger.LogInformation("Using Harmony Debug mode.");

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            HarmonyInstance.PatchAll(typeof(ModManager).Assembly);
            stopwatch.Stop();

            Harmony.DEBUG = lastDebugMode;

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
