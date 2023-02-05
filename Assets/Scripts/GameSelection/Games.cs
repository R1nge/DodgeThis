using System;

namespace GameSelection
{
    [Serializable]
    public struct Games
    {
        public string Title;
        public string SceneName;
        public bool IsSelected;
        public bool HasBeenPlayed;

        public Games(string title, string sceneName, bool isSelected, bool hasBeenPlayed)
        {
            Title = title;
            SceneName = sceneName;
            IsSelected = isSelected;
            HasBeenPlayed = hasBeenPlayed;
        }
    }
}