
using UnityEngine;
using UnityEngine.UI;

public class MuteMusicInMenu : MonoBehaviour
{
    [SerializeField] Image soundOnIcon;
    [SerializeField] Image soundOffIcon;
    private bool muted = false;

    void Start() 
    {
        if (!PlayerPrefs.HasKey("Muted")) {
            PlayerPrefs.SetInt("Muted", 0);
        }
        else {
            Load();
        }
    }

    public void OnbuttonPress()
    {
        if (muted == false) {
            muted = true;
            AudioListener.pause = true;
        }
        else {
            muted = false;
            AudioListener.pause = false;
        }

        Save();
        UpdateIcon(); // ✅ fixed name
    }

    private void Load() 
    {
        muted = PlayerPrefs.GetInt("Muted", 0) == 1;
        AudioListener.pause = muted;
        UpdateIcon(); // ✅ fixed name
    }

    private void Save() 
    {
        PlayerPrefs.SetInt("Muted", muted ? 1 : 0);
    }

    private void UpdateIcon() 
    {
        if (muted) {
            soundOnIcon.enabled = false;
            soundOffIcon.enabled = true;
        }
        else {
            soundOnIcon.enabled = true;
            soundOffIcon.enabled = false;
        }
    }
}
