using HarmonyLib;
using System.Reflection;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace DoubleSlabs;

public static class Block_DoParticalSelection_Patch
{
    public static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(Block), nameof(Block.DoParticalSelection), new[] { typeof(IWorldAccessor), typeof(BlockPos) });
    }

    public static void Postfix(Block __instance, ref bool __result, IWorldAccessor world, BlockPos pos)
    {
        if (__instance.IsSlab()
            && world.BlockAccessor.TryGetBEBehavior(pos, out BEBehaviorDoubleSlab bebehavior)
            && !bebehavior.IsEmpty)
        {
            __result = true;
        }
    }
}