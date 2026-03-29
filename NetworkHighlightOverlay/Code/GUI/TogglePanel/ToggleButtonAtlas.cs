using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using NetworkHighlightOverlay.Utility;
using UnityEngine;

namespace NetworkHighlightOverlay.GUI.TogglePanel
{
    public sealed class ToggleButtonAtlas : IDisposable
    {
        private const string TextureFileName = "ToggleButton.png";
        private const string AtlasName = "NHO_ToggleButtonAtlas";

        public const string InactiveSpriteName = "NHO_ToggleInactive";
        public const string ActiveSpriteName = "NHO_ToggleActive";
        public const string HoveredSpriteName = "NHO_ToggleHovered";
        public const string PressedSpriteName = "NHO_TogglePressed";

        private UITextureAtlas _atlas;

        public UITextureAtlas GetOrCreate()
        {
            if (_atlas != null)
                return _atlas;

            Texture2D texture = ModResources.GetTexture(TextureFileName);
            float spriteWidth = texture.width / 4f;
            float spriteHeight = texture.height;

            IDictionary<string, Rect> spritePixels = new Dictionary<string, Rect>(4);
            spritePixels.Add(InactiveSpriteName, new Rect(0f, 0f, spriteWidth, spriteHeight));
            spritePixels.Add(ActiveSpriteName, new Rect(spriteWidth, 0f, spriteWidth, spriteHeight));
            spritePixels.Add(HoveredSpriteName, new Rect(spriteWidth * 2f, 0f, spriteWidth, spriteHeight));
            spritePixels.Add(PressedSpriteName, new Rect(spriteWidth * 3f, 0f, spriteWidth, spriteHeight));

            _atlas = SpriteAtlasExtractor.CreateAtlas(AtlasName, texture, spritePixels);
            return _atlas;
        }

        public void Dispose()
        {
            UITextureAtlas atlas = _atlas;
            _atlas = null;

            if (atlas == null)
                return;

            if (atlas.material != null)
            {
                UnityEngine.Object.Destroy(atlas.material);
            }

            UnityEngine.Object.Destroy(atlas);
        }
    }
}
