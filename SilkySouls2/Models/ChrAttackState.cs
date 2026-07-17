// 

using SilkySouls2.enums;

namespace SilkySouls2.Models;

public class ChrAttackState(int actionId, ChrAttackActionType actionType)
{
    public ChrAttackActionType ActionType { get;} = actionType;
    public int ActionId { get; } = actionId;

    public override string ToString()
    {
        return $"{nameof(ActionType)}: {ActionType}, {nameof(ActionId)}: {ActionId}";
    }
}