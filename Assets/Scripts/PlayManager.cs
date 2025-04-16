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
    private HashSet<Vector2Int> candidateEmptyCells = new HashSet<Vector2Int>(); // �ĺ� �� ĭ���� ������ ����Ʈ

    [Header("Game Score Settings")]
    private int _score = 0; // ����
    public int Score
    {
        get { return _score; }
        set
        {
            _score = value;
            GameEvents.OnScoreChanged?.Invoke(_score);
        }
    }
    private int _tileScore = 1; // Ÿ�� �� ����
    private int _totalTileCount; // �� Ÿ�� ��

    [Header("Game Timer Settings")]
    [SerializeField] private GameMode _gameMode;
    [SerializeField] float _playTime; // ���� �ð� (��)
    public float timeRemaining; // ���� �ð�
    [SerializeField] private float penaltyTime = 5f; // Ʋ�� Ŭ�� �� ���� �ð�

    [Header("References")]
    public GameManager gameManager; // isPaused ����
    public StageGenerator stageGenerator; // grid ����

    public void Awake()
    {
        // ����
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (stageGenerator == null)
            stageGenerator = FindObjectOfType<StageGenerator>();

        _rows = stageGenerator.rows;
        _columns = stageGenerator.columns;
        _cellSize = stageGenerator.cellSize;
        _boardPos = stageGenerator.boardPos.position;

        GameEvents.OnGameStarted += Initialize; // ���� ���� �� �ʱ�ȭ
        GameEvents.OnRetryGame += Initialize; // ���� ����� �� �ʱ�ȭ

        Initialize();
    }

    void Update()
    {
        // ������ �Ͻ����� ������ ���
        if (gameManager.isPaused == true)
        {
            return;
        }

        HandleInput();

        if (_gameMode == GameMode.Infinite)
        {
            return; // ���� ��忡���� Ÿ�̸Ӹ� ������� ����
        }

        // Ÿ�̸� ������Ʈ
        timeRemaining -= Time.deltaTime;
        GameEvents.OnTimerUpdated?.Invoke(timeRemaining / _playTime);

        if (timeRemaining <= 0) // �ð� ����� ���� ���� ����
        {
            EndGame(false);
        }
    }

    // �ʱ�ȭ
    public void Initialize()
    {
        timeRemaining = _playTime;
        Score = 0;
        _totalTileCount = stageGenerator.totalTileCount;

        // �� ĭ ����Ʈ �ʱ�ȭ
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

    // ��ġ �Ǵ� ���콺 Ŭ�� �Է� ó�� - ���� ���콺 �̺�Ʈ�� ó��
    void HandleInput()
    {
        /*
        // UI ��� ������ Ŭ���� ��� �Է� ����
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

            // ��ȿ ���� �˻�
            if (IsValidGridPos(gridPos))
            {
                Debug.Log("Clicked grid position: " + gridPos);

                // Ŭ���� ��ġ�� �� ĭ�� ���� ����
                if (stageGenerator.grid[gridPos.y, gridPos.x] == TileColor.None)
                {
                    // �� ĭ���� ���� ����� ���� Ÿ�� �˻�(�����¿� �� 4���⿡�� ���� ����� Ÿ�� �˻�)
                    List<Vector2Int> matchingTiles = GetOrthogonalTiles(gridPos);
                    Debug.Log("Total adjacent tiles found: " + matchingTiles.Count);

                    // ��ųʸ��� ������ Ÿ�� ����� ��ġ ����
                    Dictionary<TileColor, List<Vector2Int>> groups = new Dictionary<TileColor, List<Vector2Int>>();
                    foreach (var pos in matchingTiles)
                    {
                        TileColor tileColor = stageGenerator.grid[pos.y, pos.x];
                        if (!groups.ContainsKey(tileColor))
                            groups[tileColor] = new List<Vector2Int>();
                        groups[tileColor].Add(pos);
                    }

                    // ���� ������ Ÿ���� 2�� �̻��� ��쿡�� ���� ����Ʈ�� �߰�
                    List<Vector2Int> tilesToErase = new List<Vector2Int>();
                    foreach (var kvp in groups)
                    {
                        if (kvp.Value.Count >= 2)
                        {
                            Debug.Log("Removing group of color: " + kvp.Key + ", count: " + kvp.Value.Count);
                            tilesToErase.AddRange(kvp.Value);
                        }
                    }

                    // ������ Ÿ���� �ִ� ���
                    if (tilesToErase.Count > 0)
                    {
                        EraseTiles(tilesToErase);
                        //uiManager.UpdateScore(_score); // ���� ������Ʈ - ������Ƽ���� �ٷ� ó��

                        // ���� Ÿ�� �� ����
                        _totalTileCount -= tilesToErase.Count; 

                        // ��� Ÿ���� ���ŵ� ��� - �������� Ŭ����
                        if (_totalTileCount <= 0)
                        {
                            // �������� Ŭ���� ó��
                            if (IsStageCleared())
                            {
                                EndGame(true);
                            }
                        }              
                    }
                    // ������ Ÿ���� ���� ��� - �߸��� �Է� -> �г�Ƽ ����
                    else
                    {
                        GetPenaltiy();
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
        // boardPos.position�� �������� ���� ��ǥ ���
        Vector3 localPos = worldPos - _boardPos;
        int x = Mathf.FloorToInt(localPos.x / _cellSize);
        int y = Mathf.FloorToInt(localPos.y / _cellSize);
        return new Vector2Int(x, y);
    }

    // grid ���� �� ��ȿ�� ��ǥ���� �˻�
    bool IsValidGridPos(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < _columns && pos.y >= 0 && pos.y < _rows;
    }

    // ���� ��ġ���� ���� ����� Ÿ�ϵ��� ��ȯ
    List<Vector2Int> GetOrthogonalTiles(Vector2Int pos)
    {
        List<Vector2Int> matched = new List<Vector2Int>();
        // �����¿� 4���� Ž��
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
                // �� ĭ�� �ƴ� Ÿ��
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

    // grid���� Ÿ�� ����(Ÿ�� ������ None���� �����ϰ�, �ش� GameObject ����)
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
                candidateEmptyCells.Add(pos); // ���ŵ� Ÿ���� ��ġ�� �ĺ� �� ĭ ����Ʈ�� �߰�
                stageGenerator.tileObjects[pos.y, pos.x] = null;
            }
        }

        // ���� �ĺ� ����Ʈ���� ���� ������ Ÿ���� ������ ���� ����
        if (HasNoRemovableTiles())
        {
            EndGame(false);
        }
    }

    // �ĺ� �� ĭ ����Ʈ�� ��ȸ�Ͽ� �� �̻� ���� ������ Ÿ���� ������ �˻�
    bool HasNoRemovableTiles()
    {
        // �ĺ� �� ĭ�� �ϳ��� ������ ������ �� �ִ� Ÿ���� ���� ������ �Ǵ�
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

            // ���� ������ Ÿ���� 2�� �̻� �����ϸ� �̵�(����)�� ������ ������ �Ǵ�
            foreach (var kvp in colorCount)
            {
                if (kvp.Value >= 2)
                    return false;
            }
        }
        // ��� �ĺ����� �˻������� ���� ������ �׷��� ���ٸ�, �� �̻� ������ �� ����.
        return true;
    }

    // �г�Ƽ ����
    void GetPenaltiy()
    {
        timeRemaining -= penaltyTime;
    }

    // �������� Ŭ���� ���� �˻�
    // ��� Ÿ���� ���ŵ� ��� Ŭ����
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

    // ���� ����
    void EndGame(bool result)
    {
        // ���� ���� ó��
        GameEvents.OnGameEnded?.Invoke(result);
    }
}
