using System;
using Unity.Collections;
using Unity.Netcode;

public struct LeaderboardEntityState : INetworkSerializable, IEquatable<LeaderboardEntityState>
{
    public ulong clientID;
    public FixedString32Bytes playerName;
    public int kills;
    public int deaths;

    public bool Equals(LeaderboardEntityState other)
    {
        return clientID == other.clientID && playerName.Equals(other.playerName) && kills == other.kills && deaths == other.deaths;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientID);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref kills);
        serializer.SerializeValue(ref deaths);
    }
}
