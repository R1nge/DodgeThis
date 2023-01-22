using Shared;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace FallingEyeBalls
{
    public class EyeBallsManagerUI : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI colorText;

        [ServerRpc]
        public void UpdateTextServerRpc(NetworkString str)
        {
            colorText.text = str;
            UpdateTextClientRpc(str);
        }

        [ClientRpc]
        private void UpdateTextClientRpc(NetworkString str) => colorText.text = str;
    }
}