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

    float _edgeAngleSpacing;

    void Awake() {
        _meshCollider = GetComponent<MeshCollider>();

        verts = _meshCollider.sharedMesh.vertices;
        scale = transform.lossyScale;
    }

    void Start() {
        DeriveCoinGeometry(verts, scale);
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
}
