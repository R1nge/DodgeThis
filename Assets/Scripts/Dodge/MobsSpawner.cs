using Unity.Netcode;
using UnityEngine;

namespace Dodge
{
    public class MobsSpawner : NetworkBehaviour
    {
        [SerializeField] private int minWaveSize, maxWaveSize;
        [SerializeField] private Vector3 minSpawnPos, maxSpawnPos;
        [SerializeField] private GameObject mob, bigMob;
        private GameState _gameState;

        private void Awake()
        {
            _gameState = FindObjectOfType<GameState>();
            _gameState.OnGameStarted += OnGameStarted;
        }

        private void OnGameStarted()
        {
            if (!IsServer) return;
            SpawnWave();
        }

        private void SpawnWave()
        {
            int waveSize = Random.Range(minWaveSize, maxWaveSize + 1);
            for (int i = 0; i < waveSize; i++)
            {
                int mobSize = Random.Range(0, 2);
                var pos = new Vector3(
                    Random.Range(minSpawnPos.x, maxSpawnPos.x),
                    Random.Range(minSpawnPos.y, maxSpawnPos.y),
                    Random.Range(minSpawnPos.z, maxSpawnPos.z)
                );

                if (IsBig(mobSize))
                {
                    Spawn(bigMob, pos, Quaternion.identity);
                }
                else
                {
                    Spawn(mob, pos, Quaternion.identity);
                }
            }

            Invoke(nameof(SpawnWave), 10f);
        }

        private void Spawn(GameObject go, Vector3 pos, Quaternion rot)
        {
            var inst = Instantiate(go, pos, rot);
            if (inst.TryGetComponent(out NetworkObject networkObject))
            {
                networkObject.Spawn(true);
            }
        }

        private bool IsBig(int value) => value == 1;
    }
}