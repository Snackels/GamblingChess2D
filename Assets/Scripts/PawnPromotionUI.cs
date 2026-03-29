using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        gameObject.SetActive(true);
        Time.timeScale = 0f;   // freeze game while choosing
    }

    // --- wire each button's OnClick to one of these ---

    public void ChooseQueen() => Promote(queenPrefab, queenVisualPrefab);
    public void ChooseRook() => Promote(rookPrefab, rookVisualPrefab);
    public void ChooseBishop() => Promote(bishopPrefab, bishopVisualPrefab);
    public void ChooseKnight() => Promote(knightPrefab, knightVisualPrefab);

    void Promote(GameObject piecePrefab, GameObject visualPrefab) {
        if (pendingPawn == null || piecePrefab == null) return;

        Cell targetCell = pendingPawn.mCurrentCell;
        int spawnTurn = pendingPawn.spawnTurn;
        int turnsOnBoard = pendingPawn.turnsOnBoard;

        // Remove pawn silently (no refund, no event spam)
        if (targetCell != null) targetCell.mCurrentPiece = null;
        ThreatManager.Instance.UnregisterThreats(pendingPawn, pendingPawn.GetCurrentThreats());
        Destroy(pendingPawn.gameObject);
        pendingPawn = null;

        // Spawn the chosen piece in the same canvas parent
        Canvas canvas = FindFirstObjectByType<Canvas>();
        GameObject newObj = Instantiate(piecePrefab, canvas.transform);

        ChessPieces newPiece = newObj.GetComponent<ChessPieces>();
        newPiece.spawnTurn = spawnTurn;
        newPiece.turnsOnBoard = turnsOnBoard;

        // Hook up visual if provided
        if (visualPrefab != null && visualParent != null) {
            GameObject newVisual = Instantiate(visualPrefab, visualParent);
            ChessPieceVisual visual = newVisual.GetComponent<ChessPieceVisual>();
            newPiece.chessPieceVisual = visual;
            visual.Initialize(newPiece);
        }

        // Place it on the same cell the pawn was on
        newPiece.Place(targetCell);

        // Close popup
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}