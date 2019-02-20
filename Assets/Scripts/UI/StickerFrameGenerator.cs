using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerFrameGenerator : MonoBehaviour {

    private const int MAX_IN_ROW = 4;
    private const int MAX_IN_COLUMN = 4;

    public GameObject StickerFramePrefab;
    public Vector3 Margin;

	public void GenerateStickers(int fromIndex, int toIndex)
    {
        foreach(Transform t in transform)
        {
            Destroy(t.gameObject);
        }

        Vector3 cursor = Vector3.zero;
        int colCounter = 0;

        int maxToIndex = toIndex;
        int count = (toIndex - fromIndex) + 1;
        int diff = count - (MAX_IN_ROW * MAX_IN_COLUMN);
        if (diff > 0)
        {
            maxToIndex = toIndex - diff;
        }

        for (int i = fromIndex; i <= maxToIndex; ++i)
        {
            colCounter++;
            GameObject sticker = Instantiate(StickerFramePrefab, transform);
            sticker.transform.localPosition = cursor;
            cursor.x += Margin.x;

            StickerFrame stickerFrame = sticker.GetComponent<StickerFrame>();
            stickerFrame.InitWithBoxNumber(i);

            if (colCounter == MAX_IN_ROW)
            {
                cursor.x = 0;
                cursor.y -= Margin.y;
                colCounter = 0;
            }
        }
    }
}
