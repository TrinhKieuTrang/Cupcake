using UnityEngine;
using UnityEngine.UI;
using System;

public class DailyRewardManager : MonoBehaviour
{
    [Header("UI Buttons")]
    private Button[] dayButtons;

    private const string LAST_CLAIM_DATE = "LastClaimDate";
    private const string DAY_INDEX = "DayIndex";

    private int currentDayIndex;
    private DateTime today;

    void Start()
    {
        today = DateTime.Now.Date;
        dayButtons = new Button[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            dayButtons[i] = transform.GetChild(i).GetComponent<Button>();
        }
        SetupUI();
    }


    void SetupUI()
    {
       
        string lastDateStr = PlayerPrefs.GetString(LAST_CLAIM_DATE, "");
        currentDayIndex = PlayerPrefs.GetInt(DAY_INDEX, 0);

        bool canClaimToday = false;

        if (string.IsNullOrEmpty(lastDateStr))
        {
            currentDayIndex = 0;
            canClaimToday = true;
        }
        else
        {
            DateTime lastDate = DateTime.Parse(lastDateStr);

            if (today > lastDate)
            {
                if ((today - lastDate).TotalDays == 1)
                {
                    currentDayIndex++;
                    if (currentDayIndex > 6) currentDayIndex = 0;
                }

                canClaimToday = true;
            }
        }

        for (int i = 0; i < dayButtons.Length; i++)
        {
            int index = i;
            dayButtons[i].interactable = false;
            dayButtons[i].onClick.RemoveAllListeners();
            if (i < currentDayIndex || (i == currentDayIndex && !canClaimToday))
            {
                dayButtons[i].interactable = true;
                dayButtons[i].enabled = false;
                dayButtons[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            if (i == currentDayIndex && canClaimToday)
            {
                dayButtons[i].interactable = true;
                dayButtons[i].onClick.AddListener(() => ClaimReward(index));
            }
        }

        PlayerPrefs.SetInt(DAY_INDEX, currentDayIndex);
        PlayerPrefs.Save();
    }

    void ClaimReward(int dayIndex)
    {
        GiveReward(dayIndex);

        PlayerPrefs.SetString(LAST_CLAIM_DATE, today.ToString());
        PlayerPrefs.Save();

        dayButtons[dayIndex].enabled = false;
        dayButtons[dayIndex].transform.GetChild(0).gameObject.SetActive(true);

    }

    void GiveReward(int dayIndex)
    {
        switch (dayIndex)
        {
            case 0: AddCoin(100); break;    
            case 1: AddItem("Freeze", 1); break; 
            case 2: AddCoin(150); break;     
            case 3: AddItem("Shrink", 1); break; 
            case 4: AddCoin(200); break;     
            case 5: AddItem("Mega", 1); break;   
            case 6: AddCoin(500); break;    
        }
    }

    void AddCoin(int amount)
    {
        int coin = PlayerPrefs.GetInt("Coin", 0);
        coin += amount;
        PlayerPrefs.SetInt("Coin", coin);
    }

    void AddItem(string key, int amount)
    {
        int current = PlayerPrefs.GetInt(key, 0);
        current += amount;
        PlayerPrefs.SetInt(key, current);
    }
}
