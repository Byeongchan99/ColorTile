using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class StageGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int rows = 20;
    public int columns = 10;
    public float cellSize = 0.48f; // 한 칸의 크기

    [Header("Tile Pair Settings")]
    public int pairCount = 7; // 각 색상별 쌍 개수
    public int colorCount = 10; // 사용 색상 개수 (None 제외)

    [Header("Tile Prefab")]
    public GameObject[] tilePrefabs; // 타일 프리팹 (Inspector에 할당)

    // 스테이지 전체를 관리하는 2차원 배열 (PlayManager에서 참조)
    public static TileColor[,] grid;

    void Start()
    {
        GenerateStage();
    }

    // 스테이지를 생성하는 메서드
    void GenerateStage()
    {
        // 1. grid 초기화 (모든 칸을 None으로)
        grid = new TileColor[rows, columns];
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                grid[y, x] = TileColor.None;
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

        if (tilePrefabs[(int)color - 1] != null)
        {
            Instantiate(tilePrefabs[(int)color - 1], worldPos1, Quaternion.identity);
            Instantiate(tilePrefabs[(int)color - 1], worldPos2, Quaternion.identity);
        }
    }

    // grid 좌표 → 월드 좌표 변환
    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        float stageWidth = columns * cellSize;
        float stageHeight = rows * cellSize;
        // 각 타일의 원래 위치: (gridPos.x * cellSize + cellSize/2, gridPos.y * cellSize + cellSize/2)
        // 이를 스테이지 중앙 정렬을 위해 전체 크기의 절반만큼 빼줍니다.
        return new Vector3(gridPos.x * cellSize + cellSize / 2, gridPos.y * cellSize + cellSize / 2, 0)
               - new Vector3(stageWidth / 2, stageHeight / 2, 0);
    }

}
