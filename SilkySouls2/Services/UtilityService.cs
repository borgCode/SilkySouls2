using System;
using System.IO;
using System.Threading.Tasks;
using SilkySouls2.Memory;
using SilkySouls2.Memory.DLLShared;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class UtilityService
    {
        private readonly MemoryIo _memoryIo;
        private readonly HookManager _hookManager;
        private readonly DllManager _dllManager;

        public UtilityService(MemoryIo memoryIo, HookManager hookManager, DllManager dllManager)
        {
            _memoryIo = memoryIo;
            _hookManager = hookManager;
            _dllManager = dllManager;
        }

        public void SetEventOn(long gameId)
        {
            var eventFlagMan = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.EventManager,
                GameManagerImp.EventManagerOffsets.EventFlagManager
            }, true);

            var setEventFunc = Funcs.SetEvent;
            var bytes = AsmLoader.GetAsmBytes("SetEventOn");
            AsmHelper.WriteAbsoluteAddresses(bytes, new[]
            {
                (eventFlagMan.ToInt64(), 0x0 + 2),
                (gameId, 0xA + 2),
                (setEventFunc, 0x1A + 2)
            });

            _memoryIo.AllocateAndExecute(bytes);
        }

        public void SetMultipleEventOn(long[] gameIds)
        {
            foreach (var gameId in gameIds)
            {
                SetEventOn(gameId);
            }
        }

        public void SetEventOff(long gameId)
        {
            var eventFlagMan = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.EventManager,
                GameManagerImp.EventManagerOffsets.EventFlagManager
            }, true);

            var setEventFunc = Funcs.SetEvent;
            var bytes = AsmLoader.GetAsmBytes("SetEventOff");
            AsmHelper.WriteAbsoluteAddresses(bytes, new[]
            {
                (eventFlagMan.ToInt64(), 0x0 + 2),
                (gameId, 0xA + 2),
                (setEventFunc, 0x1A + 2)
            });

            _memoryIo.AllocateAndExecute(bytes);
        }

        public void SetMultipleEventOff(long[] gameIds)
        {
            foreach (var gameId in gameIds)
            {
                SetEventOff(gameId);
            }
        }

        public void ForceSave()
        {
            var saveLoadSystem = _memoryIo.ReadInt64((IntPtr)_memoryIo.ReadInt64(GameManagerImp.Base) +
                                                     GameManagerImp.Offsets.SaveLoadSystem);
            _memoryIo.WriteInt32((IntPtr)saveLoadSystem + GameManagerImp.SaveLoadSystem.ForceSaveFlag1, 2);
            _memoryIo.WriteByte((IntPtr)saveLoadSystem + GameManagerImp.SaveLoadSystem.ForceSaveFlag2, 1);
        }

        public void ToggleCreditSkip(bool isCreditSkipEnabled)
        {
            var code = CodeCaveOffsets.Base + (int)CodeCaveOffsets.CreditSkip.Code;

            if (isCreditSkipEnabled)
            {
                var hookLoc = Hooks.CreditSkip;
                var modifyOnceFlag = CodeCaveOffsets.Base + (int)CodeCaveOffsets.CreditSkip.ModifyOnceFlag;
                _memoryIo.WriteInt32(modifyOnceFlag, 0);
                var codeBytes = AsmLoader.GetAsmBytes("CreditSkip");
                AsmHelper.WriteRelativeOffsets(codeBytes, new[]
                {
                    (code.ToInt64() + 0x7, modifyOnceFlag.ToInt64(), 7, 0x7 + 2),
                    (code.ToInt64() + 0x17, modifyOnceFlag.ToInt64(), 10, 0x17 + 2),
                    (code.ToInt64() + 0x21, hookLoc + 7, 5, 0x21 + 1)
                });
                _memoryIo.WriteBytes(code, codeBytes);
                _hookManager.InstallHook(code.ToInt64(), hookLoc, new byte[]
                    { 0x48, 0x81, 0xEC, 0x20, 0x02, 0x00, 0x00 });
            }
            else
            {
                _hookManager.UninstallHook(code.ToInt64());
            }
        }

        public void Toggle100Drop(bool is100DropEnabled)
        {
            var dropCountHook = Hooks.NumOfDrops;
            var dropCountCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.Drop100.DropCount;

            if (is100DropEnabled)
            {
                _memoryIo.WriteBytes(Patches.DropRate, new byte[] { 0x90, 0x90, 0x90 });

                var codeBytes = AsmLoader.GetAsmBytes("DropCount");
                var bytes = AsmHelper.GetJmpOriginOffsetBytes(dropCountHook, 5, dropCountCode + 0xA);
                Array.Copy(bytes, 0, codeBytes, 0x5 + 1, 4);
                _memoryIo.WriteBytes(dropCountCode, codeBytes);


                _hookManager.InstallHook(dropCountCode.ToInt64(), dropCountHook, new byte[]
                    { 0x41, 0x0F, 0xB6, 0x47, 0x01 });
            }
            else
            {
                _memoryIo.WriteBytes(Patches.DropRate, new byte[] { 0x41, 0xF7, 0xF2 });
                _hookManager.UninstallHook(dropCountCode.ToInt64());
            }
        }

        public void ToggleNoClip(bool isNoClipEnabled)
        {
            
            var triggersAndSpaceCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.TriggersAndSpaceCheck;
            var ctrlCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.CtrlCheck;
            var coordsCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.UpdateCoords;
            var raycastCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.RayCastCode;

            if (isNoClipEnabled)
            {
                var triggersAndSpaceHook = Hooks.TriggersAndSpace;
                var ctrlHook = Hooks.Ctrl;
                var coordsHook = Hooks.NoClipUpdateCoords;
                var rayCastHook = 0x7ff759444f35;

                var zDirectionLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.ZDirection;
                
                var codeBytes = AsmLoader.GetAsmBytes("NoClip_TriggersAndSpace");
                AsmHelper.WriteRelativeOffsets(codeBytes, new []
                {
                    (triggersAndSpaceCode.ToInt64() + 0x1C, zDirectionLoc.ToInt64(), 7, 0x1C + 2),
                    (triggersAndSpaceCode.ToInt64() + 0x35, zDirectionLoc.ToInt64(), 7, 0x35 + 2),
                    (triggersAndSpaceCode.ToInt64() + 0x4E, zDirectionLoc.ToInt64(), 7, 0x4E + 2),
                    (triggersAndSpaceCode.ToInt64() + 0x56, triggersAndSpaceHook + 0x9, 5, 0x56 + 1),
                });
                _memoryIo.WriteBytes(triggersAndSpaceCode, codeBytes);

                codeBytes = AsmLoader.GetAsmBytes("NoClip_CtrlCheck");
                AsmHelper.WriteRelativeOffsets(codeBytes, new []
                {
                    (ctrlCode.ToInt64(), zDirectionLoc.ToInt64(), 7, 0x0 + 2),
                    (ctrlCode.ToInt64() + 0x7, ctrlHook + 0xA, 5, 0x7 + 1)
                });
                
                _memoryIo.WriteBytes(ctrlCode, codeBytes);
                
                codeBytes = AsmLoader.GetAsmBytes("NoClip_UpdateCoords");
                AsmHelper.WriteAbsoluteAddresses(codeBytes, new []
                {
                    (GameManagerImp.Base.ToInt64(), 0x1 + 2),
                    (GameManagerImp.Base.ToInt64(), 0x29 + 2),
                    (GameManagerImp.Base.ToInt64(), 0x73 + 2),
                    (HkHardwareInfo.Base.ToInt64(), 0xFB + 2)
                });
                
                AsmHelper.WriteRelativeOffsets(codeBytes, new []
                {
                    (coordsCode.ToInt64() + 0xC2, zDirectionLoc.ToInt64(), 6, 0xC2 + 2),
                    (coordsCode.ToInt64() + 0xEC, zDirectionLoc.ToInt64(), 7, 0xEC + 2),
                    (coordsCode.ToInt64() + 0x17F, coordsHook + 0x8, 5, 0x17F + 1),
                    (coordsCode.ToInt64() + 0x18D, coordsHook + 0x8, 5, 0x18D + 1)
                });
                
                _memoryIo.WriteBytes(coordsCode, codeBytes);

                codeBytes = AsmLoader.GetAsmBytes("NoClip_RayCast");

                var frameCounter = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.FrameCounter;
                var rayInput = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.RayInput;
                var rayOutput = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.RayOutput;
                var mapId = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.MapId;
                
                var raycastFunc = Funcs.HavokRayCast;
                var convertToMap = Funcs.ConvertPxRigidToMapEntity;
                var convertToMapId = Funcs.ConvertMapEntityToGameId;
                
                AsmHelper.WriteRelativeOffsets(codeBytes, new []
                {
                    (raycastCode.ToInt64() + 0x26, frameCounter.ToInt64(), 6, 0x26 + 2),
                    (raycastCode.ToInt64() + 0x2C, frameCounter.ToInt64(), 7, 0x2C + 2),
                    (raycastCode.ToInt64() + 0x3C, frameCounter.ToInt64(), 6, 0x3C + 2),
                    (raycastCode.ToInt64() + 0x64, rayInput.ToInt64(), 7, 0x64 + 3),
                    (raycastCode.ToInt64() + 0xA2, rayOutput.ToInt64(), 7, 0xA2 + 3),
                    (raycastCode.ToInt64() + 0xB9, raycastFunc, 5, 0xB9 + 1),
                    (raycastCode.ToInt64() + 0xC5, rayOutput.ToInt64() + 0x10, 7, 0xC5 + 2),
                    (raycastCode.ToInt64() + 0xD2, rayInput.ToInt64(), 7, 0xD2 + 3),
                    (raycastCode.ToInt64() + 0xFD, rayOutput.ToInt64(), 7, 0xFD + 3),
                    (raycastCode.ToInt64() + 0x11B, raycastFunc, 5, 0x11B + 1),
                    (raycastCode.ToInt64() + 0x127, rayOutput.ToInt64() + 0x10, 7, 0x127 + 2),
                    (raycastCode.ToInt64() + 0x134, rayInput.ToInt64(), 7, 0x134 + 3),
                    (raycastCode.ToInt64() + 0x15F, rayOutput.ToInt64(), 7, 0x15F + 3),
                    (raycastCode.ToInt64() + 0x17D, raycastFunc, 5, 0x17D + 1),
                    (raycastCode.ToInt64() + 0x189, rayOutput.ToInt64() + 0x10, 7, 0x189 + 2),
                    (raycastCode.ToInt64() + 0x196, rayInput.ToInt64(), 7, 0x196 + 3),
                    (raycastCode.ToInt64() + 0x1C1, rayOutput.ToInt64(), 7, 0x1C1 + 3),
                    (raycastCode.ToInt64() + 0x1DF, raycastFunc, 5, 0x1DF + 1),
                    (raycastCode.ToInt64() + 0x1EB, rayOutput.ToInt64() + 0x10, 7, 0x1EB + 2),
                    (raycastCode.ToInt64() + 0x1F4, rayInput.ToInt64(), 7, 0x1F4 + 3),
                    (raycastCode.ToInt64() + 0x21F, rayOutput.ToInt64(), 7, 0x21F + 3),
                    (raycastCode.ToInt64() + 0x23D, raycastFunc, 5, 0x23D + 1),
                    (raycastCode.ToInt64() + 0x249, rayOutput.ToInt64() + 0x10, 7, 0x249 + 2),
                    (raycastCode.ToInt64() + 0x263, convertToMap, 5, 0x263 + 1),
                    (raycastCode.ToInt64() + 0x26F, mapId.ToInt64(), 7, 0x26F + 3),
                    (raycastCode.ToInt64() + 0x27A, convertToMapId, 5, 0x27A + 1),
                    (raycastCode.ToInt64() + 0x293, rayCastHook + 0x7, 5, 0x293 + 1)
                });
                
                AsmHelper.WriteAbsoluteAddresses(codeBytes, new []
                {
                    (GameManagerImp.Base.ToInt64(), 0x8 + 2),
                    (GameManagerImp.Base.ToInt64(), 0x49 + 2),
                    (rayOutput.ToInt64() + 0x18, 0x252 + 2)
                });

                _memoryIo.WriteBytes(raycastCode, codeBytes);
                
                
                _memoryIo.WriteByte(GetGravityPtr(), 1);
                
                _hookManager.InstallHook(triggersAndSpaceCode.ToInt64(), triggersAndSpaceHook, new byte[]
                    { 0x4C, 0x8B, 0x7C, 0x24, 0x70, 0x48, 0x8B, 0x43, 0x08 });
                _hookManager.InstallHook(ctrlCode.ToInt64(), ctrlHook, new byte[]
                    { 0x81, 0x8B, 0x28, 0x02, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00 });
                _hookManager.InstallHook(coordsCode.ToInt64(), coordsHook, 
                new byte[] { 0x66, 0x0F, 0x7F, 0xB8, 0x90, 0x00, 0x00, 0x00 });
                _hookManager.InstallHook(raycastCode.ToInt64(), rayCastHook, new byte[]
                    { 0x48, 0x8D, 0x9E, 0xD8, 0x00, 0x00, 0x00 });

            }
            else
            {
                _hookManager.UninstallHook(coordsCode.ToInt64());
                _memoryIo.WriteByte(GetGravityPtr(), 0);
                _hookManager.UninstallHook(triggersAndSpaceCode.ToInt64());
                _hookManager.UninstallHook(ctrlCode.ToInt64());
                _hookManager.UninstallHook(raycastCode.ToInt64());
            }
            
        }

        public void SetNoClipSpeed(byte[] xBytes, byte[] yBytes)
        {
            _memoryIo.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.UpdateCoords + 0xA4 + 1,
                xBytes);
            _memoryIo.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.UpdateCoords + 0x60 + 1,
                yBytes);
        }

        private IntPtr GetGravityPtr() => _memoryIo.FollowPointers(GameManagerImp.Base, new[]
        {
            GameManagerImp.Offsets.PlayerCtrl,
            GameManagerImp.ChrCtrlOffsets.ChrPhysicsCtrlPtr,
            GameManagerImp.ChrCtrlOffsets.ChrPhysicsCtrl.Gravity
        }, false);
        
        public void ToggleKillboxHook(bool isEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.Killbox;

            if (isEnabled)
            {
                var hookLoc = Hooks.KillboxFlagSet;
                var playerCtrl = _memoryIo.ReadInt64((IntPtr)_memoryIo.ReadInt64(GameManagerImp.Base) +
                                                     GameManagerImp.Offsets.PlayerCtrl);
                
                var codeBytes = AsmLoader.GetAsmBytes("Killbox");
                var bytes = BitConverter.GetBytes(playerCtrl);
                Array.Copy(bytes, 0, codeBytes, 0x1 + 2, 8);
                AsmHelper.WriteRelativeOffsets(codeBytes, new[]
                {
                    (code.ToInt64() + 0x21, hookLoc + 0xA, 5, 0x21 + 1)
                });
                
                _memoryIo.WriteBytes(code, codeBytes);
                _hookManager.InstallHook(code.ToInt64(), hookLoc, new byte[]
                    { 0x48, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00 });
            }
            else
            {
                _hookManager.UninstallHook(code.ToInt64());
            }
        }

        public void ToggleDrawHitbox(bool isDrawHitboxEnabled)
        {
            _dllManager.ToggleRender(DrawType.Hitbox, isDrawHitboxEnabled);
        }

        public void Inject()
        {
            _dllManager.InjectDrawDll();
        }

        public void ToggleDrawEvent(bool isDrawEventEnabled)
        {
            _dllManager.ToggleRender(DrawType.Event, isDrawEventEnabled);
        }

        public void ToggleDrawSound(bool isDrawSoundEnabled) =>
            _dllManager.ToggleRender(DrawType.Sound, isDrawSoundEnabled);
        public void ToggleTargetingView(bool isTargetingViewEnabled) =>
            _dllManager.ToggleRender(DrawType.TargetingView, isTargetingViewEnabled);

        public void ToggleRagdoll(bool isDrawRagrollEnabled) =>
            _dllManager.ToggleRender(DrawType.Ragdoll, isDrawRagrollEnabled);

        public void ToggleHideChr(bool isHideCharactersEnabled) =>
            _memoryIo.WriteBytes(Patches.HideChrModels,
                isHideCharactersEnabled ? new byte[] { 0x75, 0x5 } : new byte[] { 0x74, 0x5 });

        public void ToggleHideMap(bool isHideMapEnabled) => 
            _memoryIo.WriteBytes(Patches.HideMap + 0x1, //js rel to jns rel
                isHideMapEnabled ? new byte[] { 0x89 } : new byte[] { 0x88 });

        public void SetGameSpeed(float value) => _dllManager.SetSpeed(value);

        public void ToggleRagdollEsp(bool isSeeThroughwallsEnabled) => 
            _dllManager.ToggleRender(DrawType.RagdollEsp, isSeeThroughwallsEnabled);

        public void ToggleDrawCol(bool isDrawCollisionEnabled) =>
            _dllManager.ToggleRender(DrawType.Collision, isDrawCollisionEnabled);

        public void ToggleDrawKillbox(bool isDrawKillboxEnabled) =>
            _dllManager.ToggleRender(DrawType.CollisionKillbox, isDrawKillboxEnabled);

        public void ToggleColWireframe(bool isColWireframeEnabled) =>
            _dllManager.ToggleRender(DrawType.CollisionWireframe, isColWireframeEnabled);

        public void ToggleSnowstormHook(bool isSnowstormDisabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.Snowstorm;

            if (isSnowstormDisabled)
            {
                var origin = Hooks.EzStateSetEvent;
                var bytes = AsmLoader.GetAsmBytes("DisableSnowstorm");
                var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 11, code + 0x1C);
                Array.Copy(jmpBytes, 0, bytes, 0x17 + 1, 4);
                _memoryIo.WriteBytes(code, bytes);
                _hookManager.InstallHook(code.ToInt64(), origin, new byte[]
                    { 0x41, 0x0F, 0xB6, 0xF8, 0x48, 0x8B, 0x88, 0xF0, 0x22, 0x00, 0x00 });
            }
            else
            {
                _hookManager.UninstallHook(code.ToInt64());
            }
        }

        public void ToggleMemoryTimer(bool isMemoryTimerDisabled)
        {
            // var code = CodeCaveOffsets.Base + CodeCaveOffsets.DisableMemoryTimer;
            //
            // if (isMemoryTimerDisabled)
            // {
            //     var origin = Hooks.ConditionGroupSetFlag;
            //     var bytes = AsmLoader.GetAsmBytes("DisableMemoryTimer");
            //     AsmHelper.WriteRelativeOffsets(bytes, new []
            //     {
            //         (code.ToInt64() + 0x18, origin + 7, 5, 0x18 + 1),
            //         (code.ToInt64() + 0x30, origin + 7, 5, 0x30 + 1),
            //         (code.ToInt64() + 0x3D, origin + 7, 5, 0x3D + 1),
            //     });
            //     
            //     _memoryIo.WriteBytes(code, bytes);
            //     _hookManager.InstallHook(code.ToInt64(), origin, new byte[]
            //         { 0x80, 0x7B, 0x1D, 0x00, 0x89, 0x73, 0x18 });
            // }
            // else
            // {
            //     _hookManager.UninstallHook(code.ToInt64());
            // }
        }

        public void ToggleIvorySkip(bool isIvorySkipEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.IvorySkip;
            var knightsCode = CodeCaveOffsets.Base + CodeCaveOffsets.IvoryKnights;

            if (isIvorySkipEnabled)
            {
                var origin = Funcs.SetEvent;
                var getComponent = Funcs.GetMapObjStateActComponent;
                var getMapEntity = Funcs.GetMapEntityWithAreaIdAndObjId;

                var bytes = AsmLoader.GetAsmBytes("IvorySkip");
                
                AsmHelper.WriteAbsoluteAddresses(bytes, new []
                {
                  (getMapEntity, 0x5C + 2),
                  (getComponent, 0x66 + 2),
                  (origin, 0x70 + 2)
                });

                var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 5, code + 0xDB);
                Array.Copy(jmpBytes, 0, bytes, 0xD6 + 1, 4);
                
                _memoryIo.WriteBytes(code, bytes);

                var setSharedFlag = Hooks.SetSharedFlag;
                bytes = AsmLoader.GetAsmBytes("IvoryKnights");
                jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(setSharedFlag, 8, knightsCode + 0x24);
                Array.Copy(jmpBytes, 0, bytes, 0x1F + 1, 4);
                _memoryIo.WriteBytes(knightsCode, bytes);
                
                _hookManager.InstallHook(code.ToInt64(), origin, new byte[]
                    { 0x48, 0x89, 0x74, 0x24, 0x10 });
                _hookManager.InstallHook(knightsCode.ToInt64(), setSharedFlag, new byte[]
                    { 0x44, 0x88, 0x84, 0x08, 0xA1, 0x03, 0x00, 0x00 });
            }
            else
            {
                _hookManager.UninstallHook(code.ToInt64());
                _hookManager.UninstallHook(knightsCode.ToInt64());
            }
        }
    }
    
}