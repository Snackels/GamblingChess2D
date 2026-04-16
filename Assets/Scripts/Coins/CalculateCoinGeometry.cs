using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class CalculateCoinGeometry : MonoBehaviour {

    public float Radius { get; private set; }
    public float Height { get; private set; }

    Vector3[] _verts;
    Vector3 _scale;

    MeshCollider _meshCollider;

    void Awake() {
        _meshCollider = GetComponent<MeshCollider>();
        _verts = _meshCollider.sharedMesh.vertices;
        _scale = transform.lossyScale;
    }

    void Start() {
        DeriveCoinGeometry(_verts, _scale);
        Debug.Log("Radius :" + Radius + " Height :" + Height);
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

        if (spanY <= spanX && spanY <= spanZ) Height = spanY;
        else if (spanX <= spanY && spanX <= spanZ) Height = spanX;
        else Height = spanZ;

        Radius = Mathf.Max(spanX, spanZ) * 0.5f;
    }
}
