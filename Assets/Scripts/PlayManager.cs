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
    public float playTime; // 게임 시간 (초)
    public float timeRemaining; // 남은 시간
    public float penaltyTime = 5f;    // 틀린 클릭 시 감점 시간

    public int score = 0; // 점수
    public int tileScore = 1; // 타일 개당 점수
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

        // 타이머 업데이트
        timeRemaining -= Time.deltaTime;
        timerSlider.value = timeRemaining / playTime; // 180초로 고정
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
                Debug.Log("Clicked grid position: " + gridPos);

                // 클릭한 위치가 빈 칸일 때만 인접 타일 탐색
                if (StageGenerator.grid[gridPos.y, gridPos.x] == TileColor.None)
                {
                    // 상하좌우 방향에서 가장 가까운 타일들을 가져옴
                    List<Vector2Int> matchingTiles = GetAdjacentTiles(gridPos);
                    Debug.Log("Total adjacent tiles found: " + matchingTiles.Count);

                    // 각 인접 타일의 색상을 기준으로 그룹화
                    Dictionary<TileColor, List<Vector2Int>> groups = new Dictionary<TileColor, List<Vector2Int>>();
                    foreach (var pos in matchingTiles)
                    {
                        TileColor tileColor = StageGenerator.grid[pos.y, pos.x];
                        // 이미 그룹에 있다면 추가, 아니면 새로 생성
                        if (!groups.ContainsKey(tileColor))
                            groups[tileColor] = new List<Vector2Int>();

                        groups[tileColor].Add(pos);
                    }

                    // 그룹 내 타일이 2개 이상인 경우만 지울 대상으로 선정
                    List<Vector2Int> tilesToErase = new List<Vector2Int>();
                    foreach (var kvp in groups)
                    {
                        if (kvp.Value.Count >= 2)
                        {
                            Debug.Log("Removing group of color: " + kvp.Key + ", count: " + kvp.Value.Count);
                            tilesToErase.AddRange(kvp.Value);
                        }
                    }

                    // 타일을 제거하거나, 해당하는 그룹이 없으면 페널티 적용
                    if (tilesToErase.Count > 0)
                    {
                        EraseTiles(tilesToErase);
                        scoreText.text = score.ToString();

                        // 모든 타일 제거 여부 확인 (승리 조건)
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

    // 클릭한 위치에서 상하좌우 방향으로 가장 가까운 타일들을 반환(한 방향에서 첫번째로 만난 타일만 검사)
    List<Vector2Int> GetAdjacentTiles(Vector2Int pos)
    {
        List<Vector2Int> matched = new List<Vector2Int>();

        // 방향 배열: up, down, left, right
        Vector2Int[] directions = {
            new Vector2Int(0, 1),   // 위
            new Vector2Int(0, -1),  // 아래
            new Vector2Int(-1, 0),  // 왼쪽
            new Vector2Int(1, 0)    // 오른쪽
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
            // tileObjects 배열에서 해당 위치의 GameObject를 찾아 삭제
            GameObject tileObj = StageGenerator.tileObjects[pos.y, pos.x];
            if (tileObj != null)
            {
                Destroy(tileObj);
                score += tileScore; // 타일 제거 시 점수 추가
                StageGenerator.tileObjects[pos.y, pos.x] = null;
            }
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
        // 추가 종료 처리 (씬 전환, UI 표시 등)
        enabled = false;
    }
}
