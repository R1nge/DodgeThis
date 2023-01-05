using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MapLobby
{
    public class MapLobby : NetworkBehaviour
    {
        [SerializeField] private GameObject data;
        [SerializeField] private MinigameInstructionsSO[] minigameInstructions;
        private MapLobbyUI _mapLobbyUI;
        private NetworkVariable<int> _pickedMap;
        private List<LobbyData> _players;

        private void Awake()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            _mapLobbyUI = FindObjectOfType<MapLobbyUI>();
            _pickedMap = new NetworkVariable<int>();
            _players = new List<LobbyData>();
        }

        private void OnClientConnected(ulong obj)
        {
            UpdateUI();
            UpdateButtonUIServer();
            if (!IsServer)
            {
                _mapLobbyUI.UpdateButton(false, false);
            }
        }

        private void OnClientDisconnected(ulong obj)
        {
            if (!IsServer) return;
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].OwnerClientId ==
                    NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(obj).OwnerClientId)
                {
                    _players.RemoveAt(i);
                }
            }

            UpdateButtonUIServer();
        }

        private void UpdateUI()
        {
            if (IsServer)
            {
                UpdateUIClientRpc(_pickedMap.Value);
            }

            _mapLobbyUI.UpdateUI(minigameInstructions[_pickedMap.Value]);
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                PickMap();
                _mapLobbyUI.UpdateUI(minigameInstructions[_pickedMap.Value]);
                UpdateUIClientRpc(_pickedMap.Value);
                SpawnDataServer();
                UpdateButtonUIServer();
            }
            else
            {
                SpawnDataServerRpc();
                UpdateButtonUIServer();
                if (!IsServer)
                {
                    _mapLobbyUI.UpdateButton(false, false);
                }
            }
        }

        private void SpawnDataServer()
        {
            var net = Instantiate(data).GetComponent<NetworkObject>();
            net.SpawnWithOwnership(NetworkManager.LocalClientId, true);
            net.GetComponent<LobbyData>().ReadyServerRpc();
            _players.Add(net.GetComponent<LobbyData>());
        }

        private void SpawnData(ulong ID)
        {
            var net = Instantiate(data).GetComponent<NetworkObject>();
            net.SpawnWithOwnership(ID, true);
            _players.Add(net.GetComponent<LobbyData>());
            net.GetComponent<LobbyData>().IsReady().OnValueChanged += (_, _) => { UpdateButtonUIServer(); };
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnDataServerRpc(ServerRpcParams rpcParams = default) =>
            SpawnData(rpcParams.Receive.SenderClientId);

        [ClientRpc]
        private void UpdateUIClientRpc(int index) => _mapLobbyUI.UpdateUI(minigameInstructions[index]);

        public void StartGame()
        {
            if (IsServer)
            {
                if (_players.All(players => players.IsReady().Value))
                {
                    NetworkManager.Singleton.SceneManager.LoadScene(minigameInstructions[_pickedMap.Value].gameMap.name,
                        LoadSceneMode.Single);
                }
            }
            else
            {
                var players = FindObjectsOfType<LobbyData>();
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].OwnerClientId == NetworkManager.LocalClientId)
                    {
                        players[i].ReadyServerRpc();
                        UpdateButtonUIClient(!players[i].IsReady().Value);
                    }
                }
            }
        }

        private void UpdateButtonUIServer()
        {
            if (IsServer)
            {
                if (_players.Count == 1)
                {
                    _mapLobbyUI.UpdateButton(false, true);
                    return;
                }

                if (_players.All(players => players.IsReady().Value))
                {
                    _mapLobbyUI.UpdateButton(true, true);
                }
                else
                {
                    _mapLobbyUI.UpdateButton(false, true);
                }
            }
        }

        private void UpdateButtonUIClient(bool state)
        {
            var players = FindObjectsOfType<LobbyData>();
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].OwnerClientId == NetworkManager.LocalClientId)
                {
                    _mapLobbyUI.UpdateButton(state, false);
                }
            }
        }

        private void PickMap() => _pickedMap.Value = Random.Range(0, minigameInstructions.Length);

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (NetworkManager.Singleton == null) return;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
}