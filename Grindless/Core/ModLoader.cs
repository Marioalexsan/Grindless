using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Configuration.Assemblies;
using System.IO.Compression;
using static SoG.HitEffectMap;
using SoG;

namespace Grindless;

internal static class ModLoader
{
    public static List<Mod> ObtainMods()
    {
        Program.Logger.LogInformation("Loading mods...");

        var paths = GetLoadableMods();

        var mods = LoadModsFromAssemblies(paths);

        var modOrder = BuildLoadOrder(mods);

        return modOrder;
    }

    private static List<Mod> BuildLoadOrder(IEnumerable<Mod> mods)
    {
        static bool CheckDependency(Mod mod, ModDependencyAttribute dep)
        {
            if (dep.NameID != mod.Name)
            {
                return false;
            }

            if (string.IsNullOrEmpty(dep.ModVersion))
            {
                return true;
            }

            bool validVersion = Version.TryParse(dep.ModVersion, out Version parsed);
            bool higherVersion = validVersion && mod.Version > parsed;
            bool equalVersion = validVersion && mod.Version == parsed;

            bool dependencyOK =
                validVersion &&
                (equalVersion || dep.AllowHigherVersions && higherVersion);

            return dependencyOK;
        }

        var dependencyGraph = new Dictionary<Mod, List<Mod>>();
        var dependencies = new Dictionary<Mod, List<ModDependencyAttribute>>();

        foreach (var mod in mods)
        {
            dependencies[mod] = mod.GetType().GetCustomAttributes<ModDependencyAttribute>().ToList();
            dependencyGraph[mod] = new List<Mod>();
        }

        List<Mod> loadOrder = new();
        List<Mod> readyMods = null;

        while ((readyMods = dependencies.Where(x => x.Value.Count == 0).Select(x => x.Key).ToList()).Count() > 0)
        {
            // Sort nodes that can't be "compared" in the graph.
            // This, along with the dependency sort, ensures a deterministic order

            readyMods.Sort((x, y) => string.Compare(x.Name, y.Name));

            foreach (var mod in readyMods)
            {
                dependencies.Remove(mod);
                loadOrder.Add(mod);

                foreach (var pair in dependencies)
                {
                    var depList = pair.Value;

                    for (int i = 0; i < depList.Count; i++)
                    {
                        if (CheckDependency(mod, depList[i]))
                        {
                            dependencyGraph[mod].Add(pair.Key);
                            depList.RemoveAt(i);
                            i -= 1;
                        }
                    }
                }
            }
        }

        // Any leftover mods in dependency graph will fail to load due to missing dependencies

        foreach (var pair in dependencies)
        {
            Program.Logger.LogError("Mod {pair.Key} cannot be loaded. It is missing the following mod dependencies:", pair.Key);

            foreach (var dep in pair.Value)
            {
                string id = $"    {dep.NameID}";
                string version = string.IsNullOrEmpty(dep.ModVersion) ? "" : ($", Version {dep.ModVersion}" + (dep.AllowHigherVersions ? " or higher" : ""));

                Program.Logger.LogError("{id}{version}", id, version);
            }
        }

        return loadOrder;
    }

    private static List<string> GetLoadableMods()
    {
        // TODO: Rewrite ignored mods source
        var ignoredMods = Program.ReadConfig()?.GetSection("IgnoredMods")?.Get<List<string>>() ?? Enumerable.Empty<string>();

        var modFolder = Path.Combine(Directory.GetCurrentDirectory(), "Mods");

        List<string> fullPathIgnored = ignoredMods.Select(x => Path.Combine(modFolder, x)).ToList();

        var candidates = Directory.GetFiles(modFolder)
            .Where(x => x.EndsWith(".dll") || x.EndsWith(".zip"))
            .Concat(Directory.GetDirectories(modFolder).Where(x => x.EndsWith("_dev")))
            .ToList();

        var selected = candidates
            .Where(x => !fullPathIgnored.Contains(x))
            .ToList();

        int totalCount = candidates.Count;
        int selectedCount = selected.Count;
        int ignoreCount = totalCount - selectedCount;

        Program.Logger.LogInformation("Found {ignoreCount} mods that are present in the ignore list.", ignoreCount);
        Program.Logger.LogInformation("Selecting {selectedCount} other mods for loading.", selectedCount);

        return selected;
    }

    private static List<Mod> LoadModsFromAssemblies(IEnumerable<string> paths)
    {
        var mods = new List<Mod>()
        {
            new VanillaMod(),
            new GrindlessMod()
        };

        mods.AddRange(paths.Select(path =>
        {
            if (path.EndsWith(".dll"))
                return LoadCSharpMod(path);

            if (path.EndsWith(".zip"))
                return LoadJavaScriptArchive(path);

            if (Directory.Exists(path))
                return LoadJavaScriptFolder(path);

            return null;
        }).Where(mod => mod != null));

        return mods;
    }

    /// <summary>
    /// Finds and replaces commonly occuring game paths with shortened forms.
    /// </summary>
    private static string ShortenModPaths(string path)
    {
        return path
            .Replace('/', '\\')
            .Replace(Directory.GetCurrentDirectory() + @"\Content\ModContent", "(ModContent)")
            .Replace(Directory.GetCurrentDirectory() + @"\Content\Mods", "(Mods)")
            .Replace(Directory.GetCurrentDirectory() + @"\Content", "(Content)")
            .Replace(Directory.GetCurrentDirectory(), "(SoG)");
    }

    private static Mod LoadJavaScriptFolder(string folderPath)
    {
        string shortPath = ShortenModPaths(folderPath);

        Program.Logger.LogInformation("Loading JavaScript mod from {path}.", shortPath);

        try
        {
            Queue<(string, string)> paths = new();

            paths.Enqueue((folderPath, ""));

            Dictionary<string, string> sources = new();

            var split = new[]
            {
                '/'
            };

            while (paths.Count > 0)
            {
                var (subFolderPath, root) = paths.Dequeue();

                foreach (var path in Directory.GetDirectories(subFolderPath))
                    paths.Enqueue((Path.Combine(subFolderPath, Path.GetDirectoryName(path)), root == "" ? Path.GetDirectoryName(path) : Path.Combine(root, Path.GetDirectoryName(path))));

                foreach (var file in Directory.GetFiles(subFolderPath))
                {
                    using StreamReader reader = new StreamReader(File.OpenRead(file));
                    sources[Path.Combine(root, Path.GetFileName(file))] = reader.ReadToEnd();
                }
            }

            Program.Logger.LogInformation("Loaded modules: ");

            foreach (var item in sources)
            {
                Program.Logger.LogInformation(item.Key);
            }

            if (!sources.Any(x => x.Key == "index.js"))
            {
                Program.Logger.LogError("Failed to load development mod. No index.js entry point found.", shortPath);
                return null;
            }

            var mod = new JavaScriptMod(sources);

            return mod;
        }
        catch (Exception e)
        {
            Program.Logger.LogError("Failed to load mod {modPath}. An unknown exception occurred: {e}", shortPath, ShortenModPaths(e.ToString()));
        }

        return null;
    }

    private static Mod LoadJavaScriptArchive(string zipPath)
    {
        string shortPath = ShortenModPaths(zipPath);

        Program.Logger.LogInformation("Loading JavaScript mod from {path}.", shortPath);

        try
        {
            using ZipArchive archive = ZipFile.OpenRead(zipPath);

            var split = new[]
            {
                '/'
            };

            var pathParts = archive.Entries.First().FullName.Split(split, 2);
            var isStacked = pathParts.Length == 2 && pathParts[0] == Path.GetFileNameWithoutExtension(zipPath);

            if (!archive.Entries.Any(x => (isStacked ? x.FullName.Split(split, 2)[1] : x.FullName) == "index.js"))
            {
                Program.Logger.LogError("Failed to load mod. No index.js entry point found.", shortPath);
                return null;
            }

            Dictionary<string, string> sources = new();

            foreach (var entry in archive.Entries.Where(x => x.Name.EndsWith(".js")))
            {
                using StreamReader reader = new StreamReader(entry.Open());
                sources[isStacked ? entry.FullName.Split(split, 2)[1] : entry.FullName] = reader.ReadToEnd();
            }

            var mod = new JavaScriptMod(sources);

            return mod;
        }
        catch (Exception e)
        {
            Program.Logger.LogError("Failed to load mod {modPath}. An unknown exception occurred: {e}", shortPath, ShortenModPaths(e.ToString()));
        }

        return null;
    }

    private static Mod LoadCSharpMod(string assemblyPath)
    {
        string shortPath = ShortenModPaths(assemblyPath);

        Program.Logger.LogInformation("Loading C# mod from {path}.", shortPath);

        try
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            Type type = assembly.DefinedTypes.First(t => t.BaseType == typeof(Mod));
            Mod mod = Activator.CreateInstance(type, true) as Mod;

            bool conflictingID = ModManager.Mods.Any(x => x.Name == mod.Name);

            if (conflictingID)
            {
                Program.Logger.LogError("Mod {shortPath} with NameID {mod.NameID} conflicts with a previously loaded mod.", shortPath, mod.Name);
                return null;
            }


            return mod;
        }
        catch (MissingMethodException)
        {
            Program.Logger.LogError("Failed to load mod {modPath}. No parameterless constructor found!", shortPath);
            Program.Logger.LogError("You need to define a parameterless constructor so that the mod tool can instantiate the mod.");
        }
        catch (BadImageFormatException) { /* Ignore non-managed DLLs */ }
        catch (Exception e)
        {
            Program.Logger.LogError("Failed to load mod {modPath}. An unknown exception occurred: {e}", shortPath, ShortenModPaths(e.ToString()));
        }

        return null;
    }
}
