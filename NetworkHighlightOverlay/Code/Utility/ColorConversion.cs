using UnityEngine;

namespace NetworkHighlightOverlay.Utility
{
    public static class ColorConversion
    {
        public static Color FromHue(float hue, float strength)
        {
            Color color = Color.HSVToRGB(hue, 1f, 1f);
            
            color.a = Mathf.Clamp01(strength);

            return color;
        }
    }
}
