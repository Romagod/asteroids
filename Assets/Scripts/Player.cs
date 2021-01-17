using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public static Action OnPlayerDead;
    public static Action<int> OnScoreSet;
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
    [SerializeField]
    private float startTimeoutShots = 1.0f;

    [SerializeField] private int score;
    public int Score
    {
        get { return score; }
        protected set { score = value; }
    }
    
    public BulletSpawnPoint point;
    
    public Rigidbody2D rb;
    public float timeoutShots;


    public int hPoints;

    
    private AudioSource audioSource;

    private void Start()
    {
        transform.position = startPosition;
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        point = gameObject.transform.GetChild(0).gameObject.GetComponent<BulletSpawnPoint>();
        hPoints = healths;
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
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rb.AddForce(transform.up * speed, (ForceMode2D) ForceMode.Impulse);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            rb.AddForce(-transform.up * speed, (ForceMode2D) ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.forward, rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
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

    private void shootHandler()
    {
        if (timeoutShots <= 0)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                var prefab = Instantiate(bulletPrefab);
                var script = prefab.GetComponent<Enemy>();
            
                script.Init(bulletData, gameObject);
                prefab.transform.position = point.transform.position;
                prefab.transform.rotation = gameObject.transform.rotation;
                timeoutShots = startTimeoutShots;
                if (bulletData.Sound != null)
                    audioSource.PlayOneShot(bulletData.Sound, 1);
            }
        }
        else
        {
            timeoutShots -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var obj = other.gameObject;
        if (EnemySpawner.Enemies.ContainsKey(obj))
        {
            if (!EnemySpawner.Enemies[obj].IsBullet)
                Hit(EnemySpawner.Enemies[obj].Attack);
        }
    }
    
    public void Hit(int damage)
    {
        hPoints -= damage;
        OnHealthsSet(hPoints);
        if (hPoints <= 0)
        {
            gameObject.SetActive(false);
            OnPlayerDead();
        }
    }
    
    public void Restart()
    {
        gameObject.SetActive(false);
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
