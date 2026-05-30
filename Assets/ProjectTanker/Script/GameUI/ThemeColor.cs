using UnityEngine;

namespace ProjectTanker.UI
{
    [CreateAssetMenu(fileName = "ThemeColor", menuName = "ProjectTanker/ThemeColor")]
    public class ThemeColor : ScriptableObject
    {
        [Header("背景・パネル")]
        public Color background  = new Color(0.941f, 0.929f, 0.910f); // #F0EDE8
        public Color panel       = new Color(0.914f, 0.898f, 0.875f); // #E9E5DF
        public Color border      = new Color(0.812f, 0.788f, 0.761f); // #CFC9C2

        [Header("テキスト")]
        public Color textPrimary   = new Color(0.239f, 0.220f, 0.200f); // #3D3833
        public Color textSecondary = new Color(0.478f, 0.463f, 0.435f); // #7A766F

        [Header("ボタン")]
        public Color buttonDefault  = new Color(0.867f, 0.847f, 0.820f); // #DDD8D1
        public Color buttonAccent   = new Color(0.710f, 0.686f, 0.655f); // #B5AFA7

        [Header("属性アクセント")]
        public Color earth = new Color(0.486f, 0.722f, 0.494f); // #7CB87E
        public Color water = new Color(0.482f, 0.686f, 0.831f); // #7BAFD4
        public Color fire  = new Color(0.878f, 0.478f, 0.373f); // #E07A5F
        public Color wind  = new Color(0.949f, 0.800f, 0.416f); // #F2CC6A
        public Color none  = new Color(0.690f, 0.671f, 0.639f); // #B0ABA3
    }
}
