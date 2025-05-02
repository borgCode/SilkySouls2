namespace SilkySouls2.Models
{
    public class NpcInfo
    {
        public NpcInfo(string npcName, int deathFlag, int hostileFlag, params int[] majulaFlags)
        {
            Name = npcName;
            DeathFlagId = deathFlag;
            HostileFlagId = hostileFlag;
            MoveToMajulaFlagIds = majulaFlags;
        }

        public string Name { get; set; }
        public int DeathFlagId { get; set; }
        public int HostileFlagId { get; set; }
        public int[] MoveToMajulaFlagIds { get; }
        
        public bool HasMajulaFlags => MoveToMajulaFlagIds != null && MoveToMajulaFlagIds.Length > 0;
    }
}