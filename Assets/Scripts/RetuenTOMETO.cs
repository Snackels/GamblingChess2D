using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnTOMETO : MonoBehaviour
{
    public string MainMenuScene;
    public void ChangeScene()
    {
        SceneManager.LoadScene(MainMenuScene);
    }   
}
