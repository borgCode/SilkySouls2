using System.Collections.Generic;
using SilkySouls2.Models;

namespace SilkySouls2.Interfaces;

public interface IAttunementService
{
    List<InventorySpell> GetInventorySpells();
    List<EquippedSpell> GetEquippedSpells();
    int GetTotalAvailableSlots();
    void AttuneSpell(int slotIndex, nint entryAddr);
}