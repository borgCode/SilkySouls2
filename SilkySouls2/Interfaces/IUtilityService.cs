// 

using System;
using System.Collections.Generic;
using SilkySouls2.GameIds;
using SilkySouls2.Memory.DLLShared;
using SilkySouls2.Models;

namespace SilkySouls2.Interfaces
{
    public interface IUtilityService
    {
        void ForceSave();
        void ToggleCreditSkip(bool isCreditSkipEnabled);
        void Toggle100Drop(bool is100DropEnabled);
        void ToggleNoClip(bool isNoClipEnabled);
        void SetNoClipSpeed(byte[] xBytes, byte[] yBytes);
        void ToggleKillboxHook(bool isEnabled);
        void ToggleDrawHitbox(bool isDrawHitboxEnabled);
        void ToggleDrawEvent(DrawType eventType, bool isDrawEventEnabled);
        void ToggleDrawSound(bool isDrawSoundEnabled);
        void ToggleTargetingView(bool isTargetingViewEnabled);
        void ToggleRagdoll(bool isDrawRagrollEnabled);
        void ToggleHideChr(bool isHideCharactersEnabled);
        void ToggleHideMap(bool isHideMapEnabled);
        void SetGameSpeed(float value);
        void ToggleRagdollEsp(bool isSeeThroughwallsEnabled);
        void ToggleDrawCol(bool isDrawCollisionEnabled);
        void ToggleDrawKillbox(bool isDrawKillboxEnabled);
        void ToggleColWireframe(bool isColWireframeEnabled);
        void ToggleDrawObj(bool isDrawObjEnabled);
        void ToggleSnowstormHook(bool isSnowstormDisabled);
        void ToggleMemoryTimer(bool isMemoryTimerDisabled);
        void ToggleIvorySkip(bool isIvorySkipEnabled);
        void SetObjState(long areaId, Obj obj);
        void DisableNavimesh(long areaId, Navimesh naviData);
        void DisableWhiteDoor(long areaId, WhiteDoor whiteDoorData);
        List<InventorySpell> GetInventorySpells();
        List<EquippedSpell> GetEquippedSpells();
        int GetTotalAvailableSlots();
        void AttuneSpell(int slotIndex, IntPtr entryAddr);
        void Reset();
        void ToggleLightGutter(bool isEnabled);
        void ToggleShadedFog(bool isNoFogEnabled);

    }
}