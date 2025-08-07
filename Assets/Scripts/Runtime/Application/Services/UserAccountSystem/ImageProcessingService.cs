using System;
using UnityEngine;

namespace Runtime.Application.UserAccountSystem
{
    public class ImageProcessingService
    {
        public string ConvertToBase64(Sprite sprite, int maxSize)
        {
            var readableTexture = ProcessTexture(sprite.texture, maxSize);
            return Convert.ToBase64String(readableTexture.EncodeToPNG());
        }

        public Sprite CreateAvatarSprite(string avatarBase64)
        {
            var imageData = Convert.FromBase64String(avatarBase64);
            var texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        private Texture2D ProcessTexture(Texture2D source, int maxSize)
        {
            var newWidth = source.width;
            var newHeight = source.height;

            if (maxSize > 0)
            {
                var scale = Mathf.Min((float)maxSize / source.width, (float)maxSize / source.height);
                newWidth = Mathf.RoundToInt(source.width * scale);
                newHeight = Mathf.RoundToInt(source.height * scale);
            }

            var renderTex = RenderTexture.GetTemporary(newWidth, newHeight, 0, RenderTextureFormat.Default,
                RenderTextureReadWrite.sRGB);
            Graphics.Blit(source, renderTex);

            var newTexture = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);
            var previous = RenderTexture.active;
            RenderTexture.active = renderTex;

            newTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
            newTexture.Apply();

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);

            return newTexture;
        }
    }
}