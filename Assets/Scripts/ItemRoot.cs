using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public enum TYPE
    { // ������ ����.
        NONE = -1, IRON = 0, APPLE, PLANT, LUMBER, ROCK,  // ����, ö����, ���, �Ĺ�, ����, ������
        NUM,
    }; // �������� �� �����ΰ� ��Ÿ����(=3).
};

public class ItemRoot : MonoBehaviour
{
    public GameObject RockPrefab = null; // Prefab 'ROCK'
    public GameObject plantPrefab = null; // Prefab 'Plant'
    public GameObject applePrefab = null; // Prefab 'Apple'

    GameObject applerespawn;

    protected List<Vector3> respawn_points; // ���� ���� List.

    public float step_timer = 0.0f;
    public static float RESPAWN_TIME_APPLE = 5.0f; // ��� ���� �ð� ���.
    public static float RESPAWN_TIME_ROCK = 12.0f; // ö���� ���� �ð� ���.
    public static float RESPAWN_TIME_PLANT = 6.0f; // �Ĺ� ���� �ð� ���.

    private float respawn_timer_apple = 0.0f; // ����� ���� �ð�.
    private float respawn_timer_rock = 0.0f; // ö������ ���� �ð�. 
    private float respawn_timer_plant = 0.0f; // �Ĺ��� ���� �ð�.

    

    // �ʱ�ȭ �۾��� �����Ѵ�.
    void Start()
    {
        // �޸� ���� Ȯ��.
        this.respawn_points = new List<Vector3>();
        // "PlantRespawn" �±װ� ���� ��� ������Ʈ�� �迭�� ����.
        GameObject[] respawns = GameObject.FindGameObjectsWithTag("PlantRespawn");
       
        // �迭 respawns ���� ������ GameObject�� �������� ó���Ѵ�.
        foreach (GameObject go in respawns)
        {
            // ������ ȹ��.
            MeshRenderer renderer = go.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            { // �������� �����ϸ�.
                renderer.enabled = false; // �� �������� ������ �ʰ�.
            }
            // ���� ����Ʈ List�� ��ġ ������ �߰�.
            this.respawn_points.Add(go.transform.position);
        }
        // ����� ���� ����Ʈ�� ����ϰ�, �������� ������ �ʰ�.
        applerespawn = GameObject.Find("Tree");
        //applerespawn.GetComponent<MeshRenderer>().enabled = false;
        // ö������ ���� ����Ʈ�� ����ϰ�, �������� ������ �ʰ�.
        GameObject rockrespawn = GameObject.Find("RockRespawn");
        rockrespawn.GetComponent<MeshRenderer>().enabled = false;

        this.respawnRock();
        this.respawnPlant();

        this.respawnPlant();
        this.respawnPlant();
    }

    // �� �������� Ÿ�̸� ���� ���� �ð��� �ʰ��ϸ� �ش� �������� ����.
    void Update()
    {
        respawn_timer_apple += Time.deltaTime;
        respawn_timer_rock += Time.deltaTime;
        respawn_timer_plant += Time.deltaTime;
        if (respawn_timer_apple > RESPAWN_TIME_APPLE  && applerespawn.transform.GetChild(1).transform.childCount < 8)
        {
            respawn_timer_apple = 0.0f;
            this.respawnApple(); // ����� ������Ų��.
        }
        if (respawn_timer_rock > RESPAWN_TIME_ROCK)
        {
            respawn_timer_rock = 0.0f;
            this.respawnRock(); // ö������ ������Ų��.
        }
        if (respawn_timer_plant > RESPAWN_TIME_PLANT)
        {
            respawn_timer_plant = 0.0f;
            this.respawnPlant(); // �Ĺ��� ������Ų��.
        }

       
    }

    // �������� ������ Item.TYPE������ ��ȯ�ϴ� �޼ҵ�.
    public Item.TYPE getItemType(GameObject item_go)
    {
        Item.TYPE type = Item.TYPE.NONE;
        if (item_go != null)
        { // �μ��� ���� GameObject�� ������� ������.
            switch (item_go.tag)
            { // �±׷� �б�.
                case "Rock": type = Item.TYPE.ROCK; break;
                case "Apple": type = Item.TYPE.APPLE; break;
                case "Plant": type = Item.TYPE.PLANT; break;
                case "Iron": type = Item.TYPE.IRON; break;
                case "Lumber": type = Item.TYPE.LUMBER; break;
            }
        }
        return (type);
    }

    // ö������ ������Ų��.
    public void respawnRock()
    {
        // ö���� �������� �ν��Ͻ�ȭ.
        GameObject go = GameObject.Instantiate(this.RockPrefab) as GameObject;
        // ö������ ���� ����Ʈ�� ���.
        Vector3 pos = GameObject.Find("RockRespawn").transform.position;
        // ���� ��ġ�� ����.
        pos.y = 1.0f;
        pos.x += Random.Range(-1.0f, 1.0f);
        pos.z += Random.Range(-1.0f, 1.0f);
        // ö������ ��ġ�� �̵�.
        go.transform.position = pos;
    }

    // ����� ������Ų��.
    public void respawnApple()
    {
        // ��� �������� �ν��Ͻ�ȭ.
        GameObject go = GameObject.Instantiate(this.applePrefab) as GameObject;
        // ����� ���� ����Ʈ�� ���.
        Vector3 pos = GameObject.Find("Tree").transform.position;
        // ���� ��ġ�� ����.
        pos.y = 0.5f;
        float radius = applerespawn.GetComponent<SphereCollider>().radius;
        pos.x += Random.Range(-radius, radius);
        pos.z += Random.Range(-radius, radius);
        // ����� ��ġ�� �̵�.
        go.transform.position = pos;
        go.gameObject.name = applePrefab.name;
        go.transform.SetParent(applerespawn.transform.GetChild(1));
    }

    // �Ĺ��� ������Ų��.
    public void respawnPlant()
    {
        if (this.respawn_points.Count > 0)
        { // List�� ������� ������.
          // �Ĺ� �������� �ν��Ͻ�ȭ.
            GameObject go = GameObject.Instantiate(this.plantPrefab) as GameObject;
            // �Ĺ��� ���� ����Ʈ�� �����ϰ� ���.
            int n = Random.Range(0, this.respawn_points.Count);
            Vector3 pos = this.respawn_points[n];
            // ���� ��ġ�� ����.
            pos.y = 1.0f;
            pos.x += Random.Range(-1.0f, 1.0f);
            pos.z += Random.Range(-1.0f, 1.0f);
            // �Ĺ��� ��ġ�� �̵�.
            go.transform.position = pos;
        }
    }

    // ��� �ִ� �����ۿ� ���� ������ ��ô ���¡��� ��ȯ
    public float getGainRepairment(GameObject item_go)
    {
        float gain = 0.0f;
        if (item_go == null)
        {
            gain = 0.0f;
        }
        else
        {
            Item.TYPE type = this.getItemType(item_go);
            switch (type)
            { // ��� �ִ� �������� ������ ��������.
                case Item.TYPE.ROCK:
                    gain = GameStatus.GAIN_REPAIRMENT_ROCK; break;
                case Item.TYPE.PLANT:
                    gain = GameStatus.GAIN_REPAIRMENT_PLANT; break;
            }
        }
        return (gain);
    }
    // ��� �ִ� �����ۿ� ���� ��ü�� ���� ���¡��� ��ȯ
    public float getConsumeSatiety(GameObject item_go)
    {
        float consume = 0.0f;
        if (item_go == null)
        {
            consume = 0.0f;
        }
        else
        {
            Item.TYPE type = this.getItemType(item_go);
            switch (type)
            { // ��� �ִ� �������� ������ ��������.
                case Item.TYPE.ROCK:
                    consume = GameStatus.CONSUME_SATIETY_ROCK; break;
                case Item.TYPE.APPLE:
                    consume = GameStatus.CONSUME_SATIETY_APPLE; break;
                case Item.TYPE.PLANT:
                    consume = GameStatus.CONSUME_SATIETY_PLANT; break;
            }
        }
        return (consume);
    }
    // ��� �ִ� �����ۿ� ���� ��ü�� ȸ�� ���¡��� ��ȯ
    public float getRegainSatiety(GameObject item_go)
    {
        float regain = 0.0f;
        if (item_go == null)
        {
            regain = 0.0f;
        }
        else
        {
            Item.TYPE type = this.getItemType(item_go);
            switch (type)
            { // ��� �ִ� �������� ������ ��������.
                case Item.TYPE.APPLE:
                    regain = GameStatus.REGAIN_SATIETY_APPLE; break;
                case Item.TYPE.PLANT:
                    regain = GameStatus.REGAIN_SATIETY_PLANT; break;
            }
        }
        return (regain);
    }

    public float getRegainTemperature(GameObject item_go)
    {
        float regain = 0.0f;
        if (item_go == null)
        {
            regain = 0.0f;
        }
        else
        {
            Item.TYPE type = this.getItemType(item_go);
            switch (type)
            { // ��� �ִ� �������� ������ ��������.
                case Item.TYPE.APPLE:
                    regain = GameStatus.REGAIN_TEMPERATURE_APPLE; break;
                case Item.TYPE.PLANT:
                    regain = GameStatus.REGAIN_TEMPERATURE_PLANT; break;
            }
        }
        return (regain);
    }
}
