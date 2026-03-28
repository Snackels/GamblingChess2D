using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CoinFlattener : MonoBehaviour {
    [Header("Flat Detection")]
    [Range(0f, 1f)]
    public float flatDotThreshold = 0.85f;

    [Header("Floor Detection")]
    public float floorZ = 8f;

    public float floorTolerance = 0.3f;

    [Header("Settle Detection")]
    public float settleSpeedThreshold = 0.3f;

    public float settleWaitTime = 0.4f;

    [Header("Bounce")]
    public float bounceForce = 4f;

    public float bounceTorque = 6f;

    public int maxBounceAttempts = 5;

    Rigidbody _rb;
    float _slowTimer;
    int _attempts;
    bool _settled;

    void Awake() => _rb = GetComponent<Rigidbody>();

    void FixedUpdate() {
        if (_settled) return;

        bool isSlow = _rb.linearVelocity.magnitude < settleSpeedThreshold &&
                      _rb.angularVelocity.magnitude < settleSpeedThreshold;

        _slowTimer = isSlow ? _slowTimer + Time.fixedDeltaTime : 0f;

        if (_slowTimer < settleWaitTime) return;
        _slowTimer = 0f;

        bool flat = IsFlat();
        bool onFloor = IsOnFloor();

        if (flat && onFloor) {
            _settled = true;
            return;
        }

        if (_attempts >= maxBounceAttempts) {
            ForceFlat();
            _settled = true;
            return;
        }

        Bounce();
        _attempts++;
    }


    bool IsFlat() {
        float dot = Mathf.Abs(Vector3.Dot(transform.right, Vector3.forward));
        return dot >= flatDotThreshold;
    }

    bool IsOnFloor() {
        return Mathf.Abs(transform.position.z - floorZ) <= floorTolerance;
    }


    void Bounce() {
        _rb.WakeUp();
        _rb.AddForce(0f, 0f, -bounceForce, ForceMode.Impulse);
        _rb.AddTorque(Random.onUnitSphere * bounceTorque, ForceMode.Impulse);
    }

    void ForceFlat() {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        bool heads = Random.value > 0.5f;
        transform.rotation = Quaternion.Euler(0f, heads ? 90f : -90f, 0f);

        Vector3 pos = transform.position;
        pos.z = floorZ;
        transform.position = pos;

        _rb.Sleep();
    }
}