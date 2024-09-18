using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class InventoryHotBarController : MonoBehaviour
{
    private GameFlow GameFlow => GameFlow.Instance;

    public RawImage OverlayImage;
    public Camera OverlayCamera;
    public float OverlayItemDepth = 5f;

    public RectTransform[] InventorySlots;

    public float SpinAngle = 0f;
    public float SpinSpeed = 50f;

    private RenderTexture OverlayTexture
    {
        get => OverlayCamera.targetTexture;
        set => OverlayImage.texture = OverlayCamera.targetTexture = value;
    }

    private void Awake()
    {
        OverlayImage.texture = OverlayCamera.targetTexture;
        OverlayImage.color = Color.white;
        Debug.Assert(InventorySlots.Length == GameFlow.Inventory.Length, $"{nameof(InventoryHotBarController)} does not have the same number of slots as {nameof(GameFlow)}!");
    }

    private HashSet<NestItem> _VisibleItems = new();
    private Vector3[] _Corners = new Vector3[4];
    private void Update()
    {
        // Resize render target if necessary
        Vector2 size = OverlayImage.rectTransform.rect.size;
        Vector2Int targetSize = new(Math.Max(1, (int)size.x), Math.Max(1, (int)size.y));
        if (OverlayTexture.width != targetSize.x || OverlayTexture.height != targetSize.y)
        {
            RenderTexture newTexture = new(OverlayTexture);
            newTexture.width = targetSize.x;
            newTexture.height = targetSize.y;
            OverlayTexture.Release();
            OverlayTexture = newTexture;
        }

        // Update inventory item visuals
        SpinAngle += Time.deltaTime * SpinSpeed;
        Quaternion rotation = Quaternion.Euler(0f, 180f, -20f) * Quaternion.AngleAxis(SpinAngle, Vector3.up);

        _VisibleItems.Clear();
        for (int i = 0; i < GameFlow.Inventory.Length; i++)
        {
            NestItem item = GameFlow.Inventory[i];
            if (item == null)
                continue;

            RectTransform slot = InventorySlots[i];
            slot.GetWorldCorners(_Corners);
            Vector3 center = (_Corners[0] + _Corners[1] + _Corners[2] + _Corners[3]) * 0.25f;

            Vector3 screenPosition = new(center.x, center.y, OverlayItemDepth);
            GameObject inventoryShadow = item.InventoryItemShadowObject;
            inventoryShadow.transform.position = OverlayCamera.ScreenToWorldPoint(screenPosition);
            inventoryShadow.transform.rotation = rotation;
            inventoryShadow.SetActive(true);
            _VisibleItems.Add(item);
        }

        // Hide items which are no longer visible
        foreach (NestItem item in GameFlow.AllNestItems)
        {
            if (!_VisibleItems.Contains(item))
                item.InventoryItemShadowObject.SetActive(false);
        }
    }
}
