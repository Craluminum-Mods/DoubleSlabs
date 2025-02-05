using HarmonyLib;
using System.Reflection;
using System.Runtime.CompilerServices;
using Vintagestory.API.Common;

namespace DoubleSlabs;

public static class Block_OnBeingLookedAt_Patch
{
    public static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(Block), nameof(Block.OnBeingLookedAt), new[] { typeof(IPlayer), typeof(BlockSelection), typeof(bool) });
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Base(this Block instance, IPlayer byPlayer, BlockSelection blockSel, bool firstTick)
    {
    }

    public static bool Prefix(Block __instance, IPlayer byPlayer, BlockSelection blockSel, bool firstTick)
    {
        if (__instance.IsSlab()
            && blockSel?.SelectionBoxIndex == 1
            && byPlayer?.Entity?.World?.BlockAccessor.TryGetBEBehavior(blockSel.Position, out BEBehaviorDoubleSlab bebehavior) == true
            && !bebehavior.IsEmpty)
        {
            Base(bebehavior.ContainedBlock, byPlayer, blockSel, firstTick);
            return false;
        }
        return true;
    }
}