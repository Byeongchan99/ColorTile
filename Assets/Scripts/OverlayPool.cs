// OverlayPool.cs
using System.Collections.Generic;
using UnityEngine;

public class OverlayPool : MonoBehaviour
{
    [Header("Pool Settings")]
    public GameObject overlayPrefab; // 오버레이 프리팹
    public Transform overlayContainer; // 오버레이들을 담을 부모 오브젝트
    public int initialSize = 30; // 초기 풀 크기

    private Queue<GameObject> _pool = new Queue<GameObject>();

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
        _pool.Enqueue(go);
    }
}
