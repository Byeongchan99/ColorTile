using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public class TilePool : MonoBehaviour
{
    [Serializable]
    class Pool
    {
        public TileColor color;
        public GameObject prefab;
        public int preloadCount;
    }

    [SerializeField] private Pool[] pools;
    private Dictionary<TileColor, Queue<GameObject>> _queues;

    void Awake()
    {
        _queues = pools.ToDictionary(p => p.color, p => new Queue<GameObject>());
        foreach (var p in pools)
            Preload(p.color, p.prefab, p.preloadCount);
    }

    void Preload(TileColor color, GameObject prefab, int count)
    {
        var q = _queues[color];
        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(prefab, transform);
            go.SetActive(false);
            q.Enqueue(go);
        }
    }

    public GameObject Get(TileColor color, Vector3 position)
    {
        var q = _queues[color];
        GameObject tile = q.Count > 0 ? q.Dequeue() : Instantiate(pools.First(p => p.color == color).prefab, transform);
        tile.transform.position = position;
        tile.transform.rotation = Quaternion.identity; // Reset rotation
        tile.SetActive(true);
        return tile;
    }

    public void Return(GameObject tile, TileColor color)
    {
        tile.SetActive(false);
        _queues[color].Enqueue(tile);
    }
}
