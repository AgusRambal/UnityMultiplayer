using System;
using Unity.Collections;
using Unity.Netcode;

public struct LeaderboardEntityState : INetworkSerializable, IEquatable<LeaderboardEntityState>
{
    public ulong clientID;
    public FixedString32Bytes playerName;
    public int points;
    public int deaths;
    public int totalKills;

    public bool Equals(LeaderboardEntityState other)
    {
        return clientID == other.clientID && playerName.Equals(other.playerName) && points == other.points && deaths == other.deaths && totalKills == other.totalKills;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientID);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref points);
        serializer.SerializeValue(ref deaths);
        serializer.SerializeValue(ref totalKills);
    }
}
