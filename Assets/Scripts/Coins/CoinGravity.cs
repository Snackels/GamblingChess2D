using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CoinGravity : MonoBehaviour {

    [SerializeField] float _gravity = 9.81f;

    Rigidbody _rigidbody;

    void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Start() {
        _rigidbody.maxAngularVelocity = 100f;
    }

    void FixedUpdate() {
        _rigidbody.AddForce(0f, 0f, _gravity * _rigidbody.mass, ForceMode.Force);
    }
}
