using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private GameObject menuRoot;
    [SerializeField] private List<Button> menuButtons;
    [SerializeField] private List<Button> optionButtons;
    [SerializeField] private List<Button> creditsButtons;
    [SerializeField] private List<Button> deathMenuButtons;
    [SerializeField] private List<Button> winMenuButtons;

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject deathMenu;
    [SerializeField] private GameObject winMenu;
    public GameObject ActiveMenu;
    public static MenuManager Instance { get; private set; }

    private int currentIndex = 0;
    private PlayerInputHandler input;
    public List<Button> activeButtonList;
    public Button buttonActive;
    public PlayerStatTracker playerStatTracker;
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

    public bool IsTheDeathOrWinMenu()
    {
        if (ActiveMenu == deathMenu || ActiveMenu == winMenu)
        {
            return true;
        }
        return false;
    }

    private void OnDisable()
    {
        if (input == null) return;

        input.NavigateUp -= OnNavigateUp;
        input.NavigateDown -= OnNavigateDown;
        input.MenuSubmit -= OnSubmit;
    }

    public void SetStatTracker(PlayerStatTracker statTracker)
    {
        playerStatTracker = statTracker;
        playerStatTracker.LoseGame += ActivateDeathScreen;
    }

    public void AssignControllingInput(PlayerInputHandler playerInput)
    {
        if (input != null)
        {
            input.NavigateUp -= OnNavigateUp;
            input.NavigateDown -= OnNavigateDown;
            input.MenuSubmit -= OnSubmit;
        }

        input = playerInput;
        input.NavigateUp += OnNavigateUp;
        input.NavigateDown += OnNavigateDown;
        input.MenuSubmit += OnSubmit;

        currentIndex = 0;
        HighlightCurrentButton();
    }

    // === PANEL SWITCHING ===
    public void SetActivePanel(string panelName)
    {

        CloseAllPanels();
        ActiveMenu = null;
        Time.timeScale = 0f;
        switch (panelName.ToLower())
        {
            case "menu":
                activeButtonList = menuButtons;
                menuPanel.SetActive(true);
                ActiveMenu = menuPanel;
                break;
            case "options":
                activeButtonList = optionButtons;
                optionsPanel.SetActive(true);
                ActiveMenu = optionsPanel;
                break;
            case "credits":
                activeButtonList = creditsButtons;
                creditsPanel.SetActive(true);
                ActiveMenu = creditsPanel;
                break;
            case "death":
                activeButtonList = deathMenuButtons;
                deathMenu.SetActive(true);
                deathMenu.GetComponent<DeathScreen>().Init();
                ActiveMenu = deathMenu;
                break;
            case "win":
                activeButtonList = winMenuButtons;
                winMenu.SetActive(true);
                winMenu.GetComponent<WinScreen>().Init();
                ActiveMenu = winMenu;
                break;
            default:
                activeButtonList = menuButtons;
                menuPanel.SetActive(true);
                ActiveMenu = menuPanel;
                break;
        }

        currentIndex = 0;
        HighlightCurrentButton();
    }

    private void OnNavigateUp()
    {
        if (activeButtonList == null || activeButtonList.Count == 0) return;

        currentIndex = (currentIndex - 1 + activeButtonList.Count) % activeButtonList.Count;
        HighlightCurrentButton();
    }

    private void OnNavigateDown()
    {
        if (activeButtonList == null || activeButtonList.Count == 0) return;

        currentIndex = (currentIndex + 1) % activeButtonList.Count;
        HighlightCurrentButton();
    }

    private void HighlightCurrentButton()
    {
        if (activeButtonList == null || activeButtonList.Count == 0) return;

        foreach (Button b in activeButtonList)
        {
            if (b != null && b.image != null)
                b.image.color = new Color(0f, 0f, 0f, 0.2f); ;
        }

        EventSystem.current.SetSelectedGameObject(null);
        Button button = activeButtonList[currentIndex];
        buttonActive = button;
        button.Select();
        if (button.image != null)
            button.image.color = new Color(1f, 0f, 0f, 0.2f);
    }

    private void OnSubmit()
    {
        if (activeButtonList == null || activeButtonList.Count == 0) return;

        // Button button = activeButtonList[currentIndex];
        buttonActive.onClick.Invoke();
    }
    public void CloseAllPanels()
    {
        menuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        deathMenu.SetActive(false);
        winMenu.SetActive(false);
    }

    public void ActivateDeathScreen()
    {
        PlayerInputHandler input = FindFirstObjectByType<PlayerInputHandler>();
        AssignControllingInput(input);
        SetActivePanel("death");
    }

    public void ActivateWinScreen()
    {
        SetActivePanel("win");
    }


}
