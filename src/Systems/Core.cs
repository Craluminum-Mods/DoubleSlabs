global using static DoubleSlabs.Constants;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

[assembly: ModInfo(name: "Double Slabs", modID: modDomain)]

namespace DoubleSlabs;

public class Core : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        api.RegisterBlockBehaviorClass("DoubleSlabs.DoubleSlab", typeof(BlockBehaviorDoubleSlab));
        api.RegisterBlockEntityBehaviorClass("DoubleSlabs.DoubleSlab", typeof(BEBehaviorDoubleSlab));
        api.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        foreach (Block block in api.World.Blocks)
        {
            if (!block.IsSlab())
            {
                continue;
            }
            block.CollectibleBehaviors = block.CollectibleBehaviors.Append(new BlockBehaviorDoubleSlab(block));
            block.BlockBehaviors = block.BlockBehaviors.Append(new BlockBehaviorDoubleSlab(block));
            block.BlockEntityBehaviors = block.BlockEntityBehaviors.Append(new BlockEntityBehaviorType()
            {
                Name = "DoubleSlabs.DoubleSlab",
                properties = null
            });
        }
    }
}