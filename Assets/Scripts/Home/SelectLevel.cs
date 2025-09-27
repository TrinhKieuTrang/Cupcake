using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SelectLevel : MonoBehaviour
{
    [SerializeField] private GameObject[] levels;
    [SerializeField] private Sprite bgLock, bgDone;

    private void Awake()
    {
        SetUpLevel();
    }

    private void SetUpLevel()
    {
        levels = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            levels[i] = transform.GetChild(i).gameObject;
        }
        for (int i = 0; i < levels.Length; i++)
        {
            int indexLevel = i;

            levels[i].transform.GetChild(0).GetComponentInChildren<Text>().text = (i + 1).ToString();

            if (i < PlayerPrefs.GetInt("LevelUnlocked", 0))
            {
                SetLevelDone(i);
            }
            else if (i == PlayerPrefs.GetInt("LevelUnlocked", 0))
            {
                SetLevelPick(i);
            }
            else
            {
                SetLevelLock(i);
            }
        }
    }


    private void SetLevelPick(int levelIndex)
    {
        Button btn = levels[levelIndex].AddComponent<Button>();
        btn.GetComponent<Button>().onClick.AddListener(() =>
        {
            SelectLevelGame(levelIndex);
        });

    }

    private void SetLevelDone(int levelIndex)
    {
        levels[levelIndex].GetComponent<Image>().sprite = bgDone;

        GameObject done = levels[levelIndex].transform.GetChild(0).GetChild(1).gameObject;
        
        done.SetActive(true);

        SetLevelPick(levelIndex);

    }

    private void SetLevelLock(int levelIndex)
    {
        GameObject text = levels[levelIndex].transform.GetChild(0).gameObject;
        text.SetActive(false);

        levels[levelIndex].GetComponent<Image>().sprite = bgLock;
    }
    private void SelectLevelGame(int levelIndex)
    {
        PlayerPrefs.SetInt("Level", levelIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainGame");
    }
}