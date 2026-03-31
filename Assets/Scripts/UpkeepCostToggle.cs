using UnityEngine;
using TMPro;

public class UpkeepCostToggle : MonoBehaviour {

    [SerializeField] UpkeepDisplay upkeepDisplay;

    TextMeshProUGUI _tmp;
    ChessPieces _piece;

    void Awake() {
        _tmp = upkeepDisplay.GetComponent<TextMeshProUGUI>();
        _piece = upkeepDisplay.Piece;
    }

    void Start() {
        upkeepDisplay.OnUpkeepChanged += Refresh;
        _piece.OnPiecePlaced += OnPiecePlaced;
        _piece.OnPieceSold += OnPieceSold;
        Refresh(upkeepDisplay.CurrentUpkeep);
    }

    void OnPiecePlaced(ChessPieces p) => Refresh(upkeepDisplay.CurrentUpkeep);
    void OnPieceSold(ChessPieces p) => gameObject.SetActive(false);

    void Refresh(float upkeep) {
        bool isOnBoard = _piece != null && _piece.mCurrentCell != null;
        gameObject.SetActive(isOnBoard && upkeep > 0f);
    }

    void OnDestroy() {
        if (upkeepDisplay != null)
            upkeepDisplay.OnUpkeepChanged -= Refresh;
        if (_piece != null) {
            _piece.OnPiecePlaced -= OnPiecePlaced;
            _piece.OnPieceSold -= OnPieceSold;
        }
    }
}