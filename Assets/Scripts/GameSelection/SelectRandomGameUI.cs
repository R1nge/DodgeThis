using System.Linq;
using Shared;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace GameSelection
{
    public class SelectRandomGameUI : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI title, description;
        [SerializeField] private TextMeshProUGUI[] score, nickname;
        [SerializeField] private GameObject[] playersUI;
        [SerializeField] private Image icon;
        private SelectRandomGame _selectRandomGame;

        private void Awake()
        {
            _selectRandomGame = FindObjectOfType<SelectRandomGame>();
            _selectRandomGame.OnGameSelectedEvent += UpdateUI;
        }

        private void UpdateUI(int index)
        {
            if (IsServer)
            {
                var gameUI = GameSelectionSingleton.Instance.GetGamesUI();
                UpdateTitle(gameUI[index]);
                UpdateIcon(index);
                UpdateDescription(index);
                EnablePlayersUI();
                UpdateScore();
                UpdateNickname();
            }
        }

        private void UpdateTitle(GamesUI UI)
        {
            title.text = UI.title;
            UpdateUIClientRpc(UI.title);
        }

        [ClientRpc]
        private void UpdateUIClientRpc(string str) => title.text = str;

        private void EnablePlayersUI()
        {
            var players = LobbySingleton.Instance.GetPlayersList();
            for (int i = 0; i < players.Count; i++)
            {
                playersUI[i].SetActive(true);
            }

            EnablePlayersUIClientRpc(players.Count);
        }

        [ClientRpc]
        private void EnablePlayersUIClientRpc(int playerCount)
        {
            for (int i = 0; i < playerCount; i++)
            {
                playersUI[i].SetActive(true);
            }
        }

        private void UpdateNickname()
        {
            if (IsServer)
            {
                var players = LobbySingleton.Instance.GetPlayersList();
                var playersByScore = players.OrderByDescending(x => x.Score).ToArray();
                for (int i = 0; i < players.Count; i++)
                {
                    var nick = playersByScore[i].Nickname;
                    nickname[i].text = nick;
                    UpdateNicknameClientRpc(nick, i);
                }
            }
        }

        [ClientRpc]
        private void UpdateNicknameClientRpc(string str, int index)
        {
            nickname[index].text = str;
        }

        private void UpdateScore()
        {
            if (IsServer)
            {
                var players = LobbySingleton.Instance.GetPlayersList();
                var playersByScore = players.OrderByDescending(x => x.Score).ToArray();
                for (int i = 0; i < players.Count; i++)
                {
                    var scoreStr = playersByScore[i].Score.ToString();
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

        private void UpdateIcon(int index)
        {
            UpdateIconClientRpc(index);
        }

        [ClientRpc]
        private void UpdateIconClientRpc(int index)
        {
            var gameUI = GameSelectionSingleton.Instance.GetGamesUI();
            icon.gameObject.SetActive(true);
            icon.sprite = gameUI[index].icon;
        }

        private void UpdateDescription(int index)
        {
            var desctiptionStr = GameSelectionSingleton.Instance.GetGamesUI()[index].description;
            description.text = desctiptionStr;
            UpdateDescriptionClientRpc(desctiptionStr);
        }

        [ClientRpc]
        private void UpdateDescriptionClientRpc(string descriptionStr)
        {
            description.text = descriptionStr;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (_selectRandomGame == null) return;
            _selectRandomGame.OnGameSelectedEvent -= UpdateUI;
        }
    }
}