using System.Reflection.Emit;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Grindless.HarmonyPatches;

[HarmonyPatch(typeof(Game1), nameof(Game1._Network_ParseServerMessage))]
internal class _Network_ParseServerMessage
{
    /// <summary>
    /// Transpiles processing of server messages by the client.
    /// First insertion allows mod packets from server to be parsed.
    /// </summary>
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> _Network_ParseServerMessage_Transpiler(IEnumerable<CodeInstruction> code, ILGenerator gen)
    {
        bool isMethodEnd(List<CodeInstruction> codeToSearch, int index)
        {
            return
                codeToSearch[index].opcode == OpCodes.Leave_S &&
                codeToSearch[index + 1].opcode == OpCodes.Ldc_I4_1 &&
                codeToSearch[index + 2].opcode == OpCodes.Ret;
        }

        bool isMessage19VersionSend(List<CodeInstruction> list, int index)
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
        int firstInsertIndex = methodEndIndex + 1;

        var firstInsert = new List<CodeInstruction>()
        {
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(OpCodes.Ldloc_1),
            new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => InNetworkParseServerMessage(null, default))),
        };

        firstInsert[0].WithLabels(codeList[firstInsertIndex].labels.ToArray());
        codeList[firstInsertIndex].labels.Clear();

        codeList.InsertAt(firstInsertIndex, firstInsert);

        // Second Insertion

        int versionCheckIndex = PatchUtils.FindPosition(codeList, isMessage19VersionSend);

        MethodInfo secondTargetMethod = typeof(Game1).GetMethod(nameof(Game1._Network_SendMessage), new Type[] { typeof(OutMessage), typeof(int), typeof(Lidgren.Network.NetDeliveryMethod) });

        var secondInsert = new List<CodeInstruction>()
        {
            new CodeInstruction(OpCodes.Ldloc_S, 81),
            new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(() => WriteModList(null)))
        };

        codeList.InsertBeforeMethod(secondTargetMethod, secondInsert, startOffset: versionCheckIndex);

        return codeList;
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
}
