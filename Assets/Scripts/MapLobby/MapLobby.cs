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
            NetworkManager.Singleton.OnClientConnectedCallback += UpdateUI;
            NetworkManager.Singleton.OnClientDisconnectCallback += UpdateUI;
            _mapLobbyUI = FindObjectOfType<MapLobbyUI>();
            _pickedMap = new NetworkVariable<int>();
        }

        private void UpdateUI(ulong obj)
        {
            if (IsServer)
            {
                _players = FindObjectsOfType<LobbyData>().ToList();
                UpdateUIClientRpc(_pickedMap.Value);
                if (_players.Count == 1)
                {
                    _mapLobbyUI.UpdateButton(false, true);
                }
                else
                {
                    _players = FindObjectsOfType<LobbyData>().ToList();
                    if (IsServer)
                    {
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
            }
            else
            {
                SpawnDataServerRpc();
            }

            OnStateUpdated(false);
        }

        private void SpawnDataServer()
        {
            var net = Instantiate(data).GetComponent<NetworkObject>();
            net.SpawnWithOwnership(NetworkManager.LocalClientId, true);
            net.GetComponent<LobbyData>().ReadyServerRpc();
        }

        private void SpawnData(ulong ID)
        {
            var net = Instantiate(data).GetComponent<NetworkObject>();
            net.SpawnWithOwnership(ID, true);
            _players = FindObjectsOfType<LobbyData>().ToList();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnDataServerRpc(ServerRpcParams rpcParams = default) =>
            SpawnData(rpcParams.Receive.SenderClientId);

        [ClientRpc]
        private void UpdateUIClientRpc(int index) => _mapLobbyUI.UpdateUI(minigameInstructions[index]);

        public void StartGame()
        {
            _players = FindObjectsOfType<LobbyData>().ToList();
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
                for (int i = 0; i < _players.Count; i++)
                {
                    if (_players[i].OwnerClientId == NetworkManager.LocalClientId)
                    {
                        _players[i].ReadyServerRpc();
                        _players[i].IsReady().OnValueChanged += (_, newValue) => { OnStateUpdated(newValue); };
                    }
                }
            }
        }

        private void OnStateUpdated(bool state)
        {
            if (IsServer)
            {
                var players = FindObjectsOfType<LobbyData>();
                if (players.Length == 1)
                {
                    _mapLobbyUI.UpdateButton(false, true);
                    return;
                }

                if (players.All(players => players.IsReady().Value))
                {
                    _mapLobbyUI.UpdateButton(true, true);
                }
                else
                {
                    _mapLobbyUI.UpdateButton(false, true);
                }
            }
            else
            {
                OnStateUpdatedServerRpc(state);
                _mapLobbyUI.UpdateButton(state, false);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnStateUpdatedServerRpc(bool state)
        {
            OnStateUpdated(state);
        }

        private void PickMap() => _pickedMap.Value = Random.Range(0, minigameInstructions.Length);

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (NetworkManager.Singleton == null) return;
            NetworkManager.Singleton.OnClientConnectedCallback -= UpdateUI;
            NetworkManager.Singleton.OnClientDisconnectCallback -= UpdateUI;
        }
    }
}