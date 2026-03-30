using UnityEngine;

public class AudioPitchController : MonoBehaviour {

    public AudioSource audioSource;

    [Header("Pitch Range")]
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;

    void Awake() {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayWithRandomPitch() {
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }
}