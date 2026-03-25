using UnityEngine;
using DG.Tweening;
using System.Collections;

public class GameManager : MonoBehaviour {
    [SerializeField] SpawnPoint[] spawnPoints;
    public Board mBoard;
    void Awake() {
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