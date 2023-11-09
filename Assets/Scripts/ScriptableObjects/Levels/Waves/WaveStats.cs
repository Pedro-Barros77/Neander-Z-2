using UnityEngine;

[CreateAssetMenu(fileName = "New Wave Stats", menuName = "Neander Z/Wave Stats", order = 5)]
public class WaveStats : AutoRevertSO
{
    public string WaveTitle;
    public int WaveNumber;
    public float Score;
    public float MoneyEarned;
    public int EnemiesKilled;
    public int HeadshotKills;
    public float Precision;
    public int RestartCount;
    public float TimeTaken;
    public float DamageTaken;
    public bool Completed;
    public bool Started;

    public WaveStats.Data GetData() => new()
    {
        WaveTitle = WaveTitle,
        WaveNumber = WaveNumber,
        Score = Score,
        MoneyEarned = MoneyEarned,
        EnemiesKilled = EnemiesKilled,
        HeadshotKills = HeadshotKills,
        Precision = Precision,
        RestartCount = RestartCount,
        TimeTaken = TimeTaken,
        DamageTaken = DamageTaken,
        Completed = Completed,
        Started = Started
    };

    public class Data
    {
        public string WaveTitle;
        public int WaveNumber;
        public float Score;
        public float MoneyEarned;
        public int EnemiesKilled;
        public int HeadshotKills;
        public float Precision;
        public int RestartCount;
        public float TimeTaken;
        public float DamageTaken;
        public bool Completed;
        public bool Started;
    }
}


