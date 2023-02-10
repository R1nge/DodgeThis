using Shared;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace FallingEyeBalls
{
    public class EyeBallsManagerUI : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI colorText;
        private EyeBallsManager _eyeBallsManager;

        private void Awake()
        {
            _eyeBallsManager = FindObjectOfType<EyeBallsManager>();
            _eyeBallsManager.OnEyeColorChangedEvent += UpdateTextServerRpc;
        }

        [ServerRpc]
        private void UpdateTextServerRpc(NetworkString str)
        {
            colorText.text = str;
            UpdateTextClientRpc(str);
        }

        [ClientRpc]
        private void UpdateTextClientRpc(NetworkString str) => colorText.text = str;
    }
}