namespace SilkySouls2.Models
{
    public class NpcInfo(string npcName, int deathFlag, int hostileFlag, params int[] majulaFlags)
    {
        public string Name { get; set; } = npcName;
        public int DeathFlagId { get; set; } = deathFlag;
        public int HostileFlagId { get; set; } = hostileFlag;
        public int[] MoveToMajulaFlagIds { get; } = majulaFlags;

        public bool HasMajulaFlags => MoveToMajulaFlagIds != null && MoveToMajulaFlagIds.Length > 0;
    }
}