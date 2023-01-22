using Shared;
using Unity.Netcode;
using UnityEngine;

namespace FallingEyeBalls
{
    public class EyeBallsSpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject redEyeBall;
        [SerializeField] private GameObject greenEyeBall;
        [SerializeField] private GameObject blueEyeBall;

        private int _redEyeBallsCount;
        private int _greenEyeBallsCount;
        private int _blueEyeBallsCount;

        private GameState _gameState;

        public int GetRedEyeBallsCount() => _redEyeBallsCount;

        public int GetGreenEyeBallsCount() => _greenEyeBallsCount;

        public int GetBlueEyeBallsCount() => _blueEyeBallsCount;

        private void Awake()
        {
            _gameState = FindObjectOfType<GameState>();
            _gameState.OnGameStarted += OnGameStarted;
        }

        private void OnGameStarted()
        {
            if (!IsServer) return;
            var amount = Random.Range(15, 30);
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
            _redEyeBallsCount++;
            Spawn(redEyeBall);
        }

        private void SpawnGreenEyeBall()
        {
            _greenEyeBallsCount++;
            Spawn(greenEyeBall);
        }

        private void SpawnBlueEyeBall()
        {
            _blueEyeBallsCount++;
            Spawn(blueEyeBall);
        }


        private void Spawn(GameObject go)
        {
            //TODO: add spawn position
            var inst = Instantiate(go);
            inst.GetComponent<NetworkObject>().Spawn(true);
        }


        public override void OnDestroy()
        {
            base.OnDestroy();
            _gameState.OnGameStarted -= OnGameStarted;
        }
    }
}