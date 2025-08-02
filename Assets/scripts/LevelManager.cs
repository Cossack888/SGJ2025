using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public GameObject Menu;

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
        if (Menu != null)
            CloseMenu();
    }

    public void OpenMenu()
    {
        Menu.SetActive(true);
    }
    public void CloseMenu()
    {
        Menu?.SetActive(false);
    }
}