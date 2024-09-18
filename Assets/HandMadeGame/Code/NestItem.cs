using UnityEngine;

public sealed class NestItem : MonoBehaviour
{
    public bool IsChristmas;
    public bool IsChristmasTree;
    public bool IsGarbage;
    public bool IsSoft;

    public GameObject InventoryItemShadowObject { get; private set; }
    public float InventoryItemScale = 1f;

    private void Awake()
    {
        GameFlow.Instance.AllNestItems.Add(this);

        GameObject inventoryObjectOverlayItems = GameObject.FindWithTag("InventoryObjectOverlayObjects");
        Debug.Assert(inventoryObjectOverlayItems != null);

        GameObject shadowObject = new($"{name} (Inventory)");
        InventoryItemShadowObject = shadowObject;
        shadowObject.SetActive(false);
        shadowObject.transform.parent = inventoryObjectOverlayItems.transform;
        shadowObject.transform.localScale = new Vector3(InventoryItemScale, InventoryItemScale, InventoryItemScale);
        shadowObject.layer = inventoryObjectOverlayItems.layer;

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            GameObject shadowChild = (GameObject)Instantiate(child, shadowObject.transform, instantiateInWorldSpace: false);
            shadowChild.layer = inventoryObjectOverlayItems.layer;
        }
    }
}
