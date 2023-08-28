using System;

public enum Map 
{ 
    Default
}
public enum GameMode
{
    Default
}

public enum GameQueue
{
    Solo,
    Team
}

[Serializable]
public class GameData
{
    public string userName;
    public string userAuthID;
    public GameInfo userGamePreferences;
}

[Serializable]
public class GameInfo
{
    public Map map;
    public GameMode mode;
    public GameQueue queue;

    public string ToMultiplayQueue()
    {
        return queue switch
        {
            GameQueue.Solo => "solo-queue",
            GameQueue.Team => "team-queue",
            _ => "solo-queue"
        };
    }
}
