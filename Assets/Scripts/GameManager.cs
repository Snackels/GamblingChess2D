using UnityEngine;
using DG.Tweening;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    [SerializeField] SpawnPoint[] spawnPoints;
    public Board mBoard;

    [Header("Piece Limit")]
    [SerializeField] public int maxPiecesOnBoard = 5;

    public int GetPiecesOnBoardCount() {
        int count = 0;
        foreach (Cell cell in mBoard.mAllCells)
            if (cell != null && cell.mCurrentPiece != null) count++;
        return count;
    }

    public bool IsBoardFull() => GetPiecesOnBoardCount() >= maxPiecesOnBoard;

    void Awake() {
        Instance = this;
        DOTween.Init(recycleAllByDefault: true, useSafeMode: true)
               .SetCapacity(200, 10);
    }

    void Start() {
        mBoard.Create();
        StartCoroutine(SpawnAllPieces());
    }

    IEnumerator SpawnAllPieces() {
        yield return new WaitForEndOfFrame();
        foreach (SpawnPoint sp in spawnPoints)
            sp.SpawnPiece();
    }
}