// 

using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services;

public class ReminderService : IReminderService
{
    private readonly IMemoryService _memoryService;
    private readonly HookManager _hookManager;

    private const string ReminderText = "SS2 Active";

    private bool _hasDoneReminder;

    public ReminderService(IMemoryService memoryService, HookManager hookManager, IStateService stateService)
    {
        _memoryService = memoryService;
        _hookManager = hookManager;
        stateService.Subscribe(State.Attached, OnAttached);
    }

    private void OnAttached() => _hasDoneReminder = false;

    public void TrySetReminder()
    {
        if (_hasDoneReminder) return;

        if (PatchManager.IsScholar()) SetReminderScholar();
        else SetReminderVanilla();
    }

    private void SetReminderScholar()
    {
        var textLoc = CustomCodeOffsets.Base + CustomCodeOffsets.ReminderText;
        _memoryService.WriteWString(textLoc, ReminderText);

        var code = CustomCodeOffsets.Base + CustomCodeOffsets.SetReminderCode;
        var bytes = AsmLoader.GetAsmBytes(AsmScript.TrySetReminder64);
        AsmHelper.WriteRelativeOffsets(bytes, [
            (code + 0x18, textLoc, 7, 0x18 + 3),
            (code + 0x20, Hooks.LoadingItemName + 6, 5, 0x20 + 1)
        ]);
        AsmHelper.WriteAbsoluteAddress64(bytes, BuildTextFieldRetAddr, 0x7 + 2);

        _memoryService.WriteBytes(code, bytes);
        _hookManager.InstallHook(code, Hooks.LoadingItemName, [0x48, 0x8B, 0x0B, 0x48, 0x8B, 0x11]);
    }

    private void SetReminderVanilla()
    {
        var textLoc = CustomCodeOffsets.Base + CustomCodeOffsets.ReminderText;
        _memoryService.WriteWString(textLoc, ReminderText);

        var code = CustomCodeOffsets.Base + CustomCodeOffsets.SetReminderCode;
        var bytes = AsmLoader.GetAsmBytes(AsmScript.TrySetReminder32);

        AsmHelper.WriteAbsoluteAddresses32(bytes, [
            (BuildTextFieldRetAddr, 0x7 + 1),
            (textLoc, 0x12 + 1)
        ]);

        AsmHelper.WriteRelativeOffset(bytes, code + 0x18, Hooks.LoadingItemName + 6, 5, 0x18 + 1);
        _memoryService.WriteBytes(code, bytes);
        _hookManager.InstallHook(code, Hooks.LoadingItemName, [0x8B, 0x0E, 0x89, 0xC7, 0x8B, 0x01]);
    }
}