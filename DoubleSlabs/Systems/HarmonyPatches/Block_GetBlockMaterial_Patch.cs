using HarmonyLib;
using System.Reflection;
using System.Runtime.CompilerServices;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace DoubleSlabs;

public static class Block_GetBlockMaterial_Patch
{
    public static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(Block), nameof(Block.GetBlockMaterial), new[] { typeof(IBlockAccessor), typeof(BlockPos), typeof(ItemStack) });
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static EnumBlockMaterial Base(this Block instance, IBlockAccessor blockAccessor, BlockPos pos, ItemStack stack = null)
    {
        return default;
    }

    public static void Postfix(Block __instance, ref EnumBlockMaterial __result, IBlockAccessor blockAccessor, BlockPos pos, ItemStack stack = null)
    {
        if (stack == null
            && blockAccessor != null
            && pos != null
            && __instance.IsSlab()
            && HarmonyPatches.api != null
            && HarmonyPatches.api is ICoreClientAPI capi
            && capi?.World?.Player?.CurrentBlockSelection?.SelectionBoxIndex == 1
            && blockAccessor.TryGetBEBehavior(pos, out BEBehaviorDoubleSlab bebehavior)
            && !bebehavior.IsEmpty)
        {
            __result = Base(bebehavior.ContainedBlock, blockAccessor, pos);
        }
    }
}