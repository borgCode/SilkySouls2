using System;
using SilkySouls2.Memory;
using SilkySouls2.Models;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class ItemService
    {
        
        private readonly MemoryIo _memoryIo;
        private bool _codeIswritten;
        public ItemService(MemoryIo memoryIo)
        {
            _memoryIo = memoryIo;
        }

        public void SpawnItem(Item selectedItem,  int selectedUpgrade, int selectedQuantity, int selectedInfusion, float durability = 0.0f)
        {

            var shouldExitFlag = CodeCaveOffsets.Base + (int)CodeCaveOffsets.ItemSpawn.ShouldExitFlag;
            var shouldProcessFlag = CodeCaveOffsets.Base + (int)CodeCaveOffsets.ItemSpawn.ShouldProcessFlag;
            var adjustQuantityFlag = CodeCaveOffsets.Base + (int)CodeCaveOffsets.ItemSpawn.AdjustQuantityFlag;
            var maxQuantity = CodeCaveOffsets.Base + (int)CodeCaveOffsets.ItemSpawn.MaxQuantity;
            var itemCount = CodeCaveOffsets.Base + (int)CodeCaveOffsets.ItemSpawn.ItemCount;
            var currentQuantity = CodeCaveOffsets.Base + (int)CodeCaveOffsets.ItemSpawn.CurrentQuantity;
            var stackCount = CodeCaveOffsets.Base + (int)CodeCaveOffsets.ItemSpawn.StackCount;
            var itemStruct = CodeCaveOffsets.Base + (int)CodeCaveOffsets.ItemSpawn.ItemStruct;
            var stackSpace = CodeCaveOffsets.Base + (int)CodeCaveOffsets.ItemSpawn.StackSpace;
            var code = CodeCaveOffsets.Base + (int)CodeCaveOffsets.ItemSpawn.Code;

            var getCurrentQuantity = Funcs.CurrentItemQuantityCheck;
            var itemGive = Funcs.ItemGive;
            var buildItemDialog = Funcs.BuildItemDialog;
            var showItemPopup = Funcs.ShowItemDialog;
            var sleepAddr = _memoryIo.GetProcAddress("kernel32.dll", "Sleep");
            
            
            _memoryIo.WriteInt32(currentQuantity, 0);
            _memoryIo.WriteInt32(stackCount, 0);
            
            _memoryIo.WriteInt32(maxQuantity, selectedItem.StackSize);
            _memoryIo.WriteInt32(itemStruct + 0x4, selectedItem.Id);
            _memoryIo.WriteFloat(itemStruct + 0x8, durability);
            _memoryIo.WriteInt16(itemStruct + 0xC, (short)selectedQuantity);
            _memoryIo.WriteByte(itemStruct + 0xE, (byte) selectedUpgrade);
            _memoryIo.WriteByte(itemStruct + 0xF, (byte) selectedInfusion);
            _memoryIo.WriteInt32(itemCount, 1);
            _memoryIo.WriteByte(adjustQuantityFlag, selectedItem.StackSize > 1 ? 1 : 0);
            _memoryIo.WriteByte(shouldProcessFlag, 1);

            if (!_codeIswritten)
            {
                var bytes = AsmLoader.GetAsmBytes("ItemSpawn");
            
                AsmHelper.WriteAbsoluteAddresses64(bytes, new []
                {
                    (GameManagerImp.Base.ToInt64(), 0xD + 2),
                    (getCurrentQuantity, 0x55 + 2),
                    (itemGive, 0xAD + 2),
                    (buildItemDialog, 0xD5 + 2),
                    (showItemPopup, 0xF0 + 2),
                    (sleepAddr.ToInt64(), 0x104 + 2)
                });
            
                AsmHelper.WriteRelativeOffsets(bytes, new []
                {
                    (code.ToInt64(), shouldProcessFlag.ToInt64(), 7, 0x0 + 2),
                    (code.ToInt64() + 0x2C, shouldProcessFlag.ToInt64(), 7, 0x2C + 2),
                    (code.ToInt64() + 0x33, adjustQuantityFlag.ToInt64(), 7, 0x33 + 2),
                    (code.ToInt64() + 0x40, currentQuantity.ToInt64(), 7, 0x40 + 3),
                    (code.ToInt64() + 0x47, stackCount.ToInt64(), 7, 0x47 + 3),
                    (code.ToInt64() + 0x4E, itemStruct.ToInt64() + 0x4, 7, 0x4E + 3),
                    (code.ToInt64() + 0x6A, itemStruct.ToInt64() + 0xC, 7, 0x6A + 3),
                    (code.ToInt64() + 0x71, currentQuantity.ToInt64(), 6, 0x71 + 2),
                    (code.ToInt64() + 0x77, maxQuantity.ToInt64(), 6, 0x77 + 2),
                    (code.ToInt64() + 0x7F, maxQuantity.ToInt64(), 6, 0x7F + 2),
                    (code.ToInt64() + 0x85, currentQuantity.ToInt64(), 6, 0x85 + 2),
                    (code.ToInt64() + 0x8B, itemStruct.ToInt64() + 0xC, 7, 0x8B + 3),
                    (code.ToInt64() + 0x9C, itemStruct.ToInt64(), 7, 0x9C + 3),
                    (code.ToInt64() + 0xA3, itemCount.ToInt64(), 7, 0xA3 + 3),
                    (code.ToInt64() + 0xBA, stackSpace.ToInt64(), 7, 0xBA + 3),
                    (code.ToInt64() + 0xC1, itemStruct.ToInt64(), 7, 0xC1 + 3),
                    (code.ToInt64() + 0xC8, itemCount.ToInt64(), 7, 0xC8 + 3),
                    (code.ToInt64() + 0xE9, stackSpace.ToInt64(), 7, 0xE9 + 3),
                    (code.ToInt64() + 0x117, shouldExitFlag.ToInt64(), 7, 0x117 + 2),
                });
                _memoryIo.WriteBytes(code, bytes);
                _codeIswritten = true;
                _memoryIo.RunPersistentThread(code);
            }
        }

        public void SetAutoSpawnWeapon(int wepId)
        {
            var startingWeapon = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.CharacterManager,
                GameManagerImp.CharacterManagerOffsets.PlayerStatusParamPtr,
                GameManagerImp.CharacterManagerOffsets.PlayerStatusParam,
                GameManagerImp.CharacterManagerOffsets.StartingWeapon,
            }, false);
            _memoryIo.WriteInt32(startingWeapon, wepId);
        }

        public void Reset()
        {
            _codeIswritten = false;
        }

        public void SignalClose()
        {
            _memoryIo.WriteByte(CodeCaveOffsets.Base + (int)CodeCaveOffsets.ItemSpawn.ShouldExitFlag, 1);
        }
    }
}