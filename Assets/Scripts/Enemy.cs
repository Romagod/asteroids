using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public static Action<GameObject> OnEnemyDead;
    public static Action<GameObject, EnemyData> OnEnemyStage;
    [HideInInspector]
    private EnemyData data;
    private bool _isdataNull;
    private bool _isrbNull;
    private bool _isStart;
    private float _gcBorder;
    private Rigidbody2D rb;
    private float timeoutShots = 0f;
    private GameObject Player;
    private GameObject Owner;
    private AudioSource audioSource;
    [HideInInspector]
    public int HPoints = 0;

    public BulletSpawnPoint point;

    public int Attack
    {
        get { return data.Attack; }
        protected set {}
    }

    public float Speed
    {
        get { return data.Speed; }
        protected set {}
    }


    public bool IsBullet
    {
        get
        {
            if (data == null)
                return false;
            return data.IsBullet;
        }
        protected set {}
    }
    public GameObject UfosBulletTemplate
    {
        get
        {
            if (data == null)
                return null;
            return data.UfosBulletTemplate;
        }
        protected set {}
    }
    public EnemyData UfosBulletData
    {
        get
        {
            if (data == null)
                return null;
            return data.UfosBulletData;
        }
        protected set {}
    }
    public float UfosShootTimeout
    {
        get
        {
            if (data == null)
                return 1.00f;
            return data.UfosShootTimeout;
        }
        protected set {}
    }

    public bool IsUfo
    {
        get
        {
            if (data == null)
                return false;
            return data.IsUfo;
        }
        protected set {}
    }

    public int Points
    {
        get
        {
            if (data == null)
                return 0;
            return data.Points;
        }
        protected set {}
    }

    public AudioClip Sound
    {
        get
        {
            if (data == null)
                return null;
            return data.Sound;
        }
        protected set {}
    }

    public AudioClip DeadSound
    {
        get
        {
            if (data == null)
                return null;
            return data.DeadSound;
        }
        protected set {}
    }

    public EnemyData SecondStage
    {
        get { return data.SecondStage; }
        protected set {}
    }
    
    /// <summary>
    /// Initialize enemy
    /// </summary>
    /// <param name="_data"></param>
    /// <param name="owner"></param>
    public void Init(EnemyData _data, GameObject _owner = null)
    {
        data = _data;
        _isStart = true;
        HPoints = data.Hp;
        Owner = _owner;
        GetComponent<SpriteRenderer>().sprite = data.MainSprite;
        
        gameObject.transform.localScale = data.Size;
        GetComponent<BoxCollider2D>().size = data.ColliderSize;;
        if (!IsBullet)
        {
            if (IsUfo)
            {
                var child = gameObject.transform.GetChild(0);
                if (child != null)
                    point = child.gameObject.GetComponent<BulletSpawnPoint>();
                
                Player = GameObject.Find("Player"); 
            }
            
            StartCoroutine(Move());
        }
    }

    public void Hit(int damage, GameObject owner = null)
    {
        HPoints -= damage;
        if (HPoints <= 0)
        {
            if (SecondStage == null)
            {
                audioSource.PlayOneShot(DeadSound);

                OnEnemyDead(gameObject);
                _isStart = false;

                StopCoroutine(Move());
            }
            else
            {
                audioSource.PlayOneShot(DeadSound);

                Init(SecondStage);
                OnEnemyStage(gameObject, data);

            }

            if (owner != null)
            {
                var player = owner.GetComponent<Player>();
                if (player != null)
                {
                    player.AddScore(Points);
                }
            }
        }
    }

    private void Start()
    {
        
        _isdataNull = data == null;
        _isStart = true;
        _gcBorder = GameCamera.Border;

        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        _isrbNull = rb == null;
        actions();
    }

    private void FixedUpdate()
    {
        checkPosition();
        if (IsBullet)
        {
            rb.AddForce(transform.up * Speed, (ForceMode2D) ForceMode.Impulse);
        }

        
        actions();
    }

    private void actions()
    {
        if (_isdataNull)
        {
            return;
        }

        if (_isrbNull)
        {
            return;
        }
        
        if (IsUfo)
        {
            shoot();
            if (Player != null)
            {
                Vector3 diff =
                    Player.transform.position
                    - gameObject.transform.position;
                diff.Normalize();

                float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

                Debug.DrawRay(transform.position, Player.transform.position - transform.position, Color.red);
            }
        }
    }
    
    private void shoot()
    {
        
        
        if (timeoutShots <= 0 && _isStart)
        {
            
            var prefab = Instantiate(UfosBulletTemplate);
            var script = prefab.GetComponent<Enemy>();
        
            script.Init(UfosBulletData, gameObject);
            prefab.transform.position = point.transform.position;
            prefab.transform.rotation = gameObject.transform.rotation;
            timeoutShots = UfosShootTimeout;
            
            audioSource.PlayOneShot(Sound);
        }
        else
        {
            timeoutShots -= Time.deltaTime;
        }
    }
    
    private IEnumerator Move()
    {
        float timeout = data.Timeout;

        if (timeout == 0)
        {
            Debug.LogError("Time Of Spawn is not set, default value is 1 sec.");
            timeout = 1;
        }

        while (true)
        {
            yield return new WaitForSeconds(timeout);
            float xPos = Random.Range(-_gcBorder, _gcBorder);
            float YPos = Random.Range(-_gcBorder, _gcBorder);
            rb.AddForce(new Vector2(xPos, YPos));
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        var obj = other.gameObject;

        if (IsBullet)
        {
            var enemy = obj.GetComponent<Enemy>();
            var player = obj.GetComponent<Player>();
            if (enemy != null)
            {
                enemy.Hit(Attack, Owner);
            }
            else if (player != null)
            {
                player.Hit(Attack);
            }
            Destroy(gameObject);
            return;
        }
        else if (EnemySpawner.Enemies.ContainsKey(obj))
        {
            if (EnemySpawner.Enemies[obj].IsBullet)
                Hit(EnemySpawner.Enemies[obj].Attack); 
        }
        
        
    }

    /// <summary>
    /// Checking position
    /// </summary>
    private void checkPosition()
    {
        if (transform.position.y > (_gcBorder - 2f))
        {
            transform.position = new Vector3(transform.position.x, -transform.position.y, transform.position.z);
            if (IsBullet)
                Destroy(gameObject);
        }
        else if (transform.position.y < -(_gcBorder - 2f))
        {
            transform.position = new Vector3(transform.position.x, -transform.position.y, transform.position.z);
            if (IsBullet)
                Destroy(gameObject);
        }

        if (transform.position.x > _gcBorder)
        {
            transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
            if (IsBullet)
                Destroy(gameObject);
        }
        else if (transform.position.x < -_gcBorder)
        {
            transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
            if (IsBullet)
                Destroy(gameObject);
        }
    }

}
