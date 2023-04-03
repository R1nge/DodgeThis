using UnityEngine;

namespace Shared
{
    [CreateAssetMenu(fileName = "Skins", menuName = "Skins")]
    public class Skin : ScriptableObject
    {
        [SerializeField] private GameObject[] skins;
        [SerializeField] private Vector3[] skinLobbyOffset;

        public GameObject GetSkin(int skinIndex) => skins[skinIndex];

        public Vector3 GetOffset(int offsetIndex) => skinLobbyOffset[offsetIndex];
    }
}