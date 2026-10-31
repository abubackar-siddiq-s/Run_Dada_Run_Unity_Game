using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameMenu;
    public GameObject mainMenu;
    public GameObject gameOverMenu;
    public GameObject winMenu;

    private static bool replaying = false;

    void Start()
    {
        if (replaying)
        {
            replaying = false;

            mainMenu.SetActive(false);
            gameMenu.SetActive(true);
            gameOverMenu.SetActive(false);
            winMenu.SetActive(false);

            Time.timeScale = 1f;
            LockCursor();
        }
        else
        {
            mainMenu.SetActive(true);
            gameMenu.SetActive(false);
            gameOverMenu.SetActive(false);
            winMenu.SetActive(false);

            Time.timeScale = 0f;
            UnlockCursor();
        }
    }

    
public void StartGame()
{
    // Reload the scene to reset the player and game state.
    replaying = true;
    Time.timeScale = 1f;

    SceneManager.LoadScene(
        SceneManager.GetActiveScene().buildIndex
    );
}

    public void ShowGameOverMenu()
    {
        gameMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        winMenu.SetActive(false);

        Time.timeScale = 0f;
        UnlockCursor();
    }


public void ShowWinMenu()
{
    gameMenu.SetActive(false);
    gameOverMenu.SetActive(false);
    mainMenu.SetActive(false);
    winMenu.SetActive(true);

    Time.timeScale = 0f;
    UnlockCursor();
}

    public void ReplayGame()
    {
        replaying = true;
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}