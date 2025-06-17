using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    private void Start()
    {
        SetCursor();
    }

    public void SetCursor()
    {
        Vector2 hotspot = new Vector2(cursorTexture.width / 2f, cursorTexture.height / 2f);
        Cursor.SetCursor(cursorTexture, hotspot, cursorMode);
    }

    public void ResetToDefault()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}
