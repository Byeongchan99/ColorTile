// OverlayPool.cs
using System.Collections.Generic;
using UnityEngine;

public class OverlayPool : MonoBehaviour
{
    [Header("Pool Settings")]
    public GameObject overlayPrefab;
    public Transform overlayContainer; // 인스펙터에서 할당
    public int initialSize = 30;

    Queue<GameObject> _pool = new Queue<GameObject>();

    void Awake()
    {
        // 미리 풀 채워두기
        for (int i = 0; i < initialSize; i++)
        {
            var go = Instantiate(overlayPrefab, overlayContainer);
            go.SetActive(false);
            _pool.Enqueue(go);
        }
    }

    // 풀에서 꺼내 활성화
    public GameObject Get()
    {
        GameObject go;
        if (_pool.Count > 0)
        {
            go = _pool.Dequeue();
            go.SetActive(true);
        }
        else
        {
            go = Instantiate(overlayPrefab, overlayContainer);
        }
        return go;
    }

    // 사용이 끝난 뒤 비활성화하고 반납
    public void Return(GameObject go)
    {
        go.SetActive(false);
        go.transform.SetParent(overlayContainer, worldPositionStays: false);
        _pool.Enqueue(go);
    }
}
