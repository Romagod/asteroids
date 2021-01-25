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
    
    [Tooltip("It's the Controls UI object")]
    [SerializeField] private GameObject controlsUI;
    public GameObject ControlsUI
    {
        get { return controlsUI; }
        protected set {}
    }
    
    [Tooltip("It's the winner UI object")]
    [SerializeField] private GameObject winnerUI;
    public GameObject WinnerUI
    {
        get { return winnerUI; }
        protected set {}
    }
    
    [Tooltip("It's the next target UI object")]
    [SerializeField] private GameObject nextTargetUI;
    public GameObject NextTargetUI
    {
        get { return nextTargetUI; }
        protected set {}
    }
    
    [Tooltip("It's the background UI object")]
    [SerializeField] private GameObject backgroundUI;
    public GameObject BackgroundUI
    {
        get { return backgroundUI; }
        protected set {}
    }
    
    [Tooltip("It's soundtrack")]
    [SerializeField] private List<AudioClip> soundtrack;
    public List<AudioClip> Soundtrack
    {
        get { return soundtrack; }
        protected set {}
    }
    
    [Tooltip("It's list of BonusData")]
    [SerializeField] private List<BonusData> bonuses;
    public List<BonusData> Bonuses
    {
        get { return bonuses; }
        protected set {}
    }
    
    [Tooltip("It's Level data")]
    [SerializeField] private LevelData levelOptions;
    public LevelData LevelOptions
    {
        get { return levelOptions; }
        protected set { levelOptions = value; }
    }

    private bool _playerDead = false;
    private AudioSource audioSource;
    private int _levelGamescore = 0;

    /// <summary>
    /// Initialize component
    /// </summary>
    private void Start()
    {

        if (!PlayerPrefs.HasKey("soundOff"))
        {
            PlayerPrefs.SetInt("soundOff", 1);
            PlayerPrefs.Save();
        }

        audioSource = gameObject.GetComponent<AudioSource>();

        if (PlayerObject != null)
        {
            Player.OnPlayerDead += GameOver;
            Player.OnScoreSet += OnScoreUpdate;
            Player.OnHealthsSet += OnHealthsUpdate;
            Player.OnBuyBonus += OnBuyBonus;
            PlayerObject.SetActive(false);
        }

        if (Spawner != null)
        {
            Spawner.GetComponent<EnemySpawner>().SetLevelSettings(LevelOptions);
            Spawner.SetActive(false);
        }
            
        if(GameOverUI != null)
            GameOverUI.SetActive(false);
            
        if(WinnerUI != null)
            WinnerUI.SetActive(false);
        
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

    /// <summary>
    /// Restart button click event listener. Starts game
    /// </summary>
    public void Restart()
    {
        ControlsUI.SetActive(false);
        WinnerUI.SetActive(false);
        PlayerObject.GetComponent<Player>().Restart();
        Spawner.GetComponent<EnemySpawner>().Restart();
        BackgroundUI.GetComponent<Image>().sprite = LevelOptions.Background;
        StartGameBtn.SetActive(false);
        if (_playerDead)
        {
            GameOverUI.SetActive(false);
            _playerDead = false;
        }
        OnHealthsUpdate(PlayerObject.GetComponent<Player>().hPoints);

    }

    /// <summary>
    /// Switches level options
    /// </summary>
    public void NextLevel()
    {
        if (LevelOptions.NextLevel != null)
        {
            _levelGamescore = 0;
            LevelOptions = LevelOptions.NextLevel;
            Spawner.GetComponent<EnemySpawner>().SetLevelSettings(LevelOptions);

            
            Spawner.SetActive(false);
            PlayerObject.SetActive(false);
            if (StartGameBtn != null && !StartGameBtn.activeSelf)
                StartGameBtn.SetActive(true);
            if(ControlsUI != null)
                ControlsUI.SetActive(true);
            if (WinnerUI != null)
            {
                if (NextTargetUI != null)
                {
                    NextTargetUI.GetComponent<Text>().text = LevelOptions.Score.ToString();
                    WinnerUI.SetActive(true);
                }

            }
        }
    }

    /// <summary>
    /// MusicToggle button click event listener. Turn on or turn off music
    /// </summary>
    public void MusicToggle()
    {
        // musicToggleUI.GetComponent<Toggle>().isOn = !musicToggleUI.GetComponent<Toggle>().isOn;
        PlayerPrefs.SetInt("soundOff", musicToggleUI.GetComponent<Toggle>().isOn ? 1 : 0);
        PlayerPrefs.Save();
        if (!musicToggleUI.GetComponent<Toggle>().isOn)
        {
            if (audioSource.clip == null)
            {
                var clip = Soundtrack.ElementAt(0);
                audioSource.clip = clip;
                audioSource.loop = true;
                audioSource.Play();
            }
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }        
    }

    /// <summary>
    /// OnPlayerDead event listener
    /// </summary>
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

    /// <summary>
    /// OnBuyBonus event listener
    /// </summary>
    private void OnBuyBonus()
    {
        Player _player = PlayerObject.GetComponent<Player>();
        var bonus = Bonuses.ElementAt(0);
        if (bonus.IsHealths)
        {
            if (_player.Score >= bonus.Price)
            {
                _player.AddHealths(bonus);
                _player.Score -= bonus.Price;
            }
        }
    }
    /// <summary>
    /// OnScoreUpdate event listener
    /// </summary>
    private void OnScoreUpdate(int value)
    {
        Player _player = PlayerObject.GetComponent<Player>();
        if (value >= 1000 && _player.hPoints < 3)
        {
            OnBuyBonus();
        } 
        
        _levelGamescore = value;
        if (_levelGamescore >= LevelOptions.Score)
        {
            NextLevel();
        }
        if(ScoreUI != null)
            ScoreUI.GetComponent<Text>().text = _player.Score.ToString();
    }
    
    /// <summary>
    /// OnHealthsUpdate event listener
    /// </summary>
    private void OnHealthsUpdate(int value)
    {
        if(HealthsUI != null)
            HealthsUI.GetComponent<Text>().text = value.ToString();

        PlayerPrefs.SetInt("Player.Healths", value);
        PlayerPrefs.Save();
    }
}


