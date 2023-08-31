using System;
using Unity.Collections;
using Unity.Netcode;

public struct LeaderboardEntityState : INetworkSerializable, IEquatable<LeaderboardEntityState>
{
    public ulong clientID;
    public FixedString32Bytes playerName;
    public int coins;
    public int kills;

    public bool Equals(LeaderboardEntityState other)
    {
        return clientID == other.clientID && playerName.Equals(other.playerName) && coins == other.coins && kills == other.kills;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientID);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref coins);
        serializer.SerializeValue(ref kills);
    }
}
