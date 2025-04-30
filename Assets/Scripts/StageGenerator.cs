using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class StageGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int rows;
    public int columns;
    public float cellSize; // 한 칸의 크기
    public Transform normalboardPos; // 노말 모드 보드 오브젝트의 위치
    public Transform infiniteboardPos; // 무한 모드 보드 오브젝트의 위치
    public Transform boardPos; // 보드 오브젝트의 위치(Normal 또는 Infinite에 따라 다름)

    [Header("Tile Pair Settings")]
    [SerializeField] private int _pairCount; // 각 색상별 쌍 개수
    [SerializeField] private int _colorCount; // 사용 색상 개수 (None 제외)
    public int totalTileCount; // 총 타일 개수(_pairCount * _colorCount * 2)

    [Header("Tile Prefab")]
    public GameObject[] TilePrefabs; // 타일 프리팹 (Inspector에 할당)
    public Transform tileContainer; // 타일 오브젝트들을 담을 부모 오브젝트

    // 스테이지 전체를 관리하는 2차원 배열 (PlayManager에서 참조)
    public TileColor[,] grid;
    // 실제 타일 오브젝트들을 관리하는 배열
    public GameObject[,] tileObjects;

    private void Awake()
    {
        GameEvents.OnGameStarted += GenerateStage; // 게임 시작 시 초기화
        GameEvents.OnRetryGame += GenerateStage; // 게임 재시작 시 초기화
        GameEvents.OnClearBoard += GenerateStage; // 보드 클리어 시 초기화
    }

    // 스테이지 초기화
    public void InitStage()
    {
        // 모드에 따라 값 설정
        // columns은 동일
        if (GameManager.gameMode == GameMode.Normal)
        {
            _pairCount = 5; // Normal 모드의 쌍 개수
            _colorCount = 10; // Normal 모드의 색상 개수
            rows = 16;
            boardPos = normalboardPos;
        }
        else if (GameManager.gameMode == GameMode.Infinite)
        {
            _pairCount = 5; // Infinite 모드의 쌍 개수
            _colorCount = 10; // Infinite 모드의 색상 개수
            rows = 18;
            boardPos = infiniteboardPos;
        }
        else
        {
            //Debug.LogError("Invalid game mode.");
            return;
        }

        // 1. grid와 tileObjects 초기화 (모든 칸을 None, null로 설정)
        grid = new TileColor[rows, columns];
        tileObjects = new GameObject[rows, columns];
        totalTileCount = _pairCount * _colorCount * 2; // 총 타일 개수 계산

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                grid[y, x] = TileColor.None;
                tileObjects[y, x] = null;
            }
        }

        // 기존의 타일들 제거
        foreach (Transform child in tileContainer)
        {
            Destroy(child.gameObject);
        }
    }

    // 스테이지를 생성하는 메서드 (GenerateStage ver.2)
    public void GenerateStage()
    {
        // 1. 스테이지 초기화
        InitStage();
        // 2. 색상별 쌍 리스트 생성 (총 pairCount * colorCount 쌍)
        List<TileColor> pairColors = new List<TileColor>();
        for (int colorIndex = 1; colorIndex <= _colorCount; colorIndex++)
        {
            for (int i = 0; i < _pairCount; i++)
            {
                pairColors.Add((TileColor)colorIndex);
            }
        }

        // 3. 쌍 순서를 랜덤으로 섞기
        for (int i = 0; i < pairColors.Count; i++)
        {
            TileColor temp = pairColors[i];
            int randomIndex = Random.Range(i, pairColors.Count);
            pairColors[i] = pairColors[randomIndex];
            pairColors[randomIndex] = temp;
        }

        // 4. 각 쌍을 ver.2 로직에 따라 배치
        int MAX_ATTEMPTS = 1000;
        foreach (TileColor color in pairColors)
        {
            bool placed = false;
            int attempt = 0;
            while (!placed && attempt < MAX_ATTEMPTS)
            {
                attempt++;

                // 1: 빈 셀(트리거 셀)을 랜덤 선택
                int triggerX = Random.Range(0, columns);
                int triggerY = Random.Range(0, rows);
                Vector2Int triggerPos = new Vector2Int(triggerX, triggerY);
                if (grid[triggerPos.y, triggerPos.x] != TileColor.None)
                {
                    continue;
                }

                // 2: 상하좌우 방향 중 2가지를 랜덤하게 선택 (순서를 섞어서 첫 두 개 사용)
                List<Vector2Int> directions = new List<Vector2Int>
            {
                new Vector2Int(0, 1),   // 위
                new Vector2Int(0, -1),  // 아래
                new Vector2Int(-1, 0),  // 왼쪽
                new Vector2Int(1, 0)    // 오른쪽
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

                // 3: 각 방향으로 이동할 수 있는 최대 거리를 계산 (트리거 셀 기준)
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

                // 이동 거리는 최소 1 이상이어야 함
                if (maxDist1 < 1 || maxDist2 < 1)
                {
                    continue;
                }

                // 4: 각 방향에 대해, 1부터 최대 거리(maxDist)까지 중 랜덤한 거리 선택
                int dist1 = Random.Range(1, maxDist1 + 1);
                int dist2 = Random.Range(1, maxDist2 + 1);

                // 5: 각 방향으로 이동하며 배치할 위치 결정 (다른 타일을 만나기 전, 혹은 최대 거리 도달 시까지)
                // 첫 번째 방향에 대해:
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

                // 두 번째 방향에 대해:
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

                // 6: 배치할 두 위치가 서로 다르고, 트리거 셀과도 겹치지 않으며, 모두 빈 칸인지 확인
                if (posTile1 == posTile2 || posTile1 == triggerPos || posTile2 == triggerPos)
                {
                    continue;
                }
                if (grid[posTile1.y, posTile1.x] != TileColor.None || grid[posTile2.y, posTile2.x] != TileColor.None)
                {
                    continue;
                }

                // 7: 두 위치에 타일 쌍 배치
                SetTiles(posTile1, posTile2, color);
                placed = true;
            }

            if (!placed)
            {
                Debug.LogWarning("Failed to place a pair of color " + color + " after many attempts.");
            }
        }
    }

    // 스테이지를 생성하는 메서드 (GenerateStage ver.1)
    #region
    /*
    void GenerateStage()
    {
        // 1. grid 초기화 (모든 칸을 None으로)
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

        // 2. 색상별 쌍 리스트 생성 (총 pairCount * colorCount 쌍)
        List<TileColor> pairColors = new List<TileColor>();
        for (int colorIndex = 1; colorIndex <= colorCount; colorIndex++)
        {
            for (int i = 0; i < pairCount; i++)
            {
                pairColors.Add((TileColor)colorIndex);
            }
        }

        // 3. 쌍 순서를 랜덤으로 섞기
        for (int i = 0; i < pairColors.Count; i++)
        {
            TileColor temp = pairColors[i];
            int randomIndex = Random.Range(i, pairColors.Count);
            pairColors[i] = pairColors[randomIndex];
            pairColors[randomIndex] = temp;
        }

        // 4. 각 쌍을 배치
        foreach (TileColor color in pairColors)
        {
            bool placed = false;
            int attempt = 0;
            while (!placed && attempt < 1000)
            { // 무한루프 방지
                attempt++;
                // (1) 직사각형 크기 생성 (1 <= width, height <= MAX, width + height > 3)
                int rectWidth = Random.Range(1, columns + 1);
                int rectHeight = Random.Range(1, rows + 1);
                if (rectWidth + rectHeight <= 3)
                    continue;

                // (2) 직사각형이 grid 안에 들어가도록 원점(바닥 왼쪽) 결정
                int originX = Random.Range(0, columns - rectWidth + 1);
                int originY = Random.Range(0, rows - rectHeight + 1);

                // (3) 대각선 선택: 옵션에 따라 두 모서리 좌표 결정
                bool option = (Random.value < 0.5f);
                Vector2Int pos1, pos2;
                if (option)
                {
                    // 왼쪽 위와 오른쪽 아래
                    pos1 = new Vector2Int(originX, originY + rectHeight - 1);      // top-left
                    pos2 = new Vector2Int(originX + rectWidth - 1, originY);         // bottom-right
                }
                else
                {
                    // 오른쪽 위와 왼쪽 아래
                    pos1 = new Vector2Int(originX + rectWidth - 1, originY + rectHeight - 1); // top-right
                    pos2 = new Vector2Int(originX, originY);                                 // bottom-left
                }

                // (4) 두 위치에 타일을 둘 수 있는지 확인
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

    // 해당 두 위치가 비어있는지 검사
    bool CanSetTiles(Vector2Int pos1, Vector2Int pos2)
    {
        return grid[pos1.y, pos1.x] == TileColor.None && grid[pos2.y, pos2.x] == TileColor.None;
    }

    // grid에 타일 색상을 기록하고, 타일 프리팹을 생성
    void SetTiles(Vector2Int pos1, Vector2Int pos2, TileColor color)
    {
        grid[pos1.y, pos1.x] = color;
        grid[pos2.y, pos2.x] = color;

        // grid 좌표를 월드 좌표로 변환하여 타일 생성 (각 칸의 중심은 (cellSize/2, cellSize/2)만큼 오프셋)
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

    // grid 좌표 → 월드 좌표 변환
    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        // boardPos.position을 기준으로 각 셀의 오프셋을 더함.
        return boardPos.position + new Vector3(gridPos.x * cellSize + cellSize / 2,
                                                 gridPos.y * cellSize + cellSize / 2, 0);
    }
}
