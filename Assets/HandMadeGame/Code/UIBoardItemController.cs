using UnityEngine;

public class UIBoardItemController : UiItemSlot
{
    public Vector2Int boardPos;
    public override ref ArrangementController.DummyItem ItemSlot => ref ac.GetBoardItemSlot(boardPos);
}
