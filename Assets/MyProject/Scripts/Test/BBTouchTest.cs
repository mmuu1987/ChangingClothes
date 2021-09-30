using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BBTouchTest : MonoBehaviour
{
    List<RaycastResult> list;

    public RectTransform MouseImage;

    void Start()
    {
        list = new List<RaycastResult>();
    }
    void Update()
    {
        
            list = GraphicRaycaster(MouseImage.anchoredPosition);
            foreach (var item in list)
            {
                Debug.Log(item);
            }
        
    }
   
    public EventSystem _mEventSystem;
    public GraphicRaycaster gra;
    private List<RaycastResult> GraphicRaycaster(Vector2 pos)
    {
        var mPointerEventData = new PointerEventData(_mEventSystem);
        mPointerEventData.position = pos;
        List<RaycastResult> results = new List<RaycastResult>();

        gra.Raycast(mPointerEventData, results);
        return results;
    }

}