using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [SerializeField] int POINTS_ON_VICTORY = 10;

    [SerializeField] GameObject victoryScreen;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject skillPointsUI;
    [SerializeField] GameObject bounceCounterUI;
    [SerializeField] GameObject possessionTimer;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject instructions;

    private int numberOfScenes;
    public bool Victory
    {
        set
        {
            if (IsGameOver) return;

            if (victoryScreen == null)
            {
                ChangeScene(SceneManager.GetActiveScene().name);
                return;
            }
            AudioManager.Instance.StopWalk();
            if (value)
            {
                instructions.SetActive(false);
                StartCoroutine(ShowVictory());
            }
            else
            {
                instructions.SetActive(true);
                IsGameOver = false;
                victoryScreen.SetActive(false);
            }
        }

        get
        { 
            return IsGameOver;
        }
    }

    public bool GameOver
    {
        set
        {
            if (IsGameOver) return;
            
            AudioManager.Instance.StopWalk();
            if (gameOverScreen == null) return;

            if (value)
            {
                instructions.SetActive(false);
                StartCoroutine(ShowGameOver());
            }
            else
            {
                IsGameOver = false;
                gameOverScreen.SetActive(false);
            }
        }
    }

    public Nullable<int> SkillPoints
    {
        set
        {
            if (skillPointsUI == null) return;

            skillPointsUI.SetActive(value != null);

            if (value.HasValue)
            {
                TextMeshProUGUI tmp = skillPointsUI.GetComponentInChildren<TextMeshProUGUI>();
                tmp.text = Skills.Points.ToString();
            }
        }
    }

    public Nullable<int> BounceCount
    {
        set
        {
            if (bounceCounterUI == null) return;

            bounceCounterUI.SetActive(value != null);

            if (value.HasValue)
            {
                TextMeshProUGUI tmp = bounceCounterUI.GetComponentInChildren<TextMeshProUGUI>();
                tmp.text = "Bounces Left: " + value.ToString();
            }
        }
    }

    public Nullable<float> PossessionTimeLeft
    {
        set
        {
            if (possessionTimer == null) return;

            possessionTimer.SetActive(value != null);

            if (value.HasValue)
            {
                Slider slider = possessionTimer.GetComponentInChildren<Slider>();
                slider.value = value.Value;
            }
        }
    }

    public bool IsGameOver { get; private set; } = false;

    void Start()
    {
        GameOver = false;
        SkillPoints = Skills.Points;
        BounceCount = null;
        PossessionTimeLeft = null;
        numberOfScenes = NumberOfScenes();
        Debug.Log("Number of scenes: " + numberOfScenes);
        // set the time scale to 1
        Time.timeScale = 1;
        // get button of victory screen
        if (victoryScreen == null) return;
        if (gameOverScreen == null) return;
        Button victoryButton = victoryScreen.transform.Find("classic_victory").transform.Find("Button").GetComponent<Button>();
        victoryButton.onClick.AddListener(() => ChangeScene("SceneSkillTree"));
        Button finalVictoryButton = victoryScreen.transform.Find("final_victory").transform.Find("Button").GetComponent<Button>();
        finalVictoryButton.onClick.AddListener(() => ChangeScene("MainMenu"));
        // get button of game over screen
        Button gameOverButton = gameOverScreen.GetComponentInChildren<Button>();
        gameOverButton.onClick.AddListener(() => ChangeScene("SceneSkillTree"));

        
    }

    private void Update()
    {
        if (IsGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                string scene = "SceneSkillTree";
                ChangeScene(scene);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                string scene = SceneManager.GetActiveScene().name;
                ChangeScene(scene);
            }
            if(Input.GetKeyDown(KeyCode.I))
            {
                print("I");
                FindFirstObjectByType<PlayerController>().initialCameraTour = false;
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
            if(Input.GetKeyDown(KeyCode.L))
            {
                Victory = true;
            }
            if(Input.GetKeyDown(KeyCode.K))
            {
                GameOver = true;
            }
        }
    }

    public void TogglePause()
    {
        if (IsGameOver) return;

        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            AudioManager.Instance.PlaySound("pause", 0.5f);
            AudioManager.Instance.UnPause();
        }
        else
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            AudioManager.Instance.PlaySound("pause", 0.5f);
            AudioManager.Instance.Pause();
        }
    }

    public void CheckForVictory()
    {
        StartCoroutine(CheckForVictoryAfterDelay());
    }

    private int NumberOfScenes()
    {
        int sceneNumber = 0;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            if (sceneName.ToLower().Contains("level"))
            {
                sceneNumber++;
            }
        }
        return sceneNumber;
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator CheckForVictoryAfterDelay()
    {
        yield return new WaitForSeconds(1);

        int enemiesRemaining = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None).Length;
        if (enemiesRemaining == 0) Victory = true;
    }

    private IEnumerator ShowVictory()
    {
        if (Utils.NextSceneToPlay == numberOfScenes-1)
        {
            AudioManager.Instance.PlaySound("big_win", 0.15f);
            victoryScreen.transform.Find("classic_victory").gameObject.SetActive(false);
            victoryScreen.transform.Find("final_victory").gameObject.SetActive(true);
        }
        else
        {
            AudioManager.Instance.PlaySound("win", 0.35f);
            victoryScreen.transform.Find("classic_victory").gameObject.SetActive(true);
            victoryScreen.transform.Find("final_victory").gameObject.SetActive(false);
        }
        
        yield return new WaitForSeconds(1);
        if (!IsGameOver)
        {
            Skills.GainPoints(POINTS_ON_VICTORY);
            if (Utils.NextSceneToPlay < numberOfScenes-1)
            {
                Utils.NextSceneToPlay++;
            }
        }
        victoryScreen.SetActive(IsGameOver = true);
    }

    private IEnumerator ShowGameOver()
    {
        AudioManager.Instance.PlaySound("lose", 0.35f);
        yield return new WaitForSeconds(2);
        gameOverScreen.SetActive(IsGameOver = true);
    }
}
