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

    public int score = 0; // 점수
    public int tileScore = 1; // 타일 당 점수

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

    // 터치 또는 마우스 클릭 입력 처리
    void HandleInput()
    {
        // UI 요소 위에서 클릭한 경우, 입력 무시
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

                // 클릭한 위치가 빈 칸일 때만 인접 타일 탐색
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
                        uiManager.UpdateScore(score); // 점수 업데이트

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

    // 화면 좌표를 월드 좌표로 변환
    Vector3 GetWorldPos(Vector3 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
        return worldPos;
    }

    // 월드 좌표를 grid 좌표로 변환
    Vector2Int GetGridPos(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.y / cellSize);
        return new Vector2Int(x, y);
    }

    // grid 범위 내 유효한 좌표인지 검사
    bool IsValidGridPos(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < columns && pos.y >= 0 && pos.y < rows;
    }

    // 인접 타일들을 반환
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

    // grid에서 타일 제거 (타일 색상을 None으로 변경하고, 해당 GameObject 제거)
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

    // 스테이지 클리어 여부 검사
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
