using System.Threading.Tasks;
using SilkySouls2.enums;
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
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.EzStateEventCode;
            var hookLoc = Hooks.GameManUpdate;
            var paramsLocation = CodeCaveOffsets.Base + CodeCaveOffsets.EzStateEventParams;
            
            for (int i = 0; i < command.Params.Length; i++)
            {
                memoryService.Write(paramsLocation + i * 4, command.Params[i]);
            }

            var shouldExecuteFlag = CodeCaveOffsets.Base + CodeCaveOffsets.ShouldExecuteFlag;

            var bytes = AsmLoader.GetAsmBytes(AsmScript.EzStateEventExecuteGameThread64);
           
            AsmHelper.WriteRelativeOffsets(bytes, [
                (code.ToInt64(), shouldExecuteFlag.ToInt64(), 7, 0x0 + 2),
                (code.ToInt64() + 0xD, shouldExecuteFlag.ToInt64(), 7, 0xD + 2),
                (code.ToInt64() + 0x62, Functions.EzStateExternalEventCtor, 5, 0x62 + 1),
                (code.ToInt64() + 0xA4, paramsLocation.ToInt64(), 7, 0xA4 + 3),
                (code.ToInt64() + 0xE7, Functions.EzStateEventExecuteCommand, 5, 0xE7 + 1),
                (code.ToInt64() + 0x135, hookLoc + 11, 5, 0x135 + 1)
            ]);
            
            AsmHelper.WriteImmediateDwords(bytes, [
                (command.CommandId, 0x5D + 1),
                (areaId, 0x7C + 4),
                (areaIndex, 0x84 + 4),
                (command.Params.Length, 0x97 + 1)
            ]);
            
            memoryService.Write(shouldExecuteFlag, (byte)1);
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code.ToInt64(), hookLoc,
                [0x40, 0x53, 0x55, 0x56, 0x48, 0x81, 0xEC, 0x00, 0x04, 0x00, 0x00]);
            Task.Delay(100).Wait();
            hookManager.UninstallHook(code.ToInt64());
        }

        public void ExecuteEvent(EventCommand command, int areaId = 0, int areaIndex = 0)
        {
            if (PatchManager.IsScholar())
            {
                ExecuteEvent64(command, areaId, areaIndex);
            }
            
        }
        
        private void ExecuteEvent64(EventCommand command, int areaId, int areaIndex)
        {
            
            var paramsLocation = CodeCaveOffsets.Base + CodeCaveOffsets.EzStateEventParams;
            
            for (int i = 0; i < command.Params.Length; i++)
            {
                memoryService.Write(paramsLocation + i * 4, command.Params[i]);
            }
            
            var bytes = AsmLoader.GetAsmBytes(AsmScript.EzStateExecuteEvent64);
            
            AsmHelper.WriteAbsoluteAddresses64(bytes, [
            (Functions.EzStateExternalEventCtor, 0x16 + 2),
            (paramsLocation.ToInt64(), 0x66 + 2),
            (Functions.EzStateEventExecuteCommand, 0xAF + 2)
            ]);
            
            AsmHelper.WriteImmediateDwords(bytes, [
                (command.CommandId, 0x11 + 1),
                (areaId, 0x37 + 4),
                (areaIndex, 0x3F + 4),
                (command.Params.Length, 0x59 + 1)
            ]);
            
            memoryService.AllocateAndExecute(bytes);
        }
    }
}