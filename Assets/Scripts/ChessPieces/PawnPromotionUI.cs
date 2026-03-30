using UnityEngine;

public class PawnPromotionUI : MonoBehaviour {
    public static PawnPromotionUI Instance { get; private set; }

    [Header("Piece Prefabs to promote into")]
    [SerializeField] GameObject queenPrefab;
    [SerializeField] GameObject rookPrefab;
    [SerializeField] GameObject bishopPrefab;
    [SerializeField] GameObject knightPrefab;

    [Header("Visual Prefabs (optional, match order above)")]
    [SerializeField] GameObject queenVisualPrefab;
    [SerializeField] GameObject rookVisualPrefab;
    [SerializeField] GameObject bishopVisualPrefab;
    [SerializeField] GameObject knightVisualPrefab;

    [Header("Visual Parent for piece visuals")]
    [SerializeField] Transform visualParent;

    Pawn pendingPawn;

    void Awake() {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show(Pawn pawn) {
        pendingPawn = pawn;

        RectTransform popupRect = GetComponent<RectTransform>();
        RectTransform pawnRect = pawn.GetComponent<RectTransform>();
        popupRect.anchoredPosition = pawnRect.anchoredPosition + Vector2.up * 80f;

        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ChooseQueen() => Promote(queenPrefab, queenVisualPrefab);
    public void ChooseRook() => Promote(rookPrefab, rookVisualPrefab);
    public void ChooseBishop() => Promote(bishopPrefab, bishopVisualPrefab);
    public void ChooseKnight() => Promote(knightPrefab, knightVisualPrefab);

    void Promote(GameObject piecePrefab, GameObject visualPrefab) {
        if (pendingPawn == null || piecePrefab == null) return;

        Cell targetCell = pendingPawn.mCurrentCell;
        int spawnTurn = pendingPawn.spawnTurn;
        int turnsOnBoard = pendingPawn.turnsOnBoard;
        float pawnUpkeep = pendingPawn.GetUpkeepCost(); // Preserve the pawn's upkeep value
        Vector3 localPos = pendingPawn.transform.localPosition;
        Transform canvasParent = pendingPawn.transform.parent;

        if (targetCell != null) targetCell.mCurrentPiece = null;
        pendingPawn.RemoveVisual();
        ThreatManager.Instance.UnregisterThreats(pendingPawn, pendingPawn.GetCurrentThreats());
        Destroy(pendingPawn.gameObject);
        pendingPawn = null;

        GameObject newObj = Instantiate(piecePrefab, canvasParent);
        ChessPieces newPiece = newObj.GetComponent<ChessPieces>();

        newPiece.spawnTurn = spawnTurn;
        newPiece.turnsOnBoard = turnsOnBoard;
        newPiece.lockedUpkeep = pawnUpkeep; // Keep paying the pawn's upkeep, not the promoted piece's

        newObj.GetComponent<RectTransform>().localPosition = localPos;
        newPiece.SyncOriginalPosition();

        if (visualPrefab != null && visualParent != null) {
            GameObject newVisual = Instantiate(visualPrefab, visualParent);
            ChessPieceVisual visual = newVisual.GetComponent<ChessPieceVisual>();
            newPiece.chessPieceVisual = visual;
            visual.Initialize(newPiece);
        }

        newPiece.mCurrentCell = targetCell;
        targetCell.mCurrentPiece = newPiece;
        newPiece.isSelectedForUpkeep = true;

        newPiece.RecalculateThreats();
        GameManager.Instance.NotifyBoardChanged();

        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}