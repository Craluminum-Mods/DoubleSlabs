global using static DoubleSlabs.Constants;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace DoubleSlabs;

public class Core : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        api.RegisterBlockBehaviorClass("DoubleSlabs.DoubleSlab", typeof(BlockBehaviorDoubleSlab));

        api.RegisterBlockClass("DoubleSlabs.DoubleSlab", typeof(BlockDoubleSlab));
        api.RegisterBlockEntityClass("DoubleSlabs.DoubleSlab", typeof(BlockEntityDoubleSlab));

        Mod.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        foreach (Block block in api.World.Blocks)
        {
            if (block.IsSlab())
            {
                block.CollectibleBehaviors = block.CollectibleBehaviors.Append(new BlockBehaviorDoubleSlab(block));
                block.BlockBehaviors = block.BlockBehaviors.Append(new BlockBehaviorDoubleSlab(block));
            }
        }
    }
}