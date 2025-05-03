using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;
using UnityEngine.UI;
using static Enums;

public class PlayManager : MonoBehaviour
{
    [Header("Grid Settings")]
    private float _cellSize = 0.48f;
    private int _rows = 20;
    private int _columns = 10;
    private Vector3 _boardPos;

    [Header("Tile Erase")]
    private static readonly Vector2Int[] _directions = {
      new Vector2Int(0,1), new Vector2Int(0,-1),
      new Vector2Int(-1,0), new Vector2Int(1,0)
    };

    [Tooltip("버퍼 재사용")]
    private Dictionary<TileColor, int> _colorCountDict = new Dictionary<TileColor, int>(); // 타일 색상과 개수를 저장할 딕셔너리
    private Dictionary<TileColor, List<Vector2Int>> _tileGroupsDict = new Dictionary<TileColor, List<Vector2Int>>(); // 타일 색상과 위치를 저장할 딕셔너리
    private HashSet<Vector2Int> _emptyCellsSet = new HashSet<Vector2Int>(); // 검사할 빈 칸들을 저장할 리스트
    private HashSet<Vector2Int> _overlayPathPositionsSet = new HashSet<Vector2Int>(); // 오버레이 표시할 경로를 저장할 리스트
    private List<(GameObject, TileColor color)> _tilesToEraseList = new List<(GameObject, TileColor color)>(); // 제거할 타일과 색상을 저장할 리스트
    private List<Vector2Int> _closestTiles = new List<Vector2Int>(); // 클릭한 위치에서 가장 가까운 타일을 저장할 리스트 / 조기 종료 검사할 때 사용
    private List<Vector2Int> _matchingTiles = new List<Vector2Int>(); // 클릭한 위치에서 가장 가까운 타일을 저장할 리스트 / 타일 제거 시 사용

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
    private int _remainTileCount; // 남은 타일 수

    [Header("Game Timer Settings")]
    [SerializeField] private GameMode _gameMode;
    [SerializeField] float _playTime; // 게임 시간 (초)
    public float timeRemaining; // 남은 시간
    [SerializeField] private float penaltyTime = 5f; // 틀린 클릭 시 감점 시간

    [Header("References")]
    public GameManager gameManager; // isPaused 참조
    public StageGenerator stageGenerator; // grid 참조
    public TilePool tilePool; // 타일 오브젝트 풀

    [Header("Overlay Settings")]
    public OverlayPool overlayPool; // 오버레이 오브젝트 풀
    private List<GameObject> activeOverlayList = new List<GameObject>();

    [Header("Tile Animation Settings")]
    [SerializeField] float _minHeightCoeff;
    [SerializeField] float _maxHeightCoeff;
    [SerializeField] float _minLengthCoeff;
    [SerializeField] float _maxLengthCoeff;

    public void Awake()
    {
        // 참조
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (stageGenerator == null)
            stageGenerator = FindObjectOfType<StageGenerator>();

        // 이벤트 구독
        GameEvents.OnGameStarted += Initialize; // 게임 시작 시 초기화
        GameEvents.OnRetryGame += Initialize; // 게임 재시작 시 초기화
        GameEvents.OnClearBoard += InitStage; // 무한 모드에서 보드 클리어 시 스테이지 초기화
    }

    void Update()
    {
        // 게임이 일시정지 상태인 경우
        if (gameManager.isPaused == true)
        {
            return;
        }

        // 입력 처리
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
            EndGame(GameResult.TimeOver);
        }
    }

    // 초기화
    public void Initialize()
    {
        timeRemaining = _playTime;
        Score = 0;

        InitStage();
    }

    // 스테이지 초기화
    public void InitStage()
    {
        // 게임 모드 설정 및 모드에 따른 값 설정
        _gameMode = GameManager.gameMode;
        _rows = stageGenerator.rows;
        _columns = stageGenerator.columns;
        
        _totalTileCount = stageGenerator.totalTileCount;
        _remainTileCount = _totalTileCount;
        
        // stageGenerator에서 보드판의 칸 크기와 위치를 가져옴
        _cellSize = stageGenerator.cellSize;
        _boardPos = stageGenerator.boardPos.position;

        // 빈 칸 리스트 초기화
        _emptyCellsSet.Clear();
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _columns; x++)
            {
                if (stageGenerator.grid[y, x] == TileColor.None)
                {
                    _emptyCellsSet.Add(new Vector2Int(x, y));
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
            Vector3 worldPos = ScreenToWorldPosition(screenPos);
            Vector2Int gridPos = WorldToGridPosition(worldPos);

            // 유효 범위 검사
            if (IsValidGridPosition(gridPos))
            {
                //Debug.Log("Clicked grid position: " + gridPos);

                // 클릭한 위치가 빈 칸일 때만 진행
                if (stageGenerator.grid[gridPos.y, gridPos.x] == TileColor.None)
                {
                    // 빈 칸에서 가장 가까운 직교 타일 검색(상하좌우 각 4방향에서 가장 가까운 타일 검색)
                    _matchingTiles = GetClosestOrthogonalTiles(gridPos, _matchingTiles);
                    //Debug.Log("Total adjacent tiles found: " + matchingTiles.Count);
                                  
                    // 딕셔너리에 가져온 타일 색상과 위치 저장
                    foreach (var pos in _matchingTiles)
                    {
                        TileColor tileColor = stageGenerator.grid[pos.y, pos.x];
                        
                        if (!_tileGroupsDict.ContainsKey(tileColor)) // 딕셔너리에 없는 색상인 경우 추가
                            _tileGroupsDict[tileColor] = new List<Vector2Int>();

                        _tileGroupsDict[tileColor].Add(pos);
                    }

                    // 같은 색상의 타일이 2개 이상인 경우에만 제거 리스트에 추가
                    List<Vector2Int> tilesToErase = new List<Vector2Int>();
                    foreach (var kvp in _tileGroupsDict) // kvp = KeyValuePair<TileColor, List<Vector2Int>>
                    {
                        if (kvp.Value.Count >= 2)
                        {
                            //Debug.Log("Removing group of color: " + kvp.Key + ", count: " + kvp.Value.Count);
                            tilesToErase.AddRange(kvp.Value);
                        }
                    }

                    _tileGroupsDict.Clear(); // 딕셔너리 초기화

                    // 제거할 타일이 있는 경우
                    if (tilesToErase.Count > 0)
                    {
                        // 오버레이 표시
                        ShowOverlay(gridPos, tilesToErase);

                        // 타일 제거
                        EraseTiles(tilesToErase);
                        
                        // 남은 타일 수 감소
                        _totalTileCount -= tilesToErase.Count; 

                        // 모든 타일이 제거된 경우 - 스테이지 클리어
                        if (_totalTileCount <= 0)
                        {
                            // 스테이지 클리어 처리
                            if (IsStageCleared())
                            {
                                if (_gameMode == GameMode.Normal)
                                {
                                    EndGame(GameResult.Cleared);
                                }
                                else
                                {
                                    GameEvents.OnClearBoardRequest?.Invoke(); // 무한 모드에서 보드 클리어 시 호출
                                }
                            }
                        }              
                    }
                    // 제거할 타일이 없는 경우 = 잘못된 입력 -> 패널티 적용
                    else
                    {
                        GetPenaltiy();
                        //Debug.Log("No matching adjacent groups found. Penalty applied.");
                    }
                }
            }
        }
    }

    #region // 타일 제거 오버레이 관련
    // 타일을 제거하기 전 경로상 모든 칸에 오버레이를 표시
    void ShowOverlay(Vector2Int clickPos, List<Vector2Int> removedTiles)
    {
        _overlayPathPositionsSet.Clear(); // 리스트 초기화
        GetPathPositions(clickPos, removedTiles);

        // 오버레이 표시
        foreach (var cell in _overlayPathPositionsSet)
        {
            Vector3 worldPos = GridToWorldPosition(cell);
            GameObject ov = overlayPool.Get();
            ov.transform.position = worldPos;
            activeOverlayList.Add(ov);
        }
     
        StartCoroutine(HideOverlaysAfterDelay(0.2f));
    }

    // 클릭 위치와 제거될 각 타일의 위치 사이에 놓인 모든 셀의 좌표를 반환
    void GetPathPositions(Vector2Int clickPos, List<Vector2Int> removedTiles)
    {      
        foreach (var tile in removedTiles)
        {
            // 같은 행
            if (tile.x == clickPos.x)
            {
                int minY = Mathf.Min(tile.y, clickPos.y);
                int maxY = Mathf.Max(tile.y, clickPos.y);
                
                for (int y = minY; y <= maxY; y++)
                    _overlayPathPositionsSet.Add(new Vector2Int(clickPos.x, y));
            }
            // 같은 열
            else if (tile.y == clickPos.y)
            {
                int minX = Mathf.Min(tile.x, clickPos.x);
                int maxX = Mathf.Max(tile.x, clickPos.x);
                
                for (int x = minX; x <= maxX; x++)
                    _overlayPathPositionsSet.Add(new Vector2Int(x, clickPos.y));
            }
        }
    }

    // 화면에 띄운 오버레이를 일정 시간 후에 제거
    IEnumerator HideOverlaysAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (var ov in activeOverlayList)
            overlayPool.Return(ov);
        
        activeOverlayList.Clear();
    }

    // 그리드 좌표를 월드 좌표로 변환
    Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(
            _boardPos.x + (gridPos.x + 0.5f) * _cellSize,
            _boardPos.y + (gridPos.y + 0.5f) * _cellSize,
            0f
        );
    }
    #endregion

    // 화면 좌표를 월드 좌표로 변환
    Vector3 ScreenToWorldPosition(Vector3 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
        
        return worldPos;
    }

    // 월드 좌표를 grid 좌표로 변환
    Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        // boardPos.position을 기준으로 로컬 좌표 계산
        Vector3 localPos = worldPos - _boardPos;
        int x = Mathf.FloorToInt(localPos.x / _cellSize);
        int y = Mathf.FloorToInt(localPos.y / _cellSize);
        
        return new Vector2Int(x, y);
    }

    // grid 범위 내 유효한 좌표인지 검사
    bool IsValidGridPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < _columns && pos.y >= 0 && pos.y < _rows;
    }

    // 현재 위치에서 가장 가까운 타일들을 반환
    List<Vector2Int> GetClosestOrthogonalTiles(Vector2Int pos, List<Vector2Int> tileList)
    {
        tileList.Clear();
        //Debug.Log("Checking adjacent tiles from " + pos);

        foreach (Vector2Int dir in _directions)
        {
            Vector2Int nextPos = pos + dir;
            
            while (IsValidGridPosition(nextPos))
            {
                // 빈 칸이 아닌 타일
                if (stageGenerator.grid[nextPos.y, nextPos.x] != TileColor.None)
                {
                    //Debug.Log("Matched tile found at " + checkPos + " color: " + stageGenerator.grid[checkPos.y, checkPos.x]);
                    tileList.Add(nextPos);
                    break;
                }

                nextPos += dir;
            }
        }
        return tileList;
    }

    // grid에서 타일 제거(타일 색상을 None으로 변경하고, 해당 GameObject 제거)
    void EraseTiles(List<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions)
        {
            TileColor color = stageGenerator.grid[pos.y, pos.x];
            stageGenerator.grid[pos.y, pos.x] = TileColor.None;

            GameObject tileObj = stageGenerator.tileObjects[pos.y, pos.x];
            stageGenerator.tileObjects[pos.y, pos.x] = null;
            
            if (tileObj != null)
            {
                _tilesToEraseList.Add((tileObj, color));            
                Score += _tileScore;
                _remainTileCount--;
                _emptyCellsSet.Add(pos); // 제거된 타일의 위치를 검사할 빈 칸 리스트에 추가
            }
        }

        // 효과음 재생
        GameEvents.OnPlaySFX?.Invoke(1); // SFX 인덱스 1으로 재생
        // 떨어지는 애니메이션 코루틴 실행
        StartCoroutine(DropTileAndDestroy());

        // 남은 후보 리스트에서 제거 가능한 타일이 없으면 게임 종료
        if (HasNoRemovableTiles() && _remainTileCount > 0)
        {
            EndGame(GameResult.NoRemovableTiles);
        }
    }

    #region // 타일 제거 애니메이션
    // 타일을 떨어뜨리고 파괴하는 애니메이션 코루틴
    IEnumerator DropTileAndDestroy()
    {      
        foreach (var(tile, color) in _tilesToEraseList)
        {
            float duration = 0.5f;
            float delay = 0f;
            StartCoroutine(MoveTileParabola(tile, duration, delay, color));
        }

        // 애니메이션이 끝난 후 리스트 초기화
        _tilesToEraseList.Clear();
        yield return null;
    }

    // 포물선 움직임 구현
    IEnumerator MoveTileParabola(GameObject tile, float duration, float delay, TileColor color)
    {
        yield return new WaitForSeconds(delay);

        // 변수 설정
        float minHeight = _cellSize * _minHeightCoeff; // 튕겨오를 최소 높이
        float maxHeight = _cellSize * _maxHeightCoeff; // 튕겨오를 최대 높이
        float bounceHeight = Random.Range(minHeight, maxHeight); // 튕겨오를 높이
        float minLength = _cellSize * _minLengthCoeff; // 튕겨나갈 최소 거리
        float maxLength = _cellSize * _maxLengthCoeff; // 튕겨나갈 최대 거리
        int direction = Random.Range(0, 1) > 0.5 ? -1 : 1; // 튕겨나갈 방향
        float bounceLength = _cellSize * direction * Random.Range(minLength, maxLength); // 튕겨나갈 거리
        float downDistance = _rows * _cellSize + 1f; // 떨어지는 거리

        Vector3 startPos = tile.transform.position;
        Vector3 endPos = startPos + new Vector3(bounceLength, -downDistance, 0);

        float g = -2f * bounceHeight / (duration * duration);
        // 포물선 최고점(peakHeight)을 만들기 위한 중력값 → negative
        // peakHeight = -0.5 * g * (duration/2)^2  에서 유도됨

        Vector3 accel = new Vector3(0, g, 0);
        // 초기 속도: v0 = (endPos - startPos - 0.5*a*T^2) / T
        Vector3 v0 = (endPos - startPos - 0.5f * accel * duration * duration) / duration;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // 포물선 운동 위치 계산
            Vector3 pos = startPos
                        + v0 * elapsed
                        + 0.5f * accel * elapsed * elapsed;

            tile.transform.position = pos;
            tile.transform.Rotate(Vector3.forward, 90f * Time.deltaTime); // 조금 회전

            yield return null;
        }

        tilePool.Return(tile, color); // 타일 풀에 반환
    }
    #endregion

    #region // 게임 조기 종료 검사
    bool isEmptyLine(Vector2Int pos)
    {
        // 해당 빈 칸의 행과 열에 타일이 없는지 검사
        for (int x = 0; x < _columns; x++)
        {
            if (stageGenerator.grid[pos.y, x] != TileColor.None && stageGenerator.tileObjects[pos.y, x] != null)
                return false;
        }
        
        for (int y = 0; y < _rows; y++)
        {
            if (stageGenerator.grid[y, pos.x] != TileColor.None && stageGenerator.tileObjects[y, pos.x] != null)
                return false;
        }

        return true;
    }

    // 빈 칸 리스트만 순회하여 더 이상 제거 가능한 타일이 없는지 검사
    bool HasNoRemovableTiles()
    {
        // 빈 칸이 하나도 없으면 제거할 수 있는 타일이 없는 것으로 판단
        if (_emptyCellsSet.Count == 0)
            return true;

        // 검사할 빈 칸의 행과 열에 타일이 없는 경우는 리스트에서 제거
        _emptyCellsSet.RemoveWhere(pos => isEmptyLine(pos));

        foreach (var emptyPos in _emptyCellsSet)
        {
            _colorCountDict.Clear(); // 리스트 초기화

            _closestTiles = GetClosestOrthogonalTiles(emptyPos, _closestTiles);
            
            foreach (Vector2Int tilePos in _closestTiles)
            {
                TileColor color = stageGenerator.grid[tilePos.y, tilePos.x];
                
                if (color == TileColor.None) continue;
                
                if (!_colorCountDict.ContainsKey(color))
                    _colorCountDict[color] = 0;

                _colorCountDict[color]++;
            }

            // 동일 색상의 타일이 2개 이상 존재하면 제거 가능
            foreach (var kvp in _colorCountDict)
            {
                if (kvp.Value >= 2)
                    return false;
            }
        }      

        // 모든 빈 칸에서 검사했지만 제거 가능한 타일이 없다면
        return true;
    }
    #endregion

    // 패널티 적용
    void GetPenaltiy()
    {
        // 무한 모드에서는 패널티 없음  
        if (_gameMode == GameMode.Infinite)
            return;

        // 효과음 재생
        GameEvents.OnPlaySFX?.Invoke(2);
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
    void EndGame(GameResult result)
    {
        // 게임 종료 처리
        GameEvents.OnGameEnded?.Invoke(result);
    }
}
