using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockControl : MonoBehaviour
{
    public MapCreator map_creator = null; // MapCreator�� �����ϴ� ����
    void Start()
    {
        // MapCreator�� �����ͼ� ��� ���� map_creato�� ����
        map_creator = GameObject.Find("GameRoot").GetComponent<MapCreator>();
    }
    void Update()
    {
        if (this.map_creator.isDelete(this.gameObject))
        { // ī�޶󿡰� �Ⱥ��̸�,
            GameObject.Destroy(this.gameObject); // �ڱ� �ڽ��� ����
        }
    }
}
