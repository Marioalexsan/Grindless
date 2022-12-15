using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using SoG;
using Microsoft.Extensions.Logging;

namespace Grindless
{
    internal static class PatchHelper
    {
        /// <summary>
        /// Executes additional code after a level's blueprint has been processed.
        /// </summary>
        public static void InLevelLoadDoStuff(Level.ZoneEnum type, bool staticOnly)
        {
            var entry = LevelEntry.Entries.Get(type);

            Debug.Assert(entry != null && entry.IsModded);

            try
            {
                entry.Loader?.Invoke(staticOnly);
            }
            catch (Exception e)
            {
                Program.Logger.LogError("Loader threw an exception for level {type}! Exception: {e}", type, e);
            }
        }

        // If this method returns true, the vanilla switch case is skipped
        public static bool InActivatePerk(PlayerView view, RogueLikeMode.Perks perk)
        {
            var entry = PerkEntry.Entries.Get(perk);

            if (entry == null)
            {
                return false;  // Unknown mod entry??
            }

            // This callback accepts vanilla perks because the vanilla activators are in a for loop
            if (entry.IsVanilla && entry.RunStartActivator == null)
            {
                return false;
            }

            try
            {
                entry.RunStartActivator?.Invoke(view);
            }
            catch (Exception e)
            {
                Program.Logger.LogError("Run start activator threw an exception for perk {perk}! Exception: {e}", perk, e);
            }

            return true;
        }

        /// <summary>
        /// For modded enemies, creates an enemy and runs its "constructor".
        /// </summary>
        public static Enemy InGetEnemyInstance(EnemyCodex.EnemyTypes gameID, Level.WorldRegion assetRegion)
        {
            var entry = EnemyEntry.Entries.Get(gameID);

            Debug.Assert(entry != null && entry.IsModded);

            Enemy enemy = new Enemy() { enType = gameID };

            enemy.xRenderComponent.xOwnerObject = enemy;

            entry.Constructor?.Invoke(enemy);

            return enemy;
        }

        /// <summary>
        /// For modded enemies, checks if the enemy has an elite scaler defined.
        /// If yes, then the enemy is made elite.
        /// Returns true if the enemy was made elite, false otherwise.
        /// The return value is used for subsequent vanilla code.
        /// </summary>
        public static bool InEnemyMakeElite(Enemy enemy)
        {
            var entry = EnemyEntry.Entries.Get(enemy.enType);

            Debug.Assert(entry != null && entry.IsModded);

            if (entry.EliteScaler != null)
            {
                entry.EliteScaler.Invoke(enemy);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Called when a server parses a client message. The message type's parser also receives the connection ID of the sender.
        /// </summary>
        public static void InNetworkParseClientMessage(InMessage msg, byte messageType)
        {
            if (messageType != NetUtils.ModPacketType)
            {
                return;
            }

            try
            {
                NetUtils.ReadModData(msg, out Mod mod, out ushort packetID);

                ServerSideParser parser = null;
                mod.GetNetwork()?.ServerSide.TryGetValue(packetID, out parser);

                if (parser == null)
                {
                    return;
                }

                byte[] content = msg.ReadBytes((int)(msg.BaseStream.Length - msg.BaseStream.Position));

                parser.Invoke(new BinaryReader(new MemoryStream(content)), msg.iConnectionIdentifier);
            }
            catch (Exception e)
            {
                Program.Logger.LogError("ParseClientMessage failed! Exception: {e}", e.Message);
            }
        }

        /// <summary>
        /// Called when a client parses a server message.
        /// </summary>
        public static void InNetworkParseServerMessage(InMessage msg, byte messageType)
        {
            if (messageType != NetUtils.ModPacketType)
            {
                return;
            }

            try
            {
                NetUtils.ReadModData(msg, out Mod mod, out ushort packetID);

                ClientSideParser parser = null;
                mod.GetNetwork()?.ClientSide.TryGetValue(packetID, out parser);

                if (parser == null)
                {
                    return;
                }

                byte[] content = msg.ReadBytes((int)(msg.BaseStream.Length - msg.BaseStream.Position));

                parser.Invoke(new BinaryReader(new MemoryStream(content)));
            }
            catch (Exception e)
            {
                Program.Logger.LogError("ParseServerMessage failed! Exception: {e}", e.Message);
            }
        }

        public static void GauntletEnemySpawned(Enemy enemy)
        {
            foreach (Mod mod in ModManager.Mods)
                mod.PostArcadeGauntletEnemySpawned(enemy);
        }

        public static void AddModdedPinsToList(List<PinCodex.PinType> list)
        {
            foreach (PinEntry entry in PinEntry.Entries)
            {
                if (entry.ConditionToDrop == null || entry.ConditionToDrop.Invoke())
                {
                    list.Add(entry.GameID);
                }
            }
        }

        public static string GetCueName(string GSID)
        {
            if (!ModUtils.SplitAudioID(GSID, out int entryID, out bool isMusic, out int cueID))
                return "";

            var entry = AudioEntry.Entries.Get((GrindlessID.AudioID)entryID);

            if (entry == null)
            {
                return "";
            }

            return isMusic ? entry.indexedMusicCues[cueID] : entry.indexedEffectCues[cueID];
        }

        public static SoundBank GetEffectSoundBank(string audioID)
        {
            bool success = ModUtils.SplitAudioID(audioID, out int entryID, out bool isMusic, out _);
            if (!(success && !isMusic))
                return null;

            var entry = AudioEntry.Entries.Get((GrindlessID.AudioID)entryID);

            return entry?.effectsSB;
        }

        public static SoundBank GetMusicSoundBank(string audioID)
        {
            bool success = ModUtils.SplitAudioID(audioID, out int entryID, out bool isMusic, out _);

            if (!(success && isMusic))
                return null;

            var entry = AudioEntry.Entries.Get((GrindlessID.AudioID)entryID);

            return entry?.musicSB;
        }

        public static bool IsUniversalMusicBank(string bank)
        {
            if (bank == "UniversalMusic")
                return true;

            foreach (var mod in ModManager.Mods)
            {
                if (mod.Name == bank)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whenever the given WaveBank is persistent. <para/>
        /// Persistent WaveBanks are never unloaded.
        /// </summary>
        public static bool IsUniversalMusicBank(WaveBank bank)
        {
            if (bank == null)
                return false;

            foreach (var mod in ModManager.Mods)
            {
                var entry = AudioEntry.Entries.Get(mod, "");

                if (entry != null && entry.universalWB == bank)
                    return true;
            }

            FieldInfo universalWaveBankField = typeof(SoundSystem).GetTypeInfo().GetField("universalMusicWaveBank", BindingFlags.NonPublic | BindingFlags.Instance);
            if (bank == universalWaveBankField.GetValue(Globals.Game.xSoundSystem))
                return true;

            return false;
        }


        public static SpriteBatch SpriteBatch => Globals.SpriteBatch;

        #region Delicate Versioning and Mod List Comparison callbacks

        public static bool CheckModListCompatibility(bool didVersionCheckPass, InMessage msg)
        {
            if (!didVersionCheckPass)
            {
                // It's actually just a crappy implementation of short circuiting for AND ¯\_(ツ)_/¯

                Program.Logger.LogInformation("Denying connection due to vanilla version discrepancy.");
                Program.Logger.LogInformation("Check if client is on the same game version, and is running Grindless.");
                return false;
            }

            int failReason = 0;

            Program.Logger.LogDebug("Reading mod list!");

            long savedStreamPosition = msg.BaseStream.Position;

            _ = msg.ReadByte(); // Game mode byte skipped

            bool readGSVersion = Version.TryParse(msg.ReadString(), out Version result);

            if (readGSVersion)
            {
                Program.Logger.LogInformation("Received GS version from client: {version}", result);
            }
            else
            {
                Program.Logger.LogInformation("Couldn't parse GS version from client!");
            }

            if (!readGSVersion || result != GrindlessMod.Instance.Version)
            {
                Program.Logger.LogInformation("Denying connection due to Grindless version discrepancy.");
                Program.Logger.LogInformation("Check that server and client are running on the same Grindless version.");
                return false;
            }

            var clientMods = new List<(string NameID, Version Version)>();
            var serverMods = ModManager.Mods;

            int clientModCount = msg.ReadInt32();
            int serverModCount = serverMods.Count;

            for (int index = 0; index < clientModCount; index++)
            {
                clientMods.Add((msg.ReadString(), Version.Parse(msg.ReadString())));
            }

            if (clientModCount == serverModCount)
            {
                for (int index = 0; index < clientModCount; index++)
                {
                    if (serverMods[index].DisableObjectCreation)
                    {
                        continue;
                    }

                    if (clientMods[index].NameID != serverMods[index].Name || clientMods[index].Version != serverMods[index].Version)
                    {
                        failReason = 2;
                        break;
                    }
                }
            }
            else
            {
                failReason = 1;
            }

            Program.Logger.LogDebug("Mods received from client: ");
            foreach (var meta in clientMods)
            {
                Program.Logger.LogDebug("{id}, v{Version}", meta.NameID, meta.Version?.ToString() ?? "Unknown");
            }

            Program.Logger.LogDebug("Mods on server: ");
            foreach (var mod in serverMods)
            {
                Program.Logger.LogDebug("{id}, v{Version}", mod.Name, mod.Version?.ToString() ?? "Unknown");
            }

            if (failReason == 1)
            {
                Program.Logger.LogDebug("Client has {clientModCount} mods, while server has {serverModCount}.", clientModCount, serverModCount);
            }
            else if (failReason == 2)
            {
                Program.Logger.LogDebug($"Client's mod list doesn't seem compatible with server's mod list.");
            }

            if (failReason != 0)
            {
                Program.Logger.LogDebug("Denying connection due to mod discrepancy.");
            }
            else
            {
                Program.Logger.LogDebug("Client mod list seems compatible!");
            }

            msg.BaseStream.Position = savedStreamPosition;

            return failReason == 0;
        }

        public static void WriteModList(OutMessage msg)
        {
            Program.Logger.LogDebug("Writing mod list!");

            msg.Write(GrindlessMod.Instance.Version.ToString());

            msg.Write(ModManager.Mods.Count);

            foreach (Mod mod in ModManager.Mods)
            {
                if (mod.DisableObjectCreation)
                {
                    continue;
                }

                msg.Write(mod.Name);
                msg.Write(mod.Version.ToString());
            }

            Program.Logger.LogDebug("Done with mod list!");
        }

        #endregion
    }
}
