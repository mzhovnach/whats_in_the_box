using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerFrame : MonoBehaviour
{
    public Sprite[] DigitSprites;
    public SpriteRenderer[] Digits;
    public SpriteRenderer QRCodeImage;

    public void InitWithBoxNumber(int number)
    {
        string numberStr = number.ToString("D4");
        int digitsCount = numberStr.Length;
        int digitIndex = 0;
        foreach (var digit in Digits)
        {
            bool active = digitIndex < digitsCount;
            digit.gameObject.SetActive(active);
            if (active)
            {
                digit.sprite = DigitSprites[(int)Char.GetNumericValue(numberStr[digitIndex])];
            }
            digitIndex++;
        }

        Texture2D boxImage = QRHelper.GenerateQRTexture(numberStr);
        QRCodeImage.sprite = Sprite.Create(boxImage, new Rect(0, 0, boxImage.width, boxImage.height), new Vector2(0.5f, 0.5f));
    }
}
