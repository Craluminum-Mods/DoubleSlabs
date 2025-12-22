using Vintagestory.API.Common;
using Vintagestory.GameContent;
using Vintagestory.ServerMods;

namespace DoubleSlabs;

public static class HelperExtensions
{
    public static bool IsSlab(this CollectibleObject obj)
    {
        return obj != null && (obj is BlockSlab || obj is BlockSlabSnowRemove || (obj.HasBehavior<BlockBehaviorOmniRotatable>() && obj.Code.ToString().Contains("slab")));
    }

    public static bool IsProtected(this BlockSelection selection, IWorldAccessor world, IPlayer forPlayer, EnumBlockAccessFlags accessFlags)
    {
        bool _protected = false;
        if (world.Claims != null && forPlayer?.WorldData.CurrentGameMode == EnumGameMode.Survival && world.Claims.TestAccess(forPlayer, selection.Position, accessFlags) != 0)
        {
            _protected = true;
        }
        return _protected;
    }
}