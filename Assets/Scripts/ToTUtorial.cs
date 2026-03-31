using UnityEngine;
using UnityEngine.SceneManagement;

public class TOUtorial : MonoBehaviour
{
    public string TutorialScene;
    public void ChangeScene()
    {
        SceneManager.LoadScene(TutorialScene);
    }   
}
