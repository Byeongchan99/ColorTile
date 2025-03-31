using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class PlayManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public float cellSize = 0.48f;
    public int rows = 20;
    public int columns = 10;

    [Header("Game Timer")]
    public TMP_Text resultText;
    public Slider timerSlider;
    public float playTime; // ���� �ð� (��)
    public float timeRemaining; // ���� �ð�
    public float penaltyTime = 5f;    // Ʋ�� Ŭ�� �� ���� �ð�

    public int score = 0; // ����
    public int tileScore = 1; // Ÿ�� ���� ����
    public TMP_Text scoreText;

    public void Awake()
    {
        resultText.gameObject.SetActive(false);
        timeRemaining = playTime;
        scoreText.text = "0";
    }

    void Update()
    {
        HandleInput();

        // Ÿ�̸� ������Ʈ
        timeRemaining -= Time.deltaTime;
        timerSlider.value = timeRemaining / playTime; // 180�ʷ� ����
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
                Debug.Log("Clicked grid position: " + gridPos);

                // Ŭ���� ��ġ�� �� ĭ�� ���� ���� Ÿ�� Ž��
                if (StageGenerator.grid[gridPos.y, gridPos.x] == TileColor.None)
                {
                    // �����¿� ���⿡�� ���� ����� Ÿ�ϵ��� ������
                    List<Vector2Int> matchingTiles = GetAdjacentTiles(gridPos);
                    Debug.Log("Total adjacent tiles found: " + matchingTiles.Count);

                    // �� ���� Ÿ���� ������ �������� �׷�ȭ
                    Dictionary<TileColor, List<Vector2Int>> groups = new Dictionary<TileColor, List<Vector2Int>>();
                    foreach (var pos in matchingTiles)
                    {
                        TileColor tileColor = StageGenerator.grid[pos.y, pos.x];
                        // �̹� �׷쿡 �ִٸ� �߰�, �ƴϸ� ���� ����
                        if (!groups.ContainsKey(tileColor))
                            groups[tileColor] = new List<Vector2Int>();

                        groups[tileColor].Add(pos);
                    }

                    // �׷� �� Ÿ���� 2�� �̻��� ��츸 ���� ������� ����
                    List<Vector2Int> tilesToErase = new List<Vector2Int>();
                    foreach (var kvp in groups)
                    {
                        if (kvp.Value.Count >= 2)
                        {
                            Debug.Log("Removing group of color: " + kvp.Key + ", count: " + kvp.Value.Count);
                            tilesToErase.AddRange(kvp.Value);
                        }
                    }

                    // Ÿ���� �����ϰų�, �ش��ϴ� �׷��� ������ ���Ƽ ����
                    if (tilesToErase.Count > 0)
                    {
                        EraseTiles(tilesToErase);
                        scoreText.text = score.ToString();

                        // ��� Ÿ�� ���� ���� Ȯ�� (�¸� ����)
                        if (IsStageCleared())
                        {
                            EndGame(true);
                        }
                    }
                    else
                    {
                        timeRemaining -= penaltyTime;
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

    // Ŭ���� ��ġ���� �����¿� �������� ���� ����� Ÿ�ϵ��� ��ȯ(�� ���⿡�� ù��°�� ���� Ÿ�ϸ� �˻�)
    List<Vector2Int> GetAdjacentTiles(Vector2Int pos)
    {
        List<Vector2Int> matched = new List<Vector2Int>();

        // ���� �迭: up, down, left, right
        Vector2Int[] directions = {
            new Vector2Int(0, 1),   // ��
            new Vector2Int(0, -1),  // �Ʒ�
            new Vector2Int(-1, 0),  // ����
            new Vector2Int(1, 0)    // ������
        };

        Debug.Log("Checking adjacent tiles from " + pos);

        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = pos + dir;
            while (IsValidGridPos(checkPos))
            {
                if (StageGenerator.grid[checkPos.y, checkPos.x] != TileColor.None)
                {
                    Debug.Log("Matched tile found at " + checkPos + " color" + StageGenerator.grid[checkPos.y, checkPos.x]);
                    matched.Add(checkPos);
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
            // tileObjects �迭���� �ش� ��ġ�� GameObject�� ã�� ����
            GameObject tileObj = StageGenerator.tileObjects[pos.y, pos.x];
            if (tileObj != null)
            {
                Destroy(tileObj);
                score += tileScore; // Ÿ�� ���� �� ���� �߰�
                StageGenerator.tileObjects[pos.y, pos.x] = null;
            }
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
        resultText.gameObject.SetActive(true);
        if (win)
        {
            resultText.text = "Stage cleared!\nYou win!\n" + "score: " + score;
            Debug.Log("Stage cleared! You win!");
        }
        else
        {
            resultText.text = "Time's up!\nGame Over!\n" + "score: " + score;
            Debug.Log("Time's up! \n Game Over!");
        }
        // �߰� ���� ó�� (�� ��ȯ, UI ǥ�� ��)
        enabled = false;
    }
}
