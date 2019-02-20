using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureRotate
{

    static Color32[] colorResult;

    public enum Rotate
    {
        Left,
        Right
    }

    public static void RotateTexture(Texture2D source, Rotate rotation)
    {
        Color32[] colorSource = source.GetPixels32();
        colorResult = new Color32[colorSource.Length];

        int count = 0;
        int newWidth = source.height;
        int newHeight = source.width;
        int index = 0;

        for (int i = 0; i < source.width; i++)
        {
            for (int j = 0; j < source.height; j++)
            {
                if (rotation == Rotate.Left)
                    index = (source.width * (source.height - j)) - source.width + i;
                else
                    index = (source.width * (j + 1)) - (i + 1);

                colorResult[count] = colorSource[index];
                count++;
            }
        }

        source.Resize(newWidth, newHeight);
        source.SetPixels32(colorResult);
        source.Apply();

        colorResult = null;
    }
}