using System;
using System.Collections.Generic;
using SilkySouls2.Memory;
using SilkySouls2.Memory.DLLShared;
using SilkySouls2.Models;
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
                var rayCastHook = Hooks.ProcessPhysics;

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
                    (raycastCode.ToInt64() + 0x24, frameCounter.ToInt64(), 6, 0x24 + 2),
                    (raycastCode.ToInt64() + 0x2A, frameCounter.ToInt64(), 7, 0x2A + 2),
                    (raycastCode.ToInt64() + 0x3A, frameCounter.ToInt64(), 6, 0x3A + 2),
                    (raycastCode.ToInt64() + 0x8B, rayInput.ToInt64(), 7, 0x8B + 3),
                    (raycastCode.ToInt64() + 0xC9, rayOutput.ToInt64(), 7, 0xC9 + 3),
                    (raycastCode.ToInt64() + 0xE0, raycastFunc, 5, 0xE0 + 1),
                    (raycastCode.ToInt64() + 0xEC, rayOutput.ToInt64() + 0x10, 7, 0xEC + 2),
                    (raycastCode.ToInt64() + 0xF9, rayInput.ToInt64(), 7, 0xF9 + 3),
                    (raycastCode.ToInt64() + 0x124, rayOutput.ToInt64(), 7, 0x124 + 3),
                    (raycastCode.ToInt64() + 0x142, raycastFunc, 5, 0x142 + 1),
                    (raycastCode.ToInt64() + 0x14E, rayOutput.ToInt64() + 0x10, 7, 0x14E + 2),
                    (raycastCode.ToInt64() + 0x15B, rayInput.ToInt64(), 7, 0x15B + 3),
                    (raycastCode.ToInt64() + 0x186, rayOutput.ToInt64(), 7, 0x186 + 3),
                    (raycastCode.ToInt64() + 0x1A4, raycastFunc, 5, 0x1A4 + 1),
                    (raycastCode.ToInt64() + 0x1B0, rayOutput.ToInt64() + 0x10, 7, 0x1B0 + 2),
                    (raycastCode.ToInt64() + 0x1BD, rayInput.ToInt64(), 7, 0x1BD + 3),
                    (raycastCode.ToInt64() + 0x1E8, rayOutput.ToInt64(), 7, 0x1E8 + 3),
                    (raycastCode.ToInt64() + 0x206, raycastFunc, 5, 0x206 + 1),
                    (raycastCode.ToInt64() + 0x212, rayOutput.ToInt64() + 0x10, 7, 0x212 + 2),
                    (raycastCode.ToInt64() + 0x21B, rayInput.ToInt64(), 7, 0x21B + 3),
                    (raycastCode.ToInt64() + 0x246, rayOutput.ToInt64(), 7, 0x246 + 3),
                    (raycastCode.ToInt64() + 0x264, raycastFunc, 5, 0x264 + 1),
                    (raycastCode.ToInt64() + 0x270, rayOutput.ToInt64() + 0x10, 7, 0x270 + 2),
                    (raycastCode.ToInt64() + 0x28A, convertToMap, 5, 0x28A + 1),
                    (raycastCode.ToInt64() + 0x296, mapId.ToInt64(), 7, 0x296 + 3),
                    (raycastCode.ToInt64() + 0x2A1, convertToMapId, 5, 0x2A1 + 1),
                    (raycastCode.ToInt64() + 0x2EA, rayCastHook + 0x5, 5, 0x2EA + 1)
                });
                
                AsmHelper.WriteAbsoluteAddresses(codeBytes, new []
                {
                    (GameManagerImp.Base.ToInt64(), 0x6 + 2),
                    (GameManagerImp.Base.ToInt64(), 0x70 + 2),
                    (rayOutput.ToInt64() + 0x18, 0x279 + 2)
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
                    { 0x48, 0x8D, 0x54, 0x24, 0x20 });

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

        public void ToggleDrawEvent(DrawType eventType, bool isDrawEventEnabled)
        {
            _dllManager.ToggleRender(eventType, isDrawEventEnabled);
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
                var origin = Hooks.EzStateCompareTimer;
                var codeBytes = AsmLoader.GetAsmBytes("DisableMemoryTimer");

                var bytes = BitConverter.GetBytes(GameManagerImp.Base.ToInt64());
                Array.Copy(bytes, 0, codeBytes, 0x2 + 2, 8);

                bytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 7, code + 0xC1);
                Array.Copy(bytes, 0, codeBytes, 0xBC + 1, 4);
                
                _memoryIo.WriteBytes(code, codeBytes);
                _hookManager.InstallHook(code.ToInt64(), origin, new byte[]
                    { 0x66, 0x0F, 0x6E, 0x30, 0x0F, 0x5B, 0xF6 });
            }
            else
            {
                _hookManager.UninstallHook(code.ToInt64());
            }
        }

        public void ToggleIvorySkip(bool isIvorySkipEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.IvorySkip;
            var knightsCode = CodeCaveOffsets.Base + CodeCaveOffsets.IvoryKnights;

            if (isIvorySkipEnabled)
            {
                var origin = Funcs.SetEvent;
                var getMapEntity = Funcs.GetMapEntityWithAreaIdAndObjId;
                var getComponent = Funcs.GetMapObjStateActComponent;

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
        

        public void SetObjState(long areaId, GameIds.Obj.SetObjState objData)
        {
            var getMapEntity = Funcs.GetMapEntityWithAreaIdAndObjId;
            var getComponent = Funcs.GetMapObjStateActComponent;

            var bytes = AsmLoader.GetAsmBytes("SetObjState");
            AsmHelper.WriteAbsoluteAddresses(bytes, new []
            {
                (areaId, 0x4 + 2),
                (objData.ObjId, 0xE + 2),
                (getMapEntity, 0x18 + 2),
                (getComponent, 0x2E + 2),
                (objData.State, 0x41 + 2)
            });
            
            _memoryIo.AllocateAndExecute(bytes);
        }

        public void DisableNavimesh(long areaId, GameIds.Navimesh.DisableNavimesh naviData)
        {
            var eventPointMan = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.EventManager,
                GameManagerImp.EventManagerOffsets.EventPointManager
            }, true);

            var getNaviLoc = Funcs.GetNavimeshLoc;
            var disableNavi = Funcs.DisableNaviMesh;

            var bytes = AsmLoader.GetAsmBytes("DisableNavimesh");
            AsmHelper.WriteAbsoluteAddresses(bytes, new []
            {
                (eventPointMan.ToInt64(),  2),
                (areaId, 0xA + 2),
                (naviData.EventId, 0x14 + 2),
                (getNaviLoc, 0x25 + 2),
                (naviData.State, 0x34 + 2),
                (disableNavi, 0x3E + 2)
            });
            
            _memoryIo.AllocateAndExecute(bytes);
        }

        public void DisableWhiteDoor(long areaId, GameIds.WhiteDoor.DisableWhiteDoor whiteDoorData)
        {
            
            var getMapEntity = Funcs.GetMapEntityWithAreaIdAndObjId;
            var getComponent = Funcs.GetWhiteDoorComponent;

            var bytes = AsmLoader.GetAsmBytes("DisableWhiteDoorKeyGuide");
            AsmHelper.WriteAbsoluteAddresses(bytes, new []
            {
                (areaId, 0x4 + 2),
                (whiteDoorData.ObjId, 0xE + 2),
                (getMapEntity, 0x18 + 2),
                (getComponent, 0x27 + 2)
            });
            
            _memoryIo.AllocateAndExecute(bytes);
        }

        public List<InventorySpell> GetInventorySpells()
        {
            var spellBase = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr,
                GameManagerImp.GameDataManagerOffsets.Inventory.InventoryLists,
                GameManagerImp.GameDataManagerOffsets.Inventory.ItemInventory2BagListPtr,
                GameManagerImp.GameDataManagerOffsets.Inventory.ItemInventory2BagList.ItemInvetory2SpellListPtr,
            }, true);

            var count = _memoryIo.ReadUInt8(spellBase + GameManagerImp.GameDataManagerOffsets.Inventory.ItemInvetory2SpellList.Count);
            if (count == 0) return new List<InventorySpell>();

            List<InventorySpell> currentSpells = new List<InventorySpell>();
            var current = (IntPtr) _memoryIo.ReadInt64(spellBase + GameManagerImp.GameDataManagerOffsets.Inventory.ItemInvetory2SpellList.ListStart);

            for (int i = 0; i < count && current != IntPtr.Zero; i++)
            {
                var spellId = _memoryIo.ReadInt32(current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.SpellId);
                var isEquipped = _memoryIo.ReadUInt8(current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.IsEquipped);
                var slotReq = _memoryIo.ReadUInt8(current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.SlotReq);
                currentSpells.Add(new InventorySpell(spellId, isEquipped == 2, current, slotReq));
                current = (IntPtr) _memoryIo.ReadInt64(current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.NextPtr);
            }

            return currentSpells;
        }

        public List<EquippedSpell> GetEquippedSpells()
        {
            var currentSpell = GetCurrentSpellPtr();

            List<EquippedSpell> currentSpells = new List<EquippedSpell>();

            for (int i = 0; i < 14; i++)
            {
                currentSpells.Add(new EquippedSpell(_memoryIo.ReadInt32(currentSpell), i));
                currentSpell += 0x10;
            }

            return currentSpells;
        }

        private IntPtr GetCurrentSpellPtr()
        {
            return _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.PlayerCtrl,
                GameManagerImp.ChrCtrlOffsets.EquippedSpellsPtr,
                GameManagerImp.ChrCtrlOffsets.EquippedSpellsStart
            }, false);
        }

        public int GetTotalAvailableSlots()
        {
            var inventory = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr
            }, true);
            var getNumOfSlots1 = Funcs.GetNumOfSpellslots1;
            var getNumOfSlots2 = Funcs.GetNumOfSpellslots2;
            var slotsLoc = CodeCaveOffsets.Base + CodeCaveOffsets.NumOfSpellSlots;

            var bytes = AsmLoader.GetAsmBytes("GetNumOfSlots");
            
            AsmHelper.WriteAbsoluteAddresses(bytes, new []
            {
                (slotsLoc.ToInt64(), 2),
                (inventory.ToInt64(), 0xA + 2),
                (getNumOfSlots1, 0x17 + 2),
                (getNumOfSlots2, 0x2C + 2)
            });

            _memoryIo.AllocateAndExecute(bytes);


            return _memoryIo.ReadInt32(slotsLoc);
        }
        
        public void AttuneSpell(int slotIndex, IntPtr entryAddr)
        {
            var inventoryLists = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr,
                GameManagerImp.GameDataManagerOffsets.Inventory.InventoryLists,
            }, true);

            var attuneFunc = Funcs.AttuneSpell;

            var bytes = AsmLoader.GetAsmBytes("AttuneSpell");
            AsmHelper.WriteAbsoluteAddresses(bytes, new []
            {
                (inventoryLists.ToInt64(), 2),
                (slotIndex + 0x1C, 0xA + 2),
                (entryAddr.ToInt64(), 0x14 + 2),
                (attuneFunc, 0x1E + 2)
            });
            
            _memoryIo.AllocateAndExecute(bytes);
            
            
        }
    }
    
}