using System;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class CoinController : MonoBehaviour {

    const float SKINWIDTH = .02f;
    const float DISTANCEBETWEENRAYS = .6f;

    MeshCollider _meshCollider;
    Vector3[] verts;
    Vector3 scale;

    [SerializeField] float _rayLength = .5f;

    float _height;
    float _radius;
    Vector3 _faceAxis;
    Vector3 _edgeAxisX;
    Vector3 _edgeAxisZ;

    int _edgeRayCount;
    float _edgeAngleSpacing;

    void Awake() {
        _meshCollider = GetComponent<MeshCollider>();

        verts = _meshCollider.sharedMesh.vertices;
        scale = transform.lossyScale;
    }

    void Start() {
        DeriveCoinGeometry(verts, scale);
        CalculateRaySpacing();
    }

    void Update() {
        CastEdgeRays();
        CastRimRays();
        CastFaceRays();
    }

    void DeriveCoinGeometry(Vector3[] verts, Vector3 scale) {
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        float minZ = float.MaxValue, maxZ = float.MinValue;

        foreach (Vector3 vert in verts) {
            Vector3 scaledVert = Vector3.Scale(vert, scale);

            if (scaledVert.x < minX) minX = scaledVert.x;
            if (scaledVert.x > maxX) maxX = scaledVert.x;

            if (scaledVert.y < minY) minY = scaledVert.y;
            if (scaledVert.y > maxY) maxY = scaledVert.y;

            if (scaledVert.z < minZ) minZ = scaledVert.z;
            if (scaledVert.z > maxZ) maxZ = scaledVert.z;
        }

        float spanX = maxX - minX;
        float spanY = maxY - minY;
        float spanZ = maxZ - minZ;

        if (spanY <= spanX && spanY <= spanZ) {
            _faceAxis = transform.up;
            _edgeAxisX = transform.right;
            _edgeAxisZ = transform.forward;
            _height = spanY;
        }
        else if (spanX <= spanY && spanX <= spanZ) {
            _faceAxis = transform.right;
            _edgeAxisX = transform.up;
            _edgeAxisZ = transform.forward;
            _height = spanX;
        }
        else {
            _faceAxis = transform.forward;
            _edgeAxisX = transform.right;
            _edgeAxisZ = transform.up;
            _height = spanZ;
        }

        _radius = Mathf.Max(spanX, spanY, spanZ) * 0.5f;
    }

    void CalculateRaySpacing() {
        float circumference = 2 * Mathf.PI * (_radius - SKINWIDTH);
        _edgeRayCount = Mathf.Max(4, Mathf.RoundToInt(circumference / DISTANCEBETWEENRAYS));
        _edgeAngleSpacing = 360f / _edgeRayCount;
    }

    void CastEdgeRays() {
        Vector3 center = transform.position;
        float innerEdgeRadius = _radius - SKINWIDTH;

        for (int i = 0; i < _edgeRayCount; i++) {
            float angle = i * _edgeAngleSpacing * Mathf.Deg2Rad;
            Vector3 rayDirection = Mathf.Cos(angle) * _edgeAxisX + Mathf.Sin(angle) * _edgeAxisZ;
            Vector3 rayOrigin = center + rayDirection * innerEdgeRadius;

            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, _rayLength)) {
                Debug.Log("Edge hit: " + hit.collider.name);
            }

            Debug.DrawRay(rayOrigin, rayDirection * _rayLength, Color.green);
        }
    }

    void CastRimRays() {
        Vector3 center = transform.position;
        float halfHeight = (_height * 0.5f) - SKINWIDTH;
        float innerRim = _radius - SKINWIDTH;

        for (int i = 0; i < _edgeRayCount; i++) {
            float angle = i * _edgeAngleSpacing * Mathf.Deg2Rad;
            Vector3 rayDirection = Mathf.Cos(angle) * _edgeAxisX + Mathf.Sin(angle) * _edgeAxisZ;

            Vector3 originTop = center + rayDirection * innerRim + _faceAxis * halfHeight;
            Vector3 originBottom = center + rayDirection * innerRim - _faceAxis * halfHeight;

            Vector3 dirTop = (rayDirection + _faceAxis).normalized;
            Vector3 dirBottom = (rayDirection - _faceAxis).normalized;

            if (Physics.Raycast(originTop, dirTop, out RaycastHit hitTop, _rayLength))
                Debug.Log("Rim top hit: " + hitTop.collider.name);

            if (Physics.Raycast(originBottom, dirBottom, out RaycastHit hitBottom, _rayLength))
                Debug.Log("Rim bottom hit: " + hitBottom.collider.name);

            Debug.DrawRay(originTop, dirTop * _rayLength, Color.blue);
            Debug.DrawRay(originBottom, dirBottom * _rayLength, Color.blue);
        }
    }

    void CastFaceRays() {
        Vector3 center = transform.position;
        float halfHeight = (_height * 0.5f) - SKINWIDTH;
        float innerRim = _radius - SKINWIDTH;

        for (int i = 0; i < _edgeRayCount; i++) {
            float angle = i * _edgeAngleSpacing * Mathf.Deg2Rad;
            Vector3 rayDirection = Mathf.Cos(angle) * _edgeAxisX + Mathf.Sin(angle) * _edgeAxisZ;

            Vector3 originTop = center + rayDirection * innerRim + _faceAxis * halfHeight;
            Vector3 originBottom = center + rayDirection * innerRim - _faceAxis * halfHeight;

            if (Physics.Raycast(originTop, _faceAxis, out RaycastHit hitTop, _rayLength))
                Debug.Log("Face top hit: " + hitTop.collider.name);

            if (Physics.Raycast(originBottom, -_faceAxis, out RaycastHit hitBottom, _rayLength))
                Debug.Log("Face bottom hit: " + hitBottom.collider.name);

            Debug.DrawRay(originTop, _faceAxis * _rayLength, Color.yellow);
            Debug.DrawRay(originBottom, -_faceAxis * _rayLength, Color.yellow);
        }
    }
}
