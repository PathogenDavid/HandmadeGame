using System;
using UnityEngine;

public class UIBoardItemController : UiItemSlot
{
    public Vector2Int boardPos;
    public GameObject VisualSlot;
    public override ref NestItem ItemSlot => ref Controller.GetBoardItemSlot(boardPos);

    private NestItem CurrentVisual;
    private bool NeedUpdateVisual;

    private void Awake()
    {
        VisualSlot.GetComponent<Renderer>().enabled = false;
        VisualSlot.transform.localScale = Vector3.one;
    }

    private void Update()
    {
        if (!Controller.IsActive)
            return;

        // Check if visual needs updating
        if (CurrentVisual != ItemSlot)
        {
            if (CurrentVisual != null)
                CurrentVisual.gameObject.SetActive(false);
            
            CurrentVisual = ItemSlot;

            if (CurrentVisual != null)
                NeedUpdateVisual = true;
        }
    }

    // This happens in late update because when items are swapped we might have our new item hidden by its old owner
    private void LateUpdate()
    {
        if (NeedUpdateVisual)
        {
            Debug.Assert(CurrentVisual != null);
            Transform transform = CurrentVisual.transform;

            // Special-case Christmas tree to make it upright and bigger than usual
            if (CurrentVisual.IsChristmasTree)
            {
                transform.parent = VisualSlot.transform.parent;
                Vector3 position = VisualSlot.transform.position;
                position.y += 0.5f;
                transform.position = position;
                transform.localRotation = Quaternion.AngleAxis(-12f, Vector3.forward);
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }
            else
            {
                transform.parent = VisualSlot.transform;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;
            }

            CurrentVisual.gameObject.SetActive(true);
            NeedUpdateVisual = false;
        }
    }
}
