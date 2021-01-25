using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public static Action OnPlayerDead;
    public static Action<int> OnScoreSet;
    public static Action OnBuyBonus;
    public static Action<int> OnHealthsSet;
    public static Action<EnemyData> OnPlaySound;
    
    [Tooltip("Initial player position")]
    [SerializeField] private Vector3 startPosition;
    
    [Tooltip("Initial healths cont")]
    [SerializeField] private  int  healths = 3;
    
    [Tooltip("Player speed")]
    [Range(0.1f, 1.5f)]
    [SerializeField] private float speed = 0.1f;
    
    [Tooltip("Rotation speed")]
    [Range(0.1f, 3.5f)]
    [SerializeField] private float rotationSpeed = 0.1f;
    
    [Tooltip("Bullet Template")] [SerializeField]
    private GameObject bulletPrefab;
    
    [Tooltip("Bullet Data")] [SerializeField]
    private EnemyData bulletData;
    
    [Tooltip("Time Between Shots")] 
    [Range(0.1f, 2.0f)]
    [HideInInspector]
    private float startTimeoutShots = 1.0f;

    [SerializeField] private int score;
    public int Score
    {
        get { return score; }
        set { score = value; OnScoreSet(score);}
    }
    [HideInInspector]
    public BulletSpawnPoint point;
    public Rigidbody2D rb;
    public float timeoutShots;
    public int hPoints;
    private bool _inHit = false;
    private AudioSource _audioSource;
    private AudioSource _bonusAudioSource;
    private Animator _animator;
    private static readonly int DamageAnimation = Animator.StringToHash("PlayerDamagedAnimation");
    private static readonly int HealthsAnimation = Animator.StringToHash("PlayerHealsAnimation");

    /// <summary>
    /// Initialize GameObject component
    /// </summary>
    private void Start()
    {
        transform.position = startPosition;
        rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        LoadData();
        point = gameObject.transform.GetChild(0).gameObject.GetComponent<BulletSpawnPoint>();
        var bonusAudioSourceGO = GameObject.Find("BonusAudioSource");
        if (_bonusAudioSource != null)
        {
            this._bonusAudioSource = bonusAudioSourceGO.GetComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Loading data from PlayerPrefs if is exists, or from EnemyData
    /// </summary>
    private void LoadData()
    {
        float timeout = PlayerPrefs.GetFloat("TimeoutShots");
        if (timeout <= 0)
            startTimeoutShots = bulletData.Timeout;
        else
            startTimeoutShots = timeout;
    }

    private void FixedUpdate()
    {
        moveHandler();
        shootHandler();
    }

    /// <summary>
    /// Checking input keys
    /// </summary>
    private void moveHandler()
    {
        if (Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            rb.AddForce(transform.up * speed, (ForceMode2D) ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow))
        {
            rb.AddForce(-transform.up * speed, (ForceMode2D) ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.forward, rotationSpeed);
        }
        if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.back, rotationSpeed);
        }

        checkPosition();
    }

    /// <summary>
    /// Checking position
    /// </summary>
    private void checkPosition()
    {
        if (transform.position.y > (GameCamera.Border - 2f))
        {
            transform.position = new Vector3(transform.position.x, -transform.position.y+0.002f, transform.position.z);
        }
        else if (transform.position.y < -(GameCamera.Border - 2f))
        {
            transform.position = new Vector3(transform.position.x, -transform.position.y-0.002f, transform.position.z);
        }

        if (transform.position.x > GameCamera.Border)
        {
            transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -GameCamera.Border)
        {
            transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
        }
    }

    /// <summary>
    /// Enter key pressed event handler for shoot.
    /// </summary>
    private void shootHandler()
    {
        if (timeoutShots <= 0)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                var prefab = Instantiate(bulletPrefab);
                var script = prefab.GetComponent<Enemy>();
            
                script.Init(bulletData, gameObject);
                prefab.transform.position = point.transform.position;
                prefab.transform.rotation = gameObject.transform.rotation;
                timeoutShots = startTimeoutShots;
                if (bulletData.Sound != null)
                    _audioSource.PlayOneShot(bulletData.Sound, 1);
            }
        }
        else
        {
            timeoutShots -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Collision trigger.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        var obj = other.gameObject;
        if (!obj.GetComponent<Enemy>().IsBonus)
        {
            if (EnemySpawner.Enemies.ContainsKey(obj))
            {
                if (!EnemySpawner.Enemies[obj].IsBullet)
                    Hit(EnemySpawner.Enemies[obj].Attack);
            }
        }
    }
    
    /// <summary>
    /// Collision exit trigger 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit2D(Collider2D other)
    {
        _inHit = false;
    }
    
    /// <summary>
    /// Hit action for player
    /// </summary>
    /// <param name="damage"></param>
    public void Hit(int damage)
    {
        if (!_inHit)
        {
            _inHit = true;
            hPoints -= damage;
            OnHealthsSet(hPoints);
            if (hPoints <= 0)
            {
                gameObject.SetActive(false);
                OnPlayerDead();
            }
            else
            {
                if (_animator)
                {
                    _animator.Play(DamageAnimation);
                }
            }
        }
    }
    
    /// <summary>
    /// Add Healths action for player
    /// </summary>
    /// <param name="_bonus"></param>
    public void AddHealths(BonusData _bonus)
    {
        hPoints += _bonus.Value;
        OnHealthsSet(hPoints);
        if (hPoints <= 0)
        {
            gameObject.SetActive(value: false);
            OnPlayerDead();
        }
        else
        {
            if (_animator)
            {
                _animator.Play(HealthsAnimation);
                if (_bonusAudioSource != null && _bonus.Sound != null)
                {
                    _bonusAudioSource.PlayOneShot(
                        clip: _bonus.Sound, 
                        volumeScale: 1
                        );
                }
            }
        }
    }
    
    /// <summary>
    /// Set weapon action for player
    /// </summary>
    /// <param name="_bonus"></param>
    public void SetWeapon(BonusData _bonus)
    {
        bulletData.SetWeapon(_bonus);
        LoadData();
        _animator.Play(HealthsAnimation);
        if (_bonusAudioSource != null && _bonus.Sound != null)
        {
            _bonusAudioSource.PlayOneShot(
                clip: _bonus.Sound, 
                volumeScale: 1
                );
        }
    }
    
    /// <summary>
    /// Restart action for player
    /// </summary>
    public void Restart()
    {
        gameObject.SetActive(false);
        int _healths = PlayerPrefs.GetInt("Player.Healths");
        if (_healths > 0)
            hPoints = _healths;
        else
            hPoints = healths;
        
        score = 0;
        gameObject.SetActive(true);
        Start();
    }
    
    /// <summary>
    /// Adding score for player
    /// </summary>
    /// <param name="count"></param>
    public void AddScore(int count)
    {
        Score += count;
        OnScoreSet(Score);
    }
}
