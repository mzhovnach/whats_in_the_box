using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxList : MonoBehaviour {

    public GameObject BoxListItemPrefab;
    [HideInInspector]
    public List<BoxListItem> BoxItemsList;

    public void UpdateList(CollectionData collection)
    {
        StartCoroutine(UpdateListCo(collection));
    }

    private IEnumerator UpdateListCo(CollectionData collection)
    {
        BoxItemsList = new List<BoxListItem>();
        foreach(Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        yield return new WaitForEndOfFrame();
        foreach(var box in collection.boxes)
        {
            if (box.box_index > 0)
            {
                GameObject boxObj = Instantiate(BoxListItemPrefab, transform);
                BoxListItem boxListItem = boxObj.GetComponent<BoxListItem>();
                boxListItem.InitWithData(box);
                BoxItemsList.Add(boxListItem);
            }
        }
    }
}
