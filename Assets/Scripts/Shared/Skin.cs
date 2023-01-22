using UnityEngine;

namespace Shared
{
    [CreateAssetMenu(fileName = "Skins", menuName = "Skins")]
    public class Skin : ScriptableObject
    {
        /// <summary>
        /// Controllers
        /// 0 - FpsCharacterCCWithJump
        /// 1 - FpsCharacterCCWithOutJump
        /// 2 - TpsCharacterCCWithJump
        /// 3 - TpsCharacterCCWithOutJump
        /// 4 - TpsCharacterRB
        /// </summary>
        [SerializeField] private GameObject[] mapsControllers;
        [SerializeField] private GameObject[] skins;
        [SerializeField] private Vector3[] skinLobbyOffset;

        public GameObject GetController(int controllerIndex) => mapsControllers[controllerIndex];

        public GameObject GetSkin(int skinIndex) => skins[skinIndex];

        public Vector3 GetOffset(int offsetIndex) => skinLobbyOffset[offsetIndex];
    }
}