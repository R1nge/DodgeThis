using UnityEngine;

namespace Lobby
{
    [CreateAssetMenu(fileName = "PlayerSkin", menuName = "PlayerSkin")]
    public class PlayerSkins : ScriptableObject
    {
        [SerializeField] private GameObject[] skins;

        public GameObject GetSkin(int index) => skins[index];
    }
}