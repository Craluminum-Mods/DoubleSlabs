using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace DoubleSlabs;

public class BEBehaviorDoubleSlab : BlockEntityBehavior, IBlockEntityContainer
{
    private InventoryGeneric inv;
    public MeshData Mesh { get; protected set; }

    public bool IsEmpty => ContainedSlot.Empty;
    public Block ContainedBlock => ContainedSlot.Itemstack.Block;
    public ItemSlot ContainedSlot => inv[0];
    public IInventory Inventory => inv;
    public string InventoryClassName => doubleSlabInvClassName;

    public BEBehaviorDoubleSlab(BlockEntity blockentity) : base(blockentity)
    {
        inv = new InventoryGeneric(1, $"{doubleSlabInvClassName}-0", Api, (id, inv) => new ItemSlotSlab(inv));
    }

    public override void Initialize(ICoreAPI api, JsonObject properties)
    {
        base.Initialize(api, properties);
        inv.LateInitialize($"{InventoryClassName}-{Pos.X}/{Pos.Y}/{Pos.Z}", api);
        inv.Pos = Pos;
        inv.ResolveBlocksOrItems();
        if (Mesh == null)
        {
            Init();
        }
    }

    protected void Init()
    {
        if (Api is not ICoreClientAPI capi)
        {
            return;
        }

        if (ContainedSlot.Empty)
        {
            Mesh = capi.TesselatorManager.GetDefaultBlockMesh(Block);
            return;
        }

        Mesh = BlockBehaviorDoubleSlab.GetOrCreateMesh(capi, inBlock: Block, forBlock: ContainedSlot.Itemstack.Block);
    }

    public override void OnBlockPlaced(ItemStack byItemStack = null) => Init();

    public void DropContents(Vec3d atPos = null)
    {
        inv.DropAll(Pos.ToVec3d().Add(atPos ?? new Vec3d(0.5, 0.5, 0.5)));
        Blockentity.MarkDirty(true);
        Init();
    }

    public override void OnBlockUnloaded() => Mesh?.Dispose();

    public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
    {
        inv.FromTreeAttributes(tree.GetTreeAttribute(attributeInventoryDoubleSlab));
        Init();
    }

    public override void ToTreeAttributes(ITreeAttribute tree)
    {
        if (inv != null)
        {
            ITreeAttribute invtree = new TreeAttribute();
            inv.ToTreeAttributes(invtree);
            tree[attributeInventoryDoubleSlab] = invtree;
        }
    }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
    {
        mesher.AddMeshData(Mesh);
        return false;
    }

    public bool TryPut(ItemSlot slot, IPlayer player)
    {
        if (ContainedSlot.Empty)
        {
            int num = slot.TryPutInto(Api.World, ContainedSlot);
            Api.World.PlaySoundAt(slot?.Itemstack?.Block?.Sounds?.Place ?? new AssetLocation("sounds/player/build"), player.Entity, player, randomizePitch: true, 16f);
            Blockentity.MarkDirty(true);
            Init();
            return num > 0;
        }
        return false;
    }
}