using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace FallingEyeBalls
{
    public class EyeBallsTimerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        private EyeBallsTimer _timer;

        private void Awake()
        {
            _timer = GetComponent<EyeBallsTimer>();
            _timer.OnTimeChangedEvent += UpdateUIServerRpc;
        }

        [ServerRpc]
        private void UpdateUIServerRpc(int time)
        {
            timerText.text = time.ToString();
            UpdateUIClientRpc(time);
        }

        [ClientRpc]
        private void UpdateUIClientRpc(int time) => timerText.text = time.ToString();

        private void OnDestroy() => _timer.OnTimeChangedEvent -= UpdateUIServerRpc;
    }
}