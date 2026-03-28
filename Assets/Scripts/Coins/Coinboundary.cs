using UnityEngine;

public class CoinBoundary : MonoBehaviour {
    [Header("Boundary XY Size")]
    public Vector3 center = Vector3.zero;

    public Vector2 size = new Vector2(6f, 4f);

    [Header("Z Depth")]
    public float frontZ = 0f;

    public float floorZ = 8f;

    [Header("Wall Settings")]
    public float wallThickness = 0.2f;

    public PhysicsMaterial wallMaterial;

    public int coinBoundaryLayer = 7;

    [Header("Starting State")]
    public bool startOpen = false;

    [Header("Gizmos")]
    public Color fillColor = new Color(0.2f, 0.85f, 1f, 0.12f);
    public Color wireColor = new Color(0.2f, 0.85f, 1f, 0.85f);
    public Color floorColor = new Color(1f, 0.55f, 0.1f, 0.9f);
    public Color frontColor = new Color(0.2f, 1f, 0.4f, 0.9f);
    public bool showLabels = true;

    GameObject _root;
    BoxCollider _left, _right, _top, _bottom, _floor, _front;
    bool _isOpen;

    void Awake() { }

    void Start() {
        Build();
        SetOpen(startOpen);
    }

    void OnValidate() {
        if (_floor != null) Refresh();
    }

    public void SetOpen(bool open) {
        _isOpen = open;
        if (_root != null) _root.SetActive(open);
    }

    public void Toggle() => SetOpen(!_isOpen);

    public Vector3 ClampInsideXY(Vector3 pos) {
        float margin = wallThickness + 0.1f;
        pos.x = Mathf.Clamp(pos.x,
            center.x - size.x * 0.5f + margin,
            center.x + size.x * 0.5f - margin);
        pos.y = Mathf.Clamp(pos.y,
            center.y - size.y * 0.5f + margin,
            center.y + size.y * 0.5f - margin);
        return pos;
    }

    void Build() {
        _root = new GameObject("_CoinBoundaryWalls");
        _root.transform.SetParent(transform);

        _left = MakeWall("Wall_L");
        _right = MakeWall("Wall_R");
        _top = MakeWall("Wall_Top");
        _bottom = MakeWall("Wall_Bot");
        _floor = MakeWall("Wall_Floor");
        _front = MakeWall("Wall_Front");

        Refresh();
    }

    BoxCollider MakeWall(string n) {
        var go = new GameObject(n);
        go.transform.SetParent(_root.transform);
        go.layer = coinBoundaryLayer;
        var col = go.AddComponent<BoxCollider>();
        if (wallMaterial != null) col.material = wallMaterial;
        return col;
    }

    void Refresh() {
        float hw = size.x * 0.5f;
        float hh = size.y * 0.5f;
        float cx = center.x;
        float cy = center.y;
        float depth = (floorZ - frontZ) + wallThickness * 2f;
        float midZ = frontZ + depth * 0.5f;
        float t = wallThickness;

        Place(_left, new Vector3(cx - hw - t * 0.5f, cy, midZ), new Vector3(t, size.y + t * 2f, depth));
        Place(_right, new Vector3(cx + hw + t * 0.5f, cy, midZ), new Vector3(t, size.y + t * 2f, depth));

        Place(_top, new Vector3(cx, cy + hh + t * 0.5f, midZ), new Vector3(size.x, t, depth));
        Place(_bottom, new Vector3(cx, cy - hh - t * 0.5f, midZ), new Vector3(size.x, t, depth));

        Place(_floor, new Vector3(cx, cy, floorZ + t * 0.5f),
                       new Vector3(size.x + t * 2f, size.y + t * 2f, t));

        Place(_front, new Vector3(cx, cy, frontZ - t * 0.5f),
                       new Vector3(size.x + t * 2f, size.y + t * 2f, t));
    }

    void Place(BoxCollider col, Vector3 pos, Vector3 sz) {
        col.transform.position = pos;
        col.transform.rotation = Quaternion.identity;
        col.center = Vector3.zero;
        col.size = sz;
        if (wallMaterial != null) col.material = wallMaterial;
    }

    void OnDrawGizmos() {
        float depth = (floorZ - frontZ) + wallThickness * 2f;
        float midZ = frontZ + depth * 0.5f;

        Gizmos.color = fillColor;
        Gizmos.DrawCube(new Vector3(center.x, center.y, midZ), new Vector3(size.x, size.y, depth));

        Gizmos.color = wireColor;
        Gizmos.DrawWireCube(new Vector3(center.x, center.y, midZ), new Vector3(size.x, size.y, depth));

        Gizmos.color = floorColor;
        Gizmos.DrawWireCube(new Vector3(center.x, center.y, floorZ), new Vector3(size.x, size.y, 0.02f));

        Gizmos.color = frontColor;
        Gizmos.DrawWireCube(new Vector3(center.x, center.y, frontZ), new Vector3(size.x, size.y, 0.02f));

#if UNITY_EDITOR
        if (!showLabels) return;
        float hw = size.x * 0.5f;
        float hh = size.y * 0.5f;

        UnityEditor.Handles.color = wireColor;
        UnityEditor.Handles.Label(new Vector3(center.x - hw - 0.15f, center.y, midZ), "L");
        UnityEditor.Handles.Label(new Vector3(center.x + hw + 0.15f, center.y, midZ), "R");
        UnityEditor.Handles.Label(new Vector3(center.x, center.y + hh + 0.15f, midZ), "Top");
        UnityEditor.Handles.Label(new Vector3(center.x, center.y - hh - 0.25f, midZ), "Bot");

        UnityEditor.Handles.color = floorColor;
        UnityEditor.Handles.Label(
            new Vector3(center.x + hw + 0.15f, center.y, floorZ),
            $"Floor  Z = {floorZ:F2}");

        UnityEditor.Handles.color = frontColor;
        UnityEditor.Handles.Label(
            new Vector3(center.x + hw + 0.15f, center.y, frontZ),
            $"Front  Z = {frontZ:F2}");
#endif
    }
}
