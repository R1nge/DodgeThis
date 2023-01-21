using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace FallingEyeBalls
{
    public class EyeBallsPlayerDataUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI amountText;

        [ServerRpc]
        public void UpdateUIServerRpc(int value)
        {
            amountText.text = value.ToString();
            UpdateUIClientRpc(value);
        }

        private void UpdateUIClientRpc(int value) => amountText.text = value.ToString();
    }
}