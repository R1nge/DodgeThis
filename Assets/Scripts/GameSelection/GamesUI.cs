using UnityEngine;

namespace GameSelection
{
    [CreateAssetMenu(fileName = "GamesUI", menuName = "GamesUI")]
    public class GamesUI : ScriptableObject
    {
        public Sprite preview, icon;
        public string title;
        public string description;
        public string sceneName;
    }
}