using HarmonyLib;
using System.Reflection;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace DoubleSlabs;

public static class Block_SideIsSolid_Patch
{
    public static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(Block), nameof(Block.SideIsSolid), new[] { typeof(BlockPos), typeof(int) });
    }

    public static void Postfix(Block __instance, ref bool __result, BlockPos pos)
    {
        if (__instance.IsSlab()
            && HarmonyPatches.api.World.BlockAccessor.TryGetBEBehavior(pos, out BEBehaviorDoubleSlab bebehavior)
            && !bebehavior.IsEmpty)
        {
            __result = true;
        }
    }
}