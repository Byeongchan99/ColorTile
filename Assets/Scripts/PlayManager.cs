using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class PlayManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public float cellSize = 0.48f;
    public int rows = 20;
    public int columns = 10;

    [Header("Game Timer")]
    public float timeRemaining = 60f; // ���� �ð� ���� (��)
    public float penaltyTime = 5f;    // Ʋ�� Ŭ�� �� ���� �ð�

    void Update()
    {
        HandleInput();

        // Ÿ�̸� ������Ʈ
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            EndGame(false);
        }
    }

    // ��ġ �Ǵ� ���콺 Ŭ�� �Է� ó��
    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPos = Input.mousePosition;
            Vector3 worldPos = GetWorldPos(screenPos);
            Vector2Int gridPos = GetGridPos(worldPos);

            if (IsValidGridPos(gridPos))
            {
                TileColor clickedColor = StageGenerator.grid[gridPos.y, gridPos.x];
                if (clickedColor != TileColor.None)
                {
                    // �����¿� ���⿡�� ���� ����� Ÿ�� �� ������ ���� �� ã��
                    List<Vector2Int> matchingTiles = GetMatchingAdjacentTiles(gridPos, clickedColor);
                    if (matchingTiles.Count > 0)
                    {
                        // Ŭ���� Ÿ�ϵ� �����Ͽ� ���� ��� �߰�
                        matchingTiles.Add(gridPos);
                        EraseTiles(matchingTiles);

                        // ��� Ÿ�� ���� ���� Ȯ�� (�¸� ����)
                        if (IsStageCleared())
                        {
                            EndGame(true);
                        }
                    }
                    else
                    {
                        // ��Ī�Ǵ� Ÿ���� ������ ���Ƽ
                        timeRemaining -= penaltyTime;
                        Debug.Log("Wrong click! Penalty applied.");
                    }
                }
            }
        }
    }

    // ȭ�� ��ǥ�� ���� ��ǥ�� ��ȯ
    Vector3 GetWorldPos(Vector3 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0; // 2D �����̹Ƿ� z�� 0����
        return worldPos;
    }

    // ���� ��ǥ�� grid ��ǥ�� ��ȯ (�� ĭ�� ũ�⸦ cellSize�� ������ ���)
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

    // Ŭ���� ��ġ���� �����¿� �������� ���� ����� Ÿ�� ��,
    // targetColor�� ���� ������ ���� Ÿ�ϵ��� ��ȯ (�� ���⿡�� ù��°�� ���� Ÿ�ϸ� �˻�)
    List<Vector2Int> GetMatchingAdjacentTiles(Vector2Int pos, TileColor targetColor)
    {
        List<Vector2Int> matched = new List<Vector2Int>();

        // ���� �迭: up, down, left, right
        Vector2Int[] directions = {
            new Vector2Int(0, 1),   // ��
            new Vector2Int(0, -1),  // �Ʒ�
            new Vector2Int(-1, 0),  // ����
            new Vector2Int(1, 0)    // ������
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = pos + dir;
            while (IsValidGridPos(checkPos))
            {
                if (StageGenerator.grid[checkPos.y, checkPos.x] != TileColor.None)
                {
                    if (StageGenerator.grid[checkPos.y, checkPos.x] == targetColor)
                    {
                        matched.Add(checkPos);
                    }
                    break; // �ش� ���⿡�� ù Ÿ���� ���� �� �ߴ�
                }
                checkPos += dir;
            }
        }
        return matched;
    }

    // grid���� ������ ��ǥ�� Ÿ���� ���� (Ÿ�� ������ None���� �ٲٰ�, �ʿ� �� ������Ʈ�� ����)
    void EraseTiles(List<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions)
        {
            StageGenerator.grid[pos.y, pos.x] = TileColor.None;
            // ���� Ÿ�� ������Ʈ�� �±׳� ���� ������ �ִٸ�, �ش� ������Ʈ�� ã�� ������ �� ����
        }
    }

    // grid�� ���� Ÿ���� ������ Ȯ�� (�¸� ����)
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

    // ���� ���� ó�� (win: true�� �¸�, false�� �й�)
    void EndGame(bool win)
    {
        if (win)
        {
            Debug.Log("Stage cleared! You win!");
        }
        else
        {
            Debug.Log("Time's up! Game Over!");
        }
        // �߰� ���� ó�� (�� ��ȯ, UI ǥ�� ��)
        enabled = false;
    }
}
