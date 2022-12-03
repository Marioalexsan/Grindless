using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Watchers;
using SoG;

namespace Grindless
{
    using Version = System.Version;

    internal class GrindlessMod : Mod
    {
        internal override bool IsBuiltin => true;

        public const string ModName = "Grindless";

        public static GrindlessMod Instance => (GrindlessMod)ModManager.Mods.First(x => x.Name == ModName);

        public override string Name => ModName;

        public override Version Version => Globals.GrindlessVersion;

        public Texture2D ErrorTexture { get; private set; }

        public Texture2D ModMenuText { get; private set; }

        public Texture2D ModListText { get; private set; }

        public Texture2D ReloadModsText { get; private set; }

        public override void PostLevelLoad(Level.ZoneEnum level, Level.WorldRegion region, bool staticOnly)
        {
            if (_colliderRCActive)
            {
                RenderMaster render = Globals.Game.xRenderMaster;

                render.UnregisterRenderComponenent(_colliderRC);
                render.RegisterComponent(RenderMaster.SubRenderLayer.AboveSorted, _colliderRC);
            }
        }

        public override void Load()
        {
            _colliderRC = new ColliderRC();

            AssetUtils.TryLoadTexture(Path.Combine(AssetPath, "NullTexGS"), Globals.Game.Content, out Texture2D tex);
            ErrorTexture = tex;

            AssetUtils.TryLoadTexture(Path.Combine(AssetPath, "ModMenu"), Globals.Game.Content, out tex);
            ModMenuText = tex;

            AssetUtils.TryLoadTexture(Path.Combine(AssetPath, "ModList"), Globals.Game.Content, out tex);
            ModListText = tex;

            AssetUtils.TryLoadTexture(Path.Combine(AssetPath, "ReloadMods"), Globals.Game.Content, out tex);
            ReloadModsText = tex;

            Dictionary<string, CommandParser> commandList = new Dictionary<string, CommandParser>
            {
                ["Mods"] = ModList,
                ["Help"] = Help,
                ["Pos"] = PlayerPos,
                ["ModItems"] = ModTotals,
                ["Colliders"] = RenderColliders,
                ["Version"] = GetVersion,
                ["SpawnItem"] = SpawnItem,
                ["ToggleDebug"] = ToggleDebugMode,
                ["SpawnPin"] = SpawnPin

            };

            CommandEntry commands = CreateCommands();

            foreach (var command in commandList)
            {
                commands.Commands[command.Key] = command.Value;
            }
        }

        public override void Unload()
        {
            _colliderRC = null;

            AssetUtils.UnloadAsset(Globals.Game.Content, Path.Combine(AssetPath, "NullTexGS"));
            ErrorTexture = null;

            AssetUtils.UnloadAsset(Globals.Game.Content, Path.Combine(AssetPath, "ModMenu"));
            ModMenuText = null;

            AssetUtils.UnloadAsset(Globals.Game.Content, Path.Combine(AssetPath, "ModList"));
            ModListText = null;

            AssetUtils.UnloadAsset(Globals.Game.Content, Path.Combine(AssetPath, "ReloadMods"));
            ReloadModsText = null;
        }

        private ColliderRC _colliderRC;

        private bool _colliderRCActive = false;

        #region Commands

        private void GetVersion(string[] args, int connection)
        {
            CAS.AddChatMessage(
                "SoG Version: " + Globals.GameVersionFull + "\n" +
                "Grindless Version:" + Version.ToString()
                );
        }

        private void Help(string[] args, int connection)
        {
            List<string> commandList;

            if (args.Length == 0)
            {
                commandList = GetCommands().Commands.Keys.ToList();
            }
            else
            {
                Mod mod = ModManager.Mods.FirstOrDefault(x => x.Name == args[0]);

                if (mod == null)
                {
                    CAS.AddChatMessage($"[{Name}] Unknown mod!");
                    return;
                }

                commandList = mod.GetCommands()?.Commands.Keys.ToList() ?? new List<string>();
            }

            if (commandList.Count == 0)
            {
                CAS.AddChatMessage($"[{Name}] No commands are defined for this mod!");
                return;
            }

            CAS.AddChatMessage($"[{Name}] Command list{(args.Length == 0 ? "" : $" for {args[0]}")}:");

            var messages = new List<string>();
            var concated = "";
            foreach (var cmd in commandList)
            {
                if (concated.Length + cmd.Length > 40)
                {
                    messages.Add(concated);
                    concated = "";
                }
                concated += cmd + " ";
            }
            if (concated != "")
                messages.Add(concated);

            foreach (var line in messages)
                CAS.AddChatMessage(line);
        }

        private void ModList(string[] args, int connection)
        {
            CAS.AddChatMessage($"[{Name}] Mod Count: {ModManager.Mods.Count}");

            var messages = new List<string>();
            var concated = "";
            foreach (var mod in ModManager.Mods)
            {
                string name = mod.Name;
                if (concated.Length + name.Length > 40)
                {
                    messages.Add(concated);
                    concated = "";
                }
                concated += name + " ";
            }
            if (concated != "")
                messages.Add(concated);

            foreach (var line in messages)
                CAS.AddChatMessage(line);
        }

        private void PlayerPos(string[] args, int connection)
        {
            var local = Globals.Game.xLocalPlayer.xEntity.xTransform.v2Pos;

            CAS.AddChatMessage($"[{Name}] Player position: {(int)local.X}, {(int)local.Y}");
        }

        private void ModTotals(string[] args, int connection)
        {
            if (args.Length != 1)
            {
                CAS.AddChatMessage($"[{Name}] Usage: /{Name}:{nameof(ModTotals)} <unique type>");
                return;
            }

            switch (args[0])
            {
                case "Items":
                    CAS.AddChatMessage($"[{Name}] Items defined: " + ItemEntry.Entries.Where(x => x.IsModded).Count());
                    break;
                case "Perks":
                    CAS.AddChatMessage($"[{Name}] Perks defined: " + PerkEntry.Entries.Where(x => x.IsModded).Count());
                    break;
                case "Treats":
                case "Curses":
                    CAS.AddChatMessage($"[{Name}] Treats and Curses defined: " + CurseEntry.Entries.Where(x => x.IsModded).Count());
                    break;
                default:
                    CAS.AddChatMessage($"[{Name}] Usage: /{Name}:{nameof(ModTotals)} <unique type>");
                    break;
            }
        }

        private void RenderColliders(string[] args, int connection)
        {
            _colliderRC.RenderCombat = args.Contains("-c");
            _colliderRC.RenderLevel = args.Contains("-l");
            _colliderRC.RenderMovement = args.Contains("-m");

            _colliderRCActive = _colliderRC.RenderCombat || _colliderRC.RenderLevel || _colliderRC.RenderMovement;

            RenderMaster render = Globals.Game.xRenderMaster;

            render.UnregisterRenderComponenent(_colliderRC);

            if (_colliderRCActive)
            {
                render.RegisterComponent(RenderMaster.SubRenderLayer.AboveSorted, _colliderRC);
                string msg = "Collider rendering enabled for ";
                msg += _colliderRC.RenderCombat ? "Combat, " : "";
                msg += _colliderRC.RenderLevel ? "Level, " : "";
                msg += _colliderRC.RenderMovement ? "Movement, " : "";

                msg = msg.Remove(msg.Length - 2, 2);

                CAS.AddChatMessage(msg);
            }
            else
            {
                CAS.AddChatMessage("Collider rendering disabled.");
            }
        }

        private void SpawnItem(string[] args, int connection)
        {
            if (NetUtils.IsClient)
            {
                CAS.AddChatMessage("Can't use this command as a client!");
                return;
            }

            if (!Globals.Game.bUseDebugInRelease)
            {
                CAS.AddChatMessage("You must switch to debug mode first!");
                return;
            }

            if (args.Length < 1 || args.Length > 2)
            {
                CAS.AddChatMessage("Usage: /Grindless:SpawnItem <Mod.NameID>:<Item.ModID> [amount]");
                return;
            }

            string[] parts = args[0].Split(':');
            long count = 1;

            if (parts.Length != 2 || args.Length == 2 && !long.TryParse(args[1], out count))
            {
                CAS.AddChatMessage("Usage: /Grindless:SpawnItem <Mod.NameID>:<Item.ModID> [amount]");
                return;
            }

            if (count < 1 || count > 128)
            {
                CAS.AddChatMessage($"You can only spawn 1 - 128 items at a time.");
                return;
            }

            Mod target;
            if ((target = ModManager.Mods.FirstOrDefault(x => x.Name == parts[0])) == null)
            {
                CAS.AddChatMessage("No such mod exists!");
                return;
            }

            var entry = ItemEntry.Entries.Get(target, parts[1]);

            if (entry == null)
            {
                CAS.AddChatMessage("The mod doesn't have an entry with that ID!");
                return;
            }

            PlayerEntity player = Globals.Game.xLocalPlayer.xEntity;

            long counter = count;
            while (counter-- > 0)
            {
                Globals.Game._EntityMaster_AddItem(entry.GameID, player.xTransform.v2Pos, player.xRenderComponent.fVirtualHeight, player.xCollisionComponent.ibitCurrentColliderLayer, Utility.RandomizeVector2Direction(Globals.Game.randomInVisual));
            }

            CAS.AddChatMessage($"Spawned {count} items.");
        }

        private void SpawnPin(string[] args, int connection)
        {
            if (NetUtils.IsClient)
            {
                CAS.AddChatMessage("Can't use this command as a client!");
                return;
            }

            if (!Globals.Game.bUseDebugInRelease)
            {
                CAS.AddChatMessage("You must switch to debug mode first!");
                return;
            }

            if (args.Length != 1)
            {
                CAS.AddChatMessage("Usage: /Grindless:SpawnPin <Mod.NameID>:<Pin.ModID>");
                return;
            }

            string[] parts = args[0].Split(':');

            Mod target = ModManager.Mods.First(x => x.Name == parts[0]);

            if (target == null)
            {
                CAS.AddChatMessage("No such mod exists!");
                return;
            }

            var entry = PinEntry.Entries.Get(target, parts[1]);

            if (entry == null)
            {
                CAS.AddChatMessage("The mod doesn't have an entry with that ID!");
                return;
            }

            PlayerEntity player = Globals.Game.xLocalPlayer.xEntity;

            Globals.Game._EntityMaster_AddWatcher(new PinSpawned(entry.GameID, new Vector2(330f, 324f), player.xTransform.v2Pos));

            CAS.AddChatMessage($"Spawned pin.");
        }

        private void WorldRegion(string[] args, int connection)
        {
            CAS.AddChatMessage($"Level: {Globals.Game.xLevelMaster.xCurrentLevel.enZone}\nWorld region: {Globals.Game.xLevelMaster.xCurrentLevel.enRegion}.");
        }

        private void ToggleDebugMode(string[] args, int connection)
        {
            Globals.Game.bUseDebugInRelease = !Globals.Game.bUseDebugInRelease;
            CAS.AddChatMessage("Debug mode is now " + (Globals.Game.bUseDebugInRelease ? "on" : "off"));

            if (Globals.Game.bUseDebugInRelease)
            {
                CAS.AddChatMessage("Try not to break anything, eh?");
            }
        }

        #endregion
    }
}
