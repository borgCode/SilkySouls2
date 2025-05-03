using System;
using System.Threading.Tasks;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class UtilityService
    {
        private readonly MemoryIo _memoryIo;
        private readonly HookManager _hookManager;

        public UtilityService(MemoryIo memoryIo, HookManager hookManager)
        {
            _memoryIo = memoryIo;
            _hookManager = hookManager;
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

        public async Task ForceSave()
        {
            var forceSavePtr = Patches.ForceSave;
            _memoryIo.WriteByte(forceSavePtr, 0x75);
            _memoryIo.WriteByte(forceSavePtr + 0x8, 0xEB);
            await Task.Delay(10);
            _memoryIo.WriteByte(forceSavePtr, 0x74);
            _memoryIo.WriteByte(forceSavePtr + 0x8, 0x76);
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
      
            

            if (isNoClipEnabled)
            {
                var inAirTimerHook = Hooks.InAirTimer;
                var triggersAndSpaceHook = Hooks.TriggersAndSpace;
                var ctrlHook = Hooks.Ctrl;
                var coordsHook = Hooks.NoClipUpdateCoords;
                
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

                var updateCoordsPIdentifier = _memoryIo.FollowPointers(HkHardwareInfo.Base, new[]
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
                                              GameManagerImp.Offsets.CamStuff);

                codeBytes = AsmLoader.GetAsmBytes("NoClip_UpdateCoords");
                AsmHelper.WriteAbsoluteAddresses(codeBytes, new []
                {
                    (updateCoordsPIdentifier.ToInt64(), 0x4 + 2),
                    (movement.ToInt64(), 0x1C + 2),
                    (cam, 0x44 + 2),
                    (movement.ToInt64(), 0x57 + 2),
                    (cam, 0x7F +2)
                });
                
                AsmHelper.WriteRelativeOffsets(codeBytes, new []
                {
                    (coordsCode.ToInt64() + 0x91, zDirectionLoc.ToInt64(), 6, 0x91 + 2),
                    (coordsCode.ToInt64() + 0xB9, zDirectionLoc.ToInt64(), 7, 0xB9 + 2),
                    (coordsCode.ToInt64() + 0xD8, coordsHook + 0x7, 5, 0xD8 + 1),
                    (coordsCode.ToInt64() + 0xE2, coordsHook + 0x7, 5, 0xE2 + 1)
                });
                
                _memoryIo.WriteBytes(coordsCode, codeBytes);
                
                _hookManager.InstallHook(inAirTimerCode.ToInt64(), inAirTimerHook, new byte[]
                    { 0xF3, 0x0F, 0x11, 0x4F, 0x10 });
                _hookManager.InstallHook(triggersAndSpaceCode.ToInt64(), triggersAndSpaceHook, new byte[]
                    { 0x4C, 0x8B, 0x7C, 0x24, 0x70, 0x48, 0x8B, 0x43, 0x08 });
                _hookManager.InstallHook(ctrlCode.ToInt64(), ctrlHook, new byte[]
                    { 0x81, 0x8B, 0x28, 0x02, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00 });
                _hookManager.InstallHook(coordsCode.ToInt64(), coordsHook, 
                new byte[] { 0x0F, 0x5C, 0xC2, 0x0F, 0x29, 0x47, 0x50 });
    
            }
            else
            {
                _hookManager.UninstallHook(inAirTimerCode.ToInt64());
                _hookManager.UninstallHook(triggersAndSpaceCode.ToInt64());
                _hookManager.UninstallHook(ctrlCode.ToInt64());
                _hookManager.UninstallHook(coordsCode.ToInt64());
            }
            
        }
    }
}