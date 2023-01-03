using System;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameState : NetworkBehaviour
{
    [SerializeField] private SceneAsset[] scenes;
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
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        _playersAlive = new NetworkVariable<int>(1);
        _gameStarted = new NetworkVariable<bool>();
        _gameEnded = new NetworkVariable<bool>();
    }

    private void OnClientConnected(ulong obj)
    {
        if (!IsServer) return;
        _playersAlive.Value++;
    }

    private void OnClientDisconnected(ulong obj)
    {
        if (!IsServer) return;
        _playersAlive.Value--;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakePlaceServerRpc()
    {
        if (!IsServer) return;
        _playersAlive.Value--;
        if (_playersAlive.Value == 1 || _playersAlive.Value == 0)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(scenes[Random.Range(0, scenes.Length)].name,
                LoadSceneMode.Single);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }
}