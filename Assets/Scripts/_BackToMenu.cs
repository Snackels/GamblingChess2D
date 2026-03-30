using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    public string MainMenu;
    public void ChangeScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
