using Shared;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace FallingEyeBalls
{
    public class EyeBallsPlayerDataUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nickname,amountText;

        [ServerRpc]
        public void UpdateNicknameServerRpc(NetworkString str)
        {
            nickname.text = str;
            UpdateNicknameClientRpc(str);
        }

        [ClientRpc]
        private void UpdateNicknameClientRpc(NetworkString str) => nickname.text = str;

        public void UpdateScore(int value) => amountText.text = value.ToString();

        [ServerRpc]
        public void UpdateScoreServerRpc(int value)
        {
            amountText.text = value.ToString();
            UpdateScoreClientRpc(value);
        }

        [ClientRpc]
        private void UpdateScoreClientRpc(int value) => amountText.text = value.ToString();
    }
}