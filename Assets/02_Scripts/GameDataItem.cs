using BackEnd;
using System;
using System.Collections.Generic;

public class GameDataItem {
    public string gameVersion;
    public bool isCleared;
    public List<int> abilities = new List<int>();

    public GameDataItem(string version, bool cleared, List<int> ab_nums)
    {
        gameVersion = version;
        isCleared = cleared;
        abilities = ab_nums;
    }

    public Param ToParam()
    {
        Param param = new Param();

        param.Add("gameVersion", gameVersion);
        param.Add("isCleared", isCleared);
        param.Add("abilities", abilities);
        param.Add("lastUpdate", DateTime.UtcNow);

        return param;
    }
}