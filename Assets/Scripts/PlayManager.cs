using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Enums;

public class PlayManager : MonoBehaviour
{
    [Header("Grid Settings")]
    private float _cellSize = 0.48f;
    private int _rows = 20;
    private int _columns = 10;
    private Vector3 _boardPos;  
    private HashSet<Vector2Int> candidateEmptyCells = new HashSet<Vector2Int>(); // 후보 빈 칸들을 저장할 리스트

    [Header("Game Score Settings")]
    private int _score = 0; // 점수
    public int Score
    {
        get { return _score; }
        set
        {
            _score = value;
            GameEvents.OnScoreChanged?.Invoke(_score);
        }
    }
    private int _tileScore = 1; // 타일 당 점수
    private int _totalTileCount; // 총 타일 수

    [Header("Game Timer Settings")]
    [SerializeField] private GameMode _gameMode;
    [SerializeField] float _playTime; // 게임 시간 (초)
    public float timeRemaining; // 남은 시간
    [SerializeField] private float penaltyTime = 5f; // 틀린 클릭 시 감점 시간

    [Header("References")]
    public GameManager gameManager; // isPaused 참조
    public StageGenerator stageGenerator; // grid 참조

    public void Awake()
    {
        // 참조
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (stageGenerator == null)
            stageGenerator = FindObjectOfType<StageGenerator>();

        _rows = stageGenerator.rows;
        _columns = stageGenerator.columns;
        _cellSize = stageGenerator.cellSize;
        _boardPos = stageGenerator.boardPos.position;

        GameEvents.OnGameStarted += Initialize; // 게임 시작 시 초기화
        GameEvents.OnRetryGame += Initialize; // 게임 재시작 시 초기화

        Initialize();
    }

    void Update()
    {
        // 게임이 일시정지 상태인 경우
        if (gameManager.isPaused == true)
        {
            return;
        }

        HandleInput();

        if (_gameMode == GameMode.Infinite)
        {
            return; // 무한 모드에서는 타이머를 사용하지 않음
        }

        // 타이머 업데이트
        timeRemaining -= Time.deltaTime;
        GameEvents.OnTimerUpdated?.Invoke(timeRemaining / _playTime);

        if (timeRemaining <= 0) // 시간 종료로 인한 게임 종료
        {
            EndGame(false);
        }
    }

    // 초기화
    public void Initialize()
    {
        timeRemaining = _playTime;
        Score = 0;
        _totalTileCount = stageGenerator.totalTileCount;

        // 빈 칸 리스트 초기화
        candidateEmptyCells.Clear();
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _columns; x++)
            {
                if (stageGenerator.grid[y, x] == TileColor.None)
                {
                    candidateEmptyCells.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    // 터치 또는 마우스 클릭 입력 처리 - 현재 마우스 이벤트만 처리
    void HandleInput()
    {
        /*
        // UI 요소 위에서 클릭한 경우 입력 무시
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Pointer is over UI element. Ignoring input.");
            return;
        }
        */

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPos = Input.mousePosition;
            Vector3 worldPos = GetWorldPos(screenPos);
            Vector2Int gridPos = GetGridPos(worldPos);

            // 유효 범위 검사
            if (IsValidGridPos(gridPos))
            {
                Debug.Log("Clicked grid position: " + gridPos);

                // 클릭한 위치가 빈 칸일 때만 진행
                if (stageGenerator.grid[gridPos.y, gridPos.x] == TileColor.None)
                {
                    // 빈 칸에서 가장 가까운 직교 타일 검색(상하좌우 각 4방향에서 가장 가까운 타일 검색)
                    List<Vector2Int> matchingTiles = GetOrthogonalTiles(gridPos);
                    Debug.Log("Total adjacent tiles found: " + matchingTiles.Count);

                    // 딕셔너리에 가져온 타일 색상과 위치 저장
                    Dictionary<TileColor, List<Vector2Int>> groups = new Dictionary<TileColor, List<Vector2Int>>();
                    foreach (var pos in matchingTiles)
                    {
                        TileColor tileColor = stageGenerator.grid[pos.y, pos.x];
                        if (!groups.ContainsKey(tileColor))
                            groups[tileColor] = new List<Vector2Int>();
                        groups[tileColor].Add(pos);
                    }

                    // 같은 색상의 타일이 2개 이상인 경우에만 제거 리스트에 추가
                    List<Vector2Int> tilesToErase = new List<Vector2Int>();
                    foreach (var kvp in groups)
                    {
                        if (kvp.Value.Count >= 2)
                        {
                            Debug.Log("Removing group of color: " + kvp.Key + ", count: " + kvp.Value.Count);
                            tilesToErase.AddRange(kvp.Value);
                        }
                    }

                    // 제거할 타일이 있는 경우
                    if (tilesToErase.Count > 0)
                    {
                        EraseTiles(tilesToErase);
                        //uiManager.UpdateScore(_score); // 점수 업데이트 - 프로퍼티에서 바로 처리

                        // 남은 타일 수 감소
                        _totalTileCount -= tilesToErase.Count; 

                        // 모든 타일이 제거된 경우 - 스테이지 클리어
                        if (_totalTileCount <= 0)
                        {
                            // 스테이지 클리어 처리
                            if (IsStageCleared())
                            {
                                EndGame(true);
                            }
                        }              
                    }
                    // 제거할 타일이 없는 경우 - 잘못된 입력 -> 패널티 적용
                    else
                    {
                        GetPenaltiy();
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
        // boardPos.position을 기준으로 로컬 좌표 계산
        Vector3 localPos = worldPos - _boardPos;
        int x = Mathf.FloorToInt(localPos.x / _cellSize);
        int y = Mathf.FloorToInt(localPos.y / _cellSize);
        return new Vector2Int(x, y);
    }

    // grid 범위 내 유효한 좌표인지 검사
    bool IsValidGridPos(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < _columns && pos.y >= 0 && pos.y < _rows;
    }

    // 직교 위치에서 가장 가까운 타일들을 반환
    List<Vector2Int> GetOrthogonalTiles(Vector2Int pos)
    {
        List<Vector2Int> matched = new List<Vector2Int>();
        // 상하좌우 4방향 탐색
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
                // 빈 칸이 아닌 타일
                if (stageGenerator.grid[checkPos.y, checkPos.x] != TileColor.None)
                {
                    Debug.Log("Matched tile found at " + checkPos + " color: " + stageGenerator.grid[checkPos.y, checkPos.x]);
                    matched.Add(checkPos);
                    break;
                }
                checkPos += dir;
            }
        }
        return matched;
    }

    // grid에서 타일 제거(타일 색상을 None으로 변경하고, 해당 GameObject 제거)
    void EraseTiles(List<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions)
        {
            stageGenerator.grid[pos.y, pos.x] = TileColor.None;
            GameObject tileObj = stageGenerator.tileObjects[pos.y, pos.x];
            if (tileObj != null)
            {
                Destroy(tileObj);
                Score += _tileScore;
                candidateEmptyCells.Add(pos); // 제거된 타일의 위치를 후보 빈 칸 리스트에 추가
                stageGenerator.tileObjects[pos.y, pos.x] = null;
            }
        }

        // 남은 후보 리스트에서 제거 가능한 타일이 없으면 게임 종료
        if (HasNoRemovableTiles())
        {
            EndGame(false);
        }
    }

    // 후보 빈 칸 리스트만 순회하여 더 이상 제거 가능한 타일이 없는지 검사
    bool HasNoRemovableTiles()
    {
        // 후보 빈 칸이 하나도 없으면 제거할 수 있는 타일이 없는 것으로 판단
        if (candidateEmptyCells.Count == 0)
            return true;

        foreach (Vector2Int emptyPos in candidateEmptyCells)
        {
            List<Vector2Int> adjacentTiles = GetOrthogonalTiles(emptyPos);
            Dictionary<TileColor, int> colorCount = new Dictionary<TileColor, int>();

            foreach (Vector2Int tilePos in adjacentTiles)
            {
                TileColor color = stageGenerator.grid[tilePos.y, tilePos.x];
                if (color == TileColor.None) continue;
                if (!colorCount.ContainsKey(color))
                    colorCount[color] = 0;
                colorCount[color]++;
            }

            // 동일 색상의 타일이 2개 이상 존재하면 이동(제거)이 가능한 것으로 판단
            foreach (var kvp in colorCount)
            {
                if (kvp.Value >= 2)
                    return false;
            }
        }
        // 모든 후보에서 검사했지만 제거 가능한 그룹이 없다면, 더 이상 제거할 수 없음.
        return true;
    }

    // 패널티 적용
    void GetPenaltiy()
    {
        timeRemaining -= penaltyTime;
    }

    // 스테이지 클리어 여부 검사
    // 모든 타일이 제거된 경우 클리어
    bool IsStageCleared()
    {
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _columns; x++)
            {
                if (stageGenerator.grid[y, x] != TileColor.None)
                    return false;
            }
        }
        return true;
    }

    // 게임 종료
    void EndGame(bool result)
    {
        // 게임 종료 처리
        GameEvents.OnGameEnded?.Invoke(result);
    }
}
