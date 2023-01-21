using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace FallingEyeBalls
{
    public class EyeBallsTimerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;

        [ServerRpc]
        public void UpdateUIServerRpc(int time)
        {
            timerText.text = time.ToString();
            UpdateUIClientRpc(time);
        }

        [ClientRpc]
        private void UpdateUIClientRpc(int time)
        {
            timerText.text = time.ToString();
        }
    }
}