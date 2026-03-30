using UnityEngine;

/// <summary>
/// Add to the same GameObject as ChessPieces.
/// Assign clips in the Inspector — any empty slot is silently skipped.
/// </summary>
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ChessPieces))]
public class ChessPieceSFX : MonoBehaviour {

    [Header("Interaction Clips")]
    public AudioClip hoverClip;
    public AudioClip clickClip;
    public AudioClip doubleClickClip;
    public AudioClip holdCompleteClip;

    [Header("Drag & Drop Clips")]
    public AudioClip pickUpClip;
    public AudioClip placedClip;
    public AudioClip invalidPlacementClip;

    [Header("Pitch Randomization")]
    public float minPitch = 0.92f;
    public float maxPitch = 1.08f;

    [Header("Volume")]
    [Range(0f, 1f)] public float sfxVolume = 1f;

    AudioSource _audio;
    ChessPieces _piece;

    void Awake() {
        _audio = GetComponent<AudioSource>();
        _audio.playOnAwake = false;
        _audio.spatialBlend = 0f;
        _piece = GetComponent<ChessPieces>();
    }

    void Start() {
        _piece.PointerEnterEvent.AddListener(p => Play(hoverClip));
        _piece.OnHoldComplete += p => Play(holdCompleteClip);
    }

    void OnDestroy() {
        if (_piece == null) return;
        _piece.PointerEnterEvent.RemoveAllListeners();
        _piece.OnHoldComplete -= p => Play(holdCompleteClip);
    }

    // ── Called directly from ChessPieces ─────────────────────────────────────

    public void PlayPickUp() => Play(pickUpClip);
    public void PlayPlaced() => Play(placedClip);
    public void PlayInvalid() => Play(invalidPlacementClip);

    // ── Called from ChessPieces.OnPointerUp ──────────────────────────────────

    float _lastClickSoundTime = -1f;

    public void HandlePointerUp(float pointerDownTime) {
        float now = Time.time;
        if (now - pointerDownTime >= 0.2f) return;
        if (_piece.wasDragged) return;
        if (_piece.mCurrentCell == null) return;

        float doubleClickWindow = 0.3f;
        if (now - _lastClickSoundTime <= doubleClickWindow) {
            _lastClickSoundTime = -1f;
            Play(doubleClickClip);
        }
        else {
            _lastClickSoundTime = now;
            Play(clickClip);
        }
    }

    // ── Playback ──────────────────────────────────────────────────────────────

    void Play(AudioClip clip) {
        if (clip == null || _audio == null) return;
        _audio.pitch = Random.Range(minPitch, maxPitch);
        _audio.PlayOneShot(clip, sfxVolume);
    }
}