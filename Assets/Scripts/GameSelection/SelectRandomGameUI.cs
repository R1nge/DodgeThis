using Shared;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace GameSelection
{
    public class SelectRandomGameUI : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI title, description;
        [SerializeField] private TextMeshProUGUI[] score;

        public void UpdateUI(GamesUI UI)
        {
            if (IsServer)
            {
                title.text = UI.title;
                UpdateUIClientRpc(UI.title);
                UpdateScore();
            }
        }

        [ClientRpc]
        private void UpdateUIClientRpc(string str) => title.text = str;

        private void UpdateScore()
        {
            if (IsServer)
            {
                var players = LobbySingleton.Instance.GetPlayersList();
                for (int i = 0; i < players.Count; i++)
                {
                    var scoreStr = players[i].Score.ToString();
                    score[i].text = scoreStr;
                    UpdateScoreClientRpc(scoreStr, i);
                }
            }
        }

        [ClientRpc]
        private void UpdateScoreClientRpc(string str, int index)
        {
            score[index].text = str;
        }
    }
}