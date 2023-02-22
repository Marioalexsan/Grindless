using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Watchers;
using SoG;
using Grindless.Core;

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

            var manager = Globals.Game.Content;

            ErrorTexture = manager.TryLoad<Texture2D>(Path.Combine(AssetPath, "NullTexGS"));
            ModMenuText = manager.TryLoad<Texture2D>(Path.Combine(AssetPath, "ModMenu"));
            ModListText = manager.TryLoad<Texture2D>(Path.Combine(AssetPath, "ModList"));
            ReloadModsText = manager.TryLoad<Texture2D>(Path.Combine(AssetPath, "ReloadMods"));

            CreateCommands().AutoAddModCommands();
        }

        public override void Unload()
        {
            _colliderRC = null;

            Globals.Game.Content.Unload(Path.Combine(AssetPath, "NullTexGS"));
            ErrorTexture = null;

            Globals.Game.Content.Unload(Path.Combine(AssetPath, "ModMenu"));
            ModMenuText = null;

            Globals.Game.Content.Unload(Path.Combine(AssetPath, "ModList"));
            ModListText = null;

            Globals.Game.Content.Unload(Path.Combine(AssetPath, "ReloadMods"));
            ReloadModsText = null;
        }

        private ColliderRC _colliderRC;

        private bool _colliderRCActive = false;

        #region Commands

        [ModCommand("Help")]
        private void Help(string[] args, int connection)
        {
            Mod mod = args.Length == 0 ? this : ModManager.Mods.FirstOrDefault(x => x.Name == args[0]);

            if (mod == null)
            {
                CAS.AddChatMessage($"[{Name}] Unknown mod!");
                return;
            }

            List<string> commandList = mod.GetCommands()?.Commands.Keys.ToList() ?? new List<string>();

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

        [ModCommand("ModList")]
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

        [ModCommand("RenderColliders")]
        private void RenderColliders(string[] args, int connection)
        {
            if (args.Any(x => !(x == "-c" || x == "-l" || x == "-m")))
            {
                CAS.AddChatMessage($"Usage: /Grindless:{nameof(RenderColliders)} [-c] [-l] [-m]");
                return;
            }

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

        [ModCommand("Spawn")]
        private void Spawn(string[] args, int connection)
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

            if (args.Length < 2)
            {
                CAS.AddChatMessage($"Usage: /Grindless:{nameof(Spawn)} <Entry Type> <Mod.NameID>:<Entry.ModID>");
                CAS.AddChatMessage("Valid Entry Types: Item, Pin");
                return;
            }

            switch (args[0])
            {
                case "Item":
                    {
                        long count = 1;

                        if (args.Length > 3 || args.Length == 3 && !long.TryParse(args[2], out count))
                        {
                            CAS.AddChatMessage($"Usage: /Grindless:{nameof(Spawn)} Item <Mod.NameID>:<Item.ModID> [amount]");
                            return;
                        }

                        if (count < 1 || count > 1000)
                        {
                            CAS.AddChatMessage($"You can only spawn between 1 and 1000 items at a time.");
                            return;
                        }

                        var entry = ResolveEntry(args[1], ItemEntry.Entries);

                        if (entry == null)
                        {
                            CAS.AddChatMessage("The mod or entry ID is invalid!");
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
                    return;
                case "Pin":
                    {
                        if (args.Length > 2)
                        {
                            CAS.AddChatMessage($"Usage: /Grindless:{nameof(Spawn)} Pin <Mod.NameID>:<Pin.ModID>");
                            return;
                        }

                        var entry = ResolveEntry(args[0], PinEntry.Entries);

                        if (entry == null)
                        {
                            CAS.AddChatMessage("The mod or entry ID is invalid!");
                            return;
                        }

                        PlayerEntity player = Globals.Game.xLocalPlayer.xEntity;

                        Globals.Game._EntityMaster_AddWatcher(new PinSpawned(entry.GameID, new Vector2(330f, 324f), player.xTransform.v2Pos));

                        CAS.AddChatMessage($"Spawned pin.");
                    }
                    return;
                default:
                    CAS.AddChatMessage($"Usage: /Grindless:{nameof(Spawn)} <Entry Type> <Mod.NameID>:<Entry.ModID>");
                    CAS.AddChatMessage("Valid Entry Types: Item, Pin");
                    return;
            }
        }

        [ModCommand("ToggleDebug")]
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

        private Entry<IDType> ResolveEntry<IDType, EntryType>(string entryId, EntryManager<IDType, EntryType> manager)
            where IDType : struct, Enum
            where EntryType : Entry<IDType>
        {
            string[] parts = entryId.Split(':');

            if (parts.Length != 2)
                return null;

            Mod target = ModManager.Mods.FirstOrDefault(x => x.Name == parts[0]);

            if (target == null)
                return null;

            return manager.Get(target, parts[1]);
        }
    }
}
