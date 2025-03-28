using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class StageGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int rows = 20;
    public int columns = 10;
    public float cellSize = 0.48f; // �� ĭ�� ũ��

    [Header("Tile Pair Settings")]
    public int pairCount = 7; // �� ���� �� ����
    public int colorCount = 10; // ��� ���� ���� (None ����)

    [Header("Tile Prefab")]
    public GameObject[] tilePrefabs; // Ÿ�� ������ (Inspector�� �Ҵ�)

    // �������� ��ü�� �����ϴ� 2���� �迭 (PlayManager���� ����)
    public static TileColor[,] grid;

    void Start()
    {
        GenerateStage();
    }

    // ���������� �����ϴ� �޼���
    void GenerateStage()
    {
        // 1. grid �ʱ�ȭ (��� ĭ�� None����)
        grid = new TileColor[rows, columns];
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                grid[y, x] = TileColor.None;
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

        if (tilePrefabs[(int)color - 1] != null)
        {
            Instantiate(tilePrefabs[(int)color - 1], worldPos1, Quaternion.identity);
            Instantiate(tilePrefabs[(int)color - 1], worldPos2, Quaternion.identity);
        }
    }

    // grid ��ǥ �� ���� ��ǥ ��ȯ
    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        float stageWidth = columns * cellSize;
        float stageHeight = rows * cellSize;
        // �� Ÿ���� ���� ��ġ: (gridPos.x * cellSize + cellSize/2, gridPos.y * cellSize + cellSize/2)
        // �̸� �������� �߾� ������ ���� ��ü ũ���� ���ݸ�ŭ ���ݴϴ�.
        return new Vector3(gridPos.x * cellSize + cellSize / 2, gridPos.y * cellSize + cellSize / 2, 0)
               - new Vector3(stageWidth / 2, stageHeight / 2, 0);
    }

}
