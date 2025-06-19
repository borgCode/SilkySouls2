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
            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                var bytes = AsmLoader.GetAsmBytes("SetEventOn64");
                AsmHelper.WriteAbsoluteAddresses64(bytes, new[]
                {
                    (eventFlagMan.ToInt64(), 0x0 + 2),
                    (gameId, 0xA + 2),
                    (setEventFunc, 0x1A + 2)
                });

                _memoryIo.AllocateAndExecute(bytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes("SetEventOn32");
                AsmHelper.WriteAbsoluteAddresses32(bytes, new[]
                {
                    (eventFlagMan.ToInt64(), 1),
                    (gameId, 0x7 + 1),
                    (setEventFunc, 0xC + 1)
                });

                _memoryIo.AllocateAndExecute(bytes);
            }
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

            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                var bytes = AsmLoader.GetAsmBytes("SetEventOff64");
                AsmHelper.WriteAbsoluteAddresses64(bytes, new[]
                {
                    (eventFlagMan.ToInt64(), 0x0 + 2),
                    (gameId, 0xA + 2),
                    (setEventFunc, 0x1A + 2)
                });

                _memoryIo.AllocateAndExecute(bytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes("SetEventOff32");
                AsmHelper.WriteAbsoluteAddresses32(bytes, new[]
                {
                    (eventFlagMan.ToInt64(), 1),
                    (gameId, 0x7 + 1),
                    (setEventFunc, 0xC + 1)
                });

                _memoryIo.AllocateAndExecute(bytes);
            }
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
            long saveLoadSystem;
            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                saveLoadSystem = _memoryIo.ReadInt64((IntPtr)_memoryIo.ReadInt64(GameManagerImp.Base) +
                                                     GameManagerImp.Offsets.SaveLoadSystemPtr);
            }
            else
            {
                saveLoadSystem = _memoryIo.ReadInt32((IntPtr)_memoryIo.ReadInt32(GameManagerImp.Base) +
                                                     GameManagerImp.Offsets.SaveLoadSystemPtr);
            }

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

                if (GameVersion.Current.Edition == GameEdition.Scholar)
                {
                    var codeBytes = AsmLoader.GetAsmBytes("CreditSkip64");
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
                    var codeBytes = AsmLoader.GetAsmBytes("CreditSkip32");
                    var bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 6, code + 0x25);
                    Array.Copy(bytes, 0, codeBytes, 0x20 + 1, 4);
                    AsmHelper.WriteAbsoluteAddresses32(codeBytes, new[]
                    {
                        (modifyOnceFlag.ToInt64(), 0x6 + 2),
                        (modifyOnceFlag.ToInt64(), 0x16 + 2)
                    });

                    _memoryIo.WriteBytes(code, codeBytes);
                    _hookManager.InstallHook(code.ToInt64(), hookLoc, new byte[]
                        { 0x81, 0xEC, 0xFC, 0x01, 0x00, 0x00 });
                }
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
                if (GameVersion.Current.Edition == GameEdition.Scholar)
                    Setup100DropScholar(dropCountCode, dropCountHook);
                else Setup100DropVanilla(dropCountCode, dropCountHook);
            }
            else
            {
                _memoryIo.WriteBytes(Patches.DropRate,
                    GameVersion.Current.Edition == GameEdition.Scholar
                        ? new byte[] { 0x41, 0xF7, 0xF2 }
                        : new byte[] { 0xF7, 0xF6 });

                _hookManager.UninstallHook(dropCountCode.ToInt64());
            }
        }

        private void Setup100DropScholar(IntPtr dropCountCode, long dropCountHook)
        {
            _memoryIo.WriteBytes(Patches.DropRate, new byte[] { 0x90, 0x90, 0x90 });

            var codeBytes = AsmLoader.GetAsmBytes("DropCount64");
            var bytes = AsmHelper.GetJmpOriginOffsetBytes(dropCountHook, 5, dropCountCode + 0xA);
            Array.Copy(bytes, 0, codeBytes, 0x5 + 1, 4);
            _memoryIo.WriteBytes(dropCountCode, codeBytes);
            _hookManager.InstallHook(dropCountCode.ToInt64(), dropCountHook, new byte[]
                { 0x41, 0x0F, 0xB6, 0x47, 0x01 });
        }

        private void Setup100DropVanilla(IntPtr dropCountCode, long dropCountHook)
        {
            _memoryIo.WriteBytes(Patches.DropRate, new byte[] { 0x90, 0x90 });

            var codeBytes = AsmLoader.GetAsmBytes("DropCount32");
            var bytes = AsmHelper.GetJmpOriginOffsetBytes(dropCountHook, 5, dropCountCode + 0xB);
            Array.Copy(bytes, 0, codeBytes, 0x6 + 1, 4);
            _memoryIo.WriteBytes(dropCountCode, codeBytes);

            _hookManager.InstallHook(dropCountCode.ToInt64(), dropCountHook, new byte[]
                { 0x0F, 0xB6, 0x51, 0x01, 0x40 });
        }


        public void ToggleNoClip(bool isNoClipEnabled)
        {
            var triggersAndSpaceCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.TriggersAndSpaceCheck;
            var ctrlCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.CtrlCheck;
            var coordsCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.UpdateCoords;
            var raycastCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.RayCastCode;

            if (isNoClipEnabled)
            {
                if (GameVersion.Current.Edition == GameEdition.Scholar)
                {
                    SetupNoClipHooks64Bit(triggersAndSpaceCode, ctrlCode, coordsCode, raycastCode);
                }
                else
                {
                    SetupNoClipHooks32Bit(triggersAndSpaceCode, ctrlCode, coordsCode, raycastCode);
                }
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

        private void SetupNoClipHooks64Bit(IntPtr triggersAndSpaceCode, IntPtr ctrlCode, IntPtr coordsCode,
            IntPtr raycastCode)
        {
            var triggersAndSpaceHook = Hooks.TriggersAndSpace;
            var ctrlHook = Hooks.Ctrl;
            var coordsHook = Hooks.NoClipUpdateCoords;
            var rayCastHook = Hooks.ProcessPhysics;

            var zDirectionLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.ZDirection;

            var codeBytes = AsmLoader.GetAsmBytes("NoClip_TriggersAndSpace64");
            AsmHelper.WriteRelativeOffsets(codeBytes, new[]
            {
                (triggersAndSpaceCode.ToInt64() + 0x1C, zDirectionLoc.ToInt64(), 7, 0x1C + 2),
                (triggersAndSpaceCode.ToInt64() + 0x35, zDirectionLoc.ToInt64(), 7, 0x35 + 2),
                (triggersAndSpaceCode.ToInt64() + 0x4E, zDirectionLoc.ToInt64(), 7, 0x4E + 2),
                (triggersAndSpaceCode.ToInt64() + 0x56, triggersAndSpaceHook + 0x9, 5, 0x56 + 1),
            });
            _memoryIo.WriteBytes(triggersAndSpaceCode, codeBytes);

            codeBytes = AsmLoader.GetAsmBytes("NoClip_CtrlCheck64");
            AsmHelper.WriteRelativeOffsets(codeBytes, new[]
            {
                (ctrlCode.ToInt64(), zDirectionLoc.ToInt64(), 7, 0x0 + 2),
                (ctrlCode.ToInt64() + 0x7, ctrlHook + 0xA, 5, 0x7 + 1)
            });

            _memoryIo.WriteBytes(ctrlCode, codeBytes);

            codeBytes = AsmLoader.GetAsmBytes("NoClip_UpdateCoords64");
            AsmHelper.WriteAbsoluteAddresses64(codeBytes, new[]
            {
                (GameManagerImp.Base.ToInt64(), 0x1 + 2),
                (GameManagerImp.Base.ToInt64(), 0x29 + 2),
                (GameManagerImp.Base.ToInt64(), 0x73 + 2),
                (HkHardwareInfo.Base.ToInt64(), 0xFB + 2)
            });

            AsmHelper.WriteRelativeOffsets(codeBytes, new[]
            {
                (coordsCode.ToInt64() + 0xC2, zDirectionLoc.ToInt64(), 6, 0xC2 + 2),
                (coordsCode.ToInt64() + 0xEC, zDirectionLoc.ToInt64(), 7, 0xEC + 2),
                (coordsCode.ToInt64() + 0x17F, coordsHook + 0x8, 5, 0x17F + 1),
                (coordsCode.ToInt64() + 0x18D, coordsHook + 0x8, 5, 0x18D + 1)
            });

            _memoryIo.WriteBytes(coordsCode, codeBytes);

            codeBytes = AsmLoader.GetAsmBytes("NoClip_RayCast64");

            var frameCounter = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.FrameCounter;
            var rayInput = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.RayInput;
            var rayOutput = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.RayOutput;
            var mapId = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.MapId;

            var raycastFunc = Funcs.HavokRayCast;
            var convertToMap = Funcs.ConvertPxRigidToMapEntity;
            var convertToMapId = Funcs.ConvertMapEntityToGameId;

            AsmHelper.WriteRelativeOffsets(codeBytes, new[]
            {
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
            });

            AsmHelper.WriteAbsoluteAddresses64(codeBytes, new[]
            {
                (GameManagerImp.Base.ToInt64(), 0x6 + 2),
                (GameManagerImp.Base.ToInt64(), 0x70 + 2)
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

        private void SetupNoClipHooks32Bit(IntPtr triggersAndSpaceCode, IntPtr ctrlCode, IntPtr coordsCode,
            IntPtr raycastCode)
        {
            var triggersAndSpaceHook = Hooks.TriggersAndSpace;
            var ctrlHook = Hooks.Ctrl;
            var coordsHook = Hooks.NoClipUpdateCoords;
            var rayCastHook = Hooks.ProcessPhysics;

            var zDirectionLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.ZDirection;
            var codeBytes = AsmLoader.GetAsmBytes("NoClip_TriggersAndSpace32");
            var bytes = AsmHelper.GetJmpOriginOffsetBytes(triggersAndSpaceHook, 9, triggersAndSpaceCode + 0x52);
            Array.Copy(bytes, 0, codeBytes, 0x4D + 1, 4);
            AsmHelper.WriteAbsoluteAddresses32(codeBytes, new[]
            {
                (zDirectionLoc.ToInt64(), 0x19 + 2),
                (zDirectionLoc.ToInt64(), 0x2F + 2),
                (zDirectionLoc.ToInt64(), 0x45 + 2),
            });
            _memoryIo.WriteBytes(triggersAndSpaceCode, codeBytes);


            codeBytes = AsmLoader.GetAsmBytes("NoClip_CtrlCheck32");
            bytes = BitConverter.GetBytes(zDirectionLoc.ToInt32());
            Array.Copy(bytes, 0, codeBytes, 2, 4);
            bytes = AsmHelper.GetJmpOriginOffsetBytes(ctrlHook, 0xA, ctrlCode + 0xC);
            Array.Copy(bytes, 0, codeBytes, 0x7 + 1, 4);

            _memoryIo.WriteBytes(ctrlCode, codeBytes);


            codeBytes = AsmLoader.GetAsmBytes("NoClip_UpdateCoords32");

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

            _memoryIo.WriteBytes(coordsCode, codeBytes);

            var frameCounter = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.FrameCounter;
            var rayInput = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.RayInput;
            var rayOutput = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.RayOutput;
            var mapId = CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.MapId;

            var raycastFunc = Funcs.HavokRayCast;
            var convertToMap = Funcs.ConvertPxRigidToMapEntity;
            var convertToMapId = Funcs.ConvertMapEntityToGameId;

            codeBytes = AsmLoader.GetAsmBytes("NoClip_RayCast32");

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

            _memoryIo.WriteBytes(raycastCode, codeBytes);

            _memoryIo.WriteByte(GetGravityPtr(), 1);

            _hookManager.InstallHook(triggersAndSpaceCode.ToInt64(), triggersAndSpaceHook, new byte[]
                { 0x8B, 0x56, 0x08, 0x89, 0x86, 0x04, 0x01, 0x00, 0x00 });
            _hookManager.InstallHook(ctrlCode.ToInt64(), ctrlHook, new byte[]
                { 0x81, 0x8E, 0x28, 0x02, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00 });
            _hookManager.InstallHook(coordsCode.ToInt64(), coordsHook, new byte[]
                { 0xF3, 0x0F, 0x7E, 0x45, 0xD0 });
            _hookManager.InstallHook(raycastCode.ToInt64(), rayCastHook, new byte[]
                { 0x8B, 0x8E, 0xB8, 0x00, 0x00, 0x00 });
        }

        public void SetNoClipSpeed(byte[] xBytes, byte[] yBytes)
        {
            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                _memoryIo.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.UpdateCoords + 0xA4 + 1,
                    xBytes);
                _memoryIo.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.UpdateCoords + 0x60 + 1,
                    yBytes);
            }
            else
            {
                _memoryIo.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.UpdateCoords + 0x71 + 1,
                    xBytes);
                _memoryIo.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.NoClip.UpdateCoords + 0x3E + 1,
                    yBytes);
            }
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
                if (GameVersion.Current.Edition == GameEdition.Scholar)
                {
                    var codeBytes = AsmLoader.GetAsmBytes("Killbox64");
                    var bytes = BitConverter.GetBytes(GameManagerImp.Base.ToInt64());
                    Array.Copy(bytes, 0, codeBytes, 0x1 + 2, 8);
                    AsmHelper.WriteRelativeOffsets(codeBytes, new[]
                    {
                        (code.ToInt64() + 0x2b, hookLoc + 0xA, 5, 0x2b + 1)
                    });

                    _memoryIo.WriteBytes(code, codeBytes);
                    _hookManager.InstallHook(code.ToInt64(), hookLoc, new byte[]
                        { 0x48, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00 });
                }
                else
                {
                    var codeBytes = AsmLoader.GetAsmBytes("Killbox32");
                    var bytes = BitConverter.GetBytes(GameManagerImp.Base.ToInt32());
                    Array.Copy(bytes, 0, codeBytes, 0x1 + 1, 4);
                    bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 6, code + 0x1E);
                    Array.Copy(bytes, 0, codeBytes, 0x19 + 1, 4);
                    _memoryIo.WriteBytes(code, codeBytes);
                    _hookManager.InstallHook(code.ToInt64(), hookLoc, new byte[]
                        { 0x53, 0x89, 0xE3, 0x83, 0xEC, 0x08 });
                }
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
                var origin = Hooks.SetEventWrapper;

                if (GameVersion.Current.Edition == GameEdition.Scholar)
                {
                    var bytes = AsmLoader.GetAsmBytes("DisableSnowstorm64");
                    var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 11, code + 0x1C);
                    Array.Copy(jmpBytes, 0, bytes, 0x17 + 1, 4);
                    _memoryIo.WriteBytes(code, bytes);
                    _hookManager.InstallHook(code.ToInt64(), origin, new byte[]
                        { 0x41, 0x0F, 0xB6, 0xF8, 0x48, 0x8B, 0x88, 0xF0, 0x22, 0x00, 0x00 });
                }
                else
                {
                    var bytes = AsmLoader.GetAsmBytes("DisableSnowstorm32");
                    AsmHelper.WriteRelativeOffsets(bytes, new[]
                    {
                        (code.ToInt64() + 0xD, origin + 6, 6, 0xD + 2),
                        (code.ToInt64() + 0x1A, origin + 6, 5, 0x1A + 1),
                    });
                    _memoryIo.WriteBytes(code, bytes);
                    _hookManager.InstallHook(code.ToInt64(), origin, new byte[]
                        { 0x8B, 0x88, 0xCC, 0x0C, 0x00, 0x00 });
                }
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
                if (GameVersion.Current.Edition == GameEdition.Scholar)
                {
                    var codeBytes = AsmLoader.GetAsmBytes("DisableMemoryTimer64");

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
                    var codeBytes = AsmLoader.GetAsmBytes("DisableMemoryTimer32");
                    var bytes = BitConverter.GetBytes(GameManagerImp.Base.ToInt32());
                    Array.Copy(bytes, 0, codeBytes, 0x2 + 2, 4);
                    bytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 7, code + 0x79);
                    Array.Copy(bytes, 0, codeBytes, 0x74 + 1, 4);

                    _memoryIo.WriteBytes(code, codeBytes);
                    _hookManager.InstallHook(code.ToInt64(), origin, new byte[]
                        { 0x66, 0x0F, 0x6E, 0x00, 0x0F, 0x5B, 0xC0 });
                }
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
                var setSharedFlag = Hooks.SetSharedFlag;

                if (GameVersion.Current.Edition == GameEdition.Scholar)
                {
                    var bytes = AsmLoader.GetAsmBytes("IvorySkip64");

                    AsmHelper.WriteAbsoluteAddresses64(bytes, new[]
                    {
                        (getMapEntity, 0x5C + 2),
                        (getComponent, 0x66 + 2),
                        (origin, 0x70 + 2)
                    });

                    var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 5, code + 0xDB);
                    Array.Copy(jmpBytes, 0, bytes, 0xD6 + 1, 4);

                    _memoryIo.WriteBytes(code, bytes);

                    bytes = AsmLoader.GetAsmBytes("IvoryKnights64");
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
                    var bytes = AsmLoader.GetAsmBytes("IvorySkip32");
                    var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 6, code + 0xA7);
                    Array.Copy(jmpBytes, 0, bytes, 0xA1 + 1, 4);
                    AsmHelper.WriteAbsoluteAddresses32(bytes, new[]
                    {
                        (origin, 0x25 + 1),
                        (getMapEntity, 0x46 + 3),
                        (getComponent, 0x4D + 3)
                    });

                    _memoryIo.WriteBytes(code, bytes);


                    bytes = AsmLoader.GetAsmBytes("IvoryKnights32");
                    jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(setSharedFlag, 7, knightsCode + 0x21);
                    Array.Copy(jmpBytes, 0, bytes, 0x1B + 1, 4);
                    _memoryIo.WriteBytes(knightsCode, bytes);

                    _hookManager.InstallHook(code.ToInt64(), origin, new byte[]
                        { 0x55, 0x89, 0xE5, 0x83, 0xEC, 0x08 });
                    _hookManager.InstallHook(knightsCode.ToInt64(), setSharedFlag, new byte[]
                        { 0x88, 0x94, 0x08, 0xA1, 0x02, 0x00, 0x00 });
                }
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

            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                var bytes = AsmLoader.GetAsmBytes("SetObjState64");
                AsmHelper.WriteAbsoluteAddresses64(bytes, new[]
                {
                    (areaId, 0x4 + 2),
                    (objData.ObjId, 0xE + 2),
                    (getMapEntity, 0x18 + 2),
                    (getComponent, 0x2E + 2),
                    (objData.State, 0x41 + 2)
                });

                _memoryIo.AllocateAndExecute(bytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes("SetObjState32");
                AsmHelper.WriteAbsoluteAddresses32(bytes, new[]
                {
                    (getMapEntity, 1),
                    (getComponent, 0x5 + 1),
                    (objData.ObjId, 0xA + 1),
                    (areaId, 0xF + 1),
                    (objData.State, 0x34 + 1)
                });
                _memoryIo.AllocateAndExecute(bytes);
            }
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

            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                var bytes = AsmLoader.GetAsmBytes("DisableNavimesh64");
                AsmHelper.WriteAbsoluteAddresses64(bytes, new[]
                {
                    (eventPointMan.ToInt64(), 2),
                    (areaId, 0xA + 2),
                    (naviData.EventId, 0x14 + 2),
                    (getNaviLoc, 0x25 + 2),
                    (naviData.State, 0x34 + 2),
                    (disableNavi, 0x3E + 2)
                });

                _memoryIo.AllocateAndExecute(bytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes("DisableNavimesh32");
                AsmHelper.WriteAbsoluteAddresses32(bytes, new[]
                {
                    (eventPointMan.ToInt64(), 1),
                    (naviData.EventId, 0x5 + 1),
                    (areaId, 0xA + 1),
                    (getNaviLoc, 0xF + 1),
                    (naviData.State, 0x18 + 1),
                    (disableNavi, 0x1D + 1)
                });
                _memoryIo.AllocateAndExecute(bytes);
            }
        }

        public void DisableWhiteDoor(long areaId, GameIds.WhiteDoor.DisableWhiteDoor whiteDoorData)
        {
            var getMapEntity = Funcs.GetMapEntityWithAreaIdAndObjId;
            var getComponent = Funcs.GetWhiteDoorComponent;

            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                var bytes = AsmLoader.GetAsmBytes("DisableWhiteDoorKeyGuide64");
                AsmHelper.WriteAbsoluteAddresses64(bytes, new[]
                {
                    (areaId, 0x4 + 2),
                    (whiteDoorData.ObjId, 0xE + 2),
                    (getMapEntity, 0x18 + 2),
                    (getComponent, 0x27 + 2)
                });

                _memoryIo.AllocateAndExecute(bytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes("DisableWhiteDoorKeyGuide32");
                AsmHelper.WriteAbsoluteAddresses32(bytes, new[]
                {
                    (whiteDoorData.ObjId, 1),
                    (areaId, 0x5 + 1),
                    (getMapEntity, 0xA + 1),
                    (getComponent, 0x16 + 1)
                });
                _memoryIo.AllocateAndExecute(bytes);
            }
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

            var count = _memoryIo.ReadUInt8(spellBase +
                                            GameManagerImp.GameDataManagerOffsets.Inventory.ItemInvetory2SpellList
                                                .Count);
            if (count == 0) return new List<InventorySpell>();

            return GameVersion.Current.Edition == GameEdition.Scholar
                ? ReadSpellList64Bit(spellBase, count)
                : ReadSpellList32Bit(spellBase, count);
        }

        private List<InventorySpell> ReadSpellList64Bit(IntPtr spellBase, int count)
        {
            List<InventorySpell> currentSpells = new List<InventorySpell>();
            var current = (IntPtr)_memoryIo.ReadInt64(spellBase +
                                                      GameManagerImp.GameDataManagerOffsets.Inventory
                                                          .ItemInvetory2SpellList.ListStart);

            for (int i = 0; i < count && current != IntPtr.Zero; i++)
            {
                var spellId =
                    _memoryIo.ReadInt32(current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.SpellId);
                var isEquipped =
                    _memoryIo.ReadUInt8(current +
                                        GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.IsEquipped);
                var slotReq =
                    _memoryIo.ReadUInt8(current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.SlotReq);
                currentSpells.Add(new InventorySpell(spellId, isEquipped == 2, current, slotReq));
                current = (IntPtr)_memoryIo.ReadInt64(current +
                                                      GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry
                                                          .NextPtr);
            }

            return currentSpells;
        }

        private List<InventorySpell> ReadSpellList32Bit(IntPtr spellBase, int count)
        {
            List<InventorySpell> currentSpells = new List<InventorySpell>();
            var current = (IntPtr)_memoryIo.ReadInt32(spellBase +
                                                      GameManagerImp.GameDataManagerOffsets.Inventory
                                                          .ItemInvetory2SpellList.ListStart);

            for (int i = 0; i < count && current != IntPtr.Zero; i++)
            {
                var spellId =
                    _memoryIo.ReadInt32(current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.SpellId);
                var isEquipped =
                    _memoryIo.ReadUInt8(current +
                                        GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.IsEquipped);
                var slotReq =
                    _memoryIo.ReadUInt8(current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.SlotReq);
                currentSpells.Add(new InventorySpell(spellId, isEquipped == 2, current, slotReq));
                current = (IntPtr)_memoryIo.ReadInt32(current +
                                                      GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry
                                                          .NextPtr);
            }

            return currentSpells;
        }

        public List<EquippedSpell> GetEquippedSpells()
        {
            var currentSpell = GetCurrentSpellPtr();
            List<EquippedSpell> currentSpells = new List<EquippedSpell>();

            int chunkSize = GameVersion.Current.Edition == GameEdition.Scholar ? 0x10 : 0x8;

            for (int i = 0; i < 14; i++)
            {
                currentSpells.Add(new EquippedSpell(_memoryIo.ReadInt32(currentSpell), i));
                currentSpell += chunkSize;
            }

            return currentSpells;
        }

        private IntPtr GetCurrentSpellPtr()
        {
            return _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.PlayerCtrl,
                GameManagerImp.ChrCtrlOffsets.ChrAsmCtrl,
                GameManagerImp.ChrCtrlOffsets.EquippedSpellsStart
            }, false);
        }

        public int GetTotalAvailableSlots()
        {
            RefreshSpellSlots();

            var inventory = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr
            }, true);
            var getNumOfSlots1 = Funcs.GetNumOfSpellslots1;
            var getNumOfSlots2 = Funcs.GetNumOfSpellslots2;
            var slotsLoc = CodeCaveOffsets.Base + CodeCaveOffsets.NumOfSpellSlots;


            byte[] bytes;

            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                bytes = AsmLoader.GetAsmBytes("GetNumOfSlots64");

                AsmHelper.WriteAbsoluteAddresses64(bytes, new[]
                {
                    (slotsLoc.ToInt64(), 2),
                    (inventory.ToInt64(), 0xA + 2),
                    (getNumOfSlots1, 0x17 + 2),
                    (getNumOfSlots2, 0x2C + 2)
                });
            }
            else
            {
                bytes = AsmLoader.GetAsmBytes("GetNumOfSlots32");
                AsmHelper.WriteAbsoluteAddresses32(bytes, new []
                {
                    (slotsLoc.ToInt64(), 1),
                    (inventory.ToInt64(), 0x5 + 1),
                    (getNumOfSlots1, 0xC + 1),
                    (getNumOfSlots2, 0x17 + 1)
                });
            }
            
            _memoryIo.AllocateAndExecute(bytes);


            return _memoryIo.ReadInt32(slotsLoc);
        }

        private void RefreshSpellSlots()
        {
            var bagList = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr,
                GameManagerImp.GameDataManagerOffsets.Inventory.InventoryLists,
                GameManagerImp.GameDataManagerOffsets.Inventory.ItemInventory2BagListPtr,
            }, true);

            var refreshFunc = Funcs.UpdateSpellSlots;
            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                var bytes = AsmLoader.GetAsmBytes("UpdateSpellSlots64");

                AsmHelper.WriteAbsoluteAddresses64(bytes, new[]
                {
                    (bagList.ToInt64(), 2),
                    (refreshFunc, 0xA + 2)
                });
                _memoryIo.AllocateAndExecute(bytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes("UpdateSpellSlots32");
                AsmHelper.WriteAbsoluteAddresses32(bytes, new[]
                {
                    (bagList.ToInt64(), 1),
                    (refreshFunc, 0x5 + 1)
                });
                _memoryIo.AllocateAndExecute(bytes);
            }
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
            
            
            byte[] bytes;

            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                bytes = AsmLoader.GetAsmBytes("AttuneSpell64");
                AsmHelper.WriteAbsoluteAddresses64(bytes, new[]
                {
                    (inventoryLists.ToInt64(), 2),
                    (slotIndex + 0x1C, 0xA + 2),
                    (entryAddr.ToInt64(), 0x14 + 2),
                    (attuneFunc, 0x1E + 2)
                });
            }
            else
            {
                bytes = AsmLoader.GetAsmBytes("AttuneSpell32");
                AsmHelper.WriteAbsoluteAddresses32(bytes, new []
                {
                    (inventoryLists.ToInt64(), 1),
                    (entryAddr.ToInt64(), 0x5 + 1),
                    (slotIndex + 0x1C, 0xB + 1),
                    (attuneFunc, 0x11 + 1)
                });
            }
            
            _memoryIo.AllocateAndExecute(bytes);
        }
    }
}