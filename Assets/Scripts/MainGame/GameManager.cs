using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Settings")]
    [SerializeField] private GameObject map;
    private int currentLevel;

    private int countTrays, currTray = 0;
    private float gameTime = 180f; 
    private float currentTime;     
    private bool isPaused = false;
    private Coroutine timerCoroutine;

    [Header("UI")]
    [SerializeField] private GameObject winPanel, losePanel;
    [SerializeField] private Text txtLevel, txTime;

    [Header("Tools")]
    [SerializeField] private bool bombMode = false;
    [SerializeField] private bool shrinkMode = false;



    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        SetUp();
       
    }

    private void SetUp()
    {
        currentTime = gameTime;
        timerCoroutine = StartCoroutine(GameTimer());
        currentLevel = PlayerPrefs.GetInt("Level", 0);
        if (currentLevel >= map.transform.childCount)
        {
            currentLevel = 0;
            PlayerPrefs.SetInt("Level", 0);
        }

        txtLevel.text = "Level " + (currentLevel + 1).ToString();
        map.transform.GetChild(currentLevel).gameObject.SetActive(true);

        Tray[] allTray = FindObjectsByType<Tray>(FindObjectsSortMode.None);
        countTrays = allTray.Length;
    }

    private IEnumerator GameTimer()
    {
        while (currentTime > 0 && !isGameOver)
        {
            if (!isPaused)   
            {
                currentTime -= Time.deltaTime;
                UpdateTimerUI(currentTime);
            }
            yield return null; 
        }

        if (!isGameOver && currentTime <= 0)
        {
            StartCoroutine(LoseGame());
        }
    }

    private void UpdateTimerUI(float time)
    {
        if (currentTime < 0) currentTime = 0;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        txTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    public void FreezeTool()
    {
        if (!isPaused) StartCoroutine(Freeze(20f));
    }

    private IEnumerator Freeze(float time)
    {
        isPaused = true;
        yield return new WaitForSeconds(time);
        isPaused = false;
    }

    public void ActivateBomb()
    {
        if (isGameOver) return;
        bombMode = !bombMode;
    }
    public void UseBombOnTray(Tray tray)
    {
        if (!bombMode || tray == null) return;
        tray.Boom();
        ClearSweet(tray, tray.SweetCount());
        bombMode = false; 
    }

    public void ClearSweet(Tray tray, int count)
    {
        GetComponent<BoardManager>().ClearByCount(tray, count);

    }

    public bool IsBombMode()
    {
        return bombMode;
    }

    public void ActivateShrink()
    {
        if (isGameOver) return;
        shrinkMode = !shrinkMode;
    }

    public void UseShrinkOnTray(Tray tray)
    {
        if (!shrinkMode || tray == null) return;
        tray.Shrink();
        shrinkMode = false;
    }

    public bool IsShrinkMode()
    {
        return shrinkMode;
    }

    public Vector3 GetPlayerPosition()
    {
        return map.transform.GetChild(currentLevel).GetChild(0).position;
    }
    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void CheckGameWin()
    {
        currTray++;
        if(currTray >= countTrays)
        {
            StartCoroutine(WinGame());
        }
    }

    public IEnumerator WinGame()
    {
        Debug.Log("Win");
        isGameOver = true;
        //winPanel.SetActive(true);
        int levelIndex = PlayerPrefs.GetInt("Level", 0) + 1;
        if (PlayerPrefs.GetInt("LevelUnlocked", 0) < levelIndex)
        {
            PlayerPrefs.SetInt("LevelUnlocked", levelIndex);
        }
        PlayerPrefs.SetInt("Level", levelIndex);
        PlayerPrefs.Save();
        yield return new WaitForSeconds(1f);

    }
    public IEnumerator LoseGame()
    {
        isGameOver = true;
        //losePanel.SetActive(true);
        yield return new WaitForSeconds(1f);

    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public void ReloadLevel()
    {
        PlayerPrefs.SetInt("Level", currentLevel);
        PlayerPrefs.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    public void NextLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

}