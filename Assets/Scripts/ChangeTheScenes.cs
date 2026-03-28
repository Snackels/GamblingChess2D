using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeTheScenes : MonoBehaviour
{
    
    public void ChangeScene()
    {
        SceneManager.LoadScene("SampleScene");
    }   
}
