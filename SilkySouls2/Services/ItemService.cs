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

            var inventoryLists = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr,
                GameManagerImp.GameDataManagerOffsets.Inventory.InventoryLists
            }, true);
            var itemIventory2BagList =
                _memoryIo.ReadInt64(inventoryLists +
                                    GameManagerImp.GameDataManagerOffsets.Inventory.ItemInventory2BagListPtr);
            var dlBackAllocator =
                _memoryIo.ReadInt64((IntPtr)_memoryIo.ReadInt64(GameManagerImp.Base) + GameManagerImp.Offsets.DLBackAllocator);
            
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
                    (itemIventory2BagList, 0x1D + 2),
                    (getCurrentQuantity, 0x3C + 2),
                    (inventoryLists.ToInt64(), 0x80 + 2),
                    (itemGive, 0x9B + 2),
                    (buildItemDialog, 0xC3 + 2),
                    (dlBackAllocator, 0xD0 + 2),
                    (showItemPopup, 0xE1 + 2),
                    (sleepAddr.ToInt64(), 0xF5 + 2)
                });
            
                AsmHelper.WriteRelativeOffsets(bytes, new []
                {
                    (code.ToInt64(), shouldProcessFlag.ToInt64(), 7, 0x0 + 2),
                    (code.ToInt64() + 0xD, shouldProcessFlag.ToInt64(), 7, 0xD + 2),
                    (code.ToInt64() + 0x14, adjustQuantityFlag.ToInt64(), 7, 0x14 + 2),
                    (code.ToInt64() + 0x27, currentQuantity.ToInt64(), 7, 0x27 + 3),
                    (code.ToInt64() + 0x2E, stackCount.ToInt64(), 7, 0x2E + 3),
                    (code.ToInt64() + 0x35, itemStruct.ToInt64() + 0x4, 7, 0x35 + 3),
                    (code.ToInt64() + 0x51, itemStruct.ToInt64() + 0xC, 7, 0x51 + 3),
                    (code.ToInt64() + 0x58, currentQuantity.ToInt64(), 6, 0x58 + 2),
                    (code.ToInt64() + 0x5E, maxQuantity.ToInt64(), 6, 0x5E + 2),
                    (code.ToInt64() + 0x66, maxQuantity.ToInt64(), 6, 0x66 + 2),
                    (code.ToInt64() + 0x6C, currentQuantity.ToInt64(), 6, 0x6C + 2),
                    (code.ToInt64() + 0x72, itemStruct.ToInt64() + 0xC, 7, 0x72 + 3),
                    (code.ToInt64() + 0x8A, itemStruct.ToInt64(), 7, 0x8A + 3),
                    (code.ToInt64() + 0x91, itemCount.ToInt64(), 7, 0x91 + 3),
                    (code.ToInt64() + 0xA8, stackSpace.ToInt64(), 7, 0xA8 + 3),
                    (code.ToInt64() + 0xAF, itemStruct.ToInt64(), 7, 0xAF + 3),
                    (code.ToInt64() + 0xB6, itemCount.ToInt64(), 7, 0xB6 + 3),
                    (code.ToInt64() + 0xDA, stackSpace.ToInt64(), 7, 0xDA + 3),
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
    }
}