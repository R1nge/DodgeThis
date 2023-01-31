using System;
using System.Collections.Generic;
using Shared;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FallingEyeBalls
{
    public class EyeBallsSpawner : NetworkBehaviour
    {
        [SerializeField] private int minAmount, maxAmount;
        [SerializeField] private GameObject redEyeBall;
        [SerializeField] private GameObject greenEyeBall;
        [SerializeField] private GameObject blueEyeBall;
        private List<int> _eyeBallCount;
        private GameState _gameState;

        public int GetEyeBallCount(int index) => _eyeBallCount[index];

        private void Awake()
        {
            _eyeBallCount = new List<int>(Enum.GetValues(typeof(EyeColors)).Length);
            for (int i = 0; i < _eyeBallCount.Capacity; i++)
            {
                _eyeBallCount.Add(0);
            }

            _gameState = FindObjectOfType<GameState>();
            _gameState.OnGameStarted += OnGameStarted;
        }

        private void OnGameStarted()
        {
            if (!IsServer) return;
            var amount = Random.Range(minAmount, maxAmount);
            for (int i = 0; i < amount; i++)
            {
                var color = Random.Range(0, 3);
                switch (color)
                {
                    case 0:
                        SpawnRedEyeBall();
                        break;
                    case 1:
                        SpawnGreenEyeBall();
                        break;
                    case 2:
                        SpawnBlueEyeBall();
                        break;
                }
            }
        }

        private void SpawnRedEyeBall()
        {
            _eyeBallCount[0]++;
            Spawn(redEyeBall);
        }

        private void SpawnGreenEyeBall()
        {
            _eyeBallCount[1]++;
            Spawn(greenEyeBall);
        }

        private void SpawnBlueEyeBall()
        {
            _eyeBallCount[2]++;
            Spawn(blueEyeBall);
        }

        private void Spawn(GameObject go)
        {
            //TODO: add spawn position
            var angleX = Random.Range(0, 360);
            var angleY = Random.Range(0, 360);
            var angleZ = Random.Range(0, 360);
            var rot = Quaternion.Euler(new Vector3(angleX, angleY, angleZ));

            var posX = Random.Range(-2, 2);
            var posY = Random.Range(7, 10);
            var posZ = Random.Range(7, 9);
            var pos = new Vector3(posX, posY, posZ);
                
            var inst = Instantiate(go,pos,rot);
            inst.GetComponent<NetworkObject>().Spawn(true);
        }


        public override void OnDestroy()
        {
            base.OnDestroy();
            _gameState.OnGameStarted -= OnGameStarted;
        }
    }
}