using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using Vintagestory.ServerMods;

namespace DoubleSlabs;

public class BlockEntityDoubleSlab : BlockEntityContainer, IInteractable
{
    private InventoryGeneric inv;

    public bool IsFull => !ContainedSlot1.Empty && !ContainedSlot2.Empty;
    public bool IsEmpty => ContainedSlot1.Empty && ContainedSlot2.Empty;

    public Block ContainedBlock1 => ContainedSlot1?.Itemstack?.Block;
    public Block ContainedBlock2 => ContainedSlot2?.Itemstack?.Block;

    public ItemSlot ContainedSlot1 => inv[0];
    public ItemSlot ContainedSlot2 => inv[1];

    public override InventoryBase Inventory => inv;
    public override string InventoryClassName => doubleSlabInvClassName;

    public BlockEntityDoubleSlab()
    {
        inv = new InventoryGeneric(2, $"{doubleSlabInvClassName}-0", Api, (id, inv) => new ItemSlotSlab(inv));
    }

    public override void OnBlockPlaced(ItemStack byItemStack = null)
    {
        base.OnBlockPlaced(byItemStack);

        if (byItemStack != null)
        {
            ContainedSlot1.Itemstack = byItemStack;
        }
    }

    public override void OnBlockBroken(IPlayer byPlayer = null)
    {
        if (Api is ICoreServerAPI sapi)
        {
            if (!Inventory.Empty)
            {
                StringBuilder sb = new($"{byPlayer?.PlayerName} broke container {Block.Code} at {Pos} dropped: ");
                foreach (var slot in Inventory)
                {
                    if (slot.Itemstack == null) continue;
                    sb.Append(slot.Itemstack.StackSize).Append("x ").Append(slot.Itemstack.Collectible?.Code).Append(", ");
                }
                sapi.Logger.Audit(sb.ToString());
            }
            DropContents(Pos.ToVec3d());
        }

        foreach (var val in Behaviors)
        {
            val.OnBlockBroken(byPlayer);
        }
    }

    public override void DropContents(Vec3d atPos)
    {
        ItemStack[] newDrops = [];

        if (!ContainedSlot1.Empty)
        {
            newDrops = newDrops.Concat(ContainedBlock1.GetDrops(Api.World, Pos, null)).ToArray();
        }
        if (!ContainedSlot2.Empty)
        {
            newDrops = newDrops.Concat(ContainedBlock2.GetDrops(Api.World, Pos, null)).ToArray();
        }

        foreach (ItemStack stack in newDrops)
        {
            if (stack == null) continue;
            Api.World.SpawnItemEntity(stack, atPos);
        }
    }

    public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
    {
        base.GetBlockInfo(forPlayer, dsc);

        dsc.AppendLine("Slot 1: " + (!ContainedSlot1.Empty ? ContainedSlot1.Itemstack.GetName() : Lang.Get("Empty")));
        dsc.AppendLine("Slot 2: " + (!ContainedSlot2.Empty ? ContainedSlot2.Itemstack.GetName() : Lang.Get("Empty")));
    }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
    {
        ICoreClientAPI capi = Api as ICoreClientAPI;

        if (!ContainedSlot1.Empty)
        {
            MeshData mesh = capi.TesselatorManager.GetDefaultBlockMesh(ContainedBlock1);
            mesher.AddMeshData(mesh);
        }
        if (!ContainedSlot2.Empty)
        {
            MeshData mesh = capi.TesselatorManager.GetDefaultBlockMesh(ContainedBlock2);
            mesher.AddMeshData(mesh);
        }
        return true;
    }

    public bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
    {
        if (IsEmpty)
        {
            Api.World.BlockAccessor.SetBlock(0, Pos);
            Api.World.BlockAccessor.TriggerNeighbourBlockUpdate(Pos);
            handling = EnumHandling.PreventDefault;
            return false;
        }
        if (IsFull)
        {
            return false;
        }

        if (blockSel.IsProtected(world, byPlayer, EnumBlockAccessFlags.BuildOrBreak))
        {
            handling = EnumHandling.PreventDefault;
            return false;
        }

        ItemSlot hotbarSlot = byPlayer.InventoryManager.ActiveHotbarSlot;
        if (hotbarSlot.Empty || hotbarSlot.Itemstack.Block == null || !hotbarSlot.Itemstack.Block.IsSlab())
        {
            return false;
        }

        if (!HasOppositeRotation(blockSel))
        {
            return false;
        }

        Block newBlock = GetFlippedBlock(world, GetCurrentSlab(), hotbarSlot.Itemstack.Block);

        bool ok = TryPut(new DummySlot(new ItemStack(newBlock)), ContainedSlot1.Empty ? ContainedSlot1 : ContainedSlot2, byPlayer, blockSel);
        if (ok)
        {
            hotbarSlot.TakeOut(1);
            handling = EnumHandling.PreventDefault;
        }
        return ok;
    }

    public bool TryPut(ItemSlot slot, ItemSlot intoSlot, IPlayer player, BlockSelection blockSel)
    {
        int num = slot.TryPutInto(Api.World, intoSlot);
        Api.World.PlaySoundAt(slot?.Itemstack?.Block?.Sounds?.Place ?? new AssetLocation("sounds/player/build"), player.Entity, player, randomizePitch: true, 16f);
        MarkDirty(redrawOnClient: true);
        return num > 0;
    }

    public bool HasOppositeRotation(BlockSelection blockSel)
    {
        BlockFacing blockFacing = BlockFacing.FromCode(GetCurrentSlab().Variant["rot"]);
        return blockFacing != null && blockFacing.Opposite == blockSel.Face;
    }

    public Block GetCurrentSlab()
    {
        return !ContainedSlot1.Empty ? ContainedBlock1 : ContainedBlock2;
    }

    public ItemSlot GetSlotBySelectionIndex(int selectionIndex)
    {
        if (IsEmpty)
        {
            return null;
        }
        if (IsFull)
        {
            return selectionIndex == 0 ? ContainedSlot1 : ContainedSlot2;
        }
        if (selectionIndex == 0)
        {
            return ContainedSlot2.Empty ? ContainedSlot1 : ContainedSlot2;
        }
        if (selectionIndex == 1)
        {
            return ContainedSlot1.Empty ? ContainedSlot2 : ContainedSlot1;
        }
        return null;
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
}
