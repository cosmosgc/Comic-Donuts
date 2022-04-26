using UnityEngine;

namespace PhotoViewer.Scripts
{
    public static class Extensions
    {
        public static Texture2D TextureFromSprite(this Sprite sprite)
        {
            if(sprite.rect.width != sprite.texture.width){
                Texture2D newText = new Texture2D((int)sprite.rect.width,(int)sprite.rect.height);
                Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x, 
                    (int)sprite.textureRect.y, 
                    (int)sprite.textureRect.width, 
                    (int)sprite.textureRect.height );
                newText.SetPixels(newColors);
                newText.Apply();
                return newText;
            } else
                return sprite.texture;
        }
    }
}