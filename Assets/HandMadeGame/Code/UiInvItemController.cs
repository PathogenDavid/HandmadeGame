public class UiInvItemController : UiItemSlot
{
    public int invIndex;
    public override ref ArrangementController.DummyItem ItemSlot => ref ac.GetInventoryItemSlot(invIndex);
}
