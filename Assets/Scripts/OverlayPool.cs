// OverlayPool.cs
using System.Collections.Generic;
using UnityEngine;

public class OverlayPool : MonoBehaviour
{
    [Header("Pool Settings")]
    public GameObject overlayPrefab; // �������� ������
    public Transform overlayContainer; // �������̵��� ���� �θ� ������Ʈ
    public int initialSize = 30; // �ʱ� Ǯ ũ��

    private Queue<GameObject> _pool = new Queue<GameObject>();

    void Awake()
    {
        // �̸� Ǯ ä���α�
        for (int i = 0; i < initialSize; i++)
        {
            var go = Instantiate(overlayPrefab, overlayContainer);
            go.SetActive(false);
            _pool.Enqueue(go);
        }
    }

    // Ǯ���� ���� Ȱ��ȭ
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

    // ����� ���� �� ��Ȱ��ȭ�ϰ� �ݳ�
    public void Return(GameObject go)
    {
        go.SetActive(false);
        _pool.Enqueue(go);
    }
}
