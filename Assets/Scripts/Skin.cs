using UnityEngine;

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

    public GameObject GetController(int controllerIndex) => mapsControllers[controllerIndex];

    public GameObject GetSkin(int skinIndex) => skins[skinIndex];
}