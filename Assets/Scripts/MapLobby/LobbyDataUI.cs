using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace MapLobby
{
    public class LobbyDataUI : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI nickname;
        private NetworkVariable<NetworkString> _networkStr;

        private void Awake()
        {
            _networkStr = new NetworkVariable<NetworkString>();
            _networkStr.OnValueChanged += OnValueChanged;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }

        private void OnClientConnected(ulong obj) => UpdateUIClientRpc(_networkStr.Value);

        private void OnValueChanged(NetworkString _, NetworkString newValue)
        {
            nickname.text = newValue.ToString();
            if (IsServer)
            {
                UpdateUIClientRpc(newValue);
            }
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            if (IsServer)
            {
                _networkStr.Value = PlayerPrefs.GetString("Nickname");
                UpdateUIClientRpc(_networkStr.Value);
            }
            else
            {
                SetNicknameServerRpc(PlayerPrefs.GetString("Nickname"));
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetNicknameServerRpc(string s)
        {
            _networkStr.Value = s;
            UpdateUIClientRpc(s);
        }

        [ClientRpc]
        private void UpdateUIClientRpc(NetworkString value) => nickname.text = value.ToString();

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (NetworkManager.Singleton == null) return;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }
}