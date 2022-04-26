using UnityEngine;

namespace PhotoViewer.Scripts.Photo
{
    public static class Extensions
    {
        public static Sprite RotateSprite(this Sprite sprite)
        {
            var texture = sprite.texture;
            var newTexture = new Texture2D(texture.height, texture.width, texture.format, false);

            for (var i = 0; i < texture.width; i++)
            {
                for (var j = 0; j < texture.height; j++)
                {
                    newTexture.SetPixel(j, i, texture.GetPixel(texture.width - i, j));
                }
            }

            newTexture.Apply();

            var rect = new Rect(0, 0, texture.height, texture.width);
            return Sprite.Create(newTexture, rect, new Vector2(0.5f, 0.5f), 100);
        }

        public static Sprite DuplicateTexture(this Sprite sprite)
        {
            var texture = sprite.texture;

            var renderTex = RenderTexture.GetTemporary(
                texture.width,
                texture.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);

            Graphics.Blit(texture, renderTex);
            var previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            var readableText = new Texture2D(texture.width, texture.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);

            Rect rect = new Rect(0, 0, texture.width, texture.height);

            return Sprite.Create(readableText, rect, new Vector2(0.5f, 0.5f), 100);
        }
    }
}