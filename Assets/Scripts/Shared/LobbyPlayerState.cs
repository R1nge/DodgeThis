using System;
using Unity.Netcode;

namespace Lobby
{
    [Serializable]
    public struct LobbyPlayerState : INetworkSerializable, IEquatable<LobbyPlayerState>
    {
        public ulong ClientId;
        public NetworkString PlayerName;
        public int SkinIndex;
        public bool IsReady;
        public int Score;

        public LobbyPlayerState(ulong clientId, string playerName, int skinIndex, bool isReady, int score)
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


        public bool Equals(LobbyPlayerState other)
        {
            return ClientId == other.ClientId && PlayerName.Equals(other.PlayerName) && SkinIndex == other.SkinIndex &&
                   IsReady == other.IsReady && Score == other.Score;
        }

        public override bool Equals(object obj)
        {
            return obj is LobbyPlayerState other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ClientId, PlayerName, SkinIndex, IsReady, Score);
        }
    }
}