using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour {
    [Header("References")]
    [SerializeField] GameObject gameOverPopup;
    [SerializeField] string mainMenuScene = "MainMenu";
    [SerializeField] string gameScene = "MainGame";

    [SerializeField] AudioSource buttonAudioSource;
    [SerializeField] AudioClip buttonClickSound;

    bool isGameOver = false;

    void Start() {
        gameOverPopup.SetActive(false);
    }

    public void GameOver() {
        if (isGameOver) return;
        isGameOver = true;
        gameOverPopup.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart() {
        PlayButtonSound();
        isGameOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameScene);
    }

    public void GoToMainMenu() {
        PlayButtonSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void PlayButtonSound() {
        if (buttonAudioSource != null && buttonClickSound != null)
            buttonAudioSource.PlayOneShot(buttonClickSound);
    }
}
