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
    public float timeRemaining = 60f; // 예시 시간 제한 (초)
    public float penaltyTime = 5f;    // 틀린 클릭 시 감점 시간

    void Update()
    {
        HandleInput();

        // 타이머 업데이트
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            EndGame(false);
        }
    }

    // 터치 또는 마우스 클릭 입력 처리
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
                    // 상하좌우 방향에서 가장 가까운 타일 중 색상이 같은 것 찾기
                    List<Vector2Int> matchingTiles = GetMatchingAdjacentTiles(gridPos, clickedColor);
                    if (matchingTiles.Count > 0)
                    {
                        // 클릭한 타일도 포함하여 제거 대상에 추가
                        matchingTiles.Add(gridPos);
                        EraseTiles(matchingTiles);

                        // 모든 타일 제거 여부 확인 (승리 조건)
                        if (IsStageCleared())
                        {
                            EndGame(true);
                        }
                    }
                    else
                    {
                        // 매칭되는 타일이 없으면 페널티
                        timeRemaining -= penaltyTime;
                        Debug.Log("Wrong click! Penalty applied.");
                    }
                }
            }
        }
    }

    // 화면 좌표를 월드 좌표로 변환
    Vector3 GetWorldPos(Vector3 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0; // 2D 게임이므로 z는 0으로
        return worldPos;
    }

    // 월드 좌표를 grid 좌표로 변환 (각 칸의 크기를 cellSize로 나누어 계산)
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

    // 클릭한 위치에서 상하좌우 방향으로 가장 가까운 타일 중,
    // targetColor와 같은 색상을 가진 타일들을 반환 (한 방향에서 첫번째로 만난 타일만 검사)
    List<Vector2Int> GetMatchingAdjacentTiles(Vector2Int pos, TileColor targetColor)
    {
        List<Vector2Int> matched = new List<Vector2Int>();

        // 방향 배열: up, down, left, right
        Vector2Int[] directions = {
            new Vector2Int(0, 1),   // 위
            new Vector2Int(0, -1),  // 아래
            new Vector2Int(-1, 0),  // 왼쪽
            new Vector2Int(1, 0)    // 오른쪽
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
                    break; // 해당 방향에서 첫 타일을 만난 후 중단
                }
                checkPos += dir;
            }
        }
        return matched;
    }

    // grid에서 지정된 좌표의 타일을 제거 (타일 색상을 None으로 바꾸고, 필요 시 오브젝트도 제거)
    void EraseTiles(List<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions)
        {
            StageGenerator.grid[pos.y, pos.x] = TileColor.None;
            // 만약 타일 오브젝트에 태그나 별도 관리가 있다면, 해당 오브젝트를 찾아 제거할 수 있음
        }
    }

    // grid에 남은 타일이 없는지 확인 (승리 조건)
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

    // 게임 종료 처리 (win: true면 승리, false면 패배)
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
        // 추가 종료 처리 (씬 전환, UI 표시 등)
        enabled = false;
    }
}
