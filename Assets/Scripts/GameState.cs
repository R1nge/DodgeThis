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
    public event Action OnGameStarted;

    public void StartGame() => OnGameStarted?.Invoke();

    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        _playersAlive = new NetworkVariable<int>(1);
        Invoke(nameof(StartGame), 5f);
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