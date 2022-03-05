
using System;
using CreatorKitCode;

public interface ICanGiveExp
{
    public event Action<int,CharacterData> DeathRattle;
    
}