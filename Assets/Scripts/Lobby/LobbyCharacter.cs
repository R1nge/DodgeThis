﻿using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Lobby
{
    public class LobbyCharacter : NetworkBehaviour
    {
        [SerializeField] private Skin skins;
        [SerializeField] private TextMeshProUGUI ready, nickname;

        [ServerRpc(RequireOwnership = false)]
        public void UpdateNicknameServerRpc(NetworkString s)
        {
            nickname.text = s;
            UpdateNicknameClientRpc(s);
        }

        [ClientRpc]
        private void UpdateNicknameClientRpc(NetworkString s)
        {
            nickname.text = s;
        }

        [ServerRpc(RequireOwnership = false)]
        public void UpdateSkinServerRpc(int index)
        {
            if (!IsServer) return;
            if (transform.childCount == 1 || transform.childCount == 2)
            {
                var newSkin = Instantiate(skins.GetSkin(index), transform.position + skins.GetOffset(index),
                    Quaternion.Euler(new Vector3(0, 180, 0)));
                newSkin.GetComponent<NetworkObject>().Spawn(true);
                newSkin.GetComponent<NetworkObject>().DontDestroyWithOwner = false;
                newSkin.transform.parent = transform;
            }

            if (transform.childCount == 3)
            {
                transform.GetChild(1).GetComponent<NetworkObject>().Despawn();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ClearUIServerRpc()
        {
            nickname.text = "";
            ready.text = "";
            ClearUIClientRpc();
        }

        [ClientRpc]
        private void ClearUIClientRpc()
        {
            nickname.text = "";
            ready.text = "";
        }

        [ServerRpc(RequireOwnership = false)]
        public void UpdateReadyStateServerRpc(bool state)
        {
            if (state)
            {
                ready.text = "Ready";
            }
            else
            {
                ready.text = "Not Ready";
            }

            UpdateReadyStateClientRpc(state);
        }

        [ClientRpc]
        private void UpdateReadyStateClientRpc(bool state)
        {
            if (state)
            {
                ready.text = "Ready";
            }
            else
            {
                ready.text = "Not Ready";
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void HideSkinServerRpc()
        {
            if (transform.childCount == 2 || transform.childCount == 3)
            {
                transform.GetChild(1).GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}