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
            
            var inAirTimerCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.InAirTimer;
            var triggersAndSpaceCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.TriggersAndSpaceCheck;
            var ctrlCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.CtrlCheck;
            var coordsCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.UpdateCoords;
            var raycastCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.RayCastCode;

            if (isNoClipEnabled)
            {
                var inAirTimerHook = Hooks.InAirTimer;
                var triggersAndSpaceHook = Hooks.TriggersAndSpace;
                var ctrlHook = Hooks.Ctrl;
                var coordsHook = Hooks.NoClipUpdateCoords;
                var rayCastHook = Hooks.MapIdWrite;
                
                
                var zDirectionLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.ZDirection;
                var codeBytes = AsmLoader.GetAsmBytes("NoClip_InAirTimer");
                var inAirPlayerIdentifier = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
                {
                    GameManagerImp.Offsets.PlayerCtrl,
                    GameManagerImp.PlayerCtrlOffsets.ChrCullingGroupCtrlPtr,
                    GameManagerImp.PlayerCtrlOffsets.ChrCullingGroupCtrl.InAirTimerEntity
                }, false);

                var bytes = BitConverter.GetBytes(inAirPlayerIdentifier.ToInt64());
                Array.Copy(bytes, 0, codeBytes, 0x1 + 2, 8);
                bytes = AsmHelper.GetJmpOriginOffsetBytes(inAirTimerHook, 5, inAirTimerCode + 0x1E);
                Array.Copy(bytes, 0, codeBytes, 0x19 + 1, 4);
                _memoryIo.WriteBytes(inAirTimerCode, codeBytes);


                codeBytes = AsmLoader.GetAsmBytes("NoClip_TriggersAndSpace");
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
                
                
                
                var updateCoordsPIdentifier = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
                {
                    GameManagerImp.Offsets.PlayerCtrl,
                }, true);
                
                var rigidBodyCoords = _memoryIo.FollowPointers(HkHardwareInfo.Base, new[]
                {
                    HkHardwareInfo.HkpWorld,
                    HkHardwareInfo.HkpChrRigidBodyPtr,
                    HkHardwareInfo.HkpChrRigidBody,
                    HkHardwareInfo.HkpRigidBodyPtr,
                    HkHardwareInfo.HkpRigidBody.PlayerIdentifier,
                } ,false);
                
                var movement = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
                {
                    GameManagerImp.Offsets.PlayerCtrl,
                    GameManagerImp.PlayerCtrlOffsets.PlayerOperatorPtr,
                    GameManagerImp.PlayerCtrlOffsets.PlayerOperator.ChrPadMan,
                    GameManagerImp.PlayerCtrlOffsets.PlayerOperator.MovementEntity
                }, true);

                var cam = _memoryIo.ReadInt64((IntPtr)_memoryIo.ReadInt64(GameManagerImp.Base) +
                                              GameManagerImp.Offsets.ViewMatrixPtr);

                codeBytes = AsmLoader.GetAsmBytes("NoClip_UpdateCoords");
                AsmHelper.WriteAbsoluteAddresses(codeBytes, new []
                {
                    (updateCoordsPIdentifier.ToInt64(), 0x1 + 2),
                    (movement.ToInt64(), 0x1C + 2),
                    (cam, 0x47 + 2),
                    (movement.ToInt64(), 0x5A + 2),
                    (cam, 0x85 +2),
                    (rigidBodyCoords.ToInt64(), 0xD0 + 2)
                });
                
                AsmHelper.WriteRelativeOffsets(codeBytes, new []
                {
                    (coordsCode.ToInt64() + 0x97, zDirectionLoc.ToInt64(), 6, 0x97 + 2),
                    (coordsCode.ToInt64() + 0xC1, zDirectionLoc.ToInt64(), 7, 0xC1 + 2),
                    (coordsCode.ToInt64() + 0x134, coordsHook + 0x8, 5, 0x134 + 1),
                    (coordsCode.ToInt64() + 0x142, coordsHook + 0x8, 5, 0x142 + 1)
                });
                
                _memoryIo.WriteBytes(coordsCode, codeBytes);

                codeBytes = AsmLoader.GetAsmBytes("NoClip_RayCast");

                var frameCounter = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.FrameCounter;
                var rayInput = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.RayInput;
                var rayOutput = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.RayOutput;
                var mapId = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.MapId;

                var pxWorld = _memoryIo.ReadInt64((IntPtr)_memoryIo.ReadInt64(GameManagerImp.Base) +
                                                  GameManagerImp.Offsets.PxWorld);
                var raycastFunc = Funcs.HavokRayCast;
                var convertToMap = Funcs.ConvertPxRigidToMapEntity;
                var convertToMapId = Funcs.ConvertMapEntityToGameId;
                
                AsmHelper.WriteRelativeOffsets(codeBytes, new []
                {
                    (raycastCode.ToInt64() + 0x1C, frameCounter.ToInt64(), 6, 0x1C + 2),
                    (raycastCode.ToInt64() + 0x22, frameCounter.ToInt64(), 7, 0x22 + 2),
                    (raycastCode.ToInt64() + 0x32, frameCounter.ToInt64(), 6, 0x32 + 2),
                    (raycastCode.ToInt64() + 0x51, rayInput.ToInt64(), 7, 0x51 + 3),
                    (raycastCode.ToInt64() + 0x8F, rayOutput.ToInt64(), 7, 0x8F + 3),
                    (raycastCode.ToInt64() + 0xA6, raycastFunc, 5, 0xA6 + 1),
                    (raycastCode.ToInt64() + 0xB2, rayOutput.ToInt64() + 0x10, 7, 0xB2 + 2),
                    (raycastCode.ToInt64() + 0xBF, rayInput.ToInt64(), 7, 0xBF + 3),
                    (raycastCode.ToInt64() + 0xEA, rayOutput.ToInt64(), 7, 0xEA + 3),
                    (raycastCode.ToInt64() + 0x10B, raycastFunc, 5, 0x10B + 1),
                    (raycastCode.ToInt64() + 0x117, rayOutput.ToInt64() + 0x10, 7, 0x117 + 2),
                    (raycastCode.ToInt64() + 0x124, rayInput.ToInt64(), 7, 0x124 + 3),
                    (raycastCode.ToInt64() + 0x14F, rayOutput.ToInt64(), 7, 0x14F + 3),
                    (raycastCode.ToInt64() + 0x170, raycastFunc, 5, 0x170 + 1),
                    (raycastCode.ToInt64() + 0x17C, rayOutput.ToInt64() + 0x10, 7, 0x17C + 2),
                    (raycastCode.ToInt64() + 0x189, rayInput.ToInt64(), 7, 0x189 + 3),
                    (raycastCode.ToInt64() + 0x1B4, rayOutput.ToInt64(), 7, 0x1B4 + 3),
                    (raycastCode.ToInt64() + 0x1D5, raycastFunc, 5, 0x1D5 + 1),
                    (raycastCode.ToInt64() + 0x1E1, rayOutput.ToInt64() + 0x10, 7, 0x1E1 + 2),
                    (raycastCode.ToInt64() + 0x1EA, rayInput.ToInt64(), 7, 0x1EA + 3),
                    (raycastCode.ToInt64() + 0x215, rayOutput.ToInt64(), 7, 0x215 + 3),
                    (raycastCode.ToInt64() + 0x236, raycastFunc, 5, 0x236 + 1),
                    (raycastCode.ToInt64() + 0x242, rayOutput.ToInt64() + 0x10, 7, 0x242 + 2),
                    
                    (raycastCode.ToInt64() + 0x25C, convertToMap, 5, 0x25C + 1),
                    (raycastCode.ToInt64() + 0x268, mapId.ToInt64(), 7, 0x268 + 3),
                    (raycastCode.ToInt64() + 0x273, convertToMapId, 5, 0x273 + 1),
                    (raycastCode.ToInt64() + 0x28A, rayCastHook + 0x7, 5, 0x28A + 1)
                });
                
                AsmHelper.WriteAbsoluteAddresses(codeBytes, new []
                {
                    (updateCoordsPIdentifier.ToInt64(), 0x8 + 2),
                    (pxWorld, 0x3D + 2),
                    (updateCoordsPIdentifier.ToInt64(), 0x47 + 2),
                    (pxWorld, 0xFA + 2),
                    (pxWorld, 0x15F + 2),
                    (pxWorld, 0x1C4 + 2),
                    (pxWorld, 0x225 + 2),
                    (rayOutput.ToInt64() + 0x18, 0x24B + 2)
                });

                _memoryIo.WriteBytes(raycastCode, codeBytes);
                
                
                _memoryIo.WriteByte(GetGravityPtr(), 1);
                
                _hookManager.InstallHook(inAirTimerCode.ToInt64(), inAirTimerHook, new byte[]
                    { 0xF3, 0x0F, 0x11, 0x4F, 0x10 });
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
                _hookManager.UninstallHook(inAirTimerCode.ToInt64());
                _hookManager.UninstallHook(raycastCode.ToInt64());
            }
            
        }

        public void SetNoClipSpeed(byte[] xBytes, byte[] yBytes)
        {
            _memoryIo.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.UpdateCoords + 0x72 + 1,
                xBytes);
            _memoryIo.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.UpdateCoords + 0x34 + 1,
                yBytes);
        }

        private IntPtr GetGravityPtr() => _memoryIo.FollowPointers(GameManagerImp.Base, new[]
        {
            GameManagerImp.Offsets.PlayerCtrl,
            GameManagerImp.PlayerCtrlOffsets.ChrPhysicsCtrlPtr,
            GameManagerImp.PlayerCtrlOffsets.ChrPhysicsCtrl.Gravity
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
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.DisableMemoryTimer;

            if (isMemoryTimerDisabled)
            {
                var origin = Hooks.ConditionGroupSetFlag;
                var bytes = AsmLoader.GetAsmBytes("DisableMemoryTimer");
                AsmHelper.WriteRelativeOffsets(bytes, new []
                {
                    (code.ToInt64() + 0x18, origin + 7, 5, 0x18 + 1),
                    (code.ToInt64() + 0x30, origin + 7, 5, 0x30 + 1),
                    (code.ToInt64() + 0x3D, origin + 7, 5, 0x3D + 1),
                });
                
                _memoryIo.WriteBytes(code, bytes);
                _hookManager.InstallHook(code.ToInt64(), origin, new byte[]
                    { 0x80, 0x7B, 0x1D, 0x00, 0x89, 0x73, 0x18 });
            }
            else
            {
                _hookManager.UninstallHook(code.ToInt64());
            }
        }

        public void ToggleIvorySkip(bool isIvorySkipEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.IvorySkip;

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
                _hookManager.InstallHook(code.ToInt64(), origin, new byte[]
                    { 0x48, 0x89, 0x74, 0x24, 0x10 });
            }
            else
            {
                _hookManager.UninstallHook(code.ToInt64());
            }
        }
    }
    
}