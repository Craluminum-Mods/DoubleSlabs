using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using Vintagestory.ServerMods;

namespace DoubleSlabs;

public class BlockBehaviorDoubleSlab : BlockBehavior
{
    private WorldInteraction[] interactions;

    public BlockBehaviorDoubleSlab(Block block) : base(block) { }

    public override void OnLoaded(ICoreAPI api)
    {
        if (api is not ICoreClientAPI capi)
        {
            return;
        }

        interactions = ObjectCacheUtil.GetOrCreate(capi, cacheKeyDoubleSlabInteractions, () =>
        {
            List<ItemStack> slabStacks = new();
            foreach (Block block in capi.World.Blocks.Where(x => x.IsSlab()))
            {
                slabStacks.AddRange(block.GetHandBookStacksArray(capi));
            }
            return new WorldInteraction[] {
                new()
                {
                    ActionLangCode = langCodeAdd,
                    MouseButton = EnumMouseButton.Right,
                    Itemstacks = slabStacks.ToArray()
                }
            };
        });
    }

    public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer, ref EnumHandling handling) => interactions;

    public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
    {
        if (blockSel == null
            || byPlayer?.Entity?.ActiveHandItemSlot?.Itemstack?.Collectible.IsSlab() == false
            || blockSel.IsProtected(world, byPlayer, EnumBlockAccessFlags.BuildOrBreak))
        {
            return base.OnBlockInteractStart(world, byPlayer, blockSel, ref handling);
        }

        bool hasBehavior = world.BlockAccessor.TryGetBEBehavior(blockSel.Position, out BEBehaviorDoubleSlab bebehavior);
        if (hasBehavior)
        {
            if (bebehavior.TryPut(byPlayer.Entity.ActiveHandItemSlot, byPlayer))
            {
                world.BlockAccessor.TriggerNeighbourBlockUpdate(blockSel.Position);
                handling = EnumHandling.PreventDefault;
                return true;
            }

            return base.OnBlockInteractStart(world, byPlayer, blockSel, ref handling);
        }

        if (world.BlockAccessor.GetBlockEntity(blockSel.Position) is null)
        {
            world.BlockAccessor.SpawnBlockEntity(genericEntityClass, blockSel.Position);
        }

        hasBehavior = world.BlockAccessor.TryGetBEBehavior(blockSel.Position, out bebehavior);
        if (hasBehavior && bebehavior.TryPut(byPlayer.Entity.ActiveHandItemSlot, byPlayer))
        {
            world.BlockAccessor.TriggerNeighbourBlockUpdate(blockSel.Position);
            handling = EnumHandling.PreventDefault;
            return true;
        }
        return base.OnBlockInteractStart(world, byPlayer, blockSel, ref handling);
    }

    public override void OnBlockRemoved(IWorldAccessor world, BlockPos pos, ref EnumHandling handling)
    {
        if (world.BlockAccessor.TryGetBEBehavior(pos, out BEBehaviorDoubleSlab bebehavior))
        {
            bebehavior.DropContents();
            world.BlockAccessor.RemoveBlockEntity(pos);
            handling = EnumHandling.PreventDefault;
        }
    }

    public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref EnumHandling handling)
    {
        if (world.BlockAccessor.TryGetBEBehavior(pos, out BEBehaviorDoubleSlab bebehavior) && !bebehavior.IsEmpty)
        {
            if (byPlayer.CurrentBlockSelection.SelectionBoxIndex == 1)
            {
                bebehavior.DropContents();
                world.BlockAccessor.TriggerNeighbourBlockUpdate(pos);
                handling = EnumHandling.PreventDefault;
                return;
            }
            else
            {
                Block toBlock = GetFlippedBlock(world, fromBlock: block, toBlock: bebehavior.ContainedBlock);
                SetBlockReplace(block, toBlock, world, pos, byPlayer);
                world.BlockAccessor.RemoveBlockEntity(pos);
                world.BlockAccessor.MarkBlockModified(pos);
                world.BlockAccessor.MarkBlockEntityDirty(pos);
                world.BlockAccessor.TriggerNeighbourBlockUpdate(pos);
                handling = EnumHandling.PreventDefault;
                return;
            }
        }
    }

    public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos, ref EnumHandling handling)
    {
        if (world.Api is ICoreClientAPI capi
            && capi.World.Player.CurrentBlockSelection.SelectionBoxIndex == 1
            && world.BlockAccessor.TryGetBEBehavior(pos, out BEBehaviorDoubleSlab bebehavior)
            && !bebehavior.IsEmpty)
        {
            handling = EnumHandling.PreventDefault;
            return new ItemStack(bebehavior.ContainedBlock);
        }
        return base.OnPickBlock(world, pos, ref handling);
    }

    public override int GetRetention(BlockPos pos, BlockFacing facing, EnumRetentionType type, ref EnumHandling handling)
    {
        ICoreAPI _api = HarmonyPatches.api;
        if (_api.World.BlockAccessor.TryGetBEBehavior(pos, out BEBehaviorDoubleSlab bebehavior) && !bebehavior.IsEmpty)
        {
            if (block.SideIsSolid(pos, facing.Index) || bebehavior.ContainedBlock.SideIsSolid(pos, facing.Index))
            {
                if (type == EnumRetentionType.Sound)
                {
                    handling = EnumHandling.PreventDefault;
                    return 10;
                }
                EnumBlockMaterial mat = bebehavior.ContainedBlock.GetBlockMaterial(_api.World.BlockAccessor, pos, null);
                if (mat == EnumBlockMaterial.Ore || mat == EnumBlockMaterial.Stone || mat == EnumBlockMaterial.Soil || mat == EnumBlockMaterial.Ceramic)
                {
                    handling = EnumHandling.PreventDefault;
                    return -1;
                }
                handling = EnumHandling.PreventDefault;
                return 1;
            }
        }
        return base.GetRetention(pos, facing, type, ref handling);
    }

    public override bool CanAttachBlockAt(IBlockAccessor blockAccessor, Block block, BlockPos pos, BlockFacing blockFace, ref EnumHandling handling, Cuboidi attachmentArea = null)
    {
        if (blockAccessor.TryGetBEBehavior(pos, out BEBehaviorDoubleSlab bebehavior) && !bebehavior.IsEmpty)
        {
            handling = EnumHandling.PreventDefault;
            return true;
        }
        return base.CanAttachBlockAt(blockAccessor, block, pos, blockFace, ref handling, attachmentArea);
    }

    public static MeshData GetOrCreateMesh(ICoreClientAPI capi, Block inBlock, Block forBlock, ITexPositionSource overrideTexturesource = null)
    {
        Dictionary<string, MeshData> Meshes = ObjectCacheUtil.GetOrCreate(capi, cacheKeyDoubleSlabMeshes, () => new Dictionary<string, MeshData>());
        Block oppositeBlock = GetFlippedBlock(capi.World, inBlock, forBlock);
        string key = $"{inBlock.Code}{oppositeBlock.Code}";
        if (overrideTexturesource != null || !Meshes.TryGetValue(key, out MeshData mesh))
        {
            mesh = new MeshData(4, 3);
            Shape shape = capi.TesselatorManager.GetCachedShape(oppositeBlock.Shape.Base);
            ITexPositionSource texSource = overrideTexturesource;
            if (texSource == null)
            {
                ShapeTextureSource stexSource = new ShapeTextureSource(capi, shape, "");
                texSource = stexSource;
                foreach (KeyValuePair<string, CompositeTexture> val in oppositeBlock.Textures)
                {
                    CompositeTexture ctex = val.Value.Clone();
                    ctex.Bake(capi.Assets);
                    stexSource.textures[val.Key] = ctex;
                }
            }
            if (shape == null)
            {
                return mesh;
            }
            capi.Tesselator.TesselateShape("DoubleSlab block", shape, out mesh, texSource);
            if (overrideTexturesource == null)
            {
                Meshes[key] = mesh;
            }
        }
        return mesh;
    }

    public static Block GetFlippedBlock(IWorldAccessor world, Block fromBlock, Block toBlock) => world.GetBlock(fromBlock.GetBehavior<BlockBehaviorOmniRotatable>()?.Rot switch
    {
        Up or Down => GetVerticallyFlippedBlockCode(fromBlock, toBlock),
        North or South => GetHorizontallyFlippedBlockCode(fromBlock, toBlock, EnumAxis.Z),
        West or East => GetHorizontallyFlippedBlockCode(fromBlock, toBlock, EnumAxis.X),
        _ => toBlock.Code
    });

    public static AssetLocation GetHorizontallyFlippedBlockCode(Block fromBlock, Block toBlock, EnumAxis axis)
    {
        BlockFacing curFacing = BlockFacing.FromCode(fromBlock.Variant[variantRot]);
        return curFacing.Axis == axis ? toBlock.CodeWithVariant(variantRot, curFacing.Opposite.Code) : toBlock.Code;
    }

    public static AssetLocation GetVerticallyFlippedBlockCode(Block fromBlock, Block toBlock)
    {
        BlockFacing curFacing = BlockFacing.FromCode(fromBlock.Variant[variantRot]);
        if (curFacing.IsVertical)
        {
            return toBlock.CodeWithVariant(variantRot, curFacing.Opposite.Code);
        }
        curFacing = BlockFacing.FromCode(fromBlock.Variant[variantVer]);
        return curFacing != null && curFacing.IsVertical
            ? toBlock.CodeWithVariant(variantVer, curFacing.Opposite.Code)
            : toBlock.Code;
    }

    public static void SetBlockReplace(Block fromBlock, Block toBlock, IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
    {
        DropBlock(fromBlock, world, pos, byPlayer, dropQuantityMultiplier);
        world.BlockAccessor.ExchangeBlock(toBlock.Id, pos);
    }

    public static void DropBlock(Block block, IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
    {
        if (world.Side == EnumAppSide.Server && (byPlayer == null || byPlayer.WorldData.CurrentGameMode != EnumGameMode.Creative))
        {
            ItemStack[] drops = block.GetDrops(world, pos, byPlayer, dropQuantityMultiplier);
            if (drops != null)
            {
                for (int j = 0; j < drops.Length; j++)
                {
                    if (block.SplitDropStacks)
                    {
                        for (int k = 0; k < drops[j].StackSize; k++)
                        {
                            ItemStack itemStack = drops[j].Clone();
                            itemStack.StackSize = 1;
                            world.SpawnItemEntity(itemStack, new Vec3d((double)pos.X + 0.5, (double)pos.Y + 0.5, (double)pos.Z + 0.5));
                        }
                    }
                    else
                    {
                        world.SpawnItemEntity(drops[j].Clone(), new Vec3d((double)pos.X + 0.5, (double)pos.Y + 0.5, (double)pos.Z + 0.5));
                    }
                }
            }

            world.PlaySoundAt(block.GetSounds(world.BlockAccessor, pos)?.GetBreakSound(byPlayer), pos.X, pos.Y, pos.Z, byPlayer);
        }
        block.SpawnBlockBrokenParticles(pos);
    }
}