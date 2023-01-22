using System;
using Unity.Netcode;

namespace Shared
{
    [Serializable]
    public struct PlayerState : INetworkSerializable, IEquatable<PlayerState>
    {
        public ulong ClientId;
        public NetworkString PlayerName;
        public int SkinIndex;
        public bool IsReady;
        public int Score;

        public PlayerState(ulong clientId, string playerName, int skinIndex, bool isReady, int score)
        {
            ClientId = clientId;
            PlayerName = playerName;
            SkinIndex = skinIndex;
            IsReady = isReady;
            Score = score;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref SkinIndex);
            serializer.SerializeValue(ref IsReady);
            serializer.SerializeValue(ref Score);
        }


        public bool Equals(PlayerState other)
        {
            return ClientId == other.ClientId && PlayerName.Equals(other.PlayerName) && SkinIndex == other.SkinIndex &&
                   IsReady == other.IsReady && Score == other.Score;
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerState other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ClientId, PlayerName, SkinIndex, IsReady, Score);
        }
    }
}