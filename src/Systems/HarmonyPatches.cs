using HarmonyLib;
using Vintagestory.API.Common;

namespace DoubleSlabs;

public class HarmonyPatches : ModSystem
{
    public static ICoreAPI api;

    private Harmony HarmonyInstance => new Harmony(Mod.Info.ModID);

    public override void Start(ICoreAPI _api)
    {
        api = _api;
        HarmonyInstance.CreateReversePatcher(original: Block_GetSelectionBoxes_Patch.TargetMethod(), standin: typeof(Block_GetSelectionBoxes_Patch).GetMethod(nameof(Block_GetSelectionBoxes_Patch.Base))).Patch(HarmonyReversePatchType.Original);
        HarmonyInstance.CreateReversePatcher(original: Block_GetCollisionBoxes_Patch.TargetMethod(), standin: typeof(Block_GetCollisionBoxes_Patch).GetMethod(nameof(Block_GetCollisionBoxes_Patch.Base))).Patch(HarmonyReversePatchType.Original);
        HarmonyInstance.CreateReversePatcher(original: Block_GetParticleCollisionBoxes_Patch.TargetMethod(), standin: typeof(Block_GetParticleCollisionBoxes_Patch).GetMethod(nameof(Block_GetParticleCollisionBoxes_Patch.Base))).Patch(HarmonyReversePatchType.Original);
        HarmonyInstance.CreateReversePatcher(original: Block_GetBlockMaterial_Patch.TargetMethod(), standin: typeof(Block_GetBlockMaterial_Patch).GetMethod(nameof(Block_GetBlockMaterial_Patch.Base))).Patch(HarmonyReversePatchType.Original);
        HarmonyInstance.CreateReversePatcher(original: Block_GetSounds_Patch.TargetMethod(), standin: typeof(Block_GetSounds_Patch).GetMethod(nameof(Block_GetSounds_Patch.Base))).Patch(HarmonyReversePatchType.Original);
        HarmonyInstance.CreateReversePatcher(original: Block_GetDecal_Patch.TargetMethod(), standin: typeof(Block_GetDecal_Patch).GetMethod(nameof(Block_GetDecal_Patch.Base))).Patch(HarmonyReversePatchType.Original);
        HarmonyInstance.CreateReversePatcher(original: Block_DoEmitSideAo_Patch.TargetMethod(), standin: typeof(Block_DoEmitSideAo_Patch).GetMethod(nameof(Block_DoEmitSideAo_Patch.Base))).Patch(HarmonyReversePatchType.Original);
        HarmonyInstance.CreateReversePatcher(original: Block_DoEmitSideAoByFlag_Patch.TargetMethod(), standin: typeof(Block_DoEmitSideAoByFlag_Patch).GetMethod(nameof(Block_DoEmitSideAoByFlag_Patch.Base))).Patch(HarmonyReversePatchType.Original);

        HarmonyInstance.Patch(original: Block_GetSelectionBoxes_Patch.TargetMethod(), prefix: typeof(Block_GetSelectionBoxes_Patch).GetMethod(nameof(Block_GetSelectionBoxes_Patch.Prefix)));
        HarmonyInstance.Patch(original: Block_GetCollisionBoxes_Patch.TargetMethod(), prefix: typeof(Block_GetCollisionBoxes_Patch).GetMethod(nameof(Block_GetCollisionBoxes_Patch.Prefix)));
        HarmonyInstance.Patch(original: Block_GetParticleCollisionBoxes_Patch.TargetMethod(), prefix: typeof(Block_GetParticleCollisionBoxes_Patch).GetMethod(nameof(Block_GetParticleCollisionBoxes_Patch.Prefix)));
        HarmonyInstance.Patch(original: Block_OnBeingLookedAt_Patch.TargetMethod(), prefix: typeof(Block_OnBeingLookedAt_Patch).GetMethod(nameof(Block_OnBeingLookedAt_Patch.Prefix)));
        HarmonyInstance.Patch(original: Block_GetDecal_Patch.TargetMethod(), prefix: typeof(Block_GetDecal_Patch).GetMethod(nameof(Block_GetDecal_Patch.Prefix)));

        HarmonyInstance.Patch(original: Block_DoParticalSelection_Patch.TargetMethod(), postfix: typeof(Block_DoParticalSelection_Patch).GetMethod(nameof(Block_DoParticalSelection_Patch.Postfix)));
        HarmonyInstance.Patch(original: Block_SideIsSolid_Patch.TargetMethod(), postfix: typeof(Block_SideIsSolid_Patch).GetMethod(nameof(Block_SideIsSolid_Patch.Postfix)));
        HarmonyInstance.Patch(original: Block_SideIsSolid_Patch2.TargetMethod(), postfix: typeof(Block_SideIsSolid_Patch2).GetMethod(nameof(Block_SideIsSolid_Patch2.Postfix)));
        HarmonyInstance.Patch(original: Block_GetLiquidBarrierHeightOnSide_Patch.TargetMethod(), postfix: typeof(Block_GetLiquidBarrierHeightOnSide_Patch).GetMethod(nameof(Block_GetLiquidBarrierHeightOnSide_Patch.Postifx)));
        HarmonyInstance.Patch(original: Block_GetSounds_Patch.TargetMethod(), postfix: typeof(Block_GetSounds_Patch).GetMethod(nameof(Block_GetSounds_Patch.Postfix)));
        HarmonyInstance.Patch(original: Block_GetBlockMaterial_Patch.TargetMethod(), postfix: typeof(Block_GetBlockMaterial_Patch).GetMethod(nameof(Block_GetBlockMaterial_Patch.Postfix)));
        HarmonyInstance.Patch(original: Block_GetLightAbsorption_Patch.TargetMethod(), postfix: typeof(Block_GetLightAbsorption_Patch).GetMethod(nameof(Block_GetLightAbsorption_Patch.Postfix)));
        HarmonyInstance.Patch(original: Block_GetLightAbsorption_Patch2.TargetMethod(), postfix: typeof(Block_GetLightAbsorption_Patch2).GetMethod(nameof(Block_GetLightAbsorption_Patch2.Postfix)));
        HarmonyInstance.Patch(original: Block_DoEmitSideAo_Patch.TargetMethod(), postfix: typeof(Block_DoEmitSideAo_Patch).GetMethod(nameof(Block_DoEmitSideAo_Patch.Postfix)));
        HarmonyInstance.Patch(original: Block_DoEmitSideAoByFlag_Patch.TargetMethod(), postfix: typeof(Block_DoEmitSideAoByFlag_Patch).GetMethod(nameof(Block_DoEmitSideAoByFlag_Patch.Postfix)));
    }

    public override void Dispose()
    {
        HarmonyInstance.Unpatch(original: Block_GetSelectionBoxes_Patch.TargetMethod(), HarmonyPatchType.All, HarmonyInstance.Id);
        HarmonyInstance.Unpatch(original: Block_GetCollisionBoxes_Patch.TargetMethod(), HarmonyPatchType.All, HarmonyInstance.Id);
        HarmonyInstance.Unpatch(original: Block_GetParticleCollisionBoxes_Patch.TargetMethod(), HarmonyPatchType.All, HarmonyInstance.Id);
        HarmonyInstance.Unpatch(original: Block_OnBeingLookedAt_Patch.TargetMethod(), HarmonyPatchType.All, HarmonyInstance.Id);
        HarmonyInstance.Unpatch(original: Block_GetDecal_Patch.TargetMethod(), HarmonyPatchType.All, HarmonyInstance.Id);

        HarmonyInstance.Unpatch(original: Block_DoParticalSelection_Patch.TargetMethod(), HarmonyPatchType.All, HarmonyInstance.Id);
        HarmonyInstance.Unpatch(original: Block_SideIsSolid_Patch.TargetMethod(), HarmonyPatchType.All, HarmonyInstance.Id);
        HarmonyInstance.Unpatch(original: Block_SideIsSolid_Patch2.TargetMethod(), HarmonyPatchType.All, HarmonyInstance.Id);
        HarmonyInstance.Unpatch(original: Block_GetLiquidBarrierHeightOnSide_Patch.TargetMethod(), HarmonyPatchType.All, HarmonyInstance.Id);
        HarmonyInstance.Unpatch(original: Block_GetSounds_Patch.TargetMethod(), HarmonyPatchType.All, HarmonyInstance.Id);
        HarmonyInstance.Unpatch(original: Block_GetBlockMaterial_Patch.TargetMethod(), HarmonyPatchType.All, HarmonyInstance.Id);
        HarmonyInstance.Unpatch(original: Block_GetLightAbsorption_Patch.TargetMethod(), HarmonyPatchType.All, HarmonyInstance.Id);
        HarmonyInstance.Unpatch(original: Block_GetLightAbsorption_Patch2.TargetMethod(), HarmonyPatchType.All, HarmonyInstance.Id);
        HarmonyInstance.Unpatch(original: Block_DoEmitSideAo_Patch.TargetMethod(), HarmonyPatchType.All, HarmonyInstance.Id);
        HarmonyInstance.Unpatch(original: Block_DoEmitSideAoByFlag_Patch.TargetMethod(), HarmonyPatchType.All, HarmonyInstance.Id);
    }
}