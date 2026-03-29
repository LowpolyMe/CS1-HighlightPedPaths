using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NetworkHighlightOverlay.Utility
{
    public static class ModResources
    {
        private const string ResourcePrefix = "NetworkHighlightOverlay.Resources.";
        private static readonly IDictionary<string, Texture2D> _sharedTextures = new Dictionary<string, Texture2D>();

        public static Texture2D GetTexture(string fileName)
        {
            Texture2D texture;
            if (_sharedTextures.TryGetValue(fileName, out texture) && texture != null)
                return texture;

            return _sharedTextures[fileName] = LoadTexture(fileName);
        }

        private static Texture2D LoadTexture(string fileName)
        {
            string error = "Failed to load embedded texture: " + fileName;
            using (Stream stream = typeof(ModResources).Assembly.GetManifestResourceStream(ResourcePrefix + fileName))
            {
                if (stream == null)
                    throw new InvalidOperationException(error);

                using (BinaryReader reader = new BinaryReader(stream))
                {
                    byte[] data = reader.ReadBytes((int)stream.Length);
                    Texture2D texture = new Texture2D(2, 2);
                    if (!texture.LoadImage(data))
                        throw new InvalidOperationException(error);

                    texture.wrapMode = TextureWrapMode.Clamp;
                    return texture;
                }
            }
        }
    }
}
