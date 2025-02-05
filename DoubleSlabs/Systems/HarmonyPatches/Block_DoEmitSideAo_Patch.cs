using HarmonyLib;
using System.Reflection;
using System.Runtime.CompilerServices;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace DoubleSlabs;

public static class Block_DoEmitSideAo_Patch
{
    public static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(Block), nameof(Block.DoEmitSideAo), new[] { typeof(IGeometryTester), typeof(BlockFacing) });
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool Base(this Block instance, IGeometryTester caller, BlockFacing facing)
    {
        return default;
    }

    public static void Postfix(Block __instance, ref bool __result, IGeometryTester caller, BlockFacing facing)
    {
        if (__instance.IsSlab()
            && caller.GetCurrentBlockEntityOnSide(facing)?.TryGetBEBehavior(out BEBehaviorDoubleSlab bebehavior) == true
            && !bebehavior.IsEmpty)
        {
            __result = Base(__instance, caller, facing) || Base(bebehavior.ContainedBlock, caller, facing);
        }
    }
}