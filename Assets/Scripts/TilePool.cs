using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class TilePool : MonoBehaviour
{
    [Serializable]
    class Pool
    {
        public TileColor color;
        public GameObject prefab;
        public int initialSize;
    }

    [SerializeField] private Pool[] _poolSettings;
    private Dictionary<TileColor, Queue<GameObject>> _pool = new Dictionary<TileColor, Queue<GameObject>>();

    void Awake()
    {
        for (int i = 0; i < _poolSettings.Length; i++)
        {
            var color = _poolSettings[i].color;
            _pool.Add(color, new Queue<GameObject>());
        }

        for (int i = 0; i < _poolSettings.Length; i++)
            Preload(_poolSettings[i].color, _poolSettings[i].prefab, _poolSettings[i].initialSize);
    }

    void Preload(TileColor color, GameObject prefab, int count)
    {
        var q = _pool[color];
        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(prefab, transform);
            go.SetActive(false);
            q.Enqueue(go);
        }
    }

    public GameObject Get(TileColor color, Vector3 position)
    {
        var q = _pool[color];
        GameObject go;

        if (q.Count > 0)
        {
            go = q.Dequeue();
        }
        else
        {
            GameObject prefabToInstantiate = null;
            for (int i = 0; i < _poolSettings.Length; i++)
            {
                if (_poolSettings[i].color == color)
                {
                    prefabToInstantiate = _poolSettings[i].prefab;
                    break;
                }
            }
            go = Instantiate(prefabToInstantiate, transform);
        }

        go.transform.position = position;
        go.transform.rotation = Quaternion.identity;
        go.SetActive(true);

        return go;
    }

    public void Return(GameObject tile, TileColor color)
    {
        tile.SetActive(false);
        _pool[color].Enqueue(tile);
    }
}
