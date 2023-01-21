using System;
using Shared;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameState : NetworkBehaviour
{
    private NetworkVariable<int> _playersAlive;
    private NetworkVariable<bool> _gameStarted;
    private NetworkVariable<bool> _gameEnded;

    public event Action OnGameStarted;
    public event Action OnGameEnded;

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
        if (_playersAlive.Value <= 1)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }

    //TODO: add places???
    [ServerRpc(RequireOwnership = false)]
    public void OnPlayerKilledServerRpc()
    {
        if (!IsServer) return;
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
    public void AddScoreServerRpc(int amount, ServerRpcParams rpcParams = default)
    {
        var players = LobbySingleton.Instance.GetPlayersList();
        var playersCount = players.Count;

        for (int i = 0; i < playersCount; i++)
        {
            if (players[i].ClientId == rpcParams.Receive.SenderClientId)
            {
                LobbySingleton.Instance.AddScore(i, amount);
                print(LobbySingleton.Instance.GetPlayersList()[i].Score);
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
        OnGameEnded?.Invoke();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }
}