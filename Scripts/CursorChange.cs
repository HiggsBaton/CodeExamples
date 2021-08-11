using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorChange : MonoBehaviour
{
    [SerializeField] Texture2D cursorTexture;
    [SerializeField] CursorMode cursorMode = CursorMode.Auto;
    [SerializeField] Vector2 hotSpot = Vector2.zero;
    
    void OnMouseEnter()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}
