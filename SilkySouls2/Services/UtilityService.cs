using System;
using System.Collections.Generic;
using SilkySouls2.enums;
using SilkySouls2.GameIds;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Memory.DLLShared;
using SilkySouls2.Models;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class UtilityService(IMemoryService memoryService, HookManager hookManager, DllManager dllManager)
        : IUtilityService
    {
        public void ForceSave()
        {
            var gameMan = memoryService.ReadPointer(GameManagerImp.Base);
            var saveLoadSystem = memoryService.ReadPointer(gameMan + GameManagerImp.SaveLoadSystem);

            memoryService.Write(saveLoadSystem + GameManagerImp.SaveLoadSystemOffsets.ForceSaveFlag1, 2);
            memoryService.Write(saveLoadSystem + GameManagerImp.SaveLoadSystemOffsets.ForceSaveFlag2, (byte)1);
        }

        public void ToggleCreditSkip(bool isCreditSkipEnabled)
        {
            var code = CustomCodeOffsets.Base + (int)CustomCodeOffsets.CreditSkip.Code;

            if (!isCreditSkipEnabled)
            {
                hookManager.UninstallHook(code);
                return;
            }

            var hookLoc = Hooks.CreditSkip;
            var modifyOnceFlag = CustomCodeOffsets.Base + (int)CustomCodeOffsets.CreditSkip.ModifyOnceFlag;
            memoryService.Write(modifyOnceFlag, 0);

            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                InstallScholarCreditSkip(modifyOnceFlag, hookLoc, code);
            }
            else
            {
                InstallVanillaCreditSkip(modifyOnceFlag, hookLoc, code);
            }
        }

        private void InstallScholarCreditSkip(nint modifyOnceFlag, nint hookLoc, nint code)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.CreditSkip64);
            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (code + 0x7, modifyOnceFlag, 7, 0x7 + 2),
                (code + 0x17, modifyOnceFlag, 10, 0x17 + 2),
                (code + 0x21, hookLoc + 7, 5, 0x21 + 1)
            ]);
            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code, hookLoc, [0x48, 0x81, 0xEC, 0x20, 0x02, 0x00, 0x00]);
        }

        private void InstallVanillaCreditSkip(nint modifyOnceFlag, nint hookLoc, nint code)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.CreditSkip32);
            var bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 6, code + 0x25);
            Array.Copy(bytes, 0, codeBytes, 0x20 + 1, 4);
            AsmHelper.WriteAbsoluteAddresses32(codeBytes, [
                (modifyOnceFlag, 0x6 + 2),
                (modifyOnceFlag, 0x16 + 2)
            ]);

            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code, hookLoc, [0x81, 0xEC, 0xFC, 0x01, 0x00, 0x00]);
        }

        public void Toggle100Drop(bool is100DropEnabled)
        {
            var dropCountHook = Hooks.NumOfDrops;
            var dropCountCode = CustomCodeOffsets.Base + (int)CustomCodeOffsets.Drop100.DropCount;

            memoryService.WriteBytes(Patches.DropRate, PatchDefinitions.DropRate.Get(is100DropEnabled));

            if (!is100DropEnabled)
            {
                hookManager.UninstallHook(dropCountCode);
                return;
            }

            if (PatchManager.IsScholar())
            {
                Setup100DropScholar(dropCountCode, dropCountHook);
            }
            else
            {
                Setup100DropVanilla(dropCountCode, dropCountHook);
            }
        }

        private void Setup100DropScholar(nint dropCountCode, nint dropCountHook)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.DropCount64);
            var bytes = AsmHelper.GetJmpOriginOffsetBytes(dropCountHook, 5, dropCountCode + 0xA);
            Array.Copy(bytes, 0, codeBytes, 0x5 + 1, 4);
            memoryService.WriteBytes(dropCountCode, codeBytes);
            hookManager.InstallHook(dropCountCode, dropCountHook, [0x41, 0x0F, 0xB6, 0x47, 0x01]);
        }

        private void Setup100DropVanilla(nint dropCountCode, nint dropCountHook)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.DropCount32);
            var bytes = AsmHelper.GetJmpOriginOffsetBytes(dropCountHook, 5, dropCountCode + 0xB);
            Array.Copy(bytes, 0, codeBytes, 0x6 + 1, 4);
            memoryService.WriteBytes(dropCountCode, codeBytes);

            hookManager.InstallHook(dropCountCode, dropCountHook, [0x0F, 0xB6, 0x51, 0x01, 0x40]);
        }

        public void ToggleNoClip(bool isNoClipEnabled)
        {
            var triggersAndSpaceCode = CustomCodeOffsets.Base + (int)CustomCodeOffsets.NoClip.TriggersAndSpaceCheck;
            var ctrlCode = CustomCodeOffsets.Base + (int)CustomCodeOffsets.NoClip.CtrlCheck;
            var coordsCode = CustomCodeOffsets.Base + (int)CustomCodeOffsets.NoClip.UpdateCoords;
            var raycastCode = CustomCodeOffsets.Base + (int)CustomCodeOffsets.NoClip.RayCastCode;

            if (!isNoClipEnabled)
            {
                hookManager.UninstallHook(coordsCode);
                memoryService.Write(GetGravityPtr(), (byte)0);
                hookManager.UninstallHook(triggersAndSpaceCode);
                hookManager.UninstallHook(ctrlCode);
                hookManager.UninstallHook(raycastCode);
                return;
            }

            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                SetupNoClipHooks64Bit(triggersAndSpaceCode, ctrlCode, coordsCode, raycastCode);
            }
            else
            {
                SetupNoClipHooks32Bit(triggersAndSpaceCode, ctrlCode, coordsCode, raycastCode);
            }
        }

        private void SetupNoClipHooks64Bit(nint triggersAndSpaceCode, nint ctrlCode, nint coordsCode,
            nint raycastCode)
        {
            var triggersAndSpaceHook = Hooks.TriggersAndSpace;
            var ctrlHook = Hooks.Ctrl;
            var coordsHook = Hooks.NoClipUpdateCoords;
            var rayCastHook = Hooks.ProcessPhysics;

            var zDirectionLoc = CustomCodeOffsets.Base + (int)CustomCodeOffsets.NoClip.ZDirection;

            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_TriggersAndSpace64);
            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (triggersAndSpaceCode + 0x1C, zDirectionLoc, 7, 0x1C + 2),
                (triggersAndSpaceCode + 0x35, zDirectionLoc, 7, 0x35 + 2),
                (triggersAndSpaceCode + 0x4E, zDirectionLoc, 7, 0x4E + 2),
                (triggersAndSpaceCode + 0x56, triggersAndSpaceHook + 0x9, 5, 0x56 + 1)
            ]);
            memoryService.WriteBytes(triggersAndSpaceCode, codeBytes);

            codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_CtrlCheck64);
            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (ctrlCode, zDirectionLoc, 7, 0x0 + 2),
                (ctrlCode + 0x7, ctrlHook + 0xA, 5, 0x7 + 1)
            ]);

            memoryService.WriteBytes(ctrlCode, codeBytes);

            codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_UpdateCoords64);
            AsmHelper.WriteAbsoluteAddresses64(codeBytes, [
                (GameManagerImp.Base, 0x1 + 2),
                (GameManagerImp.Base, 0x29 + 2),
                (GameManagerImp.Base, 0x73 + 2),
                (HkHardwareInfo.Base, 0xFB + 2)
            ]);

            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (coordsCode + 0xC2, zDirectionLoc, 6, 0xC2 + 2),
                (coordsCode + 0xEC, zDirectionLoc, 7, 0xEC + 2),
                (coordsCode + 0x17F, coordsHook + 0x8, 5, 0x17F + 1),
                (coordsCode + 0x18D, coordsHook + 0x8, 5, 0x18D + 1)
            ]);

            memoryService.WriteBytes(coordsCode, codeBytes);

            codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_RayCast64);

            var frameCounter = CustomCodeOffsets.Base + (int)CustomCodeOffsets.NoClip.FrameCounter;
            var rayInput = CustomCodeOffsets.Base + (int)CustomCodeOffsets.NoClip.RayInput;
            var rayOutput = CustomCodeOffsets.Base + (int)CustomCodeOffsets.NoClip.RayOutput;
            var mapId = CustomCodeOffsets.Base + (int)CustomCodeOffsets.NoClip.MapId;

            var raycastFunc = Functions.HavokRayCast;
            var convertToMap = Functions.ConvertPxRigidToMapEntity;
            var convertToMapId = Functions.ConvertMapEntityToGameId;

            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (raycastCode + 0x24, frameCounter, 6, 0x24 + 2),
                (raycastCode + 0x2A, frameCounter, 7, 0x2A + 2),
                (raycastCode + 0x3A, frameCounter, 6, 0x3A + 2),
                (raycastCode + 0x8B, rayInput, 7, 0x8B + 3),
                (raycastCode + 0xC9, rayOutput, 7, 0xC9 + 3),
                (raycastCode + 0xE0, raycastFunc, 5, 0xE0 + 1),
                (raycastCode + 0xE5, rayInput, 7, 0xE5 + 3),
                (raycastCode + 0x110, rayOutput + (0x30 * 1), 7, 0x110 + 3),
                (raycastCode + 0x127, raycastFunc, 5, 0x127 + 1),
                (raycastCode + 0x12C, rayInput, 7, 0x12C + 3),
                (raycastCode + 0x157, rayOutput + (0x30 * 2), 7, 0x157 + 3),
                (raycastCode + 0x16E, raycastFunc, 5, 0x16E + 1),
                (raycastCode + 0x173, rayInput, 7, 0x173 + 3),
                (raycastCode + 0x19E, rayOutput + (0x30 * 3), 7, 0x19E + 3),
                (raycastCode + 0x1B5, raycastFunc, 5, 0x1B5 + 1),
                (raycastCode + 0x1BA, rayInput, 7, 0x1BA + 3),
                (raycastCode + 0x1E5, rayOutput + (0x30 * 4), 7, 0x1E5 + 3),
                (raycastCode + 0x1FC, raycastFunc, 5, 0x1FC + 1),
                (raycastCode + 0x21E, rayOutput, 7, 0x21E + 3),
                (raycastCode + 0x24A, rayOutput, 7, 0x24A + 3),
                (raycastCode + 0x25F, convertToMap, 5, 0x25F + 1),
                (raycastCode + 0x26B, mapId, 7, 0x26B + 3),
                (raycastCode + 0x276, convertToMapId, 5, 0x276 + 1),
                (raycastCode + 0x2BF, rayCastHook + 0x5, 5, 0x2BF + 1)
            ]);

            AsmHelper.WriteAbsoluteAddresses64(codeBytes, [
                (GameManagerImp.Base, 0x6 + 2),
                (GameManagerImp.Base, 0x70 + 2)
            ]);

            memoryService.WriteBytes(raycastCode, codeBytes);


            memoryService.Write(GetGravityPtr(), (byte)1);

            hookManager.InstallHook(triggersAndSpaceCode, triggersAndSpaceHook, [
                0x4C, 0x8B, 0x7C, 0x24, 0x70, 0x48, 0x8B, 0x43, 0x08
            ]);
            hookManager.InstallHook(ctrlCode, ctrlHook, [
                0x81, 0x8B, 0x28, 0x02, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00
            ]);
            hookManager.InstallHook(coordsCode, coordsHook,
                [0x66, 0x0F, 0x7F, 0xB8, 0x90, 0x00, 0x00, 0x00]);
            hookManager.InstallHook(raycastCode, rayCastHook, [0x48, 0x8D, 0x54, 0x24, 0x20]);
        }

        private void SetupNoClipHooks32Bit(nint triggersAndSpaceCode, nint ctrlCode, nint coordsCode,
            nint raycastCode)
        {
            var triggersAndSpaceHook = Hooks.TriggersAndSpace;
            var ctrlHook = Hooks.Ctrl;
            var coordsHook = Hooks.NoClipUpdateCoords;
            var rayCastHook = Hooks.ProcessPhysics;

            var zDirectionLoc = CustomCodeOffsets.Base + (int)CustomCodeOffsets.NoClip.ZDirection;
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_TriggersAndSpace32);
            var bytes = AsmHelper.GetJmpOriginOffsetBytes(triggersAndSpaceHook, 9, triggersAndSpaceCode + 0x52);
            Array.Copy(bytes, 0, codeBytes, 0x4D + 1, 4);
            AsmHelper.WriteAbsoluteAddresses32(codeBytes, [
                (zDirectionLoc, 0x19 + 2),
                (zDirectionLoc, 0x2F + 2),
                (zDirectionLoc, 0x45 + 2)
            ]);
            memoryService.WriteBytes(triggersAndSpaceCode, codeBytes);


            codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_CtrlCheck32);
            AsmHelper.WriteAbsoluteAddress32(codeBytes, zDirectionLoc, 2);
            bytes = AsmHelper.GetJmpOriginOffsetBytes(ctrlHook, 0xA, ctrlCode + 0xC);
            Array.Copy(bytes, 0, codeBytes, 0x7 + 1, 4);

            memoryService.WriteBytes(ctrlCode, codeBytes);


            codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_UpdateCoords32);

            AsmHelper.WriteAbsoluteAddresses32(codeBytes, [
                (GameManagerImp.Base, 0x1 + 2),
                (GameManagerImp.Base, 0x4E + 2),
                (zDirectionLoc, 0x89 + 2),
                (zDirectionLoc, 0xB0 + 2),
                (GameManagerImp.Base, 0xBB + 2),
            ]);

            AsmHelper.WriteJumpOffsets(codeBytes, [
                (coordsHook, 20, coordsCode + 0x127, 0x127 + 1),
                (coordsHook, 5, coordsCode + 0x132, 0x132 + 1)
            ]);

            memoryService.WriteBytes(coordsCode, codeBytes);

            var frameCounter = CustomCodeOffsets.Base + (int)CustomCodeOffsets.NoClip.FrameCounter;
            var rayInput = CustomCodeOffsets.Base + (int)CustomCodeOffsets.NoClip.RayInput;
            var rayOutput = CustomCodeOffsets.Base + (int)CustomCodeOffsets.NoClip.RayOutput;
            var mapId = CustomCodeOffsets.Base + (int)CustomCodeOffsets.NoClip.MapId;

            var raycastFunc = Functions.HavokRayCast;
            var convertToMap = Functions.ConvertPxRigidToMapEntity;
            var convertToMapId = Functions.ConvertMapEntityToGameId;

            codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_RayCast32);

            AsmHelper.WriteAbsoluteAddresses32(codeBytes, [
                (GameManagerImp.Base, 0x1 + 1),
                (frameCounter, 0xF + 2),
                (frameCounter, 0x15 + 2),
                (frameCounter, 0x22 + 2),
                (rayOutput, 0x59 + 2),
                (rayInput, 0x6E + 2),
                (rayOutput + (0x30 * 1), 0xA9 + 2),
                (rayInput, 0xB8 + 2),
                (rayOutput + (0x30 * 2), 0xE8 + 2),
                (rayInput, 0xF7 + 2),
                (rayOutput + (0x30 * 3), 0x127 + 2),
                (rayInput, 0x136 + 2),
                (rayOutput + (0x30 * 4), 0x166 + 2),
                (rayInput, 0x175 + 2),
                (rayOutput, 0x1BB + 2),
                (rayOutput, 0x1E4 + 2),
                (mapId, 0x1FC + 2)
            ]);

            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (raycastCode + 0xA4, raycastFunc, 5, 0xA4 + 1),
                (raycastCode + 0xE3, raycastFunc, 5, 0xE3 + 1),
                (raycastCode + 0x122, raycastFunc, 5, 0x122 + 1),
                (raycastCode + 0x161, raycastFunc, 5, 0x161 + 1),
                (raycastCode + 0x1A0, raycastFunc, 5, 0x1A0 + 1),
                (raycastCode + 0x1F3, convertToMap, 5, 0x1F3 + 1),
                (raycastCode + 0x202, convertToMapId, 5, 0x202 + 1),
                (raycastCode + 0x24D, rayCastHook + 0x6, 5, 0x24D + 1)
            ]);

            memoryService.WriteBytes(raycastCode, codeBytes);

            memoryService.Write(GetGravityPtr(), (byte)1);

            hookManager.InstallHook(triggersAndSpaceCode, triggersAndSpaceHook, [0x8B, 0x56, 0x08, 0x89, 0x86, 0x04, 0x01, 0x00, 0x00
            ]);
            hookManager.InstallHook(ctrlCode, ctrlHook, [0x81, 0x8E, 0x28, 0x02, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00]);
            hookManager.InstallHook(coordsCode, coordsHook, [0xF3, 0x0F, 0x7E, 0x45, 0xD0]);
            hookManager.InstallHook(raycastCode, rayCastHook, [0x8B, 0x8E, 0xB8, 0x00, 0x00, 0x00]);
        }

        public void SetNoClipSpeed(byte[] xBytes, byte[] yBytes)
        {
            if (PatchManager.IsScholar())
            {
                memoryService.WriteBytes(CustomCodeOffsets.Base + (int)CustomCodeOffsets.NoClip.UpdateCoords + 0xA4 + 1,
                    xBytes);
                memoryService.WriteBytes(CustomCodeOffsets.Base + (int)CustomCodeOffsets.NoClip.UpdateCoords + 0x60 + 1,
                    yBytes);
            }
            else
            {
                memoryService.WriteBytes(CustomCodeOffsets.Base + (int)CustomCodeOffsets.NoClip.UpdateCoords + 0x71 + 1,
                    xBytes);
                memoryService.WriteBytes(CustomCodeOffsets.Base + (int)CustomCodeOffsets.NoClip.UpdateCoords + 0x3E + 1,
                    yBytes);
            }
        }

        private nint GetGravityPtr() => memoryService.FollowPointers(GameManagerImp.Base, [
            GameManagerImp.PlayerCtrl,
            ChrCtrl.ChrPhysicsCtrlPtr,
            ChrCtrl.ChrPhysicsCtrl.Gravity
        ], false);

        public void ToggleKillboxHook(bool isEnabled)
        {
            var code = CustomCodeOffsets.Base + CustomCodeOffsets.Killbox;

            if (!isEnabled)
            {
                hookManager.UninstallHook(code);
                return;
            }

            var hookLoc = Hooks.KillboxFlagSet;
            if (PatchManager.IsScholar())
            {
                InstallScholarKillboxHook(code, hookLoc);
            }
            else
            {
                InstallVanillaKillboxHook(code, hookLoc);
            }
        }

        private void InstallScholarKillboxHook(nint code, nint hookLoc)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.Killbox64);
            AsmHelper.WriteAbsoluteAddress64(codeBytes, GameManagerImp.Base, 0x1 + 2);
            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (code + 0x2b, hookLoc + 0xA, 5, 0x2b + 1)
            ]);

            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code, hookLoc, [
                0x48, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00
            ]);
        }

        private void InstallVanillaKillboxHook(nint code, nint hookLoc)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.Killbox32);
            AsmHelper.WriteAbsoluteAddress32(codeBytes, GameManagerImp.Base, 0x1 + 1);
            var bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 6, code + 0x1E);
            Array.Copy(bytes, 0, codeBytes, 0x19 + 1, 4);
            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code, hookLoc, [0x53, 0x89, 0xE3, 0x83, 0xEC, 0x08]);
        }

        public void ToggleDrawHitbox(bool isDrawHitboxEnabled)
        {
            dllManager.ToggleRender(DrawType.Hitbox, isDrawHitboxEnabled);
        }

        public void ToggleDrawEvent(DrawType eventType, bool isDrawEventEnabled)
        {
            dllManager.ToggleRender(eventType, isDrawEventEnabled);
        }

        public void ToggleDrawSound(bool isDrawSoundEnabled) =>
            dllManager.ToggleRender(DrawType.Sound, isDrawSoundEnabled);

        public void ToggleTargetingView(bool isTargetingViewEnabled) =>
            dllManager.ToggleRender(DrawType.TargetingView, isTargetingViewEnabled);

        public void ToggleRagdoll(bool isDrawRagrollEnabled) =>
            dllManager.ToggleRender(DrawType.Ragdoll, isDrawRagrollEnabled);

        public void ToggleHideChr(bool isHideCharactersEnabled) =>
            memoryService.WriteBytes(Patches.HideChrModels,
                isHideCharactersEnabled ? [0x75, 0x5] : [0x74, 0x5]);

        public void ToggleHideMap(bool isHideMapEnabled) =>
            memoryService.WriteBytes(Patches.HideMap + 0x1,
                isHideMapEnabled ? [0x89] : [0x88]);

        private bool _isSlowdownHookInstalled;

        public void SetGameSpeed(float value)
        {
            if (_isSlowdownHookInstalled) RemoveSlowdownHook();
            if (dllManager.IsSpeedInjected()) dllManager.SetSpeed(1);

            if (value < 1)
            {
                InstallSlowdownHook(value);
            }
            else if (value > 1)
            {
                dllManager.SetSpeed(value);
            }
        }

        private void InstallSlowdownHook(float value)
        {
            var slowdownFactor = CustomCodeOffsets.Base + CustomCodeOffsets.SlowdownFactor;
            memoryService.Write(slowdownFactor, value);

            if (_isSlowdownHookInstalled) return;
            var hookLoc = Hooks.ReduceGameSpeed;
            var code = CustomCodeOffsets.Base + CustomCodeOffsets.SlowdownCode;

            if (PatchManager.IsScholar())
            {
                InstallScholarSlowdownHook(slowdownFactor, hookLoc, code);
            }
            else
            {
                InstallVanillaSlowdownHook(slowdownFactor, hookLoc, code);
            }
        }

        private void InstallScholarSlowdownHook(nint slowdownFactor, nint hookLoc, nint code)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.ReduceSpeed64);
            AsmHelper.WriteRelativeOffsets(bytes, [
                (code + 0x4, slowdownFactor, 8, 0x4 + 4),
                (code + 0x16, hookLoc + 0x6, 5, 0x16 + 1)
            ]);

            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code, hookLoc, [0xF3, 0x0F, 0x10, 0x32, 0x31, 0xDB]);
            _isSlowdownHookInstalled = true;
        }

        private void InstallVanillaSlowdownHook(nint slowdownFactor, nint hookLoc, nint code)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.ReduceSpeed32);
            AsmHelper.WriteAbsoluteAddress32(codeBytes, slowdownFactor, 0x7 + 4);
            var bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 7, code + 0x18);
            Array.Copy(bytes, 0, codeBytes, 0x13 + 1, 4);

            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code, hookLoc, [0x8B, 0x7B, 0x08, 0xF3, 0x0F, 0x10, 0x07]);
            _isSlowdownHookInstalled = true;
        }

        private void RemoveSlowdownHook()
        {
            hookManager.UninstallHook(CustomCodeOffsets.Base + CustomCodeOffsets.SlowdownCode);
            _isSlowdownHookInstalled = false;
        }

        public void ToggleRagdollEsp(bool isSeeThroughwallsEnabled) =>
            dllManager.ToggleRender(DrawType.RagdollEsp, isSeeThroughwallsEnabled);

        public void ToggleDrawCol(bool isDrawCollisionEnabled) =>
            dllManager.ToggleRender(DrawType.Collision, isDrawCollisionEnabled);

        public void ToggleDrawKillbox(bool isDrawKillboxEnabled) =>
            dllManager.ToggleRender(DrawType.CollisionKillbox, isDrawKillboxEnabled);

        public void ToggleColWireframe(bool isColWireframeEnabled) =>
            dllManager.ToggleRender(DrawType.CollisionWireframe, isColWireframeEnabled);

        public void ToggleDrawObj(bool isDrawObjEnabled) =>
            dllManager.ToggleRender(DrawType.Objects, isDrawObjEnabled);

        public void ToggleSnowstormHook(bool isSnowstormDisabled)
        {
            var code = CustomCodeOffsets.Base + CustomCodeOffsets.Snowstorm;

            if (!isSnowstormDisabled)
            {
                hookManager.UninstallHook(code);
                return;
            }

            var origin = Hooks.SetEventWrapper;

            if (PatchManager.IsScholar())
            {
                InstallScholarDisableSnowstorm(code, origin);
            }
            else
            {
                InstallVanillaDisableSnowstorm(code, origin);
            }
        }

        private void InstallScholarDisableSnowstorm(nint code, nint origin)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.DisableSnowstorm64);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 11, code + 0x1C);
            Array.Copy(jmpBytes, 0, bytes, 0x17 + 1, 4);
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code, origin, [
                0x41, 0x0F, 0xB6, 0xF8, 0x48, 0x8B, 0x88, 0xF0, 0x22, 0x00, 0x00
            ]);
        }

        private void InstallVanillaDisableSnowstorm(nint code, nint origin)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.DisableSnowstorm32);
            AsmHelper.WriteRelativeOffsets(bytes, [
                (code + 0xD, origin + 6, 6, 0xD + 2),
                (code + 0x1A, origin + 6, 5, 0x1A + 1)
            ]);
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code, origin, [0x8B, 0x88, 0xCC, 0x0C, 0x00, 0x00]);
        }

        public void ToggleMemoryTimer(bool isMemoryTimerDisabled)
        {
            var code = CustomCodeOffsets.Base + CustomCodeOffsets.DisableMemoryTimer;

            if (!isMemoryTimerDisabled)
            {
                hookManager.UninstallHook(code);
                return;
            }

            var origin = Hooks.EzStateCompareTimer;
            if (PatchManager.IsScholar())
            {
                InstallScholarMemoryTimerHook(code, origin);
            }
            else
            {
                InstallVanillaMemoryTimerHook(code, origin);
            }
        }

        private void InstallScholarMemoryTimerHook(nint code, nint origin)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.DisableMemoryTimer64);
            AsmHelper.WriteAbsoluteAddress64(codeBytes, GameManagerImp.Base, 0x2 + 2);

            var bytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 7, code + 0xC1);
            Array.Copy(bytes, 0, codeBytes, 0xBC + 1, 4);

            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code, origin, [0x66, 0x0F, 0x6E, 0x30, 0x0F, 0x5B, 0xF6]);
        }

        private void InstallVanillaMemoryTimerHook(nint code, nint origin)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.DisableMemoryTimer32);
            AsmHelper.WriteAbsoluteAddress32(codeBytes, GameManagerImp.Base, 0x2 + 2);
            var bytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 7, code + 0x79);
            Array.Copy(bytes, 0, codeBytes, 0x74 + 1, 4);

            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code, origin, [0x66, 0x0F, 0x6E, 0x00, 0x0F, 0x5B, 0xC0]);
        }

        public void ToggleIvorySkip(bool isIvorySkipEnabled)
        {
            var code = CustomCodeOffsets.Base + CustomCodeOffsets.IvorySkip;
            var knightsCode = CustomCodeOffsets.Base + CustomCodeOffsets.IvoryKnights;

            if (!isIvorySkipEnabled)
            {
                hookManager.UninstallHook(code);
                hookManager.UninstallHook(knightsCode);
                return;
            }

            var origin = Functions.SetEvent;
            var getMapEntity = Functions.GetMapEntityWithAreaIdAndObjId;
            var getComponent = Functions.GetMapObjStateActComponent;
            var setSharedFlag = Hooks.SetSharedFlag;

            if (PatchManager.IsScholar())
            {
                InstallScholarIvoryHooks(code, knightsCode, origin, getMapEntity, getComponent, setSharedFlag);
            }
            else
            {
                InstallVanillaIvoryHooks(code, knightsCode, origin, getMapEntity, getComponent, setSharedFlag);
            }
        }

        private void InstallScholarIvoryHooks(
            nint code, nint knightsCode, nint origin, nint getMapEntity, nint getComponent, nint setSharedFlag)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.IvorySkip64);

            AsmHelper.WriteAbsoluteAddresses64(bytes, [
                (getMapEntity, 0x5C + 2),
                (getComponent, 0x66 + 2),
                (origin, 0x70 + 2)
            ]);

            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 5, code + 0xDB);
            Array.Copy(jmpBytes, 0, bytes, 0xD6 + 1, 4);

            memoryService.WriteBytes(code, bytes);

            bytes = AsmLoader.GetAsmBytes(AsmScript.IvoryKnights64);
            jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(setSharedFlag, 8, knightsCode + 0x24);
            Array.Copy(jmpBytes, 0, bytes, 0x1F + 1, 4);
            memoryService.WriteBytes(knightsCode, bytes);

            hookManager.InstallHook(code, origin, [0x48, 0x89, 0x74, 0x24, 0x10]);
            hookManager.InstallHook(knightsCode, setSharedFlag, [
                0x44, 0x88, 0x84, 0x08, 0xA1, 0x03, 0x00, 0x00
            ]);
        }

        private void InstallVanillaIvoryHooks(
            nint code, nint knightsCode, nint origin, nint getMapEntity, nint getComponent, nint setSharedFlag)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.IvorySkip32);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 6, code + 0xA7);
            Array.Copy(jmpBytes, 0, bytes, 0xA1 + 1, 4);
            AsmHelper.WriteAbsoluteAddresses32(bytes, [
                (origin, 0x25 + 1),
                (getMapEntity, 0x46 + 3),
                (getComponent, 0x4D + 3)
            ]);

            memoryService.WriteBytes(code, bytes);

            bytes = AsmLoader.GetAsmBytes(AsmScript.IvoryKnights32);
            jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(setSharedFlag, 7, knightsCode + 0x21);
            Array.Copy(jmpBytes, 0, bytes, 0x1B + 1, 4);
            memoryService.WriteBytes(knightsCode, bytes);

            hookManager.InstallHook(code, origin, [0x55, 0x89, 0xE5, 0x83, 0xEC, 0x08]);
            hookManager.InstallHook(knightsCode, setSharedFlag, [0x88, 0x94, 0x08, 0xA1, 0x02, 0x00, 0x00]);
        }

        public void SetObjState(long areaId, Obj obj)
        {
            var getMapEntity = Functions.GetMapEntityWithAreaIdAndObjId;
            var getComponent = Functions.GetMapObjStateActComponent;

            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.SetObjState64);
                AsmHelper.WriteAbsoluteAddresses64(bytes, [
                    ((nint)areaId, 0x4 + 2),
                    (obj.Id, 0xE + 2),
                    (getMapEntity, 0x18 + 2),
                    (getComponent, 0x2E + 2),
                    (obj.State, 0x41 + 2)
                ]);

                memoryService.AllocateAndExecute(bytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.SetObjState32);
                AsmHelper.WriteAbsoluteAddresses32(bytes, [
                    (getMapEntity, 1),
                    (getComponent, 0x5 + 1),
                    (obj.Id, 0xA + 1),
                    ((nint)areaId, 0xF + 1),
                    (obj.State, 0x34 + 1)
                ]);
                memoryService.AllocateAndExecute(bytes);
            }
        }

        public void DisableNavimesh(long areaId, Navimesh naviData)
        {
            var eventPointMan = memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.EventManager,
                GameManagerImp.EventManagerOffsets.EventPointManager
            ], true);

            var getNaviLoc = Functions.GetNavimeshLoc;
            var disableNavi = Functions.DisableNavimesh;

            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.DisableNavimesh64);
                AsmHelper.WriteAbsoluteAddresses64(bytes, [
                    (eventPointMan, 2),
                    ((nint)areaId, 0xA + 2),
                    (naviData.EventId, 0x14 + 2),
                    (getNaviLoc, 0x25 + 2),
                    (naviData.State, 0x34 + 2),
                    (disableNavi, 0x3E + 2)
                ]);

                memoryService.AllocateAndExecute(bytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.DisableNavimesh32);
                AsmHelper.WriteAbsoluteAddresses32(bytes, [
                    (eventPointMan, 1),
                    (naviData.EventId, 0x5 + 1),
                    ((nint)areaId, 0xA + 1),
                    (getNaviLoc, 0xF + 1),
                    (naviData.State, 0x18 + 1),
                    (disableNavi, 0x1D + 1)
                ]);
                memoryService.AllocateAndExecute(bytes);
            }
        }

        public void DisableWhiteDoor(long areaId, WhiteDoor whiteDoorData)
        {
            var getMapEntity = Functions.GetMapEntityWithAreaIdAndObjId;
            var getComponent = Functions.GetWhiteDoorComponent;

            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.DisableWhiteDoorKeyGuide64);
                AsmHelper.WriteAbsoluteAddresses64(bytes, [
                    ((nint)areaId, 0x4 + 2),
                    (whiteDoorData.ObjId, 0xE + 2),
                    (getMapEntity, 0x18 + 2),
                    (getComponent, 0x27 + 2)
                ]);

                memoryService.AllocateAndExecute(bytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.DisableWhiteDoorKeyGuide32);
                AsmHelper.WriteAbsoluteAddresses32(bytes, [
                    (whiteDoorData.ObjId, 1),
                    ((nint)areaId, 0x5 + 1),
                    (getMapEntity, 0xA + 1),
                    (getComponent, 0x16 + 1)
                ]);
                memoryService.AllocateAndExecute(bytes);
            }
        }

        public List<InventorySpell> GetInventorySpells()
        {
            var spellBase = memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr,
                GameManagerImp.GameDataManagerOffsets.Inventory.InventoryLists,
                GameManagerImp.GameDataManagerOffsets.Inventory.ItemInventory2BagListPtr,
                GameManagerImp.GameDataManagerOffsets.Inventory.ItemInventory2BagList.ItemInvetory2SpellListPtr
            ], true);

            var count = memoryService.Read<byte>(
                spellBase + GameManagerImp.GameDataManagerOffsets.Inventory.ItemInvetory2SpellList.Count
            );
            return count == 0 ? [] : ReadSpellList(spellBase, count);
        }

        private List<InventorySpell> ReadSpellList(nint spellBase, int count)
        {
            List<InventorySpell> currentSpells = [];
            var current = memoryService.ReadPointer(
                spellBase + GameManagerImp.GameDataManagerOffsets.Inventory.ItemInvetory2SpellList.ListStart);

            for (int i = 0; i < count && current != 0; i++)
            {
                var spellId = memoryService.Read<int>(
                    current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.SpellId);
                var isEquipped = memoryService.Read<byte>(
                    current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.IsEquipped);
                var slotReq = memoryService.Read<byte>(
                    current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.SlotReq);

                currentSpells.Add(new InventorySpell(spellId, isEquipped == 2, current, slotReq));

                current = memoryService.ReadPointer(
                    current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.NextPtr);
            }

            return currentSpells;
        }

        public List<EquippedSpell> GetEquippedSpells()
        {
            var currentSpell = GetCurrentSpellPtr();
            List<EquippedSpell> currentSpells = [];

            int chunkSize = PatchManager.Current.Edition == GameEdition.Scholar ? 0x10 : 0x8;

            for (int i = 0; i < 14; i++)
            {
                currentSpells.Add(new EquippedSpell(memoryService.Read<int>(currentSpell), i));
                currentSpell += chunkSize;
            }

            return currentSpells;
        }

        private nint GetCurrentSpellPtr()
        {
            return memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.PlayerCtrl,
                ChrCtrl.ChrAsmCtrl,
                ChrCtrl.EquippedSpellsStart
            ], false);
        }

        public int GetTotalAvailableSlots()
        {
            RefreshSpellSlots();

            var inventory = memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr
            ], true);
            var getNumOfSlots1 = Functions.GetNumOfSpellSlots1;
            var getNumOfSlots2 = Functions.GetNumOfSpellSlots2;
            var slotsLoc = CustomCodeOffsets.Base + CustomCodeOffsets.NumOfSpellSlots;


            byte[] bytes;

            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                bytes = AsmLoader.GetAsmBytes(AsmScript.GetNumOfSlots64);

                AsmHelper.WriteAbsoluteAddresses64(bytes, [
                    (slotsLoc, 2),
                    (inventory, 0xA + 2),
                    (getNumOfSlots1, 0x17 + 2),
                    (getNumOfSlots2, 0x2C + 2)
                ]);
            }
            else
            {
                bytes = AsmLoader.GetAsmBytes(AsmScript.GetNumOfSlots32);
                AsmHelper.WriteAbsoluteAddresses32(bytes, [
                    (slotsLoc, 1),
                    (inventory, 0x5 + 1),
                    (getNumOfSlots1, 0xC + 1),
                    (getNumOfSlots2, 0x17 + 1)
                ]);
            }

            memoryService.AllocateAndExecute(bytes);


            return memoryService.Read<int>(slotsLoc);
        }

        private void RefreshSpellSlots()
        {
            var bagList = memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr,
                GameManagerImp.GameDataManagerOffsets.Inventory.InventoryLists,
                GameManagerImp.GameDataManagerOffsets.Inventory.ItemInventory2BagListPtr
            ], true);

            var refreshFunc = Functions.UpdateSpellSlots;
            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.UpdateSpellSlots64);

                AsmHelper.WriteAbsoluteAddresses64(bytes, [
                    (bagList, 2),
                    (refreshFunc, 0xA + 2)
                ]);
                memoryService.AllocateAndExecute(bytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.UpdateSpellSlots32);
                AsmHelper.WriteAbsoluteAddresses32(bytes, [
                    (bagList, 1),
                    (refreshFunc, 0x5 + 1)
                ]);
                memoryService.AllocateAndExecute(bytes);
            }
        }

        public void AttuneSpell(int slotIndex, nint entryAddr)
        {
            var inventoryLists = memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr,
                GameManagerImp.GameDataManagerOffsets.Inventory.InventoryLists
            ], true);

            var attuneFunc = Functions.AttuneSpell;

            if (PatchManager.IsScholar())
            {
                AttuneScholarSpell(slotIndex, entryAddr, inventoryLists, attuneFunc);
            }
            else
            {
                AttuneVanillaSpell(slotIndex, entryAddr, inventoryLists, attuneFunc);
            }
        }

        private void AttuneScholarSpell(int slotIndex, nint entryAddr, nint inventoryLists, nint attuneFunc)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.AttuneSpell64);
            AsmHelper.WriteAbsoluteAddresses64(bytes, [
                (inventoryLists, 2),
                (slotIndex + 0x1C, 0xA + 2),
                (entryAddr, 0x14 + 2),
                (attuneFunc, 0x1E + 2)
            ]);
            memoryService.AllocateAndExecute(bytes);
        }

        private void AttuneVanillaSpell(int slotIndex, nint entryAddr, nint inventoryLists, nint attuneFunc)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.AttuneSpell32);
            AsmHelper.WriteAbsoluteAddresses32(bytes, [
                (inventoryLists, 1),
                (entryAddr, 0x5 + 1),
                (slotIndex + 0x1C, 0xB + 1),
                (attuneFunc, 0x11 + 1)
            ]);

            memoryService.AllocateAndExecute(bytes);
        }

        public void Reset()
        {
            _isSlowdownHookInstalled = false;
        }

        public void ToggleLightGutter(bool isEnabled)
        {
            var code = CustomCodeOffsets.Base + CustomCodeOffsets.LightGutter;

            if (isEnabled)
            {
                if (PatchManager.Current.Edition == GameEdition.Scholar) InstallLightGutter64(code);
                else InstallLightGutter32(code);
            }
            else
            {
                hookManager.UninstallHook(code);
            }
        }

        private void InstallLightGutter64(nint code)
        {
            var hookLoc = Hooks.LightGutter;
            var bytes = AsmLoader.GetAsmBytes(AsmScript.LightGutter64);
            AsmHelper.WriteRelativeOffsets(bytes, [
                (code, MapId, 10, 0x0 + 2),
                (code + 0x21, hookLoc + 0x8, 5, 0x21 + 1)
            ]);
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code, hookLoc, [0x66, 0x0F, 0x7F, 0x87, 0xE0, 0x00, 0x00, 0x00]);
        }

        private void InstallLightGutter32(nint code)
        {
            var hookLoc = Hooks.LightGutter;
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.LightGutter32);
            AsmHelper.WriteAbsoluteAddress32(codeBytes, MapId, 0x8 + 2);
            var bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 7, code + 0x25);
            Array.Copy(bytes, 0, codeBytes, 0x21 + 1, 4);
            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code, hookLoc, [0xF3, 0x0F, 0x7E, 0x86, 0x58, 0xFF, 0xFF, 0xFF]);
        }

        public void ToggleShadedFog(bool isNoFogEnabled)
        {
            var closeFogCode = CustomCodeOffsets.Base + CustomCodeOffsets.NoFogClose;
            var farFogCode = CustomCodeOffsets.Base + CustomCodeOffsets.NoFogFar;
            var fogCamCode = CustomCodeOffsets.Base + CustomCodeOffsets.NoFogCam;
            if (!isNoFogEnabled)
            {
                hookManager.UninstallHook(closeFogCode);
                hookManager.UninstallHook(farFogCode);
                hookManager.UninstallHook(fogCamCode);
                return;
            }

            if (PatchManager.IsScholar())
            {
                InstallNoFog64(closeFogCode, farFogCode, fogCamCode);
            }
            else
            {
                InstallNoFog32(closeFogCode, farFogCode, fogCamCode);
            }
        }

        private void InstallNoFog64(nint closeFogCode, nint farFogCode, nint fogCamCode)
        {
            var hookLoc = Hooks.NoShadedFogClose;
            var bytes = AsmLoader.GetAsmBytes(AsmScript.NoShadedFogClose64);
            AsmHelper.WriteRelativeOffsets(bytes, [
                (closeFogCode + 0x7, MapId, 10, 0x7 + 2),
                (closeFogCode + 0x1E, hookLoc + 0x7, 5, 0x1E + 1)
            ]);
            memoryService.WriteBytes(closeFogCode, bytes);
            hookManager.InstallHook(closeFogCode, hookLoc, [0x0F, 0x57, 0xC0, 0x66, 0x0F, 0x6E, 0xE0]);

            hookLoc = Hooks.NoShadedFogFar;
            bytes = AsmLoader.GetAsmBytes(AsmScript.NoShadedFogFar64);
            AsmHelper.WriteRelativeOffsets(bytes, [
                (farFogCode + 0x7, MapId, 10, 0x7 + 2),
                (farFogCode + 0x11, hookLoc + 0x7, 6, 0x11 + 2),
                (farFogCode + 0x17, hookLoc + 0xC, 5, 0x17 + 1)
            ]);

            memoryService.WriteBytes(farFogCode, bytes);
            hookManager.InstallHook(farFogCode, hookLoc, [0x4C, 0x89, 0x50, 0x10, 0x48, 0x89, 0xC3]);


            hookLoc = Hooks.NoShadedFogCam;
            bytes = AsmLoader.GetAsmBytes(AsmScript.NoShadedFogCam64);
            AsmHelper.WriteRelativeOffsets(bytes, [
                (fogCamCode, MapId, 10, 0x0 + 2),
                (fogCamCode + 0x13, hookLoc + 0x7, 5, 0x13 + 1)
            ]);

            memoryService.WriteBytes(fogCamCode, bytes);
            hookManager.InstallHook(fogCamCode, hookLoc, [0x89, 0x44, 0x24, 0x58, 0x8B, 0x41, 0x38]);
        }

        private void InstallNoFog32(nint closeFogCode, nint farFogCode, nint fogCamCode)
        {
            var hookLoc = Hooks.NoShadedFogClose;
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoShadedFogClose32);
            AsmHelper.WriteAbsoluteAddress32(codeBytes, MapId, 0x4 + 2);
            var bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 7, closeFogCode + 0x1D);
            Array.Copy(bytes, 0, codeBytes, 0x18 + 1, 4);

            memoryService.WriteBytes(closeFogCode, codeBytes);
            hookManager.InstallHook(closeFogCode, hookLoc, [0x0F, 0xB6, 0x46, 0x07, 0x89, 0x45, 0xAC]);


            hookLoc = Hooks.NoShadedFogFar;
            codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoShadedFogFar32);
            AsmHelper.WriteAbsoluteAddress32(codeBytes, MapId, 0x5 + 2);
            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (farFogCode + 0xF, hookLoc + 0x5, 6, 0xF + 2),
                (farFogCode + 0x17, hookLoc + 0xA, 5, 0x17 + 1)
            ]);

            memoryService.WriteBytes(farFogCode, codeBytes);
            hookManager.InstallHook(farFogCode, hookLoc, [0xF3, 0x0F, 0x11, 0x46, 0x10]);


            hookLoc = Hooks.NoShadedFogCam;
            codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoShadedFogCam32);
            AsmHelper.WriteAbsoluteAddress32(codeBytes, MapId, 0x0 + 2);
            bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 5, fogCamCode + 0x16);
            Array.Copy(bytes, 0, codeBytes, 0x11 + 1, 4);

            memoryService.WriteBytes(fogCamCode, codeBytes);
            hookManager.InstallHook(fogCamCode, hookLoc, [0x89, 0x4D, 0xE4, 0x31, 0xC9]);
        }
    }
}