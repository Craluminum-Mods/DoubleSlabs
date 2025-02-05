using HarmonyLib;
using System.Reflection;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace DoubleSlabs;

public static class Block_GetLightAbsorption_Patch2
{
    public static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(Block), nameof(Block.GetLightAbsorption), new[] { typeof(IWorldChunk), typeof(BlockPos) });
    }

    public static void Postfix(Block __instance, ref int __result, IWorldChunk chunk, BlockPos pos)
    {
        if (__instance.IsSlab()
            && HarmonyPatches.api is ICoreClientAPI capi
            && capi?.World?.Player?.CurrentBlockSelection?.SelectionBoxIndex == 1
            && chunk.GetLocalBlockEntityAtBlockPos(pos).TryGetBEBehavior(out BEBehaviorDoubleSlab bebehavior)
            && !bebehavior.IsEmpty)
        {
            __result = 99;
        }
    }
}