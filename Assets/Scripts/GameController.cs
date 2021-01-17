using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Tooltip("It's the Player")]
    [SerializeField] private GameObject player;
    public GameObject PlayerObject
    {
        get { return player; }
        protected set {}
    }
    
    [Tooltip("It's the Spawner")]
    [SerializeField] private GameObject spawner;
    public GameObject Spawner
    {
        get { return spawner; }
        protected set {}
    }
    
    [Tooltip("It's the Start Game Button")]
    [SerializeField] private GameObject startGameBtn;
    public GameObject StartGameBtn
    {
        get { return startGameBtn; }
        protected set {}
    }
    
    [Tooltip("It's the Game Over UI object")]
    [SerializeField] private GameObject gameOverUI;
    public GameObject GameOverUI
    {
        get { return gameOverUI; }
        protected set {}
    }
    
    [Tooltip("It's the Game Over UI object")]
    [SerializeField] private GameObject scoreUI;
    public GameObject ScoreUI
    {
        get { return scoreUI; }
        protected set {}
    }
    
    [Tooltip("It's the Game Over UI object")]
    [SerializeField] private GameObject healthsUI;
    public GameObject HealthsUI
    {
        get { return healthsUI; }
        protected set {}
    }
    
    [Tooltip("It's the Music Toggle UI object")]
    [SerializeField] private GameObject musicToggleUI;
    public GameObject MusicToggleUI
    {
        get { return musicToggleUI; }
        protected set {}
    }
    
    [Tooltip("It's the Music Toggle UI object")]
    [SerializeField] private GameObject controlsUI;
    public GameObject ControlsUI
    {
        get { return controlsUI; }
        protected set {}
    }
    
    [Tooltip("It's soundtrack")]
    [SerializeField] private List<AudioClip> soundtrack;
    public List<AudioClip> Soundtrack
    {
        get { return soundtrack; }
        protected set {}
    }

    private bool _playerDead = false;
    private AudioSource audioSource;

    private void Start()
    {

        if (!PlayerPrefs.HasKey("soundOff"))
        {
            PlayerPrefs.SetInt("soundOff", 1);
        }

        audioSource = gameObject.GetComponent<AudioSource>();

        if (PlayerObject != null)
        {
            Player.OnPlayerDead += GameOver;
            Player.OnScoreSet += OnScoreUpdate;
            Player.OnHealthsSet += OnHealthsUpdate;
            PlayerObject.SetActive(false);
        }

        if (Spawner != null)
        {
            Spawner.SetActive(false);
        }
            
        if(GameOverUI != null)
            GameOverUI.SetActive(false);
        
        if (MusicToggleUI != null)
        {

            musicToggleUI.GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("soundOff") == 1;
            if (!musicToggleUI.GetComponent<Toggle>().isOn)
            {
                var clip = Soundtrack.ElementAt(0);
                audioSource.clip = clip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        
    }

    public void Restart()
    {
        ControlsUI.SetActive(false);
        PlayerObject.GetComponent<Player>().Restart();
        Spawner.GetComponent<EnemySpawner>().Restart();
        StartGameBtn.SetActive(false);
        GameOverUI.SetActive(false);
        _playerDead = false;
        OnHealthsUpdate(PlayerObject.GetComponent<Player>().hPoints);
    }

    public void MusicToggle()
    {
        // musicToggleUI.GetComponent<Toggle>().isOn = !musicToggleUI.GetComponent<Toggle>().isOn;
        PlayerPrefs.SetInt("soundOff", musicToggleUI.GetComponent<Toggle>().isOn ? 1 : 0);
        if (!musicToggleUI.GetComponent<Toggle>().isOn)
            audioSource.Play();
        else
            audioSource.Stop();
        
    }

    private void GameOver()
    {
        if (!_playerDead)
        {
            _playerDead = true;
            if (StartGameBtn != null && !StartGameBtn.activeSelf)
                StartGameBtn.SetActive(true);

            if(GameOverUI != null)
                GameOverUI.SetActive(true);
            
            if(ControlsUI != null)
                ControlsUI.SetActive(true);
        }
    }

    private void OnScoreUpdate(int value)
    {
        if(ScoreUI != null)
            ScoreUI.GetComponent<Text>().text = value.ToString();
    }
    private void OnHealthsUpdate(int value)
    {
        if(HealthsUI != null)
            HealthsUI.GetComponent<Text>().text = value.ToString();
    }
}


