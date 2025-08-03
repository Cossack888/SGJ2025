using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{

    [SerializeField] private Canvas MainMenuCanvas;
    [SerializeField] private Canvas OptionsCanvas;
    [SerializeField] private Canvas CreditsCanvas;
    public void ChangeScene(string levelName)
    {
        // Only specifying the sceneName or sceneBuildIndex will load the Scene with the Single mode
        SceneManager.LoadScene(levelName);
        Time.timeScale = 1f;
        MenuManager.Instance.CloseAllPanels();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenOptionsWindow()
    {
        MainMenuCanvas.gameObject.SetActive(false);
        OptionsCanvas.gameObject.SetActive(true);
    }

    public void OpenCreditsWindow()
    {
        MainMenuCanvas.gameObject.SetActive(false);
        CreditsCanvas.gameObject.SetActive(true);
    }

    public void CloseNonMain()
    {
        if (!MainMenuCanvas.gameObject.activeSelf)
        {
            MainMenuCanvas.gameObject.SetActive(true);
        }

        if (OptionsCanvas.gameObject.activeSelf)
        {
            OptionsCanvas.gameObject.SetActive(false);
        }

        if (CreditsCanvas.gameObject.activeSelf)
        {
            CreditsCanvas.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseNonMain();
        }
    }
}
