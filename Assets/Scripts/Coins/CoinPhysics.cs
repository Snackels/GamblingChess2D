using System;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshCollider))]
public class CoinPhysics : MonoBehaviour {

    Rigidbody _rigidbody;
    MeshCollider _meshCollider;

    Vector3[] verts;
    Vector3 scale;

    float _height;
    float _radius;

    Vector3 _faceNormal;
    Vector3 _floorNormal;
    float _I1;
    float _I3;

    bool _isGrounded;
    bool _justBounced;

    [SerializeField] float _restitution = 0.6f;
    [SerializeField] float _friction = 0.4f;
    [SerializeField] float _gravity = 9.81f;
    [SerializeField] float _maxAngularVelocity = 50f;
    [SerializeField] float _maxTiltDelta = 10f;

    [SerializeField] float _wobbleDuration = 1.5f;
    float _wobbleStartTime;
    float _initialTiltAngle;
    float _initialSpin;

    Vector3 _precessionAxis;

    enum State {
        AirBorne,
        Bouncing,
        Grounded,
        EulerWobble,
        Settled
    };

    State _state;

    void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _meshCollider = GetComponent<MeshCollider>();
        verts = _meshCollider.sharedMesh.vertices;
        scale = transform.lossyScale;
    }

    void Start() {
        DeriveCoinGeometry(verts, scale);

        _rigidbody.maxAngularVelocity = _maxAngularVelocity;
        _rigidbody.useGravity = false;

        _faceNormal = transform.right;

        _I1 = (0.25f * _rigidbody.mass * _radius * _radius) + (1f / 12f * _rigidbody.mass * _height * _height);
        _I3 = 0.5f * _rigidbody.mass * _radius * _radius;

        _rigidbody.inertiaTensor = new Vector3(_I1, _I1, _I3);
        _rigidbody.inertiaTensorRotation = Quaternion.identity;

        Debug.Log($"radius: {_radius:F4} | height: {_height:F4} | mass: {_rigidbody.mass:F4} | I1: {_I1:F6} | I3: {_I3:F6}");
    }

    void FixedUpdate() {
        if (_justBounced) {
            _justBounced = false;
            Gravity();
            return;
        }

        if (!_isGrounded) {
            Vector3 omegaSpin = Vector3.Dot(_rigidbody.angularVelocity, _faceNormal) * _faceNormal;
            Vector3 omegaTilt = _rigidbody.angularVelocity - omegaSpin;

            Vector3 angularMomentum = _I1 * omegaTilt + _I3 * omegaSpin;

            float omegaN = angularMomentum.magnitude / _I1;
            Vector3 mHat = angularMomentum.normalized;

            Vector3 dNdt = omegaN * Vector3.Cross(mHat, _faceNormal);
            _faceNormal = (_faceNormal + dNdt * Time.fixedDeltaTime).normalized;

            Quaternion deltaRotation = Quaternion.FromToRotation(transform.right, _faceNormal);
            _rigidbody.MoveRotation(deltaRotation * _rigidbody.rotation);
        }
        Gravity();
    }

    void UpdateState() {
        switch (_state) {
            case State.AirBorne:
                AirBorne();
                break;
        }
    }

    void AirBorne() {
        throw new NotImplementedException();
    }

    void EnterEulerWobble() {
        _initialSpin = Vector3.Dot(_faceNormal, _rigidbody.angularVelocity);

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        _initialTiltAngle = Mathf.Acos(Vector3.Dot(_faceNormal, _floorNormal));
        _wobbleStartTime = Time.time;
        _precessionAxis = _floorNormal;
    }

    void EulerWobble() {
        float t = Time.time - _wobbleStartTime;
        float alpha = _initialTiltAngle * Mathf.Pow(1 - Mathf.Clamp01(t / _wobbleDuration), 0.333f);

        if (alpha < Mathf.Deg2Rad * 0.5f) {
            //transition into Settled State
            return;
        }

        float omega = Mathf.Sqrt(_gravity * Mathf.Cos(alpha) / (_radius * Mathf.Sin(alpha)));

        Quaternion spin = Quaternion.AngleAxis(Mathf.Rad2Deg * omega * Time.fixedDeltaTime, _floorNormal);
        _precessionAxis = spin * _precessionAxis;

        Vector3 perpAxis = Vector3.Cross(_precessionAxis, Vector3.right);
        Quaternion tilt = Quaternion.AngleAxis(Mathf.Rad2Deg * alpha, perpAxis);
        _faceNormal = tilt * _precessionAxis;

        Quaternion deltaRotation = Quaternion.FromToRotation(transform.right, _faceNormal);
        _rigidbody.MoveRotation(deltaRotation * _rigidbody.rotation);
    }

    void OnCollisionStay(Collision collision) {
        foreach (ContactPoint contact in collision.contacts) {
            _floorNormal = contact.normal;
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (_justBounced) return;
        _justBounced = true;
        _isGrounded = true;

        ContactPoint contact = collision.contacts[0];
        float separation = contact.separation;
        if (separation < 0f)
            _rigidbody.position -= contact.normal * separation;

        float e = _restitution;
        float mu = _friction;

        Vector3 floorNormal = collision.contacts[0].normal;
        float vn_minus = Vector3.Dot(_rigidbody.linearVelocity, floorNormal);
        Vector3 vn_vec = vn_minus * floorNormal;
        Vector3 vt_vec = _rigidbody.linearVelocity - vn_vec;

        Vector3 vn_plus = -e * vn_vec;

        float frictionDelta = mu * (1f + e) * Mathf.Abs(vn_minus);
        Vector3 vt_plus = vt_vec - frictionDelta * vt_vec.normalized;
        if (Vector3.Dot(vt_plus, vt_vec) < 0f)
            vt_plus = Vector3.zero;

        Vector3 oldVelocity = _rigidbody.linearVelocity;
        Vector3 newVelocity = vn_plus + vt_plus;
        _rigidbody.linearVelocity = newVelocity;

        float omega_minus = Vector3.Dot(_rigidbody.angularVelocity, _faceNormal);
        float signVt = vt_vec.magnitude > 0.001f ? Mathf.Sign(Vector3.Dot(vt_vec, Vector3.Cross(floorNormal, _faceNormal))) : 0f;
        float omega_plus = omega_minus + 2.5f * frictionDelta / _radius * signVt;
        _rigidbody.angularVelocity += (omega_plus - omega_minus) * _faceNormal;

        Vector3 contactPoint = collision.contacts[0].point;
        Vector3 r_contact = contactPoint - transform.position;

        Vector3 r_clamped;
        if (r_contact.magnitude < 0.001f)
            r_clamped = Vector3.zero;
        else
            r_clamped = r_contact.normalized * Mathf.Min(r_contact.magnitude, _radius);

        if (r_clamped.sqrMagnitude > 0f) {
            Vector3 J_vec = _rigidbody.mass * (newVelocity - oldVelocity);
            Vector3 angMomentumDelta = Vector3.Cross(r_clamped, J_vec);
            Vector3 spinComponent = Vector3.Dot(angMomentumDelta, _faceNormal) * _faceNormal;
            Vector3 tiltComponent = angMomentumDelta - spinComponent;
            Vector3 angVelDelta = tiltComponent / _I1;

            if (angVelDelta.magnitude > _maxTiltDelta)
                angVelDelta = angVelDelta.normalized * _maxTiltDelta;

            _rigidbody.angularVelocity += angVelDelta;

            Debug.Log($"angMomDelta: {angMomentumDelta.magnitude:F4} | " +
                      $"tiltComp: {tiltComponent.magnitude:F4} | " +
                      $"angVelDelta: {angVelDelta.magnitude:F4}");
        }
    }

    void OnCollisionExit(Collision collision) {
        _isGrounded = false;
    }

    void DeriveCoinGeometry(Vector3[] verts, Vector3 scale) {
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        float minZ = float.MaxValue, maxZ = float.MinValue;

        foreach (Vector3 vert in verts) {
            Vector3 sv = Vector3.Scale(vert, scale);
            if (sv.x < minX) minX = sv.x;
            if (sv.x > maxX) maxX = sv.x;
            if (sv.y < minY) minY = sv.y;
            if (sv.y > maxY) maxY = sv.y;
            if (sv.z < minZ) minZ = sv.z;
            if (sv.z > maxZ) maxZ = sv.z;
        }

        float spanX = maxX - minX;
        float spanY = maxY - minY;
        float spanZ = maxZ - minZ;

        if (spanY <= spanX && spanY <= spanZ) _height = spanY;
        else if (spanX <= spanY && spanX <= spanZ) _height = spanX;
        else _height = spanZ;

        _radius = Mathf.Max(spanX, spanZ) * 0.5f;
    }

    void Gravity() {
        _rigidbody.AddForce(0f, 0f, _gravity * _rigidbody.mass, ForceMode.Force);
    }

    void OnDrawGizmos() {
        if (!Application.isPlaying) return;
        Debug.DrawRay(transform.position, _faceNormal * 0.5f, Color.red);
        Debug.DrawRay(transform.position, transform.right * 0.5f, Color.blue);
    }
}