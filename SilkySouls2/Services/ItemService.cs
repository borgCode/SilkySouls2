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
        public ItemService(MemoryIo memoryIo)
        {
            _memoryIo = memoryIo;
        }

        public void SpawnItem(Item selectedItem,  int selectedUpgrade, int selectedQuantity, int selectedInfusion)
        {
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

            var inventoryLists = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr,
                GameManagerImp.GameDataManagerOffsets.Inventory.InventoryLists
            }, true);
            var itemIventory2BagList =
                _memoryIo.ReadInt64(inventoryLists +
                                    GameManagerImp.GameDataManagerOffsets.Inventory.ItemInventory2BagList);
            var dlBackAllocator =
                _memoryIo.ReadInt64((IntPtr)_memoryIo.ReadInt64(GameManagerImp.Base) + GameManagerImp.Offsets.DLBackAllocator);
            
            _memoryIo.WriteInt32(maxQuantity, selectedItem.StackSize);
            _memoryIo.WriteInt32(itemStruct + 0x4, selectedItem.Id);
            _memoryIo.WriteFloat(itemStruct + 0x8, -1f);
            _memoryIo.WriteInt16(itemStruct + 0xC, (short)selectedQuantity);
            _memoryIo.WriteByte(itemStruct + 0xE, (byte) selectedUpgrade);
            _memoryIo.WriteByte(itemStruct + 0xF, (byte) selectedInfusion);
            _memoryIo.WriteInt32(itemCount, 1);
            _memoryIo.WriteByte(adjustQuantityFlag, selectedItem.StackSize > 1 ? 1 : 0);

            var bytes = AsmLoader.GetAsmBytes("ItemSpawn");
            
            AsmHelper.WriteAbsoluteAddresses(bytes, new []
            {
                (itemIventory2BagList, 0x9 + 2),
                (getCurrentQuantity, 0x28 + 2),
                (inventoryLists.ToInt64(), 0x6C + 2),
                (itemGive, 0x87 + 2),
                (buildItemDialog, 0xAF + 2),
                (dlBackAllocator, 0xBC + 2),
                (showItemPopup, 0xCD + 2)
            });
            
            AsmHelper.WriteRelativeOffsets(bytes, new []
            {
                (code.ToInt64(), adjustQuantityFlag.ToInt64(), 7, 0x0 + 2),
                (code.ToInt64() + 0x13, currentQuantity.ToInt64(), 7, 0x13 + 3),
                (code.ToInt64() + 0x1A, stackCount.ToInt64(), 7, 0x1A + 3),
                (code.ToInt64() + 0x21, itemStruct.ToInt64() + 0x4, 7, 0x21 + 3),
                (code.ToInt64() + 0x3D, itemStruct.ToInt64() + 0xC, 7, 0x3D + 3),
                (code.ToInt64() + 0x44, currentQuantity.ToInt64(), 6, 0x44 + 2),
                (code.ToInt64() + 0x4A, maxQuantity.ToInt64(), 6, 0x4A + 2),
                (code.ToInt64() + 0x52, maxQuantity.ToInt64(), 6, 0x52 + 2),
                (code.ToInt64() + 0x58, currentQuantity.ToInt64(), 6, 0x58 + 2),
                (code.ToInt64() + 0x5E, itemStruct.ToInt64() + 0xC, 7, 0x5E + 3),
                (code.ToInt64() + 0x76, itemStruct.ToInt64(), 7, 0x76 + 3),
                (code.ToInt64() + 0x7D, itemCount.ToInt64(), 7, 0x7D + 3),
                (code.ToInt64() + 0x94, stackSpace.ToInt64(), 7, 0x94 + 3),
                (code.ToInt64() + 0x9B, itemStruct.ToInt64(), 7, 0x9B + 3),
                (code.ToInt64() + 0xA2, itemCount.ToInt64(), 7, 0xA2 + 3),
                (code.ToInt64() + 0xC6, stackSpace.ToInt64(), 7, 0xC6 + 3),
            });
            
            _memoryIo.WriteBytes(code, bytes);
            _memoryIo.RunThread(code);
        }
    }
}