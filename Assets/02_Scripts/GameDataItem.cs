using BackEnd;
using System;
using System.Collections.Generic;

public class GameDataItem
{
    readonly string gameVersion;
    readonly bool isCleared;
    readonly List<int> abilities;

    public GameDataItem(string version, bool cleared, List<int> abilityIds)
    {
        gameVersion = version;
        isCleared = cleared;
        abilities = abilityIds;
    }

    public Param ToParam()
    {
        Param param = new()
        {
            { "gameVersion", gameVersion },
            { "isCleared", isCleared },
            { "abilities", abilities },
            { "lastUpdate", DateTime.UtcNow }
        };

        return param;
    }
}
