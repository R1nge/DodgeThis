using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSelection
{
    public class GameSelectionManager : NetworkBehaviour
    {
        [SerializeField] private Transform parent;
        [SerializeField] private GameObject slot;
        
        //On mouse down select game
        //Add to GameSelectionSingleton  Selected games

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                for (int i = 0; i < GameSelectionSingleton.Instance.GetGames().Count; i++)
                {
                    SpawnMinigamesUI(i);
                }
            }
        }

        private void SpawnMinigamesUI(int index)
        {
            SpawnMinigamesUIClientRpc(index);
        }

        [ClientRpc]
        private void SpawnMinigamesUIClientRpc(int index)
        {
            var slotInst = Instantiate(slot, parent);
            slotInst.GetComponent<GameSlotUI>()
                .UpdateSlotInfoServerRpc(GameSelectionSingleton.Instance.GetGamesUI(index));
        }

        public void StartGame()
        {
            var games = GameSelectionSingleton.Instance.GetSelectedGames();
            NetworkManager.Singleton.SceneManager.LoadScene(games[Random.Range(0, games.Count)].SceneName,
                LoadSceneMode.Single);
        }
    }
}