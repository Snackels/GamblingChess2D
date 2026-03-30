using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeTheScenes : MonoBehaviour
{
    public string SampleScene;
    public void ChangeScene()
    {
        SceneManager.LoadScene(SampleScene);
    }   
}
