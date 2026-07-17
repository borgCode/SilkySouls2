using System.Collections.Generic;
using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Models;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services;

public class AttunementService(IMemoryService memoryService) : IAttunementService
{
    public List<InventorySpell> GetInventorySpells()
        {
            var spellBase = memoryService.FollowPointers(memoryService.ReadPointer(GameManagerImp.Base), [
                GameManagerImp.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr,
                GameManagerImp.GameDataManagerOffsets.Inventory.InventoryLists,
                GameManagerImp.GameDataManagerOffsets.Inventory.ItemInventory2BagListPtr,
                GameManagerImp.GameDataManagerOffsets.Inventory.ItemInventory2BagList.ItemInvetory2SpellListPtr
            ], true);

            var count = memoryService.Read<byte>(
                spellBase + GameManagerImp.GameDataManagerOffsets.Inventory.ItemInvetory2SpellList.Count
            );
            return count == 0 ? [] : ReadSpellList(spellBase, count);
        }

        private List<InventorySpell> ReadSpellList(nint spellBase, int count)
        {
            List<InventorySpell> currentSpells = [];
            var current = memoryService.ReadPointer(
                spellBase + GameManagerImp.GameDataManagerOffsets.Inventory.ItemInvetory2SpellList.ListStart);

            for (int i = 0; i < count && current != 0; i++)
            {
                var spellId = memoryService.Read<int>(
                    current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.SpellId);
                var isEquipped = memoryService.Read<byte>(
                    current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.IsEquipped);
                var slotReq = memoryService.Read<byte>(
                    current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.SlotReq);

                currentSpells.Add(new InventorySpell(spellId, isEquipped == 2, current, slotReq));

                current = memoryService.ReadPointer(
                    current + GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.NextPtr);
            }

            return currentSpells;
        }

        public List<EquippedSpell> GetEquippedSpells()
        {
            var currentSpell = GetCurrentSpellPtr();
            List<EquippedSpell> currentSpells = [];

            int chunkSize = PatchManager.Current.Edition == GameEdition.Scholar ? 0x10 : 0x8;

            for (int i = 0; i < 14; i++)
            {
                currentSpells.Add(new EquippedSpell(memoryService.Read<int>(currentSpell), i));
                currentSpell += chunkSize;
            }

            return currentSpells;
        }

        private nint GetCurrentSpellPtr()
        {
            return memoryService.FollowPointers(memoryService.ReadPointer(GameManagerImp.Base), [
                GameManagerImp.PlayerCtrl,
                ChrCtrl.ChrAsmCtrl,
                ChrCtrl.EquippedSpellsStart
            ], false);
        }

        public int GetTotalAvailableSlots()
        {
            RefreshSpellSlots();

            var inventory = memoryService.FollowPointers(memoryService.ReadPointer(GameManagerImp.Base), [
                GameManagerImp.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr
            ], true);
            var getNumOfSlots1 = Functions.GetNumOfSpellSlots1;
            var getNumOfSlots2 = Functions.GetNumOfSpellSlots2;
            var slotsLoc = CustomCodeOffsets.Base + CustomCodeOffsets.NumOfSpellSlots;


            byte[] bytes;

            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                bytes = AsmLoader.GetAsmBytes(AsmScript.GetNumOfSlots64);

                AsmHelper.WriteAbsoluteAddresses64(bytes, [
                    (slotsLoc, 2),
                    (inventory, 0xA + 2),
                    (getNumOfSlots1, 0x17 + 2),
                    (getNumOfSlots2, 0x2C + 2)
                ]);
            }
            else
            {
                bytes = AsmLoader.GetAsmBytes(AsmScript.GetNumOfSlots32);
                AsmHelper.WriteAbsoluteAddresses32(bytes, [
                    (slotsLoc, 1),
                    (inventory, 0x5 + 1),
                    (getNumOfSlots1, 0xC + 1),
                    (getNumOfSlots2, 0x17 + 1)
                ]);
            }

            memoryService.AllocateAndExecute(bytes);


            return memoryService.Read<int>(slotsLoc);
        }

        private void RefreshSpellSlots()
        {
            var bagList = memoryService.FollowPointers(memoryService.ReadPointer(GameManagerImp.Base), [
                GameManagerImp.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr,
                GameManagerImp.GameDataManagerOffsets.Inventory.InventoryLists,
                GameManagerImp.GameDataManagerOffsets.Inventory.ItemInventory2BagListPtr
            ], true);

            var refreshFunc = Functions.UpdateSpellSlots;
            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.UpdateSpellSlots64);

                AsmHelper.WriteAbsoluteAddresses64(bytes, [
                    (bagList, 2),
                    (refreshFunc, 0xA + 2)
                ]);
                memoryService.AllocateAndExecute(bytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.UpdateSpellSlots32);
                AsmHelper.WriteAbsoluteAddresses32(bytes, [
                    (bagList, 1),
                    (refreshFunc, 0x5 + 1)
                ]);
                memoryService.AllocateAndExecute(bytes);
            }
        }

        public void AttuneSpell(int slotIndex, nint entryAddr)
        {
            var inventoryLists = memoryService.FollowPointers(memoryService.ReadPointer(GameManagerImp.Base), [
                GameManagerImp.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr,
                GameManagerImp.GameDataManagerOffsets.Inventory.InventoryLists
            ], true);

            var attuneFunc = Functions.AttuneSpell;

            if (PatchManager.IsScholar())
            {
                AttuneScholarSpell(slotIndex, entryAddr, inventoryLists, attuneFunc);
            }
            else
            {
                AttuneVanillaSpell(slotIndex, entryAddr, inventoryLists, attuneFunc);
            }
        }

        private void AttuneScholarSpell(int slotIndex, nint entryAddr, nint inventoryLists, nint attuneFunc)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.AttuneSpell64);
            AsmHelper.WriteAbsoluteAddresses64(bytes, [
                (inventoryLists, 2),
                (slotIndex + 0x1C, 0xA + 2),
                (entryAddr, 0x14 + 2),
                (attuneFunc, 0x1E + 2)
            ]);
            memoryService.AllocateAndExecute(bytes);
        }

        private void AttuneVanillaSpell(int slotIndex, nint entryAddr, nint inventoryLists, nint attuneFunc)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.AttuneSpell32);
            AsmHelper.WriteAbsoluteAddresses32(bytes, [
                (inventoryLists, 1),
                (entryAddr, 0x5 + 1),
                (slotIndex + 0x1C, 0xB + 1),
                (attuneFunc, 0x11 + 1)
            ]);

            memoryService.AllocateAndExecute(bytes);
        }
}