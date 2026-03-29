using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loadin_Transition : MonoBehaviour
{

    public Animator Transition;
    public float transitionTime = 1f;   
        void Update()
    {
        if(Input.GetMouseButtonDown(0)) 
        {
            LoadNextLevel();
        }
    }
    public void LoadNextLevel() 
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int LevelIndex) 
    {
        Transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(LevelIndex);
    }
}
