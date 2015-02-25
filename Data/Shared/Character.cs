namespace Trellheim.Data.Shared
{
    public sealed class Character
    {
        public int Id { get; set; }
        public string CharacterName { get; set; }
        public int AccessLevelId { get; set; }
        public int Sprite { get; set; }
        public int MapId { get; set; }
        public int MapX { get; set; }
        public int MapY { get; set; }
        public int Direction { get; set; }
        public int Strength { get; set; }
        public int Defence { get; set; }
        public int Speed { get; set; }
        public int Magic { get; set; }
        public int HealthPoints { get; set; }
        public int ManaPoints { get; set; }
        public int StaminaPoints { get; set; }
        public int Level { get; set; }
        public int ExperiencePoints { get; set; }
        public int StatPoints { get; set; }
        public int ClassId { get; set; }
    }
}
