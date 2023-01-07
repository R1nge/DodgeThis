using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lobby
{
    public class LobbyManager : NetworkBehaviour
    {
        private LobbyUI _lobbyUI;

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
            for (var i = 0; i < LobbySingleton.Instance.GetPlayersList().Count; i++)
            {
                if (LobbySingleton.Instance.GetPlayersList()[i].ClientId == rpcParams.Receive.SenderClientId)
                {
                    LobbySingleton.Instance.GetPlayersList()[i] = new LobbyPlayerState(
                        LobbySingleton.Instance.GetPlayersList()[i].ClientId,
                        LobbySingleton.Instance.GetPlayersList()[i].PlayerName,
                        LobbySingleton.Instance.GetPlayersList()[i].SkinIndex,
                        !LobbySingleton.Instance.GetPlayersList()[i].IsReady
                    );

                    _lobbyUI.UpdateReadyStateServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].IsReady);
                }
            }
            
            OnLobbyPlayersStateChanged();
            PrintData();
        }

        public void ChangeSkin(int index)
        {
            ChangeSkinServerRpc(index);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChangeSkinServerRpc(int skinIndex, ServerRpcParams rpcParams = default)
        {
            for (var i = 0; i < LobbySingleton.Instance.GetPlayersList().Count; i++)
            {
                if (LobbySingleton.Instance.GetPlayersList()[i].ClientId == rpcParams.Receive.SenderClientId)
                {
                    LobbySingleton.Instance.GetPlayersList()[i] = new LobbyPlayerState(
                        LobbySingleton.Instance.GetPlayersList()[i].ClientId,
                        LobbySingleton.Instance.GetPlayersList()[i].PlayerName,
                        skinIndex,
                        LobbySingleton.Instance.GetPlayersList()[i].IsReady
                    );

                    PrintData();

                    if (LobbySingleton.Instance.GetPlayersList().Count > i)
                    {
                        _lobbyUI.UpdateSkinServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].SkinIndex);
                        _lobbyUI.UpdateReadyStateServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].IsReady);
                    }
                }
            }
        }

        private void PrintData()
        {
            for (int i = 0; i < LobbySingleton.Instance.GetPlayersList().Count; i++)
            {
                print("ID: " + LobbySingleton.Instance.GetPlayersList()[i].ClientId);
                print("Is Ready: " + LobbySingleton.Instance.GetPlayersList()[i].IsReady);
                print("Skin Index: " + LobbySingleton.Instance.GetPlayersList()[i].SkinIndex);
                print("Name: " + LobbySingleton.Instance.GetPlayersList()[i].PlayerName);
            }
        }
 
        private void Awake()
        {
            _lobbyUI = GetComponent<LobbyUI>();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        private void OnClientConnected(ulong ID)
        {
            if (!IsServer) return;
            if (ID == 0)
            {
                LobbySingleton.Instance.GetPlayersList().Add(new LobbyPlayerState
                {
                    ClientId = ID,
                    IsReady = true,
                    PlayerName = PlayerPrefs.GetString("Nickname")
                });
            }
            else
            {
                print(LobbySingleton.Instance);
                print(LobbySingleton.Instance.GetPlayersList());
                LobbySingleton.Instance.GetPlayersList().Add(new LobbyPlayerState
                {
                    ClientId = ID,
                    IsReady = false,
                    PlayerName = PlayerPrefs.GetString("Nickname")
                });
            }

            for (int i = 0; i < 4; i++)
            {
                if (LobbySingleton.Instance.GetPlayersList().Count > i)
                {
                    _lobbyUI.UpdateReadyStateServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].IsReady);
                    _lobbyUI.UpdateNicknameServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].PlayerName);
                    OnLobbyPlayersStateChanged();
                }
            }


            PrintData();
        }

        private void OnClientDisconnected(ulong ID)
        {
            if (!IsServer) return;
            for (var i = 0; i < LobbySingleton.Instance.GetPlayersList().Count; i++)
            {
                if (LobbySingleton.Instance.GetPlayersList()[i].ClientId == ID)
                {
                    LobbySingleton.Instance.GetPlayersList().Remove(LobbySingleton.Instance.GetPlayersList()[i]);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (LobbySingleton.Instance.GetPlayersList().Count > i)
                {
                    _lobbyUI.UpdateReadyStateServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].IsReady);
                    _lobbyUI.UpdateNicknameServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].PlayerName);
                    OnLobbyPlayersStateChanged();
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
                if (LobbySingleton.Instance.GetPlayersList().Count > i)
                {
                    _lobbyUI.UpdateReadyStateServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].IsReady);
                    _lobbyUI.UpdateNicknameServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].PlayerName);
                    OnLobbyPlayersStateChanged();
                }
            }
        }

        private void OnLobbyPlayersStateChanged()
        {
            for (var i = 0; i < LobbySingleton.Instance.GetPlayersList().Count; i++)
            {
                _lobbyUI.UpdateReadyStateServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].IsReady);
                _lobbyUI.UpdateNicknameServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].PlayerName);
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
            if (LobbySingleton.Instance.GetPlayersList().Count < 2)
            {
                return false;
            }

            foreach (var player in LobbySingleton.Instance.GetPlayersList())
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

            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }
        }
    }
}