using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{

    public GameObject fellingTreePrefab = null; // Prefab 'lumber'

    private void Start()
    {
        fellingTreePrefab = Resources.Load<GameObject>("Prefab/Lumber");
    }

    public void fellingTree()
    {
        // ���� ��ġ�� ����.
        Vector3 pos = transform.position;
        pos.y = 0.5f;
        
        GameObject go = GameObject.Instantiate(this.fellingTreePrefab, pos + new Vector3(1.0f, 0, -1.7f), Quaternion.Euler(90, 0, 0)) as GameObject;
        // ö������ ��ġ�� �̵�.
        go.name = fellingTreePrefab.name;
        Destroy(this.gameObject);
    }
}
