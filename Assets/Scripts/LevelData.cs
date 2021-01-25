using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Level/Create New Level Data", order = 2)]
public class LevelData : ScriptableObject
{
    [Tooltip("Enemy Settings List")] [SerializeField]
    private List<EnemyData> enemySettings;
    public List<EnemyData> EnemySettings
    {
        get { return enemySettings; }
        protected set {}
    }
    
    [Tooltip("Bonus Enemy Data List")] [SerializeField]
    private List<EnemyData> bonuses;
    public List<EnemyData> Bonuses
    {
        get { return bonuses; }
        protected set {}
    }
    
    [Tooltip("Score count for win")] [SerializeField]
    private int score;
    public int Score
    {
        get { return score; }
        protected set {}
    }
    
    [Tooltip("Next Level Data")] [SerializeField]
    private LevelData nextLevel;
    public LevelData NextLevel
    {
        get { return nextLevel; }
        protected set {}
    }
    
    [Tooltip("Probability of spawn (<Spawn More Enemies, Spawn More Bonuses>)")]
    [Range(0.00f, 100.00f)]
    [SerializeField] private float probability;
    public float Probability
    {
        get { return probability; }
        protected set {}
    }
    
    [Tooltip("Background")]
    [SerializeField] private Sprite background;
    public Sprite Background
    {
        get { return background; }
        protected set {}
    }
}
