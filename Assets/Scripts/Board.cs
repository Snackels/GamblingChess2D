using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour {
    public GameObject mCellPrefab;
    public Cell[,] mAllCells = new Cell[5, 5];

    private float cellSize;

    public void Create() {
        cellSize = mCellPrefab.GetComponent<RectTransform>().sizeDelta.x;

        float totalSize = cellSize * 5;
        float startX = -totalSize / 2 + cellSize / 2;
        float startY = -totalSize / 2 + cellSize / 2;

        for (int y = 0; y < 5; y++) {
            for (int x = 0; x < 5; x++) {
                GameObject newCell = Instantiate(mCellPrefab, transform);
                RectTransform rectTransform = newCell.GetComponent<RectTransform>();

                rectTransform.anchoredPosition = new Vector2(
                    startX + (x * cellSize),
                    startY + (y * cellSize)
                );

                mAllCells[x, y] = newCell.GetComponent<Cell>();
                mAllCells[x, y].Setup(new Vector2Int(x, y), this);

                bool isLight = (x + y) % 2 == 0;
                if (isLight)
                    newCell.GetComponent<Image>().color = new Color32(230, 220, 187, 255);
            }
        }
    }
}