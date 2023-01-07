using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lobby
{
    public class LobbyManager : NetworkBehaviour
    {
        private LobbyUI _lobbyUI;
        private NetworkList<LobbyPlayerState> _lobbyPlayers;

        public void ReadyUp()
        {
            if (!IsServer)
            {
                ReadyUpServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void ReadyUpServerRpc(ServerRpcParams rpcParams = default)
        {
            for (var i = 0; i < _lobbyPlayers.Count; i++)
            {
                if (_lobbyPlayers[i].ClientId == rpcParams.Receive.SenderClientId)
                {
                    _lobbyPlayers[i] = new LobbyPlayerState(
                        _lobbyPlayers[i].ClientId,
                        _lobbyPlayers[i].PlayerName,
                        _lobbyPlayers[i].SkinIndex,
                        !_lobbyPlayers[i].IsReady
                    );

                    _lobbyUI.UpdateReadyStateServerRpc(i, _lobbyPlayers[i].IsReady);
                }
            }

            PrintData();
        }

        public void ChangeSkin(int index)
        {
            ChangeSkinServerRpc(index);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChangeSkinServerRpc(int skinIndex, ServerRpcParams rpcParams = default)
        {
            for (var i = 0; i < _lobbyPlayers.Count; i++)
            {
                if (_lobbyPlayers[i].ClientId == rpcParams.Receive.SenderClientId)
                {
                    _lobbyPlayers[i] = new LobbyPlayerState(
                        _lobbyPlayers[i].ClientId,
                        _lobbyPlayers[i].PlayerName,
                        skinIndex,
                        _lobbyPlayers[i].IsReady
                    );

                    PrintData();

                    if (_lobbyPlayers.Count > i)
                    {
                        _lobbyUI.UpdateSkinServerRpc(i, _lobbyPlayers[i].SkinIndex);
                        _lobbyUI.UpdateReadyStateServerRpc(i, _lobbyPlayers[i].IsReady);
                    }
                }
            }
        }

        private void PrintData()
        {
            for (int i = 0; i < _lobbyPlayers.Count; i++)
            {
                print("ID: " + _lobbyPlayers[i].ClientId);
                print("Is Ready: " + _lobbyPlayers[i].IsReady);
                print("Skin Index: " + _lobbyPlayers[i].SkinIndex);
                print("Name: " + _lobbyPlayers[i].PlayerName);
            }
        }

        private void Awake()
        {
            _lobbyUI = GetComponent<LobbyUI>();
            _lobbyPlayers = new NetworkList<LobbyPlayerState>();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        private void OnClientConnected(ulong ID)
        {
            if (!IsServer) return;
            if (ID == 0)
            {
                _lobbyPlayers.Add(new LobbyPlayerState
                {
                    ClientId = ID,
                    IsReady = true,
                    PlayerName = PlayerPrefs.GetString("Nickname")
                });
            }
            else
            {
                _lobbyPlayers.Add(new LobbyPlayerState
                {
                    ClientId = ID,
                    IsReady = false,
                    PlayerName = PlayerPrefs.GetString("Nickname")
                });
            }

            for (int i = 0; i < 4; i++)
            {
                if (_lobbyPlayers.Count > i)
                {
                    _lobbyUI.UpdateReadyStateServerRpc(i, _lobbyPlayers[i].IsReady);
                    _lobbyUI.UpdateNicknameServerRpc(i, _lobbyPlayers[i].PlayerName);
                }
            }


            PrintData();
        }

        private void OnClientDisconnected(ulong ID)
        {
            if (!IsServer) return;
            for (var i = 0; i < _lobbyPlayers.Count; i++)
            {
                if (_lobbyPlayers[i].ClientId == ID)
                {
                    _lobbyPlayers.Remove(_lobbyPlayers[i]);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (_lobbyPlayers.Count > i)
                {
                    _lobbyUI.UpdateReadyStateServerRpc(i, _lobbyPlayers[i].IsReady);
                    _lobbyUI.UpdateNicknameServerRpc(i, _lobbyPlayers[i].PlayerName);
                }
            }

            PrintData();
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    OnClientConnected(client.ClientId);
                }

                _lobbyUI.UpdateStartButtonServerRpc(IsEveryoneReady());
            }

            for (int i = 0; i < 4; i++)
            {
                if (_lobbyPlayers.Count > i)
                {
                    _lobbyUI.UpdateReadyStateServerRpc(i, _lobbyPlayers[i].IsReady);
                    _lobbyUI.UpdateNicknameServerRpc(i, _lobbyPlayers[i].PlayerName);
                }
            }

            _lobbyPlayers.OnListChanged += OnLobbyPlayersStateChanged;
        }

        private void OnLobbyPlayersStateChanged(NetworkListEvent<LobbyPlayerState> changeevent)
        {
            for (var i = 0; i < _lobbyPlayers.Count; i++)
            {
                _lobbyUI.UpdateReadyStateServerRpc(i, _lobbyPlayers[i].IsReady);
                _lobbyUI.UpdateNicknameServerRpc(i, _lobbyPlayers[i].PlayerName);
            }

            _lobbyUI.UpdateStartButtonServerRpc(IsEveryoneReady());
        }

        public void StartGame()
        {
            if (!IsServer) return;
            if (IsEveryoneReady())
            {
                print("Started a game");
                NetworkManager.Singleton.SceneManager.LoadScene("Dodge", LoadSceneMode.Single);
            }
        }

        private bool IsEveryoneReady()
        {
            if (_lobbyPlayers.Count < 2)
            {
                return false;
            }

            foreach (var player in _lobbyPlayers)
            {
                if (!player.IsReady)
                {
                    return false;
                }
            }

            return true;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _lobbyPlayers.OnListChanged -= OnLobbyPlayersStateChanged;

            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }
        }
    }
}