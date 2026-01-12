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
            nint saveLoadSystem;
            if (PatchManager.IsScholar())
            {
                var gameMan = memoryService.Read<nint>(GameManagerImp.Base);
                saveLoadSystem = memoryService.Read<nint>(gameMan + GameManagerImp.SaveLoadSystem);
            }
            else
            {
                var gameMan = memoryService.Read<int>(GameManagerImp.Base);
                saveLoadSystem = memoryService.Read<int>(gameMan + GameManagerImp.SaveLoadSystem);
            }

            memoryService.Write((IntPtr)saveLoadSystem + GameManagerImp.SaveLoadSystemOffsets.ForceSaveFlag1, 2);
            memoryService.Write((IntPtr)saveLoadSystem + GameManagerImp.SaveLoadSystemOffsets.ForceSaveFlag2, (byte)1);
        }

        public void ToggleCreditSkip(bool isCreditSkipEnabled)
        {
            var code = CodeCaveOffsets.Base + (int)CodeCaveOffsets.CreditSkip.Code;

            if (!isCreditSkipEnabled)
            {
                hookManager.UninstallHook(code.ToInt64());
                return;
            }

            var hookLoc = Hooks.CreditSkip;
            var modifyOnceFlag = CodeCaveOffsets.Base + (int)CodeCaveOffsets.CreditSkip.ModifyOnceFlag;
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

        private void InstallScholarCreditSkip(IntPtr modifyOnceFlag, long hookLoc, IntPtr code)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.CreditSkip64);
            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (code.ToInt64() + 0x7, modifyOnceFlag.ToInt64(), 7, 0x7 + 2),
                (code.ToInt64() + 0x17, modifyOnceFlag.ToInt64(), 10, 0x17 + 2),
                (code.ToInt64() + 0x21, hookLoc + 7, 5, 0x21 + 1)
            ]);
            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code.ToInt64(), hookLoc, [0x48, 0x81, 0xEC, 0x20, 0x02, 0x00, 0x00]);
        }

        private void InstallVanillaCreditSkip(IntPtr modifyOnceFlag, long hookLoc, IntPtr code)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.CreditSkip32);
            var bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 6, code + 0x25);
            Array.Copy(bytes, 0, codeBytes, 0x20 + 1, 4);
            AsmHelper.WriteAbsoluteAddresses32(codeBytes, [
                (modifyOnceFlag.ToInt64(), 0x6 + 2),
                (modifyOnceFlag.ToInt64(), 0x16 + 2)
            ]);

            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code.ToInt64(), hookLoc, [0x81, 0xEC, 0xFC, 0x01, 0x00, 0x00]);
        }

        public void Toggle100Drop(bool is100DropEnabled)
        {
            var dropCountHook = Hooks.NumOfDrops;
            var dropCountCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.Drop100.DropCount;

            memoryService.WriteBytes(Patches.DropRate, PatchDefinitions.DropRate.Get(is100DropEnabled));

            if (!is100DropEnabled)
            {
                hookManager.UninstallHook(dropCountCode.ToInt64());
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

        private void Setup100DropScholar(IntPtr dropCountCode, long dropCountHook)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.DropCount64);
            var bytes = AsmHelper.GetJmpOriginOffsetBytes(dropCountHook, 5, dropCountCode + 0xA);
            Array.Copy(bytes, 0, codeBytes, 0x5 + 1, 4);
            memoryService.WriteBytes(dropCountCode, codeBytes);
            hookManager.InstallHook(dropCountCode.ToInt64(), dropCountHook, [0x41, 0x0F, 0xB6, 0x47, 0x01]);
        }

        private void Setup100DropVanilla(IntPtr dropCountCode, long dropCountHook)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.DropCount32);
            var bytes = AsmHelper.GetJmpOriginOffsetBytes(dropCountHook, 5, dropCountCode + 0xB);
            Array.Copy(bytes, 0, codeBytes, 0x6 + 1, 4);
            memoryService.WriteBytes(dropCountCode, codeBytes);

            hookManager.InstallHook(dropCountCode.ToInt64(), dropCountHook, [0x0F, 0xB6, 0x51, 0x01, 0x40]);
        }

        public void ToggleNoClip(bool isNoClipEnabled)
        {
            var triggersAndSpaceCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.TriggersAndSpaceCheck;
            var ctrlCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.CtrlCheck;
            var coordsCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.UpdateCoords;
            var raycastCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.RayCastCode;

            if (!isNoClipEnabled)
            {
                hookManager.UninstallHook(coordsCode.ToInt64());
                memoryService.WriteByte(GetGravityPtr(), 0);
                hookManager.UninstallHook(triggersAndSpaceCode.ToInt64());
                hookManager.UninstallHook(ctrlCode.ToInt64());
                hookManager.UninstallHook(raycastCode.ToInt64());
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

        private void SetupNoClipHooks64Bit(IntPtr triggersAndSpaceCode, IntPtr ctrlCode, IntPtr coordsCode,
            IntPtr raycastCode)
        {
            var triggersAndSpaceHook = Hooks.TriggersAndSpace;
            var ctrlHook = Hooks.Ctrl;
            var coordsHook = Hooks.NoClipUpdateCoords;
            var rayCastHook = Hooks.ProcessPhysics;

            var zDirectionLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.ZDirection;

            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_TriggersAndSpace64);
            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (triggersAndSpaceCode.ToInt64() + 0x1C, zDirectionLoc.ToInt64(), 7, 0x1C + 2),
                (triggersAndSpaceCode.ToInt64() + 0x35, zDirectionLoc.ToInt64(), 7, 0x35 + 2),
                (triggersAndSpaceCode.ToInt64() + 0x4E, zDirectionLoc.ToInt64(), 7, 0x4E + 2),
                (triggersAndSpaceCode.ToInt64() + 0x56, triggersAndSpaceHook + 0x9, 5, 0x56 + 1)
            ]);
            memoryService.WriteBytes(triggersAndSpaceCode, codeBytes);

            codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_CtrlCheck64);
            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (ctrlCode.ToInt64(), zDirectionLoc.ToInt64(), 7, 0x0 + 2),
                (ctrlCode.ToInt64() + 0x7, ctrlHook + 0xA, 5, 0x7 + 1)
            ]);

            memoryService.WriteBytes(ctrlCode, codeBytes);

            codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_UpdateCoords64);
            AsmHelper.WriteAbsoluteAddresses64(codeBytes, [
                (GameManagerImp.Base.ToInt64(), 0x1 + 2),
                (GameManagerImp.Base.ToInt64(), 0x29 + 2),
                (GameManagerImp.Base.ToInt64(), 0x73 + 2),
                (HkHardwareInfo.Base.ToInt64(), 0xFB + 2)
            ]);

            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (coordsCode.ToInt64() + 0xC2, zDirectionLoc.ToInt64(), 6, 0xC2 + 2),
                (coordsCode.ToInt64() + 0xEC, zDirectionLoc.ToInt64(), 7, 0xEC + 2),
                (coordsCode.ToInt64() + 0x17F, coordsHook + 0x8, 5, 0x17F + 1),
                (coordsCode.ToInt64() + 0x18D, coordsHook + 0x8, 5, 0x18D + 1)
            ]);

            memoryService.WriteBytes(coordsCode, codeBytes);

            codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_RayCast64);

            var frameCounter = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.FrameCounter;
            var rayInput = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.RayInput;
            var rayOutput = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.RayOutput;
            var mapId = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.MapId;

            var raycastFunc = Functions.HavokRayCast;
            var convertToMap = Functions.ConvertPxRigidToMapEntity;
            var convertToMapId = Functions.ConvertMapEntityToGameId;

            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (raycastCode.ToInt64() + 0x24, frameCounter.ToInt64(), 6, 0x24 + 2),
                (raycastCode.ToInt64() + 0x2A, frameCounter.ToInt64(), 7, 0x2A + 2),
                (raycastCode.ToInt64() + 0x3A, frameCounter.ToInt64(), 6, 0x3A + 2),
                (raycastCode.ToInt64() + 0x8B, rayInput.ToInt64(), 7, 0x8B + 3),
                (raycastCode.ToInt64() + 0xC9, rayOutput.ToInt64(), 7, 0xC9 + 3),
                (raycastCode.ToInt64() + 0xE0, raycastFunc, 5, 0xE0 + 1),
                (raycastCode.ToInt64() + 0xE5, rayInput.ToInt64(), 7, 0xE5 + 3),
                (raycastCode.ToInt64() + 0x110, rayOutput.ToInt64() + (0x30 * 1), 7, 0x110 + 3),
                (raycastCode.ToInt64() + 0x127, raycastFunc, 5, 0x127 + 1),
                (raycastCode.ToInt64() + 0x12C, rayInput.ToInt64(), 7, 0x12C + 3),
                (raycastCode.ToInt64() + 0x157, rayOutput.ToInt64() + (0x30 * 2), 7, 0x157 + 3),
                (raycastCode.ToInt64() + 0x16E, raycastFunc, 5, 0x16E + 1),
                (raycastCode.ToInt64() + 0x173, rayInput.ToInt64(), 7, 0x173 + 3),
                (raycastCode.ToInt64() + 0x19E, rayOutput.ToInt64() + (0x30 * 3), 7, 0x19E + 3),
                (raycastCode.ToInt64() + 0x1B5, raycastFunc, 5, 0x1B5 + 1),
                (raycastCode.ToInt64() + 0x1BA, rayInput.ToInt64(), 7, 0x1BA + 3),
                (raycastCode.ToInt64() + 0x1E5, rayOutput.ToInt64() + (0x30 * 4), 7, 0x1E5 + 3),
                (raycastCode.ToInt64() + 0x1FC, raycastFunc, 5, 0x1FC + 1),
                (raycastCode.ToInt64() + 0x21E, rayOutput.ToInt64(), 7, 0x21E + 3),
                (raycastCode.ToInt64() + 0x24A, rayOutput.ToInt64(), 7, 0x24A + 3),
                (raycastCode.ToInt64() + 0x25F, convertToMap, 5, 0x25F + 1),
                (raycastCode.ToInt64() + 0x26B, mapId.ToInt64(), 7, 0x26B + 3),
                (raycastCode.ToInt64() + 0x276, convertToMapId, 5, 0x276 + 1),
                (raycastCode.ToInt64() + 0x2BF, rayCastHook + 0x5, 5, 0x2BF + 1)
            ]);

            AsmHelper.WriteAbsoluteAddresses64(codeBytes, [
                (GameManagerImp.Base.ToInt64(), 0x6 + 2),
                (GameManagerImp.Base.ToInt64(), 0x70 + 2)
            ]);

            memoryService.WriteBytes(raycastCode, codeBytes);


            memoryService.WriteByte(GetGravityPtr(), 1);

            hookManager.InstallHook(triggersAndSpaceCode.ToInt64(), triggersAndSpaceHook, [
                0x4C, 0x8B, 0x7C, 0x24, 0x70, 0x48, 0x8B, 0x43, 0x08
            ]);
            hookManager.InstallHook(ctrlCode.ToInt64(), ctrlHook, [
                0x81, 0x8B, 0x28, 0x02, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00
            ]);
            hookManager.InstallHook(coordsCode.ToInt64(), coordsHook,
                [0x66, 0x0F, 0x7F, 0xB8, 0x90, 0x00, 0x00, 0x00]);
            hookManager.InstallHook(raycastCode.ToInt64(), rayCastHook, [0x48, 0x8D, 0x54, 0x24, 0x20]);
        }

        private void SetupNoClipHooks32Bit(IntPtr triggersAndSpaceCode, IntPtr ctrlCode, IntPtr coordsCode,
            IntPtr raycastCode)
        {
            var triggersAndSpaceHook = Hooks.TriggersAndSpace;
            var ctrlHook = Hooks.Ctrl;
            var coordsHook = Hooks.NoClipUpdateCoords;
            var rayCastHook = Hooks.ProcessPhysics;

            var zDirectionLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.ZDirection;
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_TriggersAndSpace32);
            var bytes = AsmHelper.GetJmpOriginOffsetBytes(triggersAndSpaceHook, 9, triggersAndSpaceCode + 0x52);
            Array.Copy(bytes, 0, codeBytes, 0x4D + 1, 4);
            AsmHelper.WriteAbsoluteAddresses32(codeBytes, [
                (zDirectionLoc.ToInt64(), 0x19 + 2),
                (zDirectionLoc.ToInt64(), 0x2F + 2),
                (zDirectionLoc.ToInt64(), 0x45 + 2)
            ]);
            memoryService.WriteBytes(triggersAndSpaceCode, codeBytes);


            codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_CtrlCheck32);
            bytes = BitConverter.GetBytes(zDirectionLoc.ToInt32());
            Array.Copy(bytes, 0, codeBytes, 2, 4);
            bytes = AsmHelper.GetJmpOriginOffsetBytes(ctrlHook, 0xA, ctrlCode + 0xC);
            Array.Copy(bytes, 0, codeBytes, 0x7 + 1, 4);

            memoryService.WriteBytes(ctrlCode, codeBytes);


            codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_UpdateCoords32);

            AsmHelper.WriteAbsoluteAddresses32(codeBytes, new[]
            {
                (GameManagerImp.Base.ToInt64(), 0x1 + 2),
                (GameManagerImp.Base.ToInt64(), 0x4E + 2),
                (zDirectionLoc.ToInt64(), 0x89 + 2),
                (zDirectionLoc.ToInt64(), 0xB0 + 2),
                (GameManagerImp.Base.ToInt64(), 0xBB + 2),
            });

            AsmHelper.WriteJumpOffsets(codeBytes, new[]
            {
                (coordsHook, 20, coordsCode + 0x127, 0x127 + 1),
                (coordsHook, 5, coordsCode + 0x132, 0x132 + 1)
            });

            memoryService.WriteBytes(coordsCode, codeBytes);

            var frameCounter = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.FrameCounter;
            var rayInput = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.RayInput;
            var rayOutput = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.RayOutput;
            var mapId = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.MapId;

            var raycastFunc = Functions.HavokRayCast;
            var convertToMap = Functions.ConvertPxRigidToMapEntity;
            var convertToMapId = Functions.ConvertMapEntityToGameId;

            codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_RayCast32);

            AsmHelper.WriteAbsoluteAddresses32(codeBytes, new[]
            {
                (GameManagerImp.Base.ToInt64(), 0x1 + 1),
                (frameCounter.ToInt64(), 0xF + 2),
                (frameCounter.ToInt64(), 0x15 + 2),
                (frameCounter.ToInt64(), 0x22 + 2),
                (rayOutput.ToInt64(), 0x59 + 2),
                (rayInput.ToInt64(), 0x6E + 2),
                (rayOutput.ToInt64() + (0x30 * 1), 0xA9 + 2),
                (rayInput.ToInt64(), 0xB8 + 2),
                (rayOutput.ToInt64() + (0x30 * 2), 0xE8 + 2),
                (rayInput.ToInt64(), 0xF7 + 2),
                (rayOutput.ToInt64() + (0x30 * 3), 0x127 + 2),
                (rayInput.ToInt64(), 0x136 + 2),
                (rayOutput.ToInt64() + (0x30 * 4), 0x166 + 2),
                (rayInput.ToInt64(), 0x175 + 2),
                (rayOutput.ToInt64(), 0x1BB + 2),
                (rayOutput.ToInt64(), 0x1E4 + 2),
                (mapId.ToInt64(), 0x1FC + 2)
            });

            AsmHelper.WriteRelativeOffsets(codeBytes, new[]
            {
                (raycastCode.ToInt64() + 0xA4, raycastFunc, 5, 0xA4 + 1),
                (raycastCode.ToInt64() + 0xE3, raycastFunc, 5, 0xE3 + 1),
                (raycastCode.ToInt64() + 0x122, raycastFunc, 5, 0x122 + 1),
                (raycastCode.ToInt64() + 0x161, raycastFunc, 5, 0x161 + 1),
                (raycastCode.ToInt64() + 0x1A0, raycastFunc, 5, 0x1A0 + 1),
                (raycastCode.ToInt64() + 0x1F3, convertToMap, 5, 0x1F3 + 1),
                (raycastCode.ToInt64() + 0x202, convertToMapId, 5, 0x202 + 1),
                (raycastCode.ToInt64() + 0x24D, rayCastHook + 0x6, 5, 0x24D + 1),
            });

            memoryService.WriteBytes(raycastCode, codeBytes);

            memoryService.Write(GetGravityPtr(), (byte)1);

            hookManager.InstallHook(triggersAndSpaceCode.ToInt64(), triggersAndSpaceHook, new byte[]
                { 0x8B, 0x56, 0x08, 0x89, 0x86, 0x04, 0x01, 0x00, 0x00 });
            hookManager.InstallHook(ctrlCode.ToInt64(), ctrlHook, new byte[]
                { 0x81, 0x8E, 0x28, 0x02, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00 });
            hookManager.InstallHook(coordsCode.ToInt64(), coordsHook, new byte[]
                { 0xF3, 0x0F, 0x7E, 0x45, 0xD0 });
            hookManager.InstallHook(raycastCode.ToInt64(), rayCastHook, new byte[]
                { 0x8B, 0x8E, 0xB8, 0x00, 0x00, 0x00 });
        }

        public void SetNoClipSpeed(byte[] xBytes, byte[] yBytes)
        {
            if (PatchManager.IsScholar())
            {
                memoryService.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.UpdateCoords + 0xA4 + 1,
                    xBytes);
                memoryService.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.UpdateCoords + 0x60 + 1,
                    yBytes);
            }
            else
            {
                memoryService.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.UpdateCoords + 0x71 + 1,
                    xBytes);
                memoryService.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.UpdateCoords + 0x3E + 1,
                    yBytes);
            }
        }

        private IntPtr GetGravityPtr() => memoryService.FollowPointers(GameManagerImp.Base, [
            GameManagerImp.PlayerCtrl,
            GameManagerImp.ChrCtrlOffsets.ChrPhysicsCtrlPtr,
            GameManagerImp.ChrCtrlOffsets.ChrPhysicsCtrl.Gravity
        ], false);

        public void ToggleKillboxHook(bool isEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.Killbox;

            if (!isEnabled)
            {
                hookManager.UninstallHook(code.ToInt64());
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

        private void InstallScholarKillboxHook(IntPtr code, long hookLoc)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.Killbox64);
            var bytes = BitConverter.GetBytes(GameManagerImp.Base.ToInt64());
            Array.Copy(bytes, 0, codeBytes, 0x1 + 2, 8);
            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (code.ToInt64() + 0x2b, hookLoc + 0xA, 5, 0x2b + 1)
            ]);

            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code.ToInt64(), hookLoc, [
                0x48, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00
            ]);
        }

        private void InstallVanillaKillboxHook(IntPtr code, long hookLoc)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.Killbox32);
            var bytes = BitConverter.GetBytes(GameManagerImp.Base.ToInt32());
            Array.Copy(bytes, 0, codeBytes, 0x1 + 1, 4);
            bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 6, code + 0x1E);
            Array.Copy(bytes, 0, codeBytes, 0x19 + 1, 4);
            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code.ToInt64(), hookLoc, [0x53, 0x89, 0xE3, 0x83, 0xEC, 0x08]);
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
            var slowdownFactor = CodeCaveOffsets.Base + CodeCaveOffsets.SlowdownFactor;
            memoryService.WriteFloat(slowdownFactor, value);

            if (_isSlowdownHookInstalled) return;
            var hookLoc = Hooks.ReduceGameSpeed;
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.SlowdownCode;

            if (PatchManager.IsScholar())
            {
                InstallScholarSlowdownHook(slowdownFactor, hookLoc, code);
            }
            else
            {
                InstallVanillaSlowdownHook(slowdownFactor, hookLoc, code);
            }
        }

        private void InstallScholarSlowdownHook(IntPtr slowdownFactor, long hookLoc, IntPtr code)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.ReduceSpeed64);
            AsmHelper.WriteRelativeOffsets(bytes, [
                (code.ToInt64() + 0x4, slowdownFactor.ToInt64(), 8, 0x4 + 4),
                (code.ToInt64() + 0x16, hookLoc + 0x6, 5, 0x16 + 1)
            ]);

            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code.ToInt64(), hookLoc, [0xF3, 0x0F, 0x10, 0x32, 0x31, 0xDB]);
            _isSlowdownHookInstalled = true;
        }

        private void InstallVanillaSlowdownHook(IntPtr slowdownFactor, long hookLoc, IntPtr code)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.ReduceSpeed32);
            var bytes = BitConverter.GetBytes(slowdownFactor.ToInt32());
            Array.Copy(bytes, 0, codeBytes, 0x7 + 4, 4);
            bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 7, code + 0x18);
            Array.Copy(bytes, 0, codeBytes, 0x13 + 1, 4);

            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code.ToInt64(), hookLoc, [0x8B, 0x7B, 0x08, 0xF3, 0x0F, 0x10, 0x07]);
            _isSlowdownHookInstalled = true;
        }

        private void RemoveSlowdownHook()
        {
            hookManager.UninstallHook((CodeCaveOffsets.Base + CodeCaveOffsets.SlowdownCode).ToInt64());
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
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.Snowstorm;

            if (!isSnowstormDisabled)
            {
                hookManager.UninstallHook(code.ToInt64());
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

        private void InstallScholarDisableSnowstorm(IntPtr code, long origin)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.DisableSnowstorm64);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 11, code + 0x1C);
            Array.Copy(jmpBytes, 0, bytes, 0x17 + 1, 4);
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code.ToInt64(), origin, [
                0x41, 0x0F, 0xB6, 0xF8, 0x48, 0x8B, 0x88, 0xF0, 0x22, 0x00, 0x00
            ]);
        }

        private void InstallVanillaDisableSnowstorm(IntPtr code, long origin)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.DisableSnowstorm32);
            AsmHelper.WriteRelativeOffsets(bytes, [
                (code.ToInt64() + 0xD, origin + 6, 6, 0xD + 2),
                (code.ToInt64() + 0x1A, origin + 6, 5, 0x1A + 1)
            ]);
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code.ToInt64(), origin, [0x8B, 0x88, 0xCC, 0x0C, 0x00, 0x00]);
        }

        public void ToggleMemoryTimer(bool isMemoryTimerDisabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.DisableMemoryTimer;

            if (!isMemoryTimerDisabled)
            {
                hookManager.UninstallHook(code.ToInt64());
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

        private void InstallScholarMemoryTimerHook(IntPtr code, long origin)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.DisableMemoryTimer64);

            var bytes = BitConverter.GetBytes(GameManagerImp.Base.ToInt64());
            Array.Copy(bytes, 0, codeBytes, 0x2 + 2, 8);

            bytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 7, code + 0xC1);
            Array.Copy(bytes, 0, codeBytes, 0xBC + 1, 4);

            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code.ToInt64(), origin, new byte[]
                { 0x66, 0x0F, 0x6E, 0x30, 0x0F, 0x5B, 0xF6 });
        }

        private void InstallVanillaMemoryTimerHook(IntPtr code, long origin)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.DisableMemoryTimer32);
            var bytes = BitConverter.GetBytes(GameManagerImp.Base.ToInt32());
            Array.Copy(bytes, 0, codeBytes, 0x2 + 2, 4);
            bytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 7, code + 0x79);
            Array.Copy(bytes, 0, codeBytes, 0x74 + 1, 4);

            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code.ToInt64(), origin, [0x66, 0x0F, 0x6E, 0x00, 0x0F, 0x5B, 0xC0]);
        }

        public void ToggleIvorySkip(bool isIvorySkipEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.IvorySkip;
            var knightsCode = CodeCaveOffsets.Base + CodeCaveOffsets.IvoryKnights;

            if (!isIvorySkipEnabled)
            {
                hookManager.UninstallHook(code.ToInt64());
                hookManager.UninstallHook(knightsCode.ToInt64());
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
            IntPtr code, IntPtr knightsCode, long origin, long getMapEntity, long getComponent, long setSharedFlag)
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

            hookManager.InstallHook(code.ToInt64(), origin, [0x48, 0x89, 0x74, 0x24, 0x10]);
            hookManager.InstallHook(knightsCode.ToInt64(), setSharedFlag, [
                0x44, 0x88, 0x84, 0x08, 0xA1, 0x03, 0x00, 0x00
            ]);
        }

        private void InstallVanillaIvoryHooks(
            IntPtr code, IntPtr knightsCode, long origin, long getMapEntity, long getComponent, long setSharedFlag)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.IvorySkip32);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 6, code + 0xA7);
            Array.Copy(jmpBytes, 0, bytes, 0xA1 + 1, 4);
            AsmHelper.WriteAbsoluteAddresses32(bytes, new[]
            {
                (origin, 0x25 + 1),
                (getMapEntity, 0x46 + 3),
                (getComponent, 0x4D + 3)
            });

            memoryService.WriteBytes(code, bytes);

            bytes = AsmLoader.GetAsmBytes(AsmScript.IvoryKnights32);
            jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(setSharedFlag, 7, knightsCode + 0x21);
            Array.Copy(jmpBytes, 0, bytes, 0x1B + 1, 4);
            memoryService.WriteBytes(knightsCode, bytes);

            hookManager.InstallHook(code.ToInt64(), origin, [0x55, 0x89, 0xE5, 0x83, 0xEC, 0x08]);
            hookManager.InstallHook(knightsCode.ToInt64(), setSharedFlag, [0x88, 0x94, 0x08, 0xA1, 0x02, 0x00, 0x00]);
        }

        public void SetObjState(long areaId, Obj obj)
        {
            var getMapEntity = Functions.GetMapEntityWithAreaIdAndObjId;
            var getComponent = Functions.GetMapObjStateActComponent;

            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.SetObjState64);
                AsmHelper.WriteAbsoluteAddresses64(bytes, [
                    (areaId, 0x4 + 2),
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
                    (areaId, 0xF + 1),
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
                    (eventPointMan.ToInt64(), 2),
                    (areaId, 0xA + 2),
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
                    (eventPointMan.ToInt64(), 1),
                    (naviData.EventId, 0x5 + 1),
                    (areaId, 0xA + 1),
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
                    (areaId, 0x4 + 2),
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
                    (areaId, 0x5 + 1),
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
            if (count == 0) return new List<InventorySpell>();

            return PatchManager.Current.Edition == GameEdition.Scholar
                ? ReadSpellList64Bit(spellBase, count)
                : ReadSpellList32Bit(spellBase, count);
        }

        private List<InventorySpell> ReadSpellList64Bit(IntPtr spellBase, int count)
        {
            List<InventorySpell> currentSpells = new List<InventorySpell>();
            var current = memoryService.Read<nint>(
                spellBase + GameManagerImp.GameDataManagerOffsets.Inventory.ItemInvetory2SpellList.ListStart);

            for (int i = 0; i < count && current != IntPtr.Zero; i++)
            {
                var spellId =
                    memoryService.Read<int>(
                        current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.SpellId);
                var isEquipped =
                    memoryService.Read<byte>(current +
                                             GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.IsEquipped);
                var slotReq =
                    memoryService.Read<byte>(current +
                                             GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.SlotReq);

                currentSpells.Add(new InventorySpell(spellId, isEquipped == 2, current, slotReq));

                current = memoryService.Read<nint>(current +
                                                   GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.NextPtr);
            }

            return currentSpells;
        }

        private List<InventorySpell> ReadSpellList32Bit(IntPtr spellBase, int count)
        {
            List<InventorySpell> currentSpells = new List<InventorySpell>();
            nint current = memoryService.Read<int>(spellBase +
                                                   GameManagerImp.GameDataManagerOffsets.Inventory
                                                       .ItemInvetory2SpellList.ListStart);

            for (int i = 0; i < count && current != IntPtr.Zero; i++)
            {
                var spellId =
                    memoryService.Read<int>(
                        current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.SpellId);
                var isEquipped =
                    memoryService.Read<byte>(current +
                                             GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.IsEquipped);
                var slotReq =
                    memoryService.Read<byte>(
                        current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.SlotReq);
                currentSpells.Add(new InventorySpell(spellId, isEquipped == 2, current, slotReq));
                current = memoryService.Read<int>(current +
                                                  GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry
                                                      .NextPtr);
            }

            return currentSpells;
        }

        public List<EquippedSpell> GetEquippedSpells()
        {
            var currentSpell = GetCurrentSpellPtr();
            List<EquippedSpell> currentSpells = new List<EquippedSpell>();

            int chunkSize = PatchManager.Current.Edition == GameEdition.Scholar ? 0x10 : 0x8;

            for (int i = 0; i < 14; i++)
            {
                currentSpells.Add(new EquippedSpell(memoryService.Read<int>(currentSpell), i));
                currentSpell += chunkSize;
            }

            return currentSpells;
        }

        private IntPtr GetCurrentSpellPtr()
        {
            return memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.PlayerCtrl,
                GameManagerImp.ChrCtrlOffsets.ChrAsmCtrl,
                GameManagerImp.ChrCtrlOffsets.EquippedSpellsStart
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
            var slotsLoc = CodeCaveOffsets.Base + CodeCaveOffsets.NumOfSpellSlots;


            byte[] bytes;

            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                bytes = AsmLoader.GetAsmBytes(AsmScript.GetNumOfSlots64);

                AsmHelper.WriteAbsoluteAddresses64(bytes, [
                    (slotsLoc.ToInt64(), 2),
                    (inventory.ToInt64(), 0xA + 2),
                    (getNumOfSlots1, 0x17 + 2),
                    (getNumOfSlots2, 0x2C + 2)
                ]);
            }
            else
            {
                bytes = AsmLoader.GetAsmBytes(AsmScript.GetNumOfSlots32);
                AsmHelper.WriteAbsoluteAddresses32(bytes, [
                    (slotsLoc.ToInt64(), 1),
                    (inventory.ToInt64(), 0x5 + 1),
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
                    (bagList.ToInt64(), 2),
                    (refreshFunc, 0xA + 2)
                ]);
                memoryService.AllocateAndExecute(bytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.UpdateSpellSlots32);
                AsmHelper.WriteAbsoluteAddresses32(bytes, [
                    (bagList.ToInt64(), 1),
                    (refreshFunc, 0x5 + 1)
                ]);
                memoryService.AllocateAndExecute(bytes);
            }
        }

        public void AttuneSpell(int slotIndex, IntPtr entryAddr)
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

        private void AttuneScholarSpell(int slotIndex, IntPtr entryAddr, IntPtr inventoryLists, long attuneFunc)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.AttuneSpell64);
            AsmHelper.WriteAbsoluteAddresses64(bytes, [
                (inventoryLists.ToInt64(), 2),
                (slotIndex + 0x1C, 0xA + 2),
                (entryAddr.ToInt64(), 0x14 + 2),
                (attuneFunc, 0x1E + 2)
            ]);
            memoryService.AllocateAndExecute(bytes);
        }

        private void AttuneVanillaSpell(int slotIndex, IntPtr entryAddr, IntPtr inventoryLists, long attuneFunc)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.AttuneSpell32);
            AsmHelper.WriteAbsoluteAddresses32(bytes, [
                (inventoryLists.ToInt64(), 1),
                (entryAddr.ToInt64(), 0x5 + 1),
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
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.LightGutter;

            if (isEnabled)
            {
                if (PatchManager.Current.Edition == GameEdition.Scholar) InstallLightGutter64(code);
                else InstallLightGutter32(code);
            }
            else
            {
                hookManager.UninstallHook(code.ToInt64());
            }
        }

        private void InstallLightGutter64(IntPtr code)
        {
            var hookLoc = Hooks.LightGutter;
            var bytes = AsmLoader.GetAsmBytes(AsmScript.LightGutter64);
            AsmHelper.WriteRelativeOffsets(bytes, [
                (code.ToInt64(), MapId.ToInt64(), 10, 0x0 + 2),
                (code.ToInt64() + 0x21, hookLoc + 0x8, 5, 0x21 + 1)
            ]);
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code.ToInt64(), hookLoc, [0x66, 0x0F, 0x7F, 0x87, 0xE0, 0x00, 0x00, 0x00]);
        }

        private void InstallLightGutter32(IntPtr code)
        {
            var hookLoc = Hooks.LightGutter;
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.LightGutter32);
            var bytes = BitConverter.GetBytes(MapId.ToInt32());
            Array.Copy(bytes, 0, codeBytes, 0x8 + 2, 4);
            bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 7, code + 0x25);
            Array.Copy(bytes, 0, codeBytes, 0x21 + 1, 4);
            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code.ToInt64(), hookLoc, [0xF3, 0x0F, 0x7E, 0x86, 0x58, 0xFF, 0xFF, 0xFF]);
        }

        public void ToggleShadedFog(bool isNoFogEnabled)
        {
            var closeFogCode = CodeCaveOffsets.Base + CodeCaveOffsets.NoFogClose;
            var farFogCode = CodeCaveOffsets.Base + CodeCaveOffsets.NoFogFar;
            var fogCamCode = CodeCaveOffsets.Base + CodeCaveOffsets.NoFogCam;
            if (!isNoFogEnabled)
            {
                hookManager.UninstallHook(closeFogCode.ToInt64());
                hookManager.UninstallHook(farFogCode.ToInt64());
                hookManager.UninstallHook(fogCamCode.ToInt64());
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

        private void InstallNoFog64(IntPtr closeFogCode, IntPtr farFogCode, IntPtr fogCamCode)
        {
            var hookLoc = Hooks.NoShadedFogClose;
            var bytes = AsmLoader.GetAsmBytes(AsmScript.NoShadedFogClose64);
            AsmHelper.WriteRelativeOffsets(bytes, [
                (closeFogCode.ToInt64() + 0x7, MapId.ToInt64(), 10, 0x7 + 2),
                (closeFogCode.ToInt64() + 0x1E, hookLoc + 0x7, 5, 0x1E + 1)
            ]);
            memoryService.WriteBytes(closeFogCode, bytes);
            hookManager.InstallHook(closeFogCode.ToInt64(), hookLoc, [0x0F, 0x57, 0xC0, 0x66, 0x0F, 0x6E, 0xE0]);

            hookLoc = Hooks.NoShadedFogFar;
            bytes = AsmLoader.GetAsmBytes(AsmScript.NoShadedFogFar64);
            AsmHelper.WriteRelativeOffsets(bytes, [
                (farFogCode.ToInt64() + 0x7, MapId.ToInt64(), 10, 0x7 + 2),
                (farFogCode.ToInt64() + 0x11, hookLoc + 0x7, 6, 0x11 + 2),
                (farFogCode.ToInt64() + 0x17, hookLoc + 0xC, 5, 0x17 + 1)
            ]);

            memoryService.WriteBytes(farFogCode, bytes);
            hookManager.InstallHook(farFogCode.ToInt64(), hookLoc, [0x4C, 0x89, 0x50, 0x10, 0x48, 0x89, 0xC3]);


            hookLoc = Hooks.NoShadedFogCam;
            bytes = AsmLoader.GetAsmBytes(AsmScript.NoShadedFogCam64);
            AsmHelper.WriteRelativeOffsets(bytes, [
                (fogCamCode.ToInt64(), MapId.ToInt64(), 10, 0x0 + 2),
                (fogCamCode.ToInt64() + 0x13, hookLoc + 0x7, 5, 0x13 + 1)
            ]);

            memoryService.WriteBytes(fogCamCode, bytes);
            hookManager.InstallHook(fogCamCode.ToInt64(), hookLoc, [0x89, 0x44, 0x24, 0x58, 0x8B, 0x41, 0x38]);
        }

        private void InstallNoFog32(IntPtr closeFogCode, IntPtr farFogCode, IntPtr fogCamCode)
        {
            var hookLoc = Hooks.NoShadedFogClose;
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoShadedFogClose32);
            var bytes = BitConverter.GetBytes(MapId.ToInt32());
            Array.Copy(bytes, 0, codeBytes, 0x4 + 2, 4);
            bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 7, closeFogCode + 0x1D);
            Array.Copy(bytes, 0, codeBytes, 0x18 + 1, 4);

            memoryService.WriteBytes(closeFogCode, codeBytes);
            hookManager.InstallHook(closeFogCode.ToInt64(), hookLoc, [0x0F, 0xB6, 0x46, 0x07, 0x89, 0x45, 0xAC]);


            hookLoc = Hooks.NoShadedFogFar;
            codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoShadedFogFar32);
            bytes = BitConverter.GetBytes(MapId.ToInt32());
            Array.Copy(bytes, 0, codeBytes, 0x5 + 2, 4);
            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (farFogCode.ToInt64() + 0xF, hookLoc + 0x5, 6, 0xF + 2),
                (farFogCode.ToInt64() + 0x17, hookLoc + 0xA, 5, 0x17 + 1)
            ]);

            memoryService.WriteBytes(farFogCode, codeBytes);
            hookManager.InstallHook(farFogCode.ToInt64(), hookLoc, [0xF3, 0x0F, 0x11, 0x46, 0x10]);


            hookLoc = Hooks.NoShadedFogCam;
            codeBytes = AsmLoader.GetAsmBytes(AsmScript.NoShadedFogCam32);
            bytes = BitConverter.GetBytes(MapId.ToInt32());
            Array.Copy(bytes, 0, codeBytes, 0x0 + 2, 4);
            bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 5, fogCamCode + 0x16);
            Array.Copy(bytes, 0, codeBytes, 0x11 + 1, 4);

            memoryService.WriteBytes(fogCamCode, codeBytes);
            hookManager.InstallHook(fogCamCode.ToInt64(), hookLoc, [0x89, 0x4D, 0xE4, 0x31, 0xC9]);
        }
    }
}