using HarmonyLib;
using System.Reflection;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace DoubleSlabs;

public static class Block_SideIsSolid_Patch2
{
    public static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(Block), nameof(Block.SideIsSolid), new[] { typeof(IBlockAccessor), typeof(BlockPos), typeof(int) });
    }

    public static void Postfix(Block __instance, ref bool __result, IBlockAccessor blockAccess, BlockPos pos)
    {
        if (__instance.IsSlab()
            && blockAccess.TryGetBEBehavior(pos, out BEBehaviorDoubleSlab bebehavior)
            && !bebehavior.IsEmpty)
        {
            __result = true;
        }
    }
}