using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Tooltip("Enemy Settings List")] [SerializeField]
    public List<EnemyData> enemySettings;

    [Tooltip("Count of the objects in pull")] [SerializeField]
    private int poolCount;
    
    [Tooltip("Count of the bullets for Player in pull")] [SerializeField]
    private int playerBulletsCount;
    
    [Tooltip("Count of the bullets for UFO in pull")] [SerializeField]
    private int ufoBulletsCount;

    [Tooltip("Enemy Template")] [SerializeField]
    private GameObject enemyPrefab;

    
    [Tooltip("Time for spawn")] [Range(0.1f, 5.0f)] [SerializeField]
    private float spawnTime;

    /// <summary>
    /// Dictionary for enemy scripts on scene
    /// </summary>
    public static Dictionary<GameObject, Enemy> Enemies;
    private Queue<GameObject> currentEnemies;
    
    private GameObject Player;

    private void Start()
    {
        Player = GameObject.Find("Player"); 
        
        Enemy.OnEnemyDead += ReturnEnemy;
        Enemy.OnEnemyStage += ReturnEnemyStage;
    }

    private IEnumerator Spawn()
    {
        if (spawnTime == 0)
        {
            Debug.LogError("Time Of Spawn is not set, default value is 1 sec.");
            spawnTime = 1;
        }

        while (true)
        {
            yield return new WaitForSeconds(spawnTime);
            if (currentEnemies.Count > 0)
            {
                var enemy = currentEnemies.Dequeue();
                bool keyExists = Enemies.ContainsKey(enemy);
                if (keyExists)
                {
                    var script = Enemies[enemy];
                    enemy.SetActive(true);

                    int random = Random.Range(0, enemySettings.Count);
                    script.Init(enemySettings[random]);
                    
                    enemy.transform.position = createPositions();
                }
            }
        }
    }

    private Vector2 createPositions()
    {
        float xPos = Random.Range(-GameCamera.Border, GameCamera.Border);
        float yPos = Random.Range(-GameCamera.Border + 2f, GameCamera.Border - 2f);
        Vector2 newPosition = new Vector2(xPos, yPos);
        if (Player != null)
        {
            while (Vector3.Distance (Player.gameObject.transform.position, newPosition) <= 1.2 )
            {
                xPos = Random.Range(-GameCamera.Border, GameCamera.Border);
                yPos = Random.Range(-GameCamera.Border + 2f, GameCamera.Border - 2f);
                newPosition = new Vector2(xPos, yPos);
            }
        }
        
        return newPosition;
    }
    
    /// <summary>
    /// Get back in pull and prepare for the next use
    /// </summary>
    /// <param name="_enemy"></param>
    private void ReturnEnemy(GameObject _enemy)
    {
        _enemy.transform.position = transform.position;
        _enemy.SetActive(false);
        currentEnemies.Enqueue(_enemy);
    }
    
    /// <summary>
    /// Sets stage and clone object
    /// </summary>
    /// <param name="_enemy"></param>
    /// <param name="_data"></param>
    private void ReturnEnemyStage(GameObject _enemy, EnemyData _data)
    {
        Rigidbody2D firstRigidbody = _enemy.GetComponent<Rigidbody2D>();
        BoxCollider2D firstBoxCollider = _enemy.GetComponent<BoxCollider2D>();
        Vector3 firstRandomVector = Random.insideUnitSphere;
        firstRigidbody.AddForce(firstRandomVector*100);
        _enemy.SetActive(true);
        _enemy.transform.localScale = _data.Size;
        firstBoxCollider.size = _data.ColliderSize;;
        
        var prefab = Instantiate(_enemy);
        
        Rigidbody2D secondRigidbody = prefab.GetComponent<Rigidbody2D>();
        var script = prefab.GetComponent<Enemy>();
        script.Init(_data);
        Vector3 secondRandomVector = Random.insideUnitSphere;

        secondRigidbody.AddForce(secondRandomVector*100);

        prefab.SetActive(true);
        Enemies.Add(prefab, script);
        // currentEnemies.Enqueue(_enemy);
    }

    public void Restart()
    {
        
        if (currentEnemies != null)
        {
            while (currentEnemies.Count > 0)
            {
                var enemy = currentEnemies.Dequeue();
                Destroy(enemy);
            }
        }

        if (Enemies != null)
        {
            
            foreach(KeyValuePair<GameObject, Enemy> entry in Enemies)
            {
                Destroy(entry.Key);
            }
        }
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        // StartCoroutine(Spawn());
    }
    
    private void OnDisable()
    {
        StopCoroutine(Spawn());
    }
    
    private void OnEnable()
    {
        Enemies = new Dictionary<GameObject, Enemy>();
        currentEnemies = new Queue<GameObject>();
        
        for (int i = 0; i < poolCount; ++i)
        {
            var prefab = Instantiate(enemyPrefab);
            var script = prefab.GetComponent<Enemy>();
            prefab.SetActive(false);
            Enemies.Add(prefab, script);
            currentEnemies.Enqueue(prefab);
        }
        
        StartCoroutine(Spawn());
    }
}
