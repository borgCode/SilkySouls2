using System.Collections.Generic;
using System.Threading.Tasks;
using SilkySouls2.enums;
using SilkySouls2.GameIds;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;
using static SilkySouls2.GameIds.EzState;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class EzStateService(IMemoryService memoryService, HookManager hookManager) : IEzStateService
    {
        public void ExecuteEventFromGameThread(EventCommand command, int areaId = 0, int areaIndex = 0)
        {
            var code = CustomCodeOffsets.Base + CustomCodeOffsets.EzStateEventCode;
            var hookLoc = Hooks.GameManUpdate;
            var paramsLocation = CustomCodeOffsets.Base + CustomCodeOffsets.EzStateEventParams;

            for (int i = 0; i < command.Params.Length; i++)
            {
                memoryService.Write(paramsLocation + i * 4, command.Params[i]);
            }

            var shouldExecuteFlag = CustomCodeOffsets.Base + CustomCodeOffsets.ShouldExecuteFlag;

            var bytes = AsmLoader.GetAsmBytes(AsmScript.EzStateEventExecuteGameThread64);

            AsmHelper.WriteRelativeOffsets(bytes, [
                (code, shouldExecuteFlag, 7, 0x0 + 2),
                (code + 0xD, shouldExecuteFlag, 7, 0xD + 2),
                (code + 0x62, Functions.EzStateExternalEventCtor, 5, 0x62 + 1),
                (code + 0xA4, paramsLocation, 7, 0xA4 + 3),
                (code + 0xE7, Functions.EzStateEventExecuteCommand, 5, 0xE7 + 1),
                (code + 0x135, hookLoc + 11, 5, 0x135 + 1)
            ]);

            AsmHelper.WriteImmediateDwords(bytes, [
                (command.CommandId, 0x5D + 1),
                (areaId, 0x7C + 4),
                (areaIndex, 0x84 + 4),
                (command.Params.Length, 0x97 + 1)
            ]);

            memoryService.Write(shouldExecuteFlag, (byte)1);
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code, hookLoc,
                [0x40, 0x53, 0x55, 0x56, 0x48, 0x81, 0xEC, 0x00, 0x04, 0x00, 0x00]);
            Task.Delay(100).Wait();
            hookManager.UninstallHook(code);
        }

        public void ExecuteEvent(EventCommand command, int areaId = 0, int areaIndex = 0)
        {
            var paramsLocation = CustomCodeOffsets.Base + CustomCodeOffsets.EzStateEventParams;

            for (int i = 0; i < command.Params.Length; i++)
            {
                memoryService.Write(paramsLocation + i * 4, command.Params[i]);
            }

            var bytes = PatchManager.IsScholar()
                ? WriteScholarExecuteEvent(paramsLocation, command, areaId, areaIndex)
                : WriteVanillaExecuteEvent(paramsLocation, command, areaId, areaIndex);

            memoryService.AllocateAndExecute(bytes);
        }

        public void RunScript(IEnumerable<ScriptStep> steps)
        {
            foreach (var step in steps)
                ExecuteEvent(step.Command, step.Area);
        }

        private byte[] WriteScholarExecuteEvent(nint paramsLocation, EventCommand command, int areaId, int areaIndex)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.EzStateExecuteEvent64);

            AsmHelper.WriteAbsoluteAddresses64(bytes, [
                (Functions.EzStateExternalEventCtor, 0x16 + 2),
                (paramsLocation, 0x82 + 2),
                (Functions.EzStateEventExecuteCommand, 0xCB + 2)
            ]);

            AsmHelper.WriteImmediateDwords(bytes, [
                (command.CommandId, 0x11 + 1),
                (areaId, 0x37 + 4),
                (areaIndex, 0x3F + 4),
                (command.Params.Length, 0x75 + 1)
            ]);

            return bytes;
        }

        private byte[] WriteVanillaExecuteEvent(nint paramsLocation, EventCommand command, int areaId, int areaIndex)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.EzStateExecuteEvent32);

            AsmHelper.WriteAbsoluteAddresses32(bytes, [
                (Functions.EzStateExternalEventCtor, 0x13 + 1),
                (paramsLocation, 0x66 + 1),
                (Functions.EzStateEventExecuteCommand, 0x98 + 1)
            ]);

            AsmHelper.WriteImmediateDwords(bytes, [
                (command.CommandId, 0xE + 1),
                (areaId, 0x32 + 3),
                (areaIndex, 0x39 + 3),
                (command.Params.Length, 0x5C + 1)
            ]);

            return bytes;
        }
    }
}