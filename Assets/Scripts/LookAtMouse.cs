using UnityEngine;
using UnityEngine.InputSystem;

public class LookAtMouse : MonoBehaviour {
    [Header("Rotation Limits (degrees)")]
    [SerializeField] float maxYaw = 60f;
    [SerializeField] float maxPitch = 35f;

    [Header("Sensitivity")]
    [Tooltip("How far (in world units) the mouse needs to be from the head to reach max rotation.")]
    [SerializeField] float lookRadius = 3f;

    [Header("Smoothing")]
    [Tooltip("Lower = lazier/floaty like MCPE. 6-10 is a good range.")]
    [SerializeField] float smoothSpeed = 8f;

    Camera mainCam;
    Quaternion targetRotation;

    void Awake() {
        mainCam = Camera.main;
        targetRotation = transform.localRotation;
    }

    void Update() {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(
            new Vector3(mouseScreen.x, mouseScreen.y, -mainCam.transform.position.z));

        // Offset from head to mouse, clamped to lookRadius so it doesn't overshoot
        float dx = Mathf.Clamp(mouseWorld.x - transform.position.x, -lookRadius, lookRadius);
        float dy = Mathf.Clamp(mouseWorld.y - transform.position.y, -lookRadius, lookRadius);

        // Map -lookRadius..lookRadius → -max..max degrees
        float yaw = (dx / lookRadius) * maxYaw;
        float pitch = -(dy / lookRadius) * maxPitch;

        targetRotation = Quaternion.Euler(pitch, yaw, 0f);

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            1f - Mathf.Exp(-smoothSpeed * Time.deltaTime)
        );
    }
}