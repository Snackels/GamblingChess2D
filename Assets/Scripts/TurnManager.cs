using UnityEngine;

public class TurnManager : MonoBehaviour {
    public static TurnManager Instance;
    public int currentTurn = 0;

    void Awake() {
        Instance = this;
    }

    public void EndTurn() {
        SpawnPoint[] spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        foreach (SpawnPoint sp in spawnPoints)
            sp.SpawnPiece();

        currentTurn++;
    }
}