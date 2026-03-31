using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    [Header("References")]
    [SerializeField] GameObject pausePopup;
    [SerializeField] string mainMenuScene = "MainMenu";
    [SerializeField] string gameScene = "SampleScene";

    bool isPaused = false;

    void Start() {
        pausePopup.SetActive(false);
    }

    public void OnPauseButtonPressed() {
        isPaused = !isPaused;

        pausePopup.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void Restart() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameScene);
    }

    public void GoToMainMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }
}
