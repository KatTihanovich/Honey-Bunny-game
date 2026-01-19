[System.Serializable]
public class Progress
{
    public long userId;
    public long levelId;
    public int killedEnemiesNumber;
    public int solvedPuzzlesNumber;
    public string timeSpent; // format "HH:mm:ss"
    public int stars;
    public string createdAt;
}
