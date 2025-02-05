using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using Vintagestory.ServerMods;

namespace DoubleSlabs;

public static class HelperExtensions
{
    public static bool IsSlab(this CollectibleObject obj)
    {
        return obj is BlockSlab || obj is BlockSlabSnowRemove || (obj.HasBehavior<BlockBehaviorOmniRotatable>() && obj.Code.ToString().Contains("slab"));
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

    public static bool TryGetBEBehavior<T>(this IBlockAccessor blockAccessor, BlockPos pos, out T behavior) where T : BlockEntityBehavior
    {
        behavior = blockAccessor.GetBlockEntity(pos)?.GetBehavior<T>();
        return behavior != null;
    }

    public static bool TryGetBEBehavior<T>(this BlockEntity blockEntity, out T behavior) where T : BlockEntityBehavior
    {
        behavior = blockEntity?.GetBehavior<T>();
        return behavior != null;
    }

    public static bool TryGetBlockBehavior<T>(this IBlockAccessor blockAccessor, BlockPos pos, out T behavior) where T : BlockBehavior
    {
        behavior = blockAccessor.GetBlock(pos)?.GetBehavior<T>();
        return behavior != null;
    }

    public static ItemStack[] GetHandBookStacksArray(this CollectibleObject obj, ICoreClientAPI capi)
    {
        return obj.GetHandBookStacks(capi)?.ToArray() ?? System.Array.Empty<ItemStack>();
    }

    public static string GetPlacedBlockDebugInfo(this Block block, IWorldAccessor world, BlockPos pos, IPlayer forPlayer)
    {
        StringBuilder dsc = new();
        block.GetDebugInfo(world, pos, dsc);
        return dsc.ToString();
    }

    public static void GetDebugInfo(this Block block, IWorldAccessor world, BlockPos pos, StringBuilder dsc)
    {
        if (world.Api is ICoreClientAPI capi
            && capi.World.Player.CurrentBlockSelection.SelectionBoxIndex == 1
            && world.BlockAccessor.TryGetBEBehavior(pos, out BEBehaviorDoubleSlab bebehavior)
            && !bebehavior.IsEmpty)
        {
            dsc.AppendLine("Contained Retention Heat: " + bebehavior.ContainedBlock.GetRetention(pos, capi.World.Player.CurrentBlockSelection.Face, EnumRetentionType.Heat));
            dsc.AppendLine("Contained Retention Sound: " + bebehavior.ContainedBlock.GetRetention(pos, capi.World.Player.CurrentBlockSelection.Face, EnumRetentionType.Sound));
            dsc.AppendLine("Contained Retention Water: " + bebehavior.ContainedBlock.GetRetention(pos, capi.World.Player.CurrentBlockSelection.Face, EnumRetentionType.Water));
            dsc.AppendLine("Contained BlockMaterial: " + bebehavior.ContainedBlock.GetBlockMaterial(world.BlockAccessor, pos));
        }
        else
        {
            dsc.AppendLine("Original Retention Heat: " + block.GetRetention(pos, (world.Api as ICoreClientAPI)?.World.Player.CurrentBlockSelection.Face, EnumRetentionType.Heat));
            dsc.AppendLine("Original Retention Sound: " + block.GetRetention(pos, (world.Api as ICoreClientAPI)?.World.Player.CurrentBlockSelection.Face, EnumRetentionType.Sound));
            dsc.AppendLine("Original Retention Water: " + block.GetRetention(pos, (world.Api as ICoreClientAPI)?.World.Player.CurrentBlockSelection.Face, EnumRetentionType.Water));
            dsc.AppendLine("Original BlockMaterial: " + block.GetBlockMaterial(world.BlockAccessor, pos));
        }
    }
}