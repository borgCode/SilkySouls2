// 

using System;
using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services;

public class NewGameService : INewGameService
{
    private int _clientCount;
    private readonly IMemoryService _memoryService;
    private readonly HookManager _hookManager;

    public NewGameService(IMemoryService memoryService, HookManager hookManager, GameStateService gameStateService)
    {
        _memoryService = memoryService;
        _hookManager = hookManager;
    }
    public void RequestNewGameDetect()
    {
        if (_clientCount == 0) ToggleNewGameDetect(true);
        _clientCount++;
    }

    public void ReleaseNewGameDetect()
    {
        _clientCount--;
        
        if (_clientCount == 0) ToggleNewGameDetect(false);
        
        if (_clientCount < 0) _clientCount = 0;
    }

    public int GetCount() => _clientCount;
    
    public void Reset()
    {
        _clientCount = 0;
        ToggleNewGameDetect(false);
    }
    
        
    private void ToggleNewGameDetect(bool enabled)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.NewGameDetect;

        if (!enabled)
        {
            _hookManager.UninstallHook(code.ToInt64());
            return;
        }

        var hookLoc = Hooks.NewGameDetect;
        var flag = CodeCaveOffsets.Base + CodeCaveOffsets.NewGameStartedFlag;
        if (PatchManager.IsScholar())
        {
            InstallScholarNewGameDetect(code, hookLoc, flag);
        }
        else
        {
            InstallVanillaNewGameDetect(code, hookLoc, flag);
        }
    }

    private void InstallScholarNewGameDetect(IntPtr code, long hookLoc, IntPtr flag)
    {
        var bytes = AsmLoader.GetAsmBytes(AsmScript.NewGameDetect64);

        int originalInstructionLength = 0x7;
        
        AsmHelper.WriteRelativeOffsets(bytes, [
        (code.ToInt64() + 0xE, hookLoc + originalInstructionLength, 6, 0xE + 2),
        (code.ToInt64() + 0x15, GameManagerImp.Base.ToInt64(), 7, 0x15 + 3),
        (code.ToInt64() + 0x30, flag.ToInt64(), 0x7, 0x30 + 2),
        (code.ToInt64() + 0x38, hookLoc + originalInstructionLength, 5, 0x38 + 1)
        ]);

        _memoryService.WriteBytes(code, bytes);
        _hookManager.InstallHook(code.ToInt64(), hookLoc, [0xC7, 0x47, 0x54, 0xFF, 0xFF, 0xFF, 0xFF]);
    }

    private void InstallVanillaNewGameDetect(IntPtr code, long hookLoc, IntPtr flag)
    {
        var bytes = AsmLoader.GetAsmBytes(AsmScript.NewGameDetect32);
        
        int originalInstructionLength = 0x7;
        
        AsmHelper.WriteAbsoluteAddresses32(bytes, new[]
        {
            (GameManagerImp.Base.ToInt64(), 0x15 + 1),
            (flag.ToInt64(), 0x26 + 2),
        });

        AsmHelper.WriteRelativeOffsets(bytes, new[]
        {
            (code.ToInt64() + 0xE, hookLoc + originalInstructionLength, 6, 0xE + 2),
            (code.ToInt64() + 0x2E, hookLoc + originalInstructionLength, 5, 0x2E + 1),
        });
        
        _memoryService.WriteBytes(code, bytes);
        _hookManager.InstallHook(code.ToInt64(), hookLoc, [ 0xC7, 0x46, 0x2C, 0xFF, 0xFF, 0xFF, 0xFF]);
    }
}