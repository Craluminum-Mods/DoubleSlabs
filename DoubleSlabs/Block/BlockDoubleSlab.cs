using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace DoubleSlabs;

public class BlockDoubleSlab : Block
{
    public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos)
    {
        if (world.BlockAccessor.GetBlockEntity(pos) is BlockEntityDoubleSlab blockEntity && !blockEntity.IsEmpty)
        {
            return blockEntity.GetCurrentSlab().GetDrops(world, pos, null)[0].Clone();
        }
        return null;
    }

    public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1)
    {
        return [];
    }

    public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1)
    {
        if (world.BlockAccessor.GetBlockEntity(pos) is BlockEntityDoubleSlab blockEntity && blockEntity.IsFull)
        {
            ItemSlot fromSlot = blockEntity.GetSlotBySelectionIndex(byPlayer.CurrentBlockSelection.SelectionBoxIndex);
            if (!fromSlot.Empty)
            {
                world.SpawnItemEntity(fromSlot.Itemstack.Block.GetDrops(world, pos, byPlayer)[0], pos);
                fromSlot.Itemstack = null;
                blockEntity.MarkDirty(redrawOnClient: true);
                world.BlockAccessor.TriggerNeighbourBlockUpdate(pos);
                return;
            }
        }
        base.OnBlockBroken(world, pos, byPlayer, dropQuantityMultiplier);
        world.BlockAccessor.TriggerNeighbourBlockUpdate(pos);
    }

    public override float OnGettingBroken(IPlayer player, BlockSelection blockSel, ItemSlot itemslot, float remainingResistance, float dt, int counter)
    {
        return base.OnGettingBroken(player, blockSel, itemslot, remainingResistance, dt, counter);
    }

    public override bool CanAttachBlockAt(IBlockAccessor blockAccessor, Block block, BlockPos pos, BlockFacing blockFace, Cuboidi attachmentArea = null)
    {
        if (blockAccessor.GetBlockEntity(pos) is BlockEntityDoubleSlab blockEntity && !blockEntity.IsEmpty)
        {
            return true;

            // doesn't work at all
            //EnumHandling tempHandling = EnumHandling.PreventDefault;
            //return blockEntity.GetCurrentSlab().GetBehavior<BlockBehaviorOmniAttachable>().CanAttachBlockAt(blockAccessor, block, pos, blockFace, ref tempHandling, attachmentArea);
        }
        return base.CanAttachBlockAt(blockAccessor, block, pos, blockFace, attachmentArea);
    }

    public override Cuboidf[] GetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos)
    {
        if (blockAccessor.GetBlockEntity(pos) is BlockEntityDoubleSlab blockEntity && !blockEntity.IsEmpty)
        {
            Cuboidf[] boxes = [];
            if (!blockEntity.ContainedSlot1.Empty)
            {
                boxes = boxes.Concat(blockEntity.ContainedBlock1.SelectionBoxes).ToArray();
            }
            if (!blockEntity.ContainedSlot2.Empty)
            {
                boxes = boxes.Concat(blockEntity.ContainedBlock2.SelectionBoxes).ToArray();
            }
            return boxes;
        }
        return base.GetSelectionBoxes(blockAccessor, pos);
    }

    public override Cuboidf[] GetCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos)
    {
        if (blockAccessor.GetBlockEntity(pos) is BlockEntityDoubleSlab blockEntity && !blockEntity.IsEmpty)
        {
            Cuboidf[] boxes = [];
            if (!blockEntity.ContainedSlot1.Empty)
            {
                boxes = boxes.Concat(blockEntity.ContainedBlock1.CollisionBoxes).ToArray();
            }
            if (!blockEntity.ContainedSlot2.Empty)
            {
                boxes = boxes.Concat(blockEntity.ContainedBlock2.CollisionBoxes).ToArray();
            }
            return boxes;
        }
        return base.GetCollisionBoxes(blockAccessor, pos);
    }

    public override Cuboidf[] GetParticleCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos)
    {
        if (blockAccessor.GetBlockEntity(pos) is BlockEntityDoubleSlab blockEntity && !blockEntity.IsEmpty)
        {
            Cuboidf[] boxes = [];
            if (!blockEntity.ContainedSlot1.Empty)
            {
                boxes = boxes.Concat(blockEntity.ContainedBlock1.ParticleCollisionBoxes ?? blockEntity.ContainedBlock1.CollisionBoxes).ToArray();
            }
            if (!blockEntity.ContainedSlot2.Empty)
            {
                boxes = boxes.Concat(blockEntity.ContainedBlock2.ParticleCollisionBoxes ?? blockEntity.ContainedBlock2.CollisionBoxes).ToArray();
            }
            return boxes;
        }
        return base.GetParticleCollisionBoxes(blockAccessor, pos);
    }

    public override bool DoParticalSelection(IWorldAccessor world, BlockPos pos) => true;

    public override int GetRetention(BlockPos pos, BlockFacing facing, EnumRetentionType type)
    {
        if (api.World.BlockAccessor.GetBlockEntity(pos) is BlockEntityDoubleSlab blockEntity && !blockEntity.IsEmpty)
        {
            bool isSolid = SideIsSolid(pos, facing.Index);
            if (isSolid)
            {
                if (type == EnumRetentionType.Sound)
                {
                    return 10;
                }
                EnumBlockMaterial mat = blockEntity.GetCurrentSlab().BlockMaterial;
                if (mat == EnumBlockMaterial.Ore || mat == EnumBlockMaterial.Stone || mat == EnumBlockMaterial.Soil || mat == EnumBlockMaterial.Ceramic)
                {
                    return -1;
                }
                return 1;
            }
        }
        return base.GetRetention(pos, facing, type);
    }

    public override bool SideIsSolid(BlockPos pos, int faceIndex)
    {
        if (api.World.BlockAccessor.GetBlockEntity(pos) is BlockEntityDoubleSlab blockEntity && !blockEntity.IsEmpty)
        {
            if (blockEntity.IsFull)
            {
                return true;
            }
            else
            {
                bool sideSolid1 = blockEntity.ContainedBlock1?.SideSolid[faceIndex] == true;
                bool sideSolid2 = blockEntity.ContainedBlock2?.SideSolid[faceIndex] == true;
                return sideSolid1 || sideSolid2;
            }
        }
        return base.SideIsSolid(pos, faceIndex);
    }

    public override bool SideIsSolid(IBlockAccessor blockAccess, BlockPos pos, int faceIndex)
    {
        if (blockAccess.GetBlockEntity(pos) is BlockEntityDoubleSlab blockEntity && !blockEntity.IsEmpty)
        {
            if (blockEntity.IsFull)
            {
                return true;
            }
            else
            {
                bool sideSolid1 = blockEntity.ContainedBlock1?.SideSolid[faceIndex] == true;
                bool sideSolid2 = blockEntity.ContainedBlock2?.SideSolid[faceIndex] == true;
                return sideSolid1 || sideSolid2;
            }
        }
        return base.SideIsSolid(blockAccess, pos, faceIndex);
    }

    public override bool DoEmitSideAo(IGeometryTester caller, BlockFacing facing)
    {
        if (caller.GetCurrentBlockEntityOnSide(facing) is BlockEntityDoubleSlab blockEntity && !blockEntity.IsEmpty)
        {
            if (blockEntity.IsFull)
            {
                return true;
            }
            else
            {
                bool emitSideAo1 = (blockEntity.ContainedBlock1?.EmitSideAo & facing.Flag) != 0;
                bool emitSideAo2 = (blockEntity.ContainedBlock2?.EmitSideAo & facing.Flag) != 0;
                return emitSideAo1 || emitSideAo2;
            }
        }
        return base.DoEmitSideAo(caller, facing);
    }

    public override bool DoEmitSideAoByFlag(IGeometryTester caller, Vec3iAndFacingFlags vec, int flags)
    {
        if (caller.GetCurrentBlockEntityOnSide(vec) is BlockEntityDoubleSlab blockEntity && !blockEntity.IsEmpty)
        {
            if (blockEntity.IsFull)
            {
                return true;
            }
            else
            {
                bool emitSideAo1 = (blockEntity.ContainedBlock1?.EmitSideAo & flags) != 0;
                bool emitSideAo2 = (blockEntity.ContainedBlock2?.EmitSideAo & flags) != 0;
                return emitSideAo1 || emitSideAo2;
            }
        }
        return base.DoEmitSideAoByFlag(caller, vec, flags);
    }

    public override void OnBeingLookedAt(IPlayer byPlayer, BlockSelection blockSel, bool firstTick)
    {
        if (api.World.BlockAccessor.GetBlockEntity(blockSel.Position) is BlockEntityDoubleSlab blockEntity && !blockEntity.IsEmpty)
        {
            blockEntity.GetSlotBySelectionIndex(byPlayer.CurrentBlockSelection.SelectionBoxIndex).Itemstack.Block.OnBeingLookedAt(byPlayer, blockSel, firstTick);
        }
    }

    public override BlockSounds GetSounds(IBlockAccessor blockAccessor, BlockSelection blockSel, ItemStack stack = null)
    {
        Block decorBlock = blockSel.Face == null ? null : blockAccessor.GetDecor(blockSel.Position, new DecorBits(blockSel.Face));
        if (decorBlock != null && decorBlock.Attributes?["ignoreSounds"].AsBool() != true)
        {
            return decorBlock.Sounds;
        }
        if (api.World.BlockAccessor.GetBlockEntity(blockSel.Position) is BlockEntityDoubleSlab blockEntity && !blockEntity.IsEmpty)
        {
            return blockEntity.GetSlotBySelectionIndex(blockSel.SelectionBoxIndex).Itemstack.Block.Sounds;
        }
        return base.GetSounds(blockAccessor, blockSel, stack);
    }

    public override EnumBlockMaterial GetBlockMaterial(IBlockAccessor blockAccessor, BlockPos pos, ItemStack stack = null)
    {
        if (pos != null && blockAccessor.GetBlockEntity(pos) is BlockEntityDoubleSlab blockEntity && !blockEntity.IsEmpty)
        {
            return blockEntity.GetCurrentSlab().BlockMaterial;
        }
        return base.GetBlockMaterial(blockAccessor, pos, stack);
    }

    public override float GetResistance(IBlockAccessor blockAccessor, BlockPos pos)
    {
        if (api.World.BlockAccessor.GetBlockEntity(pos) is BlockEntityDoubleSlab blockEntity && !blockEntity.IsEmpty)
        {
            return blockEntity.GetCurrentSlab().Resistance;
        }
        return base.GetResistance(blockAccessor, pos);
    }

    public override int GetLightAbsorption(IBlockAccessor blockAccessor, BlockPos pos)
    {
        if (api.World.BlockAccessor.GetBlockEntity(pos) is BlockEntityDoubleSlab blockEntity && !blockEntity.IsEmpty)
        {
            EnumBlockMaterial? firstMat = blockEntity.ContainedBlock1?.BlockMaterial;
            EnumBlockMaterial? secondMat = blockEntity.ContainedBlock2?.BlockMaterial;
            bool bothGlass = firstMat == EnumBlockMaterial.Glass && secondMat == EnumBlockMaterial.Glass;
            bool anyGlass = firstMat == EnumBlockMaterial.Glass || secondMat == EnumBlockMaterial.Glass;

            if (blockEntity.IsFull)
            {
                return bothGlass ? 0 : 99;
            }

            int lightAbsorption = blockEntity.GetCurrentSlab().LightAbsorption;
            return anyGlass ? 0 : lightAbsorption;
        }
        return base.GetLightAbsorption(blockAccessor, pos);
    }

    public override int GetLightAbsorption(IWorldChunk chunk, BlockPos pos)
    {
        if (chunk.GetLocalBlockEntityAtBlockPos(pos) is BlockEntityDoubleSlab blockEntity && !blockEntity.IsEmpty)
        {
            EnumBlockMaterial? firstMat = blockEntity.ContainedBlock1?.BlockMaterial;
            EnumBlockMaterial? secondMat = blockEntity.ContainedBlock2?.BlockMaterial;

            if (blockEntity.IsFull)
            {
                bool bothGlass = firstMat == EnumBlockMaterial.Glass && secondMat == EnumBlockMaterial.Glass;
                return bothGlass ? 0 : 99;
            }

            int lightAbsorption = blockEntity.GetCurrentSlab().LightAbsorption;
            bool anyGlass = firstMat == EnumBlockMaterial.Glass || secondMat == EnumBlockMaterial.Glass;
            return anyGlass ? 0 : lightAbsorption;
        }
        return base.GetLightAbsorption(chunk, pos);
    }

    float[] liquidBarrierHeightonSide;
    public override float GetLiquidBarrierHeightOnSide(BlockFacing face, BlockPos pos)
    {
        if (api.World.BlockAccessor.GetBlockEntity(pos) is BlockEntityDoubleSlab blockEntity && !blockEntity.IsEmpty)
        {
            if (blockEntity.IsFull)
            {
                return 1;
            }
            if (liquidBarrierHeightonSide == null)
            {
                liquidBarrierHeightonSide = new float[6];
                for (int i = 0; i < 6; i++) liquidBarrierHeightonSide[i] = blockEntity.GetCurrentSlab().SideSolid.OnSide(BlockFacing.ALLFACES[i]) ? 1f : 0f;

                var lbos = blockEntity.GetCurrentSlab().Attributes?["liquidBarrierOnSides"].AsArray<float>(null);
                for (int i = 0; lbos != null && i < lbos.Length; i++) liquidBarrierHeightonSide[i] = lbos[i];
            }

            return liquidBarrierHeightonSide[face.Index];
        }
        return base.GetLiquidBarrierHeightOnSide(face, pos);
    }

    public override void GetDecal(IWorldAccessor world, BlockPos pos, ITexPositionSource decalTexSource, ref MeshData decalModelData, ref MeshData blockModelData)
    {
        if (world.Api is ICoreClientAPI capi && world.BlockAccessor.GetBlockEntity(pos) is BlockEntityDoubleSlab blockEntity && !blockEntity.IsEmpty)
        {
            if (!blockEntity.ContainedSlot1.Empty)
            {
                blockModelData = capi.TesselatorManager.GetDefaultBlockMesh(blockEntity.ContainedBlock1);
                decalModelData = capi.TesselatorManager.CreateMesh("", blockEntity.ContainedBlock1.Shape, (shape, tex) => { return decalTexSource; }, decalTexSource);
            }
            if (!blockEntity.ContainedSlot2.Empty)
            {
                blockModelData = capi.TesselatorManager.GetDefaultBlockMesh(blockEntity.ContainedBlock2);
                decalModelData = capi.TesselatorManager.CreateMesh("", blockEntity.ContainedBlock2.Shape, (shape, tex) => { return decalTexSource; }, decalTexSource);
            }
        }
    }
}
