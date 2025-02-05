using HarmonyLib;
using System.Reflection;
using System.Runtime.CompilerServices;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace DoubleSlabs;

public static class Block_GetSounds_Patch
{
    public static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(Block), nameof(Block.GetSounds), new[] { typeof(IBlockAccessor), typeof(BlockPos), typeof(ItemStack) });
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static BlockSounds Base(this Block instance, IBlockAccessor blockAccessor, BlockPos pos, ItemStack stack = null)
    {
        return default;
    }

    public static void Postfix(Block __instance, ref BlockSounds __result, IBlockAccessor blockAccessor, BlockPos pos)
    {
        if (__instance.IsSlab()
            && HarmonyPatches.api is ICoreClientAPI capi
            && capi?.World?.Player?.CurrentBlockSelection?.SelectionBoxIndex == 1
            && blockAccessor.TryGetBEBehavior(pos, out BEBehaviorDoubleSlab bebehavior)
            && !bebehavior.IsEmpty)
        {
            __result = Base(bebehavior.ContainedBlock, blockAccessor, pos);
        }
    }
}