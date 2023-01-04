using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "MinigameInstructions", menuName = "MinigameInstructions")]
public class MinigameInstructionsSO : ScriptableObject
{
    public string title;
    public string description;
    public Sprite image;
    public SceneAsset gameMap;
}