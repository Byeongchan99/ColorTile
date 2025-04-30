using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class StageGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int rows;
    public int columns;
    public float cellSize; // �� ĭ�� ũ��
    public Transform normalboardPos; // �븻 ��� ���� ������Ʈ�� ��ġ
    public Transform infiniteboardPos; // ���� ��� ���� ������Ʈ�� ��ġ
    public Transform boardPos; // ���� ������Ʈ�� ��ġ(Normal �Ǵ� Infinite�� ���� �ٸ�)

    [Header("Tile Pair Settings")]
    [SerializeField] private int _pairCount; // �� ���� �� ����
    [SerializeField] private int _colorCount; // ��� ���� ���� (None ����)
    public int totalTileCount; // �� Ÿ�� ����(_pairCount * _colorCount * 2)

    [Header("Tile Prefab")]
    public GameObject[] TilePrefabs; // Ÿ�� ������ (Inspector�� �Ҵ�)
    public Transform tileContainer; // Ÿ�� ������Ʈ���� ���� �θ� ������Ʈ

    // �������� ��ü�� �����ϴ� 2���� �迭 (PlayManager���� ����)
    public TileColor[,] grid;
    // ���� Ÿ�� ������Ʈ���� �����ϴ� �迭
    public GameObject[,] tileObjects;

    private void Awake()
    {
        GameEvents.OnGameStarted += GenerateStage; // ���� ���� �� �ʱ�ȭ
        GameEvents.OnRetryGame += GenerateStage; // ���� ����� �� �ʱ�ȭ
        GameEvents.OnClearBoard += GenerateStage; // ���� Ŭ���� �� �ʱ�ȭ
    }

    // �������� �ʱ�ȭ
    public void InitStage()
    {
        // ��忡 ���� �� ����
        // columns�� ����
        if (GameManager.gameMode == GameMode.Normal)
        {
            _pairCount = 5; // Normal ����� �� ����
            _colorCount = 10; // Normal ����� ���� ����
            rows = 16;
            boardPos = normalboardPos;
        }
        else if (GameManager.gameMode == GameMode.Infinite)
        {
            _pairCount = 5; // Infinite ����� �� ����
            _colorCount = 10; // Infinite ����� ���� ����
            rows = 18;
            boardPos = infiniteboardPos;
        }
        else
        {
            //Debug.LogError("Invalid game mode.");
            return;
        }

        // 1. grid�� tileObjects �ʱ�ȭ (��� ĭ�� None, null�� ����)
        grid = new TileColor[rows, columns];
        tileObjects = new GameObject[rows, columns];
        totalTileCount = _pairCount * _colorCount * 2; // �� Ÿ�� ���� ���

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                grid[y, x] = TileColor.None;
                tileObjects[y, x] = null;
            }
        }

        // ������ Ÿ�ϵ� ����
        foreach (Transform child in tileContainer)
        {
            Destroy(child.gameObject);
        }
    }

    // ���������� �����ϴ� �޼��� (GenerateStage ver.2)
    public void GenerateStage()
    {
        // 1. �������� �ʱ�ȭ
        InitStage();
        // 2. ���� �� ����Ʈ ���� (�� pairCount * colorCount ��)
        List<TileColor> pairColors = new List<TileColor>();
        for (int colorIndex = 1; colorIndex <= _colorCount; colorIndex++)
        {
            for (int i = 0; i < _pairCount; i++)
            {
                pairColors.Add((TileColor)colorIndex);
            }
        }

        // 3. �� ������ �������� ����
        for (int i = 0; i < pairColors.Count; i++)
        {
            TileColor temp = pairColors[i];
            int randomIndex = Random.Range(i, pairColors.Count);
            pairColors[i] = pairColors[randomIndex];
            pairColors[randomIndex] = temp;
        }

        // 4. �� ���� ver.2 ������ ���� ��ġ
        int MAX_ATTEMPTS = 1000;
        foreach (TileColor color in pairColors)
        {
            bool placed = false;
            int attempt = 0;
            while (!placed && attempt < MAX_ATTEMPTS)
            {
                attempt++;

                // 1: �� ��(Ʈ���� ��)�� ���� ����
                int triggerX = Random.Range(0, columns);
                int triggerY = Random.Range(0, rows);
                Vector2Int triggerPos = new Vector2Int(triggerX, triggerY);
                if (grid[triggerPos.y, triggerPos.x] != TileColor.None)
                {
                    continue;
                }

                // 2: �����¿� ���� �� 2������ �����ϰ� ���� (������ ��� ù �� �� ���)
                List<Vector2Int> directions = new List<Vector2Int>
            {
                new Vector2Int(0, 1),   // ��
                new Vector2Int(0, -1),  // �Ʒ�
                new Vector2Int(-1, 0),  // ����
                new Vector2Int(1, 0)    // ������
            };
                for (int i = 0; i < directions.Count; i++)
                {
                    int j = Random.Range(i, directions.Count);
                    Vector2Int tempDir = directions[i];
                    directions[i] = directions[j];
                    directions[j] = tempDir;
                }
                Vector2Int dir1 = directions[0];
                Vector2Int dir2 = directions[1];

                // 3: �� �������� �̵��� �� �ִ� �ִ� �Ÿ��� ��� (Ʈ���� �� ����)
                int maxDist1 = 0;
                if (dir1.x > 0) maxDist1 = columns - 1 - triggerPos.x;
                else if (dir1.x < 0) maxDist1 = triggerPos.x;
                else if (dir1.y > 0) maxDist1 = rows - 1 - triggerPos.y;
                else if (dir1.y < 0) maxDist1 = triggerPos.y;

                int maxDist2 = 0;
                if (dir2.x > 0) maxDist2 = columns - 1 - triggerPos.x;
                else if (dir2.x < 0) maxDist2 = triggerPos.x;
                else if (dir2.y > 0) maxDist2 = rows - 1 - triggerPos.y;
                else if (dir2.y < 0) maxDist2 = triggerPos.y;

                // �̵� �Ÿ��� �ּ� 1 �̻��̾�� ��
                if (maxDist1 < 1 || maxDist2 < 1)
                {
                    continue;
                }

                // 4: �� ���⿡ ����, 1���� �ִ� �Ÿ�(maxDist)���� �� ������ �Ÿ� ����
                int dist1 = Random.Range(1, maxDist1 + 1);
                int dist2 = Random.Range(1, maxDist2 + 1);

                // 5: �� �������� �̵��ϸ� ��ġ�� ��ġ ���� (�ٸ� Ÿ���� ������ ��, Ȥ�� �ִ� �Ÿ� ���� �ñ���)
                // ù ��° ���⿡ ����:
                Vector2Int posTile1 = triggerPos;
                bool valid1 = true;
                for (int step = 1; step <= dist1; step++)
                {
                    Vector2Int nextPos = triggerPos + new Vector2Int(dir1.x * step, dir1.y * step);
                    if (grid[nextPos.y, nextPos.x] != TileColor.None)
                    {
                        if (step == 1)
                        {
                            valid1 = false;
                            break;
                        }
                        else
                        {
                            posTile1 = triggerPos + new Vector2Int(dir1.x * (step - 1), dir1.y * (step - 1));
                            break;
                        }
                    }
                    if (step == dist1)
                    {
                        posTile1 = nextPos;
                    }
                }
                if (!valid1)
                {
                    continue;
                }

                // �� ��° ���⿡ ����:
                Vector2Int posTile2 = triggerPos;
                bool valid2 = true;
                for (int step = 1; step <= dist2; step++)
                {
                    Vector2Int nextPos = triggerPos + new Vector2Int(dir2.x * step, dir2.y * step);
                    if (grid[nextPos.y, nextPos.x] != TileColor.None)
                    {
                        if (step == 1)
                        {
                            valid2 = false;
                            break;
                        }
                        else
                        {
                            posTile2 = triggerPos + new Vector2Int(dir2.x * (step - 1), dir2.y * (step - 1));
                            break;
                        }
                    }
                    if (step == dist2)
                    {
                        posTile2 = nextPos;
                    }
                }
                if (!valid2)
                {
                    continue;
                }

                // 6: ��ġ�� �� ��ġ�� ���� �ٸ���, Ʈ���� ������ ��ġ�� ������, ��� �� ĭ���� Ȯ��
                if (posTile1 == posTile2 || posTile1 == triggerPos || posTile2 == triggerPos)
                {
                    continue;
                }
                if (grid[posTile1.y, posTile1.x] != TileColor.None || grid[posTile2.y, posTile2.x] != TileColor.None)
                {
                    continue;
                }

                // 7: �� ��ġ�� Ÿ�� �� ��ġ
                SetTiles(posTile1, posTile2, color);
                placed = true;
            }

            if (!placed)
            {
                Debug.LogWarning("Failed to place a pair of color " + color + " after many attempts.");
            }
        }
    }

    // ���������� �����ϴ� �޼��� (GenerateStage ver.1)
    #region
    /*
    void GenerateStage()
    {
        // 1. grid �ʱ�ȭ (��� ĭ�� None����)
        grid = new TileColor[rows, columns];
        tileObjects = new GameObject[rows, columns];
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                grid[y, x] = TileColor.None;
                tileObjects[y, x] = null;
            }
        }

        // 2. ���� �� ����Ʈ ���� (�� pairCount * colorCount ��)
        List<TileColor> pairColors = new List<TileColor>();
        for (int colorIndex = 1; colorIndex <= colorCount; colorIndex++)
        {
            for (int i = 0; i < pairCount; i++)
            {
                pairColors.Add((TileColor)colorIndex);
            }
        }

        // 3. �� ������ �������� ����
        for (int i = 0; i < pairColors.Count; i++)
        {
            TileColor temp = pairColors[i];
            int randomIndex = Random.Range(i, pairColors.Count);
            pairColors[i] = pairColors[randomIndex];
            pairColors[randomIndex] = temp;
        }

        // 4. �� ���� ��ġ
        foreach (TileColor color in pairColors)
        {
            bool placed = false;
            int attempt = 0;
            while (!placed && attempt < 1000)
            { // ���ѷ��� ����
                attempt++;
                // (1) ���簢�� ũ�� ���� (1 <= width, height <= MAX, width + height > 3)
                int rectWidth = Random.Range(1, columns + 1);
                int rectHeight = Random.Range(1, rows + 1);
                if (rectWidth + rectHeight <= 3)
                    continue;

                // (2) ���簢���� grid �ȿ� ������ ����(�ٴ� ����) ����
                int originX = Random.Range(0, columns - rectWidth + 1);
                int originY = Random.Range(0, rows - rectHeight + 1);

                // (3) �밢�� ����: �ɼǿ� ���� �� �𼭸� ��ǥ ����
                bool option = (Random.value < 0.5f);
                Vector2Int pos1, pos2;
                if (option)
                {
                    // ���� ���� ������ �Ʒ�
                    pos1 = new Vector2Int(originX, originY + rectHeight - 1);      // top-left
                    pos2 = new Vector2Int(originX + rectWidth - 1, originY);         // bottom-right
                }
                else
                {
                    // ������ ���� ���� �Ʒ�
                    pos1 = new Vector2Int(originX + rectWidth - 1, originY + rectHeight - 1); // top-right
                    pos2 = new Vector2Int(originX, originY);                                 // bottom-left
                }

                // (4) �� ��ġ�� Ÿ���� �� �� �ִ��� Ȯ��
                if (CanSetTiles(pos1, pos2))
                {
                    SetTiles(pos1, pos2, color);
                    placed = true;
                }
            }

            if (!placed)
            {
                //Debug.LogWarning("Failed to place a pair of color " + color);
            }
        }
    }
    */
    #endregion

    // �ش� �� ��ġ�� ����ִ��� �˻�
    bool CanSetTiles(Vector2Int pos1, Vector2Int pos2)
    {
        return grid[pos1.y, pos1.x] == TileColor.None && grid[pos2.y, pos2.x] == TileColor.None;
    }

    // grid�� Ÿ�� ������ ����ϰ�, Ÿ�� �������� ����
    void SetTiles(Vector2Int pos1, Vector2Int pos2, TileColor color)
    {
        grid[pos1.y, pos1.x] = color;
        grid[pos2.y, pos2.x] = color;

        // grid ��ǥ�� ���� ��ǥ�� ��ȯ�Ͽ� Ÿ�� ���� (�� ĭ�� �߽��� (cellSize/2, cellSize/2)��ŭ ������)
        Vector3 worldPos1 = GetWorldPosition(pos1);
        Vector3 worldPos2 = GetWorldPosition(pos2);

        if (TilePrefabs[(int)color - 1] != null)
        {
            GameObject tile1 = Instantiate(TilePrefabs[(int)color - 1], worldPos1, Quaternion.identity, tileContainer);
            GameObject tile2 = Instantiate(TilePrefabs[(int)color - 1], worldPos2, Quaternion.identity, tileContainer);

            tileObjects[pos1.y, pos1.x] = tile1;
            tileObjects[pos2.y, pos2.x] = tile2;
        }
    }

    // grid ��ǥ �� ���� ��ǥ ��ȯ
    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        // boardPos.position�� �������� �� ���� �������� ����.
        return boardPos.position + new Vector3(gridPos.x * cellSize + cellSize / 2,
                                                 gridPos.y * cellSize + cellSize / 2, 0);
    }
}
