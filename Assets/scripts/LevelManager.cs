using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public GameObject Menu;
    public PlayerStatTracker playerStatTracker;
    public int points;
    public TMP_Text pointsText;
    public TMP_Text healthText;

    private void Awake()
    {
        Time.timeScale = 1f;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }
    private void UpdateUI()
    {
        pointsText.text = points.ToString();
        healthText.text = playerStatTracker.PlayerHealth.ToString();
    }

    public void GainPoints(int point)
    {
        points += point;
        UpdateUI();
    }
    public void SetStatTracker(PlayerStatTracker statTracker)
    {
        playerStatTracker = statTracker;
        UpdateUI();
    }


    private void Start()
    {
        if (Menu != null)
            CloseMenu();
    }

    public void OpenMenu()
    {
        //Menu.SetActive(true);
        MenuManager.Instance.SetActivePanel("menu");
    }
    public void CloseMenu()
    {
        if (!MenuManager.Instance.IsTheDeathOrWinMenu())
        {
            MenuManager.Instance.CloseAllPanels();
            Time.timeScale = 1f;

            MenuManager.Instance.activeButtonList = null; // resetuj listê
            MenuManager.Instance.currentIndex = 0;
        }
    }
}