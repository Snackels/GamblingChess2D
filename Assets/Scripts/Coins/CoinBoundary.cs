using System;
using UnityEngine;

public class CoinBoundary : MonoBehaviour {
    [Header("Boundary XY size")]
    [SerializeField] Vector3 _center = Vector3.zero;
    [SerializeField] Vector2 _size = new Vector2(1, 1);

    [Header("Z Depth")]
    [SerializeField] float _frontZ = 0f;
    [SerializeField] float _floorZ = 10f;

    [Header("Wall Settings")]
    [SerializeField] float _wallThickness = 0.2f;
    [SerializeField] PhysicsMaterial wallMaterial;

    [Header("Gizmos")]
    [SerializeField] Color fillColor = new Color(0.2f, 0.85f, 1f, 0.12f);
    [SerializeField] Color wireColor = new Color(0.2f, 0.85f, 1f, 0.85f);
    [SerializeField] Color floorColor = new Color(1f, 0.55f, 0.1f, 0.9f);
    [SerializeField] Color frontColor = new Color(0.2f, 1f, 0.4f, 0.9f);

    GameObject _root;
    BoxCollider _left, _right, _top, _bottom, _front, _floor;

    void Start() {
        Build();
    }

    void OnValidate() {
        if (_floor != null) Refresh();
    }

    void Build() {
        _root = new GameObject("CoinBoundary");
        _root.transform.SetParent(transform);

        _left = MakeWall("Left");
        _right = MakeWall("Right");
        _top = MakeWall("Top");
        _bottom = MakeWall("Bottom");
        _front = MakeWall("Front");
        _floor = MakeWall("Floor");

        Refresh();
    }

    BoxCollider MakeWall(string name) {
        GameObject wall = new GameObject(name);
        wall.transform.parent = _root.transform;
        wall.layer = gameObject.layer;
        BoxCollider col = wall.AddComponent<BoxCollider>();
        if (wallMaterial != null) col.material = wallMaterial;
        return col;
    }

    void Refresh() {
        float halfWidth = _size.x / 2f;
        float halfHeight = _size.y / 2f;
        float centerX = _center.x;
        float centerY = _center.y;
        float depth = _floorZ - _frontZ + _wallThickness * 2f;
        float midZ = (_frontZ + depth) * 0.5f;
        float thickness = _wallThickness;

        Place(_left, new Vector3(centerX - halfWidth - thickness * 0.5f, centerY, midZ), new Vector3(thickness, halfHeight * 2f + thickness * 2f, depth));
        Place(_right, new Vector3(centerX + halfWidth + thickness * 0.5f, centerY, midZ), new Vector3(thickness, halfHeight * 2f + thickness * 2f, depth));

        Place(_top, new Vector3(centerX, centerY + halfHeight + thickness * 0.5f, midZ), new Vector3(_size.x, thickness, depth));
        Place(_bottom, new Vector3(centerX, centerY - halfHeight - thickness * 0.5f, midZ), new Vector3(_size.x, thickness, depth));

        Place(_floor, new Vector3(centerX, centerY, _floorZ + thickness * 0.5f), new Vector3(_size.x + thickness * 2f, _size.y + thickness * 2f, thickness));

        Place(_front, new Vector3(centerX, centerY, _frontZ - thickness * 0.5f), new Vector3(_size.x + thickness * 2f, _size.y + thickness * 2f, thickness));
    }

    void Place(BoxCollider col, Vector3 pos, Vector3 size) {
        col.transform.localPosition = pos;
        col.size = size;
        col.center = Vector3.zero;
        col.transform.rotation = Quaternion.identity;
        if (wallMaterial != null) col.material = wallMaterial;
    }

    void OnDrawGizmos() {
        float depth = (_floorZ - _frontZ) + _wallThickness * 2f;
        float midZ = _frontZ + depth * 0.5f;

        Gizmos.color = fillColor;
        Gizmos.DrawCube(new Vector3(_center.x, _center.y, midZ), new Vector3(_size.x, _size.y, depth));

        Gizmos.color = wireColor;
        Gizmos.DrawWireCube(new Vector3(_center.x, _center.y, midZ), new Vector3(_size.x, _size.y, depth));

        Gizmos.color = floorColor;
        Gizmos.DrawWireCube(new Vector3(_center.x, _center.y, _floorZ), new Vector3(_size.x, _size.y, 0.02f));

        Gizmos.color = frontColor;
        Gizmos.DrawWireCube(new Vector3(_center.x, _center.y, _frontZ), new Vector3(_size.x, _size.y, 0.02f));
    }
}