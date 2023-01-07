using UnityEngine;

namespace Lobby
{
    [CreateAssetMenu(fileName = "PlayerSkin", menuName = "PlayerSkin")]
    public class PlayerSkins : ScriptableObject
    {
        [SerializeField] private Skin[] skins;

        public GameObject GetSkin(int skinIndex, int mapIndex) => skins[skinIndex].GetSkinForMap(mapIndex);
    }
}