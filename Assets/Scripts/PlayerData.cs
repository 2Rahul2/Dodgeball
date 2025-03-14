using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData :IEquatable<PlayerData> , INetworkSerializable
{
    public ulong clientId;
    public FixedString64Bytes playerName;
    public int teamId;
    public int playerCharacterVisualIndex;
    public bool Equals(PlayerData other){
        return clientId == other.clientId && playerCharacterVisualIndex == other.playerCharacterVisualIndex && playerName == other.playerName && teamId == other.teamId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref teamId);
        serializer.SerializeValue(ref playerCharacterVisualIndex);
        serializer.SerializeValue(ref playerName);
    }
}
