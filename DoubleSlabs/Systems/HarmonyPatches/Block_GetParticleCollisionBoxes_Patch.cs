using HarmonyLib;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace DoubleSlabs;

public static class Block_GetParticleCollisionBoxes_Patch
{
    public static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(Block), nameof(Block.GetParticleCollisionBoxes), new[] { typeof(IBlockAccessor), typeof(BlockPos) });
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Cuboidf[] Base(this Block instance, IBlockAccessor blockAccessor, BlockPos pos)
    {
        return default;
    }

    public static bool Prefix(Block __instance, ref Cuboidf[] __result, IBlockAccessor blockAccessor, BlockPos pos)
    {
        if (__instance.IsSlab()
            && blockAccessor.TryGetBEBehavior(pos, out BEBehaviorDoubleSlab bebehavior)
            && !bebehavior.IsEmpty
            && __instance.HasBehavior<BlockBehaviorDoubleSlab>())
        {
            Block oppositeBlock = BlockBehaviorDoubleSlab.GetFlippedBlock(HarmonyPatches.api.World, __instance, bebehavior.ContainedBlock);
            __result = Base(__instance, blockAccessor, pos);
            __result = __result.Append(Base(oppositeBlock, blockAccessor, pos));
            return false;
        }
        return true;
    }
}