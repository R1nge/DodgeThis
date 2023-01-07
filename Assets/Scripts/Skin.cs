using UnityEngine;

[CreateAssetMenu(fileName = "Skins", menuName = "Skins")]
public class Skin : ScriptableObject
{
    [SerializeField] private GameObject[] skinsForMaps;
    
    /// <summary>
    ///     0 - Lobby
    ///     1 - Crane
    ///     2 - NumbersFloor
    ///     3 - Dodge
    ///     4 - BallArena
    /// </summary>
    /// <param name="mapIndex"></param>
    /// <returns></returns>

    public GameObject GetSkinForMap(int mapIndex) => skinsForMaps[mapIndex];
}