using UnityEngine;

/// <summary>
/// Attach to the coin PREFAB.
///
/// Unity's built-in gravity pulls in -Y. Our coins fall in +Z.
/// This component applies a constant +Z force each FixedUpdate.
///
/// IMPORTANT: On the coin's Rigidbody set Use Gravity = OFF.
/// This script is the coin's only gravity source.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CoinGravity : MonoBehaviour {
    [Tooltip("Gravity acceleration in +Z direction (world units per second squared)")]
    public float gravity = 15f;

    private Rigidbody _rb;

    private void Awake() => _rb = GetComponent<Rigidbody>();

    private void FixedUpdate() {
        _rb.AddForce(0f, 0f, gravity * _rb.mass, ForceMode.Force);
    }
}


