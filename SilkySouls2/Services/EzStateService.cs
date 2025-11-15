using System.Threading.Tasks;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;

namespace SilkySouls2.Services
{
    public class EzStateService
    {
        private readonly MemoryIo _memoryIo;
        private readonly HookManager _hookManager;

        public EzStateService(MemoryIo memoryIo, HookManager hookManager)
        {
            _memoryIo = memoryIo;
            _hookManager = hookManager;
        }


        public void ExecuteEzStateEventCommand(GameIds.EzStateEventCalls.EzStateEventCommand command, int areaId = 0, int areaIndex = 0)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.EzStateEventCode;
            var hookLoc = Offsets.Hooks.GameManUpdate;
            var paramsLocation = CodeCaveOffsets.Base + CodeCaveOffsets.EzStateEventParams;
            
            for (int i = 0; i < command.Params.Length; i++)
            {
                _memoryIo.WriteInt32(paramsLocation + i * 4, command.Params[i]);
            }

            var shouldExecuteFlag = CodeCaveOffsets.Base + CodeCaveOffsets.ShouldExecuteFlag;

            var bytes = AsmLoader.GetAsmBytes("EzStateEventExecuteCommand64");
           
            AsmHelper.WriteRelativeOffsets(bytes, new []
            {
                (code.ToInt64(), shouldExecuteFlag.ToInt64(), 7, 0x0 + 2),
                (code.ToInt64() + 0xD, shouldExecuteFlag.ToInt64(), 7, 0xD + 2),
                (code.ToInt64() + 0x62, Offsets.Funcs.EzStateExternalEventCtor, 5, 0x62 + 1),
                (code.ToInt64() + 0xA4, paramsLocation.ToInt64(), 7, 0xA4 + 3),
                (code.ToInt64() + 0xE7, Offsets.Funcs.EzStateEventExecuteCommand, 5, 0xE7 + 1),
                (code.ToInt64() + 0x135, hookLoc + 11, 5, 0x135 + 1)
            });
            
            AsmHelper.WriteImmediateDwords(bytes, new []
            {
                (command.CommandId, 0x5D + 1),
                (areaId, 0x7C + 4),
                (areaIndex, 0x84 + 4),
                (command.Params.Length, 0x97 + 1)
            });
            
            _memoryIo.WriteByte(shouldExecuteFlag, 1);
            _memoryIo.WriteBytes(code, bytes);
            _hookManager.InstallHook(code.ToInt64(), hookLoc,
                new byte[] { 0x40, 0x53, 0x55, 0x56, 0x48, 0x81, 0xEC, 0x00, 0x04, 0x00, 0x00 });
            Task.Delay(100).Wait();
            _hookManager.UninstallHook(code.ToInt64());
        }
    }
}