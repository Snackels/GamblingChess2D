using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour {
    public Board mBoard;
    void Awake() {
        DOTween.Init(recycleAllByDefault: true, useSafeMode: true)
               .SetCapacity(200, 10);
    }

    void Start() {
        mBoard.Create();
    }
}