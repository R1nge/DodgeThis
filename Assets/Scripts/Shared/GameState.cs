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
            _playersAlive.OnValueChanged += OnPlayersAmountChanged;
            _gameStarted = new NetworkVariable<bool>();
            _gameEnded = new NetworkVariable<bool>();
        }

        private void OnClientDisconnected(ulong obj)
        {
            if (!IsServer) return;
            if (LobbySingleton.Instance.GetPlayersList().Count <= 1)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            }
        }

        //TODO: add places???

        [ServerRpc(RequireOwnership = false)]
        public void OnPlayerKilledServerRpc(ulong clientID)
        {
            var players = LobbySingleton.Instance.GetPlayersList();
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].ClientId == clientID)
                {
                    LobbySingleton.Instance.GetPlayersList()[i] = new PlayerState(
                        LobbySingleton.Instance.GetPlayersList()[i].ClientId,
                        LobbySingleton.Instance.GetPlayersList()[i].Nickname,
                        LobbySingleton.Instance.GetPlayersList()[i].SkinIndex,
                        LobbySingleton.Instance.GetPlayersList()[i].IsReady,
                        LobbySingleton.Instance.GetPlayersList()[i].Score,
                        false
                    );
                }
            }

            _playersAlive.Value--;
        }

        private void OnPlayersAmountChanged(int oldValue, int newValue)
        {
            if (newValue <= 1 && oldValue == 2)
            {
                EndGameServerRpc();
                AddScoreServerRpc();
                NetworkManager.Singleton.SceneManager.LoadScene("SelectRandomGame", LoadSceneMode.Single);
            }
        }

        public void OnCharacterSpawned()
        {
            if (!IsServer) return;
            _playersAlive.Value++;
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddScoreServerRpc(int amount = default)
        {
            var players = LobbySingleton.Instance.GetPlayersList();
            var playersCount = players.Count;

            for (int i = 0; i < playersCount; i++)
            {
                if (players[i].IsAlive)
                {
                    if (amount == default)
                    {
                        var score = 100 - _playersAlive.Value * 10;
                        LobbySingleton.Instance.AddScore(i, score);
                        print(amount + " ID " + players[i].ClientId);
                    }
                    else
                    {
                        LobbySingleton.Instance.AddScore(i, amount);
                        print(amount + " ID " + players[i].ClientId);
                    }
                }
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void AddScoreByIndexServerRpc(int index, int amount = default)
        {
            var players = LobbySingleton.Instance.GetPlayersList();

            if (amount == default)
            {
                var score = 100 - _playersAlive.Value * 10;
                LobbySingleton.Instance.AddScore(index, score);
                print(score);
            }
            else
            {
                LobbySingleton.Instance.AddScore(index, amount);
                print(amount + " ID " + players[index].ClientId);
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