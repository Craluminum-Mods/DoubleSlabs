using HarmonyLib;
using System.Reflection;
using System.Runtime.CompilerServices;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace DoubleSlabs;

public static class Block_DoEmitSideAoByFlag_Patch
{
    public static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(Block), nameof(Block.DoEmitSideAoByFlag), new[] { typeof(IGeometryTester), typeof(Vec3iAndFacingFlags), typeof(int) });
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool Base(this Block instance, IGeometryTester caller, Vec3iAndFacingFlags vec, int flags)
    {
        return default;
    }

    public static void Postfix(Block __instance, ref bool __result, IGeometryTester caller, Vec3iAndFacingFlags vec, int flags)
    {
        if (__instance.IsSlab()
            && caller.GetCurrentBlockEntityOnSide(vec)?.TryGetBEBehavior(out BEBehaviorDoubleSlab bebehavior) == true
            && !bebehavior.IsEmpty)
        {
            __result = Base(__instance, caller, vec, flags) || Base(bebehavior.ContainedBlock, caller, vec, flags);
        }
    }
}