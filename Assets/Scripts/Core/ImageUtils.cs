using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageUtils {

	public static Sprite CreateSpriteFromBytes(byte[] rawImage)
    {
        Texture2D text = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        text.LoadImage(rawImage);

        return Sprite.Create(text, new Rect(0, 0, text.width, text.height), new Vector2(.5f, .5f));
    }
}
