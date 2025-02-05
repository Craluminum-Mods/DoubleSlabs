using HarmonyLib;
using System.Reflection;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace DoubleSlabs;

public static class Block_GetLiquidBarrierHeightOnSide_Patch
{
    public static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(Block), nameof(Block.GetLiquidBarrierHeightOnSide), new[] { typeof(BlockFacing), typeof(BlockPos) });
    }

    public static void Postifx(Block __instance, ref float __result, BlockPos pos)
    {
        if (__instance.IsSlab()
            && HarmonyPatches.api.World.BlockAccessor.TryGetBEBehavior(pos, out BEBehaviorDoubleSlab bebehavior)
            && !bebehavior.IsEmpty)
        {
            __result = 1.0f;
        }
    }
}