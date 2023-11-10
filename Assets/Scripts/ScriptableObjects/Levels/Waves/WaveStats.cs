using UnityEngine;

[CreateAssetMenu(fileName = "New Wave Stats", menuName = "Neander Z/Wave Stats", order = 5)]
public class WaveStats : AutoRevertSO
{
    public int WaveNumber;
    public float Score;
    public float MoneyEarned;
    public int EnemiesKilled;
    public int HeadshotKills;
    public float Precision;
    public int RestartCount;
    public int DeathCount;
    public float TimeTaken;
    public float DamageTaken;
    public bool Completed;
    public bool Started;
    public int InputMode;

    public WaveStats.Data GetData() => new()
    {
        WaveNumber = WaveNumber,
        Score = Score,
        MoneyEarned = MoneyEarned,
        EnemiesKilled = EnemiesKilled,
        HeadshotKills = HeadshotKills,
        Precision = Precision,
        RestartCount = RestartCount,
        DeathCount = DeathCount,
        TimeTaken = TimeTaken,
        DamageTaken = DamageTaken,
        Completed = Completed,
        Started = Started,
        InputMode = InputMode
    };

    public class Data
    {
        public int WaveNumber;
        public float Score;
        public float MoneyEarned;
        public int EnemiesKilled;
        public int HeadshotKills;
        public float Precision;
        public int RestartCount;
        public int DeathCount;
        public float TimeTaken;
        public float DamageTaken;
        public bool Completed;
        public bool Started;
        public int InputMode;
    }
}


