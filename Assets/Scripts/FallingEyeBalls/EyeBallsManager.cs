﻿using System;
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
        private EyeBallsSpawner _eyeBallsSpawner;
        private PlayerSpawnerEyeBalls _playerSpawnerEyeBalls;
        private GameState _gameState;
        private EyeColors _eyeColor;

        public event Action<NetworkString> OnEyeColorChangedEvent;

        private void Awake()
        {
            _eyeBallsSpawner = FindObjectOfType<EyeBallsSpawner>();
            _playerSpawnerEyeBalls = FindObjectOfType<PlayerSpawnerEyeBalls>();
            _gameState = FindObjectOfType<GameState>();
            _gameState.OnGameEnd += Validate;

            _eyeColor = (EyeColors)Random.Range(0, Enum.GetValues(typeof(EyeColors)).Length);
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            OnEyeColorChangedEvent?.Invoke(_eyeColor.ToString());
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
                    for (int player = 0; player < _playerSpawnerEyeBalls.GetPlayerCount(); player++)
                    {
                        if (_eyeBallsSpawner.GetEyeBallCount(color) ==
                            _playerSpawnerEyeBalls.GetPlayerData(player).CurrentScore)
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