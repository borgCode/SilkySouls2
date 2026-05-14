using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Models;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class ItemService : IItemService
    {
        private readonly IMemoryService _memoryService;
        private bool _codeIsWritten;

        public ItemService(IMemoryService memoryService, IStateService stateService)
        {
            _memoryService = memoryService;
            stateService.Subscribe(State.Detached, Reset);
        }

        public void SpawnItem(Item selectedItem, int selectedUpgrade, int selectedQuantity, int selectedInfusion,
            float durability = 0.0f)
        {
            var shouldExitFlag = CustomCodeOffsets.Base + (int)CustomCodeOffsets.ItemSpawn.ShouldExitFlag;
            var shouldProcessFlag = CustomCodeOffsets.Base + (int)CustomCodeOffsets.ItemSpawn.ShouldProcessFlag;
            var adjustQuantityFlag = CustomCodeOffsets.Base + (int)CustomCodeOffsets.ItemSpawn.AdjustQuantityFlag;
            var maxQuantity = CustomCodeOffsets.Base + (int)CustomCodeOffsets.ItemSpawn.MaxQuantity;
            var itemCount = CustomCodeOffsets.Base + (int)CustomCodeOffsets.ItemSpawn.ItemCount;
            var currentQuantity = CustomCodeOffsets.Base + (int)CustomCodeOffsets.ItemSpawn.CurrentQuantity;
            var stackCount = CustomCodeOffsets.Base + (int)CustomCodeOffsets.ItemSpawn.StackCount;
            var itemStruct = CustomCodeOffsets.Base + (int)CustomCodeOffsets.ItemSpawn.ItemStruct;
            var stackSpace = CustomCodeOffsets.Base + (int)CustomCodeOffsets.ItemSpawn.StackSpace;
            var code = CustomCodeOffsets.Base + (int)CustomCodeOffsets.ItemSpawn.Code;

            var getCurrentQuantity = Functions.CurrentItemQuantityCheck;
            var itemGive = Functions.ItemGive;
            var buildItemDialog = Functions.BuildItemDialog;
            var showItemPopup = Functions.ShowItemDialog;
            var sleepAddr = _memoryService.GetProcAddress("kernel32.dll", "Sleep");

            _memoryService.Write(currentQuantity, 0);
            _memoryService.Write(stackCount, 0);

            _memoryService.Write(maxQuantity, selectedItem.StackSize);
            _memoryService.Write(itemStruct + 0x4, selectedItem.Id);
            _memoryService.Write(itemStruct + 0x8, durability);
            _memoryService.Write(itemStruct + 0xC, (short)selectedQuantity);
            _memoryService.Write(itemStruct + 0xE, (byte)selectedUpgrade);
            _memoryService.Write(itemStruct + 0xF, (byte)selectedInfusion);
            _memoryService.Write(itemCount, 1);
            _memoryService.Write(adjustQuantityFlag, selectedItem.StackSize > 1);
            _memoryService.Write(shouldProcessFlag, (byte)1);

            if (!_codeIsWritten)
            {
                byte[] bytes;
                if (PatchManager.Current.Edition == GameEdition.Scholar)
                {
                    bytes = AsmLoader.GetAsmBytes(AsmScript.ItemSpawn64);

                    AsmHelper.WriteAbsoluteAddresses64(bytes, [
                        (GameManagerImp.Base, 0xD + 2),
                        (getCurrentQuantity, 0x55 + 2),
                        (itemGive, 0xAD + 2),
                        (buildItemDialog, 0xD5 + 2),
                        (showItemPopup, 0xF0 + 2),
                        (sleepAddr, 0x104 + 2)
                    ]);

                    AsmHelper.WriteRelativeOffsets(bytes, [
                        (code, shouldProcessFlag, 7, 0x0 + 2),
                        (code + 0x2C, shouldProcessFlag, 7, 0x2C + 2),
                        (code + 0x33, adjustQuantityFlag, 7, 0x33 + 2),
                        (code + 0x40, currentQuantity, 7, 0x40 + 3),
                        (code + 0x47, stackCount, 7, 0x47 + 3),
                        (code + 0x4E, itemStruct + 0x4, 7, 0x4E + 3),
                        (code + 0x6A, itemStruct + 0xC, 7, 0x6A + 3),
                        (code + 0x71, currentQuantity, 6, 0x71 + 2),
                        (code + 0x77, maxQuantity, 6, 0x77 + 2),
                        (code + 0x7F, maxQuantity, 6, 0x7F + 2),
                        (code + 0x85, currentQuantity, 6, 0x85 + 2),
                        (code + 0x8B, itemStruct + 0xC, 7, 0x8B + 3),
                        (code + 0x9C, itemStruct, 7, 0x9C + 3),
                        (code + 0xA3, itemCount, 7, 0xA3 + 3),
                        (code + 0xBA, stackSpace, 7, 0xBA + 3),
                        (code + 0xC1, itemStruct, 7, 0xC1 + 3),
                        (code + 0xC8, itemCount, 7, 0xC8 + 3),
                        (code + 0xE9, stackSpace, 7, 0xE9 + 3),
                        (code + 0x117, shouldExitFlag, 7, 0x117 + 2)
                    ]);
                }
                else
                {
                    bytes = AsmLoader.GetAsmBytes(AsmScript.ItemSpawn32);
                    AsmHelper.WriteAbsoluteAddresses32(bytes, [
                        (shouldProcessFlag, 2),
                        (GameManagerImp.Base, 0xD + 2),
                        (shouldProcessFlag, 0x1E + 2),
                        (adjustQuantityFlag, 0x25 + 2),
                        (itemStruct + 0x4, 0x31 + 2),
                        (stackCount, 0x38 + 2),
                        (currentQuantity, 0x3F + 2),
                        (itemStruct + 0xC, 0x4B + 3),
                        (currentQuantity, 0x52 + 2),
                        (maxQuantity, 0x58 + 2),
                        (maxQuantity, 0x60 + 1),
                        (currentQuantity, 0x65 + 2),
                        (itemStruct + 0xC, 0x6B + 2),
                        (itemCount, 0x75 + 2),
                        (itemStruct, 0x7B + 2),
                        (itemCount, 0x89 + 2),
                        (itemStruct, 0x8F + 2),
                        (stackSpace, 0x96 + 2),
                        (stackSpace, 0xAB + 2),
                        (Functions.Sleep, 0xB7 + 1),
                        (shouldExitFlag, 0xC0 + 2)
                    ]);
                    AsmHelper.WriteRelativeOffsets(bytes, [
                        (code + 0x46, getCurrentQuantity, 5, 0x46 + 1),
                        (code + 0x82, itemGive, 5, 0x82 + 1),
                        (code + 0x9D, buildItemDialog, 5, 0x9D + 1),
                        (code + 0xB2, showItemPopup, 5, 0xB2 + 1)
                    ]);
                    
                }

                _memoryService.WriteBytes(code, bytes);
                _codeIsWritten = true;
                _memoryService.RunPersistentThread(code);
            }
        }
        

        public void Reset()
        {
            _codeIsWritten = false;
        }

        public void SignalClose()
        {
            _memoryService.Write(CustomCodeOffsets.Base + (int)CustomCodeOffsets.ItemSpawn.ShouldExitFlag, (byte)1);
        }
    }
}