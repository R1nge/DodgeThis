using Shared;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace FallingEyeBalls
{
    public class EyeBallsPlayerDataUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nickname,amountText;
        private EyeBallsPlayerData _playerData;

        private void Awake()
        {
            _playerData = GetComponent<EyeBallsPlayerData>();
            _playerData.OnScoreChangedEvent += UpdateScoreServerRpc;
            _playerData.OnNicknameChangedEvent += UpdateNicknameServerRpc;
            _playerData.OnLocalScoreChangedEvent += UpdateScore;
        }

        [ServerRpc]
        private void UpdateNicknameServerRpc(NetworkString str)
        {
            nickname.text = str;
            UpdateNicknameClientRpc(str);
        }

        [ClientRpc]
        private void UpdateNicknameClientRpc(NetworkString str) => nickname.text = str;

        private void UpdateScore(int value) => amountText.text = value.ToString();

        [ServerRpc]
        private void UpdateScoreServerRpc(int value)
        {
            amountText.text = value.ToString();
            UpdateScoreClientRpc(value);
        }

        [ClientRpc]
        private void UpdateScoreClientRpc(int value) => amountText.text = value.ToString();

        private void OnDestroy()
        {
            _playerData.OnScoreChangedEvent -= UpdateScoreServerRpc;
            _playerData.OnNicknameChangedEvent -= UpdateNicknameServerRpc;
            _playerData.OnLocalScoreChangedEvent -= UpdateScore;
        }
    }
}