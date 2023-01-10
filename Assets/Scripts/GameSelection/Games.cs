using System;

namespace GameSelection
{
    [Serializable]
    public struct Games
    {
        public string SceneName;
        public bool IsSelected;
        public bool HasBeenPlayed;

        public Games(string sceneName, bool isSelected, bool hasBeenPlayed)
        {
            SceneName = sceneName;
            IsSelected = isSelected;
            HasBeenPlayed = hasBeenPlayed;
        }
    }
}