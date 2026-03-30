using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CoinSFX : MonoBehaviour {

    [Header("Collision SFX")]
    public AudioClip[] wallHitClips;
    public AudioClip floorHitClip;

    [Header("Tuning")]
    public float minImpactSpeed = 0.8f;
    public float cooldown = 0.08f;
    public string floorWallName = "3D_Objects Boundary";

    [Header("Pitch Randomization")]
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    AudioSource _audio;
    float _lastPlayTime = -999f;

    void Awake() {
        _audio = GetComponent<AudioSource>();
        _audio.playOnAwake = false;
        _audio.spatialBlend = 0f;
    }

    void OnCollisionEnter(Collision col) {
        float speed = col.relativeVelocity.magnitude;
        if (speed < minImpactSpeed) return;
        if (Time.time - _lastPlayTime < cooldown) return;

        bool isFloor = col.gameObject.name == floorWallName;

        AudioClip clip = null;

        if (isFloor) {
            clip = floorHitClip;
        }
        else if (wallHitClips != null && wallHitClips.Length > 0) {
            clip = wallHitClips[Random.Range(0, wallHitClips.Length)];
        }

        if (clip == null) return;

        float vol = Mathf.Clamp01(speed / 8f);
        _audio.pitch = Random.Range(minPitch, maxPitch);
        _audio.PlayOneShot(clip, vol);
        _lastPlayTime = Time.time;
    }
}