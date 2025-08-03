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
    public GameObject SplashScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        Time.timeScale = 0f;
        SplashScreen.SetActive(true);
    }

    private void UpdateUI()
    {
        pointsText.text = points.ToString();
        healthText.text = playerStatTracker.PlayerHealth.ToString();
    }
    public void StartGame()
    {
        Time.timeScale = 1f;
        SplashScreen.SetActive(false);
        FireWallMover fireWallMover = FindAnyObjectByType<FireWallMover>();
        fireWallMover.isActive = true;
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

    public void OpenMenu()
    {

        Menu.SetActive(true);
        MenuManager.Instance.SetActivePanel("menu");


    }
    public void CloseMenu()
    {
        if (!MenuManager.Instance.IsTheDeathOrWinMenu())
        {
            MenuManager.Instance.CloseAllPanels();
            Time.timeScale = 1f;
        }
    }
}