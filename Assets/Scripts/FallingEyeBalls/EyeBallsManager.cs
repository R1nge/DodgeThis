using System;
using System.Collections;
using Shared;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace FallingEyeBalls
{
    public class EyeBallsManager : NetworkBehaviour
    {
        private EyeBallsManagerUI _eyeBallsManagerUI;
        private EyeBallsSpawner _eyeBallsSpawner;
        private EyeBallsPlayerSpawner _eyeBallsPlayerSpawner;
        private GameState _gameState;
        private EyeColors _eyeColor;

        private void Awake()
        {
            _eyeBallsManagerUI = GetComponent<EyeBallsManagerUI>();
            _eyeBallsSpawner = FindObjectOfType<EyeBallsSpawner>();
            _eyeBallsPlayerSpawner = FindObjectOfType<EyeBallsPlayerSpawner>();
            _gameState = FindObjectOfType<GameState>();
            _gameState.OnGameEnd += Validate;

            _eyeColor = (EyeColors)Random.Range(0, Enum.GetValues(typeof(EyeColors)).Length);
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            _eyeBallsManagerUI.UpdateTextServerRpc(_eyeColor.ToString());
        }

        private void Validate()
        {
            if (!IsServer) return;
            StartCoroutine(OnGameEnd());
        }

        private IEnumerator OnGameEnd()
        {
            yield return new WaitForSeconds(2);
            var colorsLength = Enum.GetValues(typeof(EyeColors)).Length;

            for (int color = 0; color < colorsLength; color++)
            {
                var eyeColor = Enum.GetValues(typeof(EyeColors)).GetValue(color);
                if (Equals(eyeColor, _eyeColor))
                {
                    for (int player = 0; player < _eyeBallsPlayerSpawner.GetPlayerCount(); player++)
                    {
                        if (_eyeBallsSpawner.GetEyeBallCount(color) ==
                            _eyeBallsPlayerSpawner.GetPlayerData(player).CurrentScore)
                        {
                            _gameState.AddScoreByIndexServerRpc(player, 50);
                            break;
                        }
                    }
                }
            }

            yield return new WaitForSeconds(3f);
            NetworkManager.Singleton.SceneManager.LoadScene("SelectRandomGame", LoadSceneMode.Single);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _gameState.OnGameEnd -= Validate;
        }
    }
}