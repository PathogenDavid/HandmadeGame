using System;
using UnityEngine;

public sealed class CursorController : MonoBehaviour
{
    private static CursorController _Instance;
    public static CursorController Instance
    {
        get
        {
            if (_Instance is not null)
                return _Instance;

            return _Instance = FindObjectOfType<CursorController>();
        }
    }

    public Texture2D DefaultCursor;
    public Vector2 DefaultCursorHotSpot;

    public Texture2D PointCursor;
    public Vector2 PointCursorHotSpot;

    public Texture2D OpenHandCursor;
    public Vector2 OpenHandCursorHotSpot;

    public Texture2D GrabbyHandCursor;
    public Vector2 GrabbyHandCursorHotSpot;

    private void Awake()
    {
        UiController.UiInteractionStart += () =>
        {
            SetCursor(CursorKind.Default);
            Cursor.visible = true;
        };

        UiController.UiInteractionEnd += () => Cursor.visible = false;
        Cursor.visible = false;
    }

    public void SetCursor(CursorKind cursor)
    {
        (Texture2D texture, Vector2 hotspot) = cursor switch
        {
            CursorKind.Default => (DefaultCursor, DefaultCursorHotSpot),
            CursorKind.Point => (PointCursor, PointCursorHotSpot),
            CursorKind.OpenHand => (OpenHandCursor, OpenHandCursorHotSpot),
            CursorKind.GrabbyHand => (GrabbyHandCursor, GrabbyHandCursorHotSpot),
            _ => throw new ArgumentException("Invalid cursor kind.", nameof(cursor))
        };
        Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
    }
}
