using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChessBoard : MonoBehaviour {
    const int Tile_Count_X = 5;
    const int Tile_Count_Y = 5;

    GameObject[,] tiles;

    void Awake() {
        GenerateAllTiles(1, Tile_Count_X, Tile_Count_Y);
    }

    void GenerateAllTiles(float tileSize, float tileCountX, float tileCountY) {
        tiles = new GameObject[(int)tileCountX, (int)tileCountY];
        for (int x = 0; x < tileCountX; x++) {
            for (int y = 0; y < tileCountY; y++) {
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
            }
        }
    }

    GameObject GenerateSingleTile(float tileSize, int x, int y) {
        GameObject tileObject = new GameObject(string.Format("Tile {0} {1}", x, y));
        tileObject.transform.parent = transform;

        return tileObject;
    }
}
