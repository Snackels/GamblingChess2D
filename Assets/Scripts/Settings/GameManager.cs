using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour {
    void Awake() {
        DOTween.Init(recycleAllByDefault: true, useSafeMode: true)
               .SetCapacity(200, 10);
    }
}