using UnityEngine;

/// <summary>
/// Attach to a UI Panel (the promotion popup).
/// Set it inactive by default in the scene.
/// Assign the 4 piece prefabs (Queen, Rook, Bishop, Knight) in the inspector.
/// </summary>
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

    // The pawn waiting to be replaced
    Pawn pendingPawn;

    void Awake() {
        Instance = this;
        gameObject.SetActive(false);
    }

    /// <summary>Called by Pawn when it lands on the last row.</summary>
    public void Show(Pawn pawn) {
        pendingPawn = pawn;

        // Snap popup to the pawn's canvas position
        RectTransform popupRect = GetComponent<RectTransform>();
        RectTransform pawnRect = pawn.GetComponent<RectTransform>();
        popupRect.anchoredPosition = pawnRect.anchoredPosition + Vector2.up * 80f;

        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    // --- wire each button's OnClick to one of these ---

    public void ChooseQueen() => Promote(queenPrefab, queenVisualPrefab);
    public void ChooseRook() => Promote(rookPrefab, rookVisualPrefab);
    public void ChooseBishop() => Promote(bishopPrefab, bishopVisualPrefab);
    public void ChooseKnight() => Promote(knightPrefab, knightVisualPrefab);

    void Promote(GameObject piecePrefab, GameObject visualPrefab) {
        if (pendingPawn == null || piecePrefab == null) return;

        // Grab everything from the pawn BEFORE destroying it
        Cell targetCell = pendingPawn.mCurrentCell;
        int spawnTurn = pendingPawn.spawnTurn;
        int turnsOnBoard = pendingPawn.turnsOnBoard;
        Vector3 localPos = pendingPawn.transform.localPosition;
        Transform canvasParent = pendingPawn.transform.parent;

        // Silently remove pawn — clear cell, unregister threats, no refund
        if (targetCell != null) targetCell.mCurrentPiece = null;
        pendingPawn.RemoveVisual();
        ThreatManager.Instance.UnregisterThreats(pendingPawn, pendingPawn.GetCurrentThreats());
        Destroy(pendingPawn.gameObject);
        pendingPawn = null;

        // Spawn new piece under the same parent as the pawn
        GameObject newObj = Instantiate(piecePrefab, canvasParent);
        ChessPieces newPiece = newObj.GetComponent<ChessPieces>();

        // Copy pawn's state
        newPiece.spawnTurn = spawnTurn;
        newPiece.turnsOnBoard = turnsOnBoard;

        // Put it exactly where the pawn was
        newObj.GetComponent<RectTransform>().localPosition = localPos;

        // Hook up visual if provided
        if (visualPrefab != null && visualParent != null) {
            GameObject newVisual = Instantiate(visualPrefab, visualParent);
            ChessPieceVisual visual = newVisual.GetComponent<ChessPieceVisual>();
            newPiece.chessPieceVisual = visual;
            visual.Initialize(newPiece);
        }

        // Directly assign cell — bypass Place() so no cost/spawn checks run
        newPiece.mCurrentCell = targetCell;
        targetCell.mCurrentPiece = newPiece;
        newPiece.isSelectedForUpkeep = true;

        // Register threats and notify board
        newPiece.RecalculateThreats();
        GameManager.Instance.NotifyBoardChanged();

        // Close popup and resume
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}