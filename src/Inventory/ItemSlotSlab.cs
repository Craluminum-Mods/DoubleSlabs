using Vintagestory.API.Common;

namespace DoubleSlabs;

public class ItemSlotSlab : ItemSlot
{
    public ItemSlotSlab(InventoryBase inventory) : base(inventory) { }

    public override int MaxSlotStackSize => 1;

    public override bool CanTakeFrom(ItemSlot sourceSlot, EnumMergePriority priority = EnumMergePriority.AutoMerge)
    {
        return sourceSlot?.Itemstack?.Collectible?.IsSlab() == true;
    }

    public override bool CanHold(ItemSlot sourceSlot)
    {
        return sourceSlot?.Itemstack?.Collectible?.IsSlab() == true;
    }
}