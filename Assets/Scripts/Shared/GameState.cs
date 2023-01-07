using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameState : NetworkBehaviour
{
    private NetworkVariable<int> _playersAlive;
    private NetworkVariable<bool> _gameStarted;
    private NetworkVariable<bool> _gameEnded;

    public event Action OnGameStarted;

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
            NetworkManager.Singleton.SceneManager.LoadScene("MapLobby", LoadSceneMode.Single);
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
            NetworkManager.Singleton.SceneManager.LoadScene("MapLobby", LoadSceneMode.Single);
        }
    }

    public void OnCharacterSpawned()
    {
        if (!IsServer) return;
        _playersAlive.Value++;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }
}