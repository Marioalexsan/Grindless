using System.Reflection.Emit;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._Network_ParseClientMessage))]
static class _Network_ParseClientMessage
{
    /// <summary>
    /// Transpiles processing of client messages by the server.
    /// </summary>
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> code, ILGenerator gen)
    {
        // Finds the method end. Used to insert mod packet parsing
        bool isMethodEnd(List<CodeInstruction> list, int index)
        {
            return
                list[index].opcode == OpCodes.Leave_S &&
                list[index + 1].opcode == OpCodes.Ldc_I4_1 &&
                list[index + 2].opcode == OpCodes.Ret;
        }

        // Finds the demo check in message 97 parser. Used to check mod list compatibility
        bool isMessage97VersionCheck(List<CodeInstruction> list, int index)
        {
            return
                list[index].opcode == OpCodes.Ldarg_0 &&
                list[index + 1].opcode == OpCodes.Ldfld &&
                ReferenceEquals(list[index + 1].operand, typeof(Game1).GetField(nameof(Game1.bIsDemo))) &&
                list[index + 2].opcode == OpCodes.Brfalse_S;
        }

        List<CodeInstruction> codeList = code.ToList();

        // First Insertion

        int methodEndIndex = PatchUtils.FindPosition(codeList, isMethodEnd);
        int firstIndex = methodEndIndex + 1;

        var firstInsert = new List<CodeInstruction>()
        {
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(OpCodes.Ldloc_3),
            new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => InNetworkParseClientMessage(null, default))),
        };

        firstInsert[0].WithLabels(codeList[firstIndex].labels.ToArray());
        codeList[firstIndex].labels.Clear();

        codeList.InsertAt(firstIndex, firstInsert);

        // Second Insertion

        int versionCheckIndex = PatchUtils.FindPosition(codeList, isMessage97VersionCheck);
        MethodInfo secondTargetMethod = typeof(string).GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public);

        var secondInsert = new List<CodeInstruction>()
        {
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => CheckModListCompatibility(default, null)))
        };

        return codeList.InsertAfterMethod(secondTargetMethod, secondInsert, startOffset: versionCheckIndex, editsReturnValue: true);
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
}
