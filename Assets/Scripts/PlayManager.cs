using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Enums;

public class PlayManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public float cellSize = 0.48f;
    public int rows = 20;
    public int columns = 10;

    public int score = 0; // ����
    public int tileScore = 1; // Ÿ�� �� ����

    [Header("References")]
    public UIManager uiManager;

    public void Awake()
    {
        if (uiManager == null)
            uiManager = FindObjectOfType<UIManager>();

        score = 0;
    }

    void Update()
    {
        HandleInput();
    }

    // ��ġ �Ǵ� ���콺 Ŭ�� �Է� ó��
    void HandleInput()
    {
        // UI ��� ������ Ŭ���� ���, �Է� ����
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPos = Input.mousePosition;
            Vector3 worldPos = GetWorldPos(screenPos);
            Vector2Int gridPos = GetGridPos(worldPos);

            if (IsValidGridPos(gridPos))
            {
                Debug.Log("Clicked grid position: " + gridPos);

                // Ŭ���� ��ġ�� �� ĭ�� ���� ���� Ÿ�� Ž��
                if (StageGenerator.grid[gridPos.y, gridPos.x] == TileColor.None)
                {
                    List<Vector2Int> matchingTiles = GetAdjacentTiles(gridPos);
                    Debug.Log("Total adjacent tiles found: " + matchingTiles.Count);

                    Dictionary<TileColor, List<Vector2Int>> groups = new Dictionary<TileColor, List<Vector2Int>>();
                    foreach (var pos in matchingTiles)
                    {
                        TileColor tileColor = StageGenerator.grid[pos.y, pos.x];
                        if (!groups.ContainsKey(tileColor))
                            groups[tileColor] = new List<Vector2Int>();
                        groups[tileColor].Add(pos);
                    }

                    List<Vector2Int> tilesToErase = new List<Vector2Int>();
                    foreach (var kvp in groups)
                    {
                        if (kvp.Value.Count >= 2)
                        {
                            Debug.Log("Removing group of color: " + kvp.Key + ", count: " + kvp.Value.Count);
                            tilesToErase.AddRange(kvp.Value);
                        }
                    }

                    if (tilesToErase.Count > 0)
                    {
                        EraseTiles(tilesToErase);
                        uiManager.UpdateScore(score); // ���� ������Ʈ

                        if (IsStageCleared())
                        {
                            uiManager.EndGame(true);
                        }
                    }
                    else
                    {
                        uiManager.GetPenaltiy();
                        Debug.Log("No matching adjacent groups found. Penalty applied.");
                    }
                }
            }
        }
    }

    // ȭ�� ��ǥ�� ���� ��ǥ�� ��ȯ
    Vector3 GetWorldPos(Vector3 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
        return worldPos;
    }

    // ���� ��ǥ�� grid ��ǥ�� ��ȯ
    Vector2Int GetGridPos(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.y / cellSize);
        return new Vector2Int(x, y);
    }

    // grid ���� �� ��ȿ�� ��ǥ���� �˻�
    bool IsValidGridPos(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < columns && pos.y >= 0 && pos.y < rows;
    }

    // ���� Ÿ�ϵ��� ��ȯ
    List<Vector2Int> GetAdjacentTiles(Vector2Int pos)
    {
        List<Vector2Int> matched = new List<Vector2Int>();
        Vector2Int[] directions = {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0)
        };

        Debug.Log("Checking adjacent tiles from " + pos);

        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = pos + dir;
            while (IsValidGridPos(checkPos))
            {
                if (StageGenerator.grid[checkPos.y, checkPos.x] != TileColor.None)
                {
                    Debug.Log("Matched tile found at " + checkPos + " color: " + StageGenerator.grid[checkPos.y, checkPos.x]);
                    matched.Add(checkPos);
                    break;
                }
                checkPos += dir;
            }
        }
        return matched;
    }

    // grid���� Ÿ�� ���� (Ÿ�� ������ None���� �����ϰ�, �ش� GameObject ����)
    void EraseTiles(List<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions)
        {
            StageGenerator.grid[pos.y, pos.x] = TileColor.None;
            GameObject tileObj = StageGenerator.tileObjects[pos.y, pos.x];
            if (tileObj != null)
            {
                Destroy(tileObj);
                score += tileScore;
                StageGenerator.tileObjects[pos.y, pos.x] = null;
            }
        }
    }

    // �������� Ŭ���� ���� �˻�
    bool IsStageCleared()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                if (StageGenerator.grid[y, x] != TileColor.None)
                    return false;
            }
        }
        return true;
    } 
}
