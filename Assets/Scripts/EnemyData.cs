using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/Enemy Data", order = 0)]
public class EnemyData : ScriptableObject
{
    [Tooltip("Main Sprite")]
    [SerializeField] private Sprite mainSprite;
    public Sprite MainSprite
    {
        get { return mainSprite; }
        protected set {}
    }
    
    [Tooltip("Enemy speed")]
    [SerializeField] private float speed;
    public float Speed
    {
        get { return speed; }
        protected set {}
    }
    
    [Tooltip("Enemy attack")]
    [SerializeField] private int attack;
    public int Attack
    {
        get { return attack; }
        protected set { attack = value; }
    }
    
    [Tooltip("Enemy healths")]
    [SerializeField] private int hp;
    public int Hp
    {
        get { return hp; }
        protected set {}
    }
    
    [Tooltip("Is Bullet Type")]
    [SerializeField] private bool isBullet;
    public bool IsBullet
    {
        get { return isBullet; }
        protected set {}
    }
    
    [Tooltip("Is Ufo Type")]
    [SerializeField] private bool isUfo;
    public bool IsUfo
    {
        get { return isUfo; }
        protected set {}
    }
    
    [Tooltip("Is Bonus Type")]
    [SerializeField] private bool isBonus;
    public bool IsBonus
    {
        get { return isBonus; }
        protected set {}
    }
    
    [Tooltip("Is Ufos Bullet Template")]
    [SerializeField] private GameObject ufosBulletTemplate;
    public GameObject UfosBulletTemplate
    {
        get { return ufosBulletTemplate; }
        protected set {}
    }
    
    [Tooltip("Is Ufos Bullet Data")]
    [SerializeField] private EnemyData ufosBulletData;
    public EnemyData UfosBulletData
    {
        get { return ufosBulletData; }
        protected set {}
    }
    
    [Tooltip("Is Ufos Shoot Timeout")]
    [SerializeField] private float ufosShootTimeout;
    public float UfosShootTimeout
    {
        get { return ufosShootTimeout; }
        protected set {}
    }
    
    [Tooltip("Points price")]
    [SerializeField] private int points;
    public int Points
    {
        get { return points; }
        protected set {}
    }
    
    [Tooltip("Action Timeout")]
    [SerializeField] private float timeout;
    public float Timeout
    {
        get { return timeout; }
        protected set { timeout = value; }
    }
    
    [Tooltip("Second Stage if Enemy was destroy")]
    [SerializeField] private EnemyData secondStage;
    public EnemyData SecondStage
    {
        get { return secondStage; }
        protected set {}
    }
    
    [Tooltip("Size of Enemy")]
    [SerializeField] private Vector2 size = new Vector2();
    public Vector2 Size
    {
        get { return size; }
        protected set {}
    }
    
    [Tooltip("Size of Enemies BoxCollider")]
    [SerializeField] private Vector2 colliderSize = new Vector2();
    public Vector2 ColliderSize
    {
        get { return colliderSize; }
        protected set {}
    }
    
    [Tooltip("Sound of enemy")]
    [SerializeField] private AudioClip sound;
    public AudioClip Sound
    {
        get { return sound; }
        protected set {}
    }
    
    [Tooltip("Sound of dead enemy")]
    [SerializeField] private AudioClip deadSound;
    public AudioClip DeadSound
    {
        get { return deadSound; }
        protected set {}
    }
    
    [Tooltip("Probability of spawn")]
    [Range(0.00f, 100.00f)]
    [SerializeField] private float probability;
    public float Probability
    {
        get { return probability; }
        protected set {}
    }
    
    [Tooltip("It's Bonus Data")]
    [SerializeField] private BonusData bonusSettings;
    public BonusData BonusSettings
    {
        get { return bonusSettings; }
        protected set {}
    }

    /// <summary>
    /// Action for setting players weapon
    /// </summary>
    /// <param name="_bonusData"></param>
    public void SetWeapon(BonusData _bonusData)
    {
        float timeout = Timeout;
        PlayerPrefs.SetInt("PlayerAttack", _bonusData.Value);
        if ((timeout / (_bonusData.Value/2f)) >= 0.08f)
            timeout /= (_bonusData.Value/2f);
        else
            timeout = 0.08f;
        PlayerPrefs.SetFloat("TimeoutShots", timeout);
        
        PlayerPrefs.Save();
    }
}
