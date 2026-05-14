// 

using System;
using System.Diagnostics.CodeAnalysis;
using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services;

public class MenuService(IMemoryService memoryService, IGameTickService gameTickService) : IMenuService
{
    private bool _chrStateHeld;
    private bool _reopenPending;
    private Action _pendingOpen;

    public void OpenMenu(NpcMenuType menuType, int startRowId = 0, int endRowId = 0)
    {
        var wasOpen = IsMenuOpen();
        if (wasOpen)
        {
            _reopenPending = true;
            CloseCurrentMenu();
        }

        if (!_chrStateHeld)
        {
            SetMenuOpenChrState(true);
            gameTickService.Subscribe(PollHasMenuClosed);
            _chrStateHeld = true;
        }

        if (wasOpen)
        {
            if (_pendingOpen != null)
            {
                gameTickService.Unsubscribe(_pendingOpen);
                _pendingOpen = null;
            }

            Action openNext = null;
            openNext = () =>
            {
                gameTickService.Unsubscribe(openNext);
                _pendingOpen = null;
                ExecuteOpenMenu(menuType, startRowId, endRowId);
                _reopenPending = false;
            };
            _pendingOpen = openNext;
            gameTickService.Subscribe(openNext);
        }
        else
        {
            ExecuteOpenMenu(menuType, startRowId, endRowId);
        }
    }

    private bool IsMenuOpen()
    {
        var fixedJob = memoryService.FollowPointers(GameManagerImp.Base,
            GameManagerImp.NpcMenuFixOrderJobSequence, false);
        return memoryService.ReadPointer(fixedJob) != 0;
    }

    private void CloseCurrentMenu()
    {
        var openFlag = memoryService.FollowPointers(GameManagerImp.Base, GameManagerImp.FeItemSelectMenuOpen, false);
        memoryService.Write(openFlag, false);

        var dlBackAllocator =
            memoryService.ReadPointer(memoryService.ReadPointer(GameManagerImp.Base) + GameManagerImp.DLBackAllocator);
        var field30f = memoryService.Read<byte>(dlBackAllocator + DLBackAllocator.UnkFlag);
        var field31c = memoryService.Read<int>(dlBackAllocator + DLBackAllocator.RefCount);
        if (field30f == 0 && field31c != 0)
        {
            memoryService.Write(dlBackAllocator + DLBackAllocator.RefCount, field31c - 1);
        }
    }

    private void SetMenuOpenChrState(bool isOpen)
    {
        if (PatchManager.IsScholar()) SetMenuOpenChrStateScholar(isOpen);
        else SetMenuOpenChrStateVanilla(isOpen);
        
    }

    private void SetMenuOpenChrStateScholar(bool isOpen)
    {
        var bytes = AsmLoader.GetAsmBytes(AsmScript.SetMenuOpenChrState64);
        var dlBackAllocator =
            memoryService.Read<nint>(memoryService.Read<nint>(GameManagerImp.Base) + GameManagerImp.DLBackAllocator);
        AsmHelper.WriteAbsoluteAddresses64(bytes, [
            (dlBackAllocator, 0x4 + 2),
            (Functions.SetMenuOpenChrState, 0x19 + 2)
        ]);

        AsmHelper.WriteImmediateDword(bytes, isOpen ? 1 : 0, 0x13 + 2);
        memoryService.AllocateAndExecute(bytes);
    }

    private void SetMenuOpenChrStateVanilla(bool isOpen)
    {
        var bytes = AsmLoader.GetAsmBytes(AsmScript.SetMenuOpenChrState32);
        
        var dlBackAllocator =
            memoryService.ReadPointer(memoryService.ReadPointer(GameManagerImp.Base) + GameManagerImp.DLBackAllocator);
        
        bytes[1] = isOpen ? (byte)1 : (byte)0;
        
        AsmHelper.WriteAbsoluteAddresses32(bytes, [
        (dlBackAllocator, 0x4 + 1),
        (Functions.SetMenuOpenChrState, 0x9 + 1)
        ]);
        memoryService.AllocateAndExecute(bytes);
    }

    private void ExecuteOpenMenu(NpcMenuType menuType, int startRowId, int endRowId)
    {
        if (PatchManager.IsScholar()) OpenMenuScholar(menuType, startRowId, endRowId);
        else OpenMenuVanilla(menuType, startRowId, endRowId);
    }

    
    private void OpenMenuScholar(NpcMenuType menuType, int startRowId, int endRowId)
    {
        var bytes = AsmLoader.GetAsmBytes(AsmScript.OpenMenu64);

        var code = CustomCodeOffsets.Base + CustomCodeOffsets.OpenShopCode;
        var availableMenus = CustomCodeOffsets.Base + CustomCodeOffsets.AvailableMenus;
        var npcPos = CustomCodeOffsets.Base + CustomCodeOffsets.NpcPos;

        var eventWindowManager = memoryService.FollowPointers(GameManagerImp.Base,
        [
            GameManagerImp.EventManager,
            GameManagerImp.EventManagerOffsets.EventWindowManager
        ], true);

        WriteMenuParams(menuType, startRowId, endRowId, is64: true);

        AsmHelper.WriteAbsoluteAddress64(bytes, eventWindowManager, 0x4 + 2);
        AsmHelper.WriteRelativeOffsets(bytes, [
            (code + 0xE, availableMenus, 7, 0xE + 3),
            (code + 0x15, npcPos, 7, 0x15 + 3),
            (code + 0x1C, Functions.OpenNpcMenu, 5, 0x1C + 1),
        ]);

        memoryService.WriteBytes(code, bytes);
        memoryService.RunThread(code);
    }

    private void OpenMenuVanilla(NpcMenuType menuType, int startRowId, int endRowId)
    {
        var bytes = AsmLoader.GetAsmBytes(AsmScript.OpenMenu32);

        var code = CustomCodeOffsets.Base + CustomCodeOffsets.OpenShopCode;
        var availableMenus = CustomCodeOffsets.Base + CustomCodeOffsets.AvailableMenus;
        var npcPos = CustomCodeOffsets.Base + CustomCodeOffsets.NpcPos;

        var eventWindowManager = memoryService.FollowPointers(GameManagerImp.Base,
        [
            GameManagerImp.EventManager,
            GameManagerImp.EventManagerOffsets.EventWindowManager
        ], true);

        WriteMenuParams(menuType, startRowId, endRowId, is64: false);

        AsmHelper.WriteAbsoluteAddresses32(bytes, [
            (npcPos, 1),
            (availableMenus, 0x5 + 1),
            (eventWindowManager, 0xA + 1)
        ]);

        AsmHelper.WriteRelativeOffset(bytes, code + 0xF, Functions.OpenNpcMenu, 5, 0xF + 1);
        memoryService.WriteBytes(code, bytes);
        memoryService.RunThread(code);
    }

    private void PollHasMenuClosed()
    {
        if (IsMenuOpen()) return;
        if (_reopenPending) return;
        CleanUp();
        gameTickService.Unsubscribe(PollHasMenuClosed);
        _chrStateHeld = false;
    }

    public void Reset()
    {
        gameTickService.Unsubscribe(PollHasMenuClosed);
        if (_pendingOpen != null)
        {
            gameTickService.Unsubscribe(_pendingOpen);
            _pendingOpen = null;
        }

        _chrStateHeld = false;
        _reopenPending = false;
    }
    
    private void WriteMenuParams(NpcMenuType menuType, int startRowId, int endRowId, bool is64)
    {
        var npcTalkParam = CustomCodeOffsets.Base + CustomCodeOffsets.NpcTalkParam;
        var availableMenus = CustomCodeOffsets.Base + CustomCodeOffsets.AvailableMenus;

        var (startOffset, endOffset) = menuType == NpcMenuType.Trade
            ? (0x14, 0x2C)
            : (0x4, 0x8);

        memoryService.Write(npcTalkParam + startOffset, startRowId);
        memoryService.Write(npcTalkParam + endOffset, endRowId);

        if (is64)
        {
            memoryService.Write(availableMenus, npcTalkParam);
            memoryService.Write(availableMenus + 0x8, (byte)menuType);
            memoryService.Write(availableMenus + 0x28, (long)1);
        }
        else
        {
            memoryService.Write(availableMenus, (int)npcTalkParam);
            memoryService.Write(availableMenus + 0x4, (byte)menuType);
            memoryService.Write(availableMenus + 0x20, 1);
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private void CleanUp()
    {
        SetMenuOpenChrState(false);
        var dlBackAllocator =
            memoryService.ReadPointer(memoryService.ReadPointer(GameManagerImp.Base) + GameManagerImp.DLBackAllocator);

        var field30f = memoryService.Read<byte>(dlBackAllocator + DLBackAllocator.UnkFlag);
        var field31c = memoryService.Read<int>(dlBackAllocator + DLBackAllocator.RefCount);

        if (field30f == 0 && field31c != 0)
        {
            memoryService.Write(dlBackAllocator + DLBackAllocator.RefCount, field31c - 1);
        }
    }
}