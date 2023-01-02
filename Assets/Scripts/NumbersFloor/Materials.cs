using UnityEngine;

namespace NumbersFloor
{
    [CreateAssetMenu(fileName = "materials", menuName = "NumbersFloor/Materials")]
    public class Materials : ScriptableObject
    {
        [SerializeField] private Material[] materials;

        public readonly Color Purple = new(111f / 255, 83f / 255, 171f / 255);
        public readonly Color DarkBlue = new(101f / 255, 155f / 255, 218f / 255);
        public readonly Color Blue = new(41f / 255, 151f / 255, 194f / 255);
        public readonly Color LightBlue = new(41f / 255, 183f / 255, 206f / 255);
        public readonly Color Emerald = new(49f / 255, 198f / 255, 170f / 255);
        public readonly Color LightGreen = new(115f / 255, 235f / 255, 139f / 255);
        public readonly Color YellowGreen = new(162f / 255, 187f / 255, 29f / 255);
        public readonly Color Orange = new(196f / 255, 161f / 255, 39f / 255);
        public readonly Color DarkOrange = new(199f / 255, 125f / 255, 56f / 255);
        public readonly Color Red = new(199f / 255, 50f / 255, 47f / 255);

        public Material GetMaterial(int index) => materials[index];
    }
}