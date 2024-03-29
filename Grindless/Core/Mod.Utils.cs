﻿using Lidgren.Network;

namespace Grindless;

public abstract partial class Mod
{
    /// <summary>
    /// Sends a packet to the chosen Client if currently playing as a Server; otherwise, it does nothing.
    /// </summary>
    public void SendToClient(ushort packetID, Action<BinaryWriter> data, PlayerView receiver, SequenceChannel channel = SequenceChannel.PrioritySpells_20, NetDeliveryMethod reliability = NetDeliveryMethod.ReliableOrdered)
    {
        if (!NetUtils.IsServer)
            return;

        Globals.Game._Network_SendMessage(NetUtils.WriteModData(this, packetID, data), receiver, channel, reliability);
    }

    /// <summary>
    /// Sends a packet to all Clients, if currently playing as a Server; otherwise, it does nothing.
    /// </summary>
    public void SendToAllClients(ushort packetID, Action<BinaryWriter> data, SequenceChannel channel = SequenceChannel.PrioritySpells_20, NetDeliveryMethod reliability = NetDeliveryMethod.ReliableOrdered)
    {
        if (!NetUtils.IsServer)
            return;

        Globals.Game._Network_SendMessage(NetUtils.WriteModData(this, packetID, data), channel, reliability);
    }

    /// <summary>
    /// Sends a packet to all Clients, except one, if currently playing as a Server; otherwise, it does nothing.
    /// </summary>
    public void SendToAllClientsExcept(ushort packetID, Action<BinaryWriter> data, PlayerView excluded, SequenceChannel channel = SequenceChannel.PrioritySpells_20, NetDeliveryMethod reliability = NetDeliveryMethod.ReliableOrdered)
    {
        if (!NetUtils.IsServer)
            return;

        foreach (PlayerView view in Globals.Game.dixPlayers.Values)
        {
            if (view == excluded)
                continue;

            Globals.Game._Network_SendMessage(NetUtils.WriteModData(this, packetID, data), channel, reliability);
        }
    }

    /// <summary>
    /// Sends a packet to the Server if currently playing as a Client; otherwise, it does nothing.
    /// </summary>
    public void SendToServer(ushort packetID, Action<BinaryWriter> data, SequenceChannel channel = SequenceChannel.PrioritySpells_20, NetDeliveryMethod reliability = NetDeliveryMethod.ReliableOrdered)
    {
        if (!NetUtils.IsClient)
            return;

        Globals.Game._Network_SendMessage(NetUtils.WriteModData(this, packetID, data), channel, reliability);
    }
}
