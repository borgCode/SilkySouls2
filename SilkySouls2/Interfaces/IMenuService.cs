//

using SilkySouls2.enums;

namespace SilkySouls2.Interfaces;

public interface IMenuService
{
    void OpenMenu(NpcMenuType menuType, int startRowId = 0, int endRowId = 0);
    void Reset();
}