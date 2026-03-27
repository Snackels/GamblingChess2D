using UnityEngine;

/// <summary>
/// 5-wall physics boundary for coins. Toggle on/off via SetOpen().
/// Wire PanelSlide.OnClick to also call CoinBoundary.Toggle().
///
/// LAYER SETUP (one-time, in Project Settings):
///   1. Tags and Layers → add "CoinBoundary" (e.g. Layer 6) and "Coin" (e.g. Layer 7)
///   2. Physics → Layer Collision Matrix:
///      - Coin vs CoinBoundary  = ON
///      - Coin vs everything else = OFF
///      - CoinBoundary vs everything except Coin = OFF
///   3. Set CoinBoundaryLayer = 6 in Inspector
///   4. Set your coin prefab's layer to 7 (Coin)
/// </summary>
public class CoinBoundary : MonoBehaviour {
    [Header("Boundary XY Size")]
    [Tooltip("World space center (XY). Z is ignored — use Front Z and Floor Z below.")]
    public Vector3 center = Vector3.zero;

    [Tooltip("Width (X) and Height (Y) of the open face of the boundary")]
    public Vector2 size = new Vector2(6f, 4f);

    [Header("Z Depth")]
    [Tooltip("Z position of the open face (toward your perspective camera)")]
    public float frontZ = 0f;

    [Tooltip("Z position of the floor wall — coins fall toward +Z and stop here")]
    public float floorZ = 8f;

    [Header("Wall Settings")]
    public float wallThickness = 0.2f;

    [Tooltip("Physic Material — set Bounciness and Bounce Combine = Maximum here")]
    public PhysicsMaterial wallMaterial;

    [Tooltip("Must match your CoinBoundary layer number in Project Settings")]
    public int coinBoundaryLayer = 6;

    [Header("Starting State")]
    public bool startOpen = false;

    [Header("Gizmos")]
    public Color fillColor = new Color(0.2f, 0.85f, 1f, 0.12f);
    public Color wireColor = new Color(0.2f, 0.85f, 1f, 0.85f);
    public Color floorColor = new Color(1f, 0.55f, 0.1f, 0.9f);
    public bool showLabels = true;

    // ── internals ───────────────────────────────────────────────────────────
    GameObject _root;
    BoxCollider _left, _right, _top, _bottom, _floor;
    bool _isOpen;

    // ── lifecycle ────────────────────────────────────────────────────────────
    void Awake() {
        Build();
        SetOpen(startOpen);
    }

    void OnValidate() {
        if (_floor != null) Refresh();
    }

    // ── public API ───────────────────────────────────────────────────────────
    public void SetOpen(bool open) {
        _isOpen = open;
        if (_root != null) _root.SetActive(open);
    }

    public void Toggle() => SetOpen(!_isOpen);

    // ── build / refresh ──────────────────────────────────────────────────────
    void Build() {
        _root = new GameObject("_CoinBoundaryWalls");
        _root.transform.SetParent(transform);

        _left = MakeWall("Wall_L");
        _right = MakeWall("Wall_R");
        _top = MakeWall("Wall_Top");
        _bottom = MakeWall("Wall_Bot");
        _floor = MakeWall("Wall_Floor");

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
        float depth = (floorZ - frontZ) + wallThickness;
        float midZ = frontZ + depth * 0.5f;
        float t = wallThickness;

        // Left / Right — full depth, cover XY gap at corners
        Place(_left, new Vector3(cx - hw - t * 0.5f, cy, midZ), new Vector3(t, size.y + t * 2, depth));
        Place(_right, new Vector3(cx + hw + t * 0.5f, cy, midZ), new Vector3(t, size.y + t * 2, depth));

        // Top / Bottom — full depth, span inner X
        Place(_top, new Vector3(cx, cy + hh + t * 0.5f, midZ), new Vector3(size.x, t, depth));
        Place(_bottom, new Vector3(cx, cy - hh - t * 0.5f, midZ), new Vector3(size.x, t, depth));

        // Floor — perpendicular slab at floorZ, spans full XY including wall corners
        Place(_floor, new Vector3(cx, cy, floorZ + t * 0.5f),
                       new Vector3(size.x + t * 2, size.y + t * 2, t));
    }

    void Place(BoxCollider col, Vector3 pos, Vector3 sz) {
        col.transform.position = pos;
        col.transform.rotation = Quaternion.identity;
        col.center = Vector3.zero;
        col.size = sz;
        if (wallMaterial != null) col.material = wallMaterial;
    }

    // ── gizmos ───────────────────────────────────────────────────────────────
    void OnDrawGizmos() {
        float depth = (floorZ - frontZ) + wallThickness;
        float midZ = frontZ + depth * 0.5f;
        var vol = new Vector3(center.x, center.y, midZ);
        var ext = new Vector3(size.x, size.y, depth);

        // Volume fill
        Gizmos.color = fillColor;
        Gizmos.DrawCube(vol, ext);

        // Volume wire
        Gizmos.color = wireColor;
        Gizmos.DrawWireCube(vol, ext);

        // Floor plane — orange highlight
        Gizmos.color = floorColor;
        Gizmos.DrawWireCube(new Vector3(center.x, center.y, floorZ), new Vector3(size.x, size.y, 0.02f));

        // Open face — green highlight
        Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.7f);
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

        UnityEditor.Handles.color = new Color(0.2f, 1f, 0.4f, 1f);
        UnityEditor.Handles.Label(
            new Vector3(center.x + hw + 0.15f, center.y, frontZ),
            $"Open face  Z = {frontZ:F2}");
#endif
    }
}