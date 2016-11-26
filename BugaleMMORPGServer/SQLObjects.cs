using System;
using System.Data.Linq.Mapping;

namespace BugaleMMORPGServer
{
    [Table(Name = "Users")]
    public class User
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)] public int UserID { get; }

        [Column] public string Username { get; set; }
        [Column] public string PasswordHash { get; set; }
        [Column] public string PasswordSalt { get; set; }
        [Column] public string Email { get; set; }
        [Column] public string FullName { get; set; }
        [Column] public DateTime DateOfBirth { get; set; }
        [Column] public DateTime CreationDate { get; set; }
        [Column] public DateTime LastLogin { get; set; }
    }

    [Table(Name = "Characters")]
    public class Character
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)] public int CharacterID { get; }

        [Column] public int UserID { get; set; }

        [Column] public string Name { get; set; }
        [Column] public int ClassID { get; set; }
        [Column] public int Level { get; set; }
        [Column] public int Experience { get; set; }

        [Column] public int MapID { get; set; }

        public int MapLocationX { get; set; }
        public int MapLocationY { get; set; }
        public int MapSpeedX { get; set; }
        public int MapSpeedY { get; set; }
    }

    [Table(Name = "MonsterTypes")]
    public class MonsterType
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)] public int MonsterTypeID { get; }

        [Column] public string Name { get; set; }
        [Column] public int Level { get; set; }
    }

    [Table(Name = "Platforms")]
    public class Platform {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)] public int PlatformID { get; }

        [Column] public int MapID { get; set; }
        [Column] public int LocationX { get; set; }
        [Column] public int LocationY { get; set; }
        [Column] public int SizeX { get; set; }
        [Column] public int SizeY { get; set; }
    }
    
    [Table(Name = "Portals")]
    public class Portal {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)] public int PortalID { get; }

        [Column] public int MapID { get; set; }
        [Column] public int LocationX { get; set; }
        [Column] public int LocationY { get; set; }
        [Column] public int DestMapID { get; set; }
        [Column] public int DestPortalID { get; set; }
    }

    [Table(Name = "MonsterSpawns")]
    public class MonsterSpawn {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)] public int MonsterSpawnID { get; }

        [Column] public int MonsterTypeID { get; set; }
        [Column] public int MapID { get; set; }
        [Column] public int PlatformID { get; set; }
    }
}
