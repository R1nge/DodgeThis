using System;
using Shared;
using Unity.Netcode;
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
            _gameState.OnGameStarted += PickColor;
            _gameState.OnGameEnded += Validate;

            _eyeColor = (EyeColors)Random.Range(0, Enum.GetValues(typeof(EyeColors)).Length);
        }

        private void PickColor()
        {
            if (!IsServer) return;
            _eyeBallsManagerUI.UpdateTextServerRpc(_eyeColor.ToString());
        }

        private void Validate()
        {
            if (!IsServer) return;
            for (int i = 0; i < _eyeBallsPlayerSpawner.GetPlayerCount(); i++)
            {
                switch (_eyeColor)
                {
                    case EyeColors.Red:
                        if (_eyeBallsSpawner.GetRedEyeBallsCount() == _eyeBallsPlayerSpawner.GetPlayerData(i).CurrentScore)
                        {
                            print("Winner red");
                        }
                        break;
                    case EyeColors.Green:
                        if (_eyeBallsSpawner.GetGreenEyeBallsCount() == _eyeBallsPlayerSpawner.GetPlayerData(i).CurrentScore)
                        {
                            //Win if amount == spawner amount or is the closest to it
                            print("Winner green");
                        }
                        break;
                    case EyeColors.Blue:
                        if (_eyeBallsSpawner.GetBlueEyeBallsCount() == _eyeBallsPlayerSpawner.GetPlayerData(i).CurrentScore)
                        {
                            _gameState.AddScoreServerRpc(10);
                            print(LobbySingleton.Instance.GetPlayersList()[i].Score);
                            print("Winner blue");
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            print("Game ended");
            NetworkManager.Singleton.SceneManager.LoadScene("SelectRandomGame", LoadSceneMode.Single);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _gameState.OnGameStarted -= PickColor;
            _gameState.OnGameEnded -= Validate;
        }
    }
}