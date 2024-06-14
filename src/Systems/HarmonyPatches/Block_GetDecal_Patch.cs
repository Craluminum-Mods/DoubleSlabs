using HarmonyLib;
using System.Reflection;
using System.Runtime.CompilerServices;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace DoubleSlabs;

public static class Block_GetDecal_Patch
{
    public static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(Block), nameof(Block.GetDecal));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Base(this Block instance, IWorldAccessor world, BlockPos pos, ITexPositionSource decalTexSource, ref MeshData decalModelData, ref MeshData blockModelData)
    {
    }

    public static bool Prefix(Block __instance, IWorldAccessor world, BlockPos pos, ITexPositionSource decalTexSource, ref MeshData decalModelData, ref MeshData blockModelData)
    {
        if (__instance.IsSlab()
            && world.Api is ICoreClientAPI capi
            && capi?.World?.Player?.CurrentBlockSelection?.SelectionBoxIndex == 1
            && world.BlockAccessor.TryGetBEBehavior(pos, out BEBehaviorDoubleSlab bebehavior)
            && !bebehavior.IsEmpty)
        {

            decalModelData = BlockBehaviorDoubleSlab.GetOrCreateMesh(capi, __instance, bebehavior.ContainedBlock, decalTexSource).Clone();
            blockModelData = BlockBehaviorDoubleSlab.GetOrCreateMesh(capi, __instance, bebehavior.ContainedBlock).Clone();
            Base(bebehavior.ContainedBlock, world, pos, decalTexSource, ref decalModelData, ref blockModelData);
            return false;
        }
        return true;
    }
}