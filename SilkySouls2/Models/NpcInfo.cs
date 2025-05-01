namespace SilkySouls2.Models
{
    public class NpcInfo
    {
        public NpcInfo(string npcName, int deathFlag, int hostileFlag, int majulaFlag)
        {
            Name = npcName;
            DeathFlagId = deathFlag;
            HostileFlagId = hostileFlag;
            MoveToMajulaFlagId = majulaFlag;
        }

        public string Name { get; set; }
        public int DeathFlagId { get; set; }
        public int HostileFlagId { get; set; }
        public int MoveToMajulaFlagId { get; set; }
    }
}