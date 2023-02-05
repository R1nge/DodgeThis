using System;
using Unity.Netcode;

namespace Shared
{
    [Serializable]
    public struct PlayerState : INetworkSerializable, IEquatable<PlayerState>
    {
        public ulong ClientId;
        public NetworkString Nickname;
        public int SkinIndex;
        public bool IsReady;
        public int Score;
        public bool IsAlive;

        public PlayerState(ulong clientId, string nickname, int skinIndex, bool isReady, int score, bool isAlive)
        {
            ClientId = clientId;
            Nickname = nickname;
            SkinIndex = skinIndex;
            IsReady = isReady;
            Score = score;
            IsAlive = isAlive;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref Nickname);
            serializer.SerializeValue(ref SkinIndex);
            serializer.SerializeValue(ref IsReady);
            serializer.SerializeValue(ref Score);
            serializer.SerializeValue(ref IsAlive);
        }

        public bool Equals(PlayerState other)
        {
            return ClientId == other.ClientId && Nickname.Equals(other.Nickname) && SkinIndex == other.SkinIndex && IsReady == other.IsReady && Score == other.Score && IsAlive == other.IsAlive;
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerState other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ClientId, Nickname, SkinIndex, IsReady, Score, IsAlive);
        }
    }
}