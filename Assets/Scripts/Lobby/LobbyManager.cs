using System;
using Shared;
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
        private void ReadyUpServerRpc(ServerRpcReceiveParams rpcParams = default)
        {
            for (var i = 0; i < LobbySingleton.Instance.GetPlayersList().Count; i++)
            {
                if (LobbySingleton.Instance.GetPlayersList()[i].ClientId == rpcParams.SenderClientId)
                {
                    LobbySingleton.Instance.GetPlayersList()[i] = new PlayerState(
                        LobbySingleton.Instance.GetPlayersList()[i].ClientId,
                        LobbySingleton.Instance.GetPlayersList()[i].Nickname,
                        LobbySingleton.Instance.GetPlayersList()[i].SkinIndex,
                        !LobbySingleton.Instance.GetPlayersList()[i].IsReady,
                        LobbySingleton.Instance.GetPlayersList()[i].Score,
                        true
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
        private void ChangeSkinServerRpc(int skinIndex, ServerRpcReceiveParams rpcParams = default)
        {
            for (var i = 0; i < LobbySingleton.Instance.GetPlayersList().Count; i++)
            {
                if (LobbySingleton.Instance.GetPlayersList()[i].ClientId == rpcParams.SenderClientId)
                {
                    LobbySingleton.Instance.GetPlayersList()[i] = new PlayerState(
                        LobbySingleton.Instance.GetPlayersList()[i].ClientId,
                        LobbySingleton.Instance.GetPlayersList()[i].Nickname,
                        skinIndex,
                        LobbySingleton.Instance.GetPlayersList()[i].IsReady,
                        LobbySingleton.Instance.GetPlayersList()[i].Score,
                        true
                    );

                    PrintData();

                    OnLobbyPlayersStateChanged();
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
                print("Name: " + LobbySingleton.Instance.GetPlayersList()[i].Nickname);
            }
        }

        private void Awake()
        {
            _lobbyUI = GetComponent<LobbyUI>();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        private void Start() => OnLobbyPlayersStateChanged();

        private void OnClientConnected(ulong ID)
        {
            if (IsServer)
            {
                if (ID == 0)
                {
                    LobbySingleton.Instance.GetPlayersList().Add(new PlayerState
                    {
                        ClientId = ID,
                        SkinIndex = PlayerPrefs.GetInt("Skin"),
                        IsReady = true,
                        Nickname = PlayerPrefs.GetString("Nickname"),
                        IsAlive = true
                    });
                }


                OnLobbyPlayersStateChanged();


                PrintData();
            }
            else
            {
                var skinIndex = PlayerPrefs.GetInt("Skin", 0);
                var nickname = PlayerPrefs.GetString("Nickname");
                OnClientConnectedServerRpc(skinIndex, nickname);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnClientConnectedServerRpc(int skinIndex, NetworkString nickname,
            ServerRpcReceiveParams rpcParams = default)
        {
            LobbySingleton.Instance.GetPlayersList().Add(new PlayerState
            {
                ClientId = rpcParams.SenderClientId,
                SkinIndex = skinIndex,
                IsReady = false,
                Nickname = nickname,
                IsAlive = true
            });

            OnLobbyPlayersStateChanged();
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
            
            OnLobbyPlayersStateChanged();

            for (int i = 0; i < 4; i++)
            {
                if (LobbySingleton.Instance.GetPlayersList().Count <= i)
                {
                    _lobbyUI.HideSkinServerRpc(i);
                    _lobbyUI.ClearUIServerRpc(i);
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

            OnLobbyPlayersStateChanged();
        }

        private void OnLobbyPlayersStateChanged()
        {
            for (var i = 0; i < LobbySingleton.Instance.GetPlayersList().Count; i++)
            {
                _lobbyUI.UpdateSkinServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].SkinIndex);
                _lobbyUI.UpdateReadyStateServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].IsReady);
                _lobbyUI.UpdateNicknameServerRpc(i, LobbySingleton.Instance.GetPlayersList()[i].Nickname);
            }

            _lobbyUI.UpdateStartButtonServerRpc(IsEveryoneReady());
        }

        public void StartGame()
        {
            if (!IsServer) return;
            if (IsEveryoneReady())
            {
                print("Started a game");
                NetworkManager.Singleton.SceneManager.LoadScene("GamesSelection", LoadSceneMode.Single);
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