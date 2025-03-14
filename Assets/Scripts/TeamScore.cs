using System;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public struct TeamScore : IEquatable<TeamScore> , INetworkSerializable
{
    public int score;
    public int roundWon;
    public int teamId;
    public bool Equals(TeamScore other)
    {
        return score == other.score && roundWon == other.roundWon && teamId == other.teamId;
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref score);
        serializer.SerializeValue(ref roundWon);
        serializer.SerializeValue(ref teamId);
    }
    public TeamScore(int score , int roundWon ,int teamId){
        this.score = score;
        this.roundWon = roundWon;
        this.teamId = teamId;
    }

}
