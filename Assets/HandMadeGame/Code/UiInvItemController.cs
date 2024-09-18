public class UiInvItemController : UiItemSlot
{
    public int invIndex;
    public override ref NestItem ItemSlot => ref Controller.GetInventoryItemSlot(invIndex);
}
