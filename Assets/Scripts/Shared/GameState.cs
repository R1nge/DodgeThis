using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Shared
{
    public class GameState : NetworkBehaviour
    {
        private NetworkVariable<int> _playersAlive;
        private NetworkVariable<bool> _gameStarted;
        private NetworkVariable<bool> _gameEnded;

        public event Action OnGameStarted;
        public event Action OnGameEnd;

        [ServerRpc(RequireOwnership = false)]
        public void StartGameServerRpc()
        {
            if (_gameEnded.Value) return;
            if (_gameStarted.Value) return;
            _gameStarted.Value = true;
            StartGameClientRpc();
        }

        [ClientRpc]
        private void StartGameClientRpc() => OnGameStarted?.Invoke();

        private void Awake()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            _playersAlive = new NetworkVariable<int>();
            _gameStarted = new NetworkVariable<bool>();
            _gameEnded = new NetworkVariable<bool>();
        }

        private void OnClientDisconnected(ulong obj)
        {
            if (!IsServer) return;
            //BUG: should be LobbySingleton.Instance.GetPlayersList()
            if (_playersAlive.Value <= 1)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            }
        }

        //TODO: add places???
        [ServerRpc(RequireOwnership = false)]
        public void OnPlayerKilledServerRpc()
        {
            AddScoreServerRpc();
            _playersAlive.Value--;
            if (_playersAlive.Value <= 1)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("SelectRandomGame", LoadSceneMode.Single);
            }
        }

        public void OnCharacterSpawned()
        {
            if (!IsServer) return;
            _playersAlive.Value++;
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddScoreServerRpc(int amount = default, ServerRpcParams rpcParams = default)
        {
            var players = LobbySingleton.Instance.GetPlayersList();
            var playersCount = players.Count;

            for (int i = 0; i < playersCount; i++)
            {
                if (players[i].ClientId == rpcParams.Receive.SenderClientId)
                {
                    if (amount == default)
                    {
                        var score = 100 - _playersAlive.Value * 10;
                        LobbySingleton.Instance.AddScore(i, score);
                        print(score);
                    }
                    else
                    {
                        LobbySingleton.Instance.AddScore(i, amount);
                        print(amount);
                    }
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void EndGameServerRpc()
        {
            if (!_gameStarted.Value) return;
            if (_gameEnded.Value) return;
            _gameEnded.Value = true;
            EndGameClientRpc();
        }

        [ClientRpc]
        private void EndGameClientRpc()
        {
            OnGameEnd?.Invoke();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (NetworkManager.Singleton == null) return;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
}