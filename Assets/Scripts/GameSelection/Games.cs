using System;

namespace GameSelection
{
    [Serializable]
    public struct Games
    {
        public string SceneName;
        public bool IsSelected;

        public Games(string sceneName, bool isSelected)
        {
            SceneName = sceneName;
            IsSelected = isSelected;
        }
    }
}