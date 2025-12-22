using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace DoubleSlabs;

public class BlockBehaviorDoubleSlab(Block block) : BlockBehavior(block)
{
    public override void OnBlockPlaced(IWorldAccessor world, BlockPos blockPos, ref EnumHandling handling)
    {
        Block prevBlock = world.BlockAccessor.GetBlock(blockPos);
        Block newBlock = world.GetBlock(AssetLocation.Create("doubleslabs:doubleslab"));
        world.BlockAccessor.SetBlock(newBlock.Id, blockPos, new ItemStack(prevBlock));
        world.BlockAccessor.TriggerNeighbourBlockUpdate(blockPos);
    }
}