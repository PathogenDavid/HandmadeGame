using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class InventoryObjectOverlay : MonoBehaviour
{
    public Camera Camera;
    private RawImage Image;

    private RenderTexture RenderTexture
    {
        get => Camera.targetTexture;
        set => Image.texture = Camera.targetTexture = value;
    }

    public GameObject Temp;
    public GameObject[] Others;
    public RectTransform[] InventorySlots;
    private float x;

    private void Awake()
    {
        Image = GetComponent<RawImage>();
        Image.texture = Camera.targetTexture;
        Image.color = Color.white;
    }

    private void Update()
    {
        Vector2 size = Image.rectTransform.rect.size;
        Vector2Int targetSize = new(Math.Max(1, (int)size.x), Math.Max(1, (int)size.y));
        if (RenderTexture.width != targetSize.x || RenderTexture.height != targetSize.y)
        {
            RenderTexture newTexture = new(RenderTexture);
            newTexture.width = targetSize.x;
            newTexture.height = targetSize.y;
            RenderTexture.Release();
            RenderTexture = newTexture;
        }

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 5f;
        //mousePosition.x = Screen.width * 0.5f;
        //mousePosition.y = Screen.height * 0.5f;
        Vector3 projectedPosition = Camera.ScreenToWorldPoint(mousePosition);
        Temp.transform.position = projectedPosition;
        //Debug.Log($"{mousePosition} -> {projectedPosition} -- cam size: {Camera.pixelRect} -- rttSize: {targetSize} - size: {size}");

        x += Time.deltaTime * 1f;
        Vector3 pos = mousePosition;
        float angle = x;
        float deltaAngle = (2f * Mathf.PI) / (float)Others.Length;
        float spread = 250f;
        foreach (GameObject other in Others)
        {
            pos.x = mousePosition.x + Mathf.Cos(angle) * spread;
            pos.y = mousePosition.y + Mathf.Sin(angle) * spread;
            other.transform.position = Camera.ScreenToWorldPoint(pos);
            angle += deltaAngle;
        }

        Vector3[] corners = new Vector3[4];
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            RectTransform slot = InventorySlots[i];
            slot.GetWorldCorners(corners);
            Vector3 center = (corners[0] + corners[1] + corners[2] + corners[3]) / 4f;
            pos.x = center.x;
            pos.y = center.y;
            //TODO: Ensure visual children of all nest items are centered within their parent
            Others[i].transform.position = Camera.ScreenToWorldPoint(pos);
        }
    }
}
