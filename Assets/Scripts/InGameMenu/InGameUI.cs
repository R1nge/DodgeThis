using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InGameMenu
{
    public class InGameUI : MonoBehaviour
    {
        private static InGameUI Instance { get; set; }

        [SerializeField] private GameObject inGameUI;

        private void Awake()
        {
            if (Instance != null)
            {
                throw new Exception("Multiple LobbySingleton defined!");
            }

            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        private void Update()
        {
            if (!NetworkManager.Singleton.IsListening) return;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (inGameUI.activeInHierarchy)
                {
                    Close();
                }
                else
                {
                    Open();
                }
            }
        }

        private void Open() => inGameUI.SetActive(true);

        public void Close() => inGameUI.SetActive(false);

        public void Disconnect()
        {
            var netManager = NetworkManager.Singleton;
            netManager.Shutdown();
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            Close();
        }
    }
}