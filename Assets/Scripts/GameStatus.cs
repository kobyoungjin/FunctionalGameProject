using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatus : MonoBehaviour
{
    // ö����, �Ĺ��� ������� �� ������ ���� ����.
    public static float GAIN_REPAIRMENT_ROCK = 0.30f;
    public static float GAIN_REPAIRMENT_PLANT = 0.10f;

    // ö����, ���, �Ĺ��� ������� �� ������ ü�� �Ҹ� ����.
    public static float CONSUME_SATIETY_ROCK = 0.06f;
    public static float CONSUME_SATIETY_APPLE = 0.03f;
    public static float CONSUME_SATIETY_PLANT = 0.03f;

    // ���, �Ĺ��� �Ծ��� �� ������ ü�� ȸ�� ����.
    public static float REGAIN_SATIETY_APPLE = 0.7f;
    public static float REGAIN_SATIETY_PLANT = 0.3f;  // ���� ȸ��
    public static float REGAIN_TEMPERATURE_PLANT = 0.3f;  // ��ں� ȸ��
    public static float REGAIN_TEMPERATURE_APPLE = 0.1f;


    public float repairment = 0.0f; // ���ּ��� ���� ����(0.0f~1.0f).
    public float satiety = 1.0f; // �����,ü��(0.0f~1.0f).
    public float bodyTemperature = 1.0f;
    public float temperature = 1.0f;
    
    public GUIStyle guistyle; // ��Ʈ ��Ÿ��.
    public GameObject bonfire = null;

    public static float CONSUME_SATIETY_ALWAYS = 0.03f;
    public static float CONSUME_BODYTEMPERATURE_ALWAYS = 0.02f;
    public static float ENJOY_THE_FIRE = 0.06f; 

    void Start()
    {
        this.guistyle.fontSize = 24; // ��Ʈ ũ�⸦ 24��.
        bonfire = GameObject.Find("Bonfire");
    }

    void OnGUI()
    {
        float x = Screen.width * 0.2f;
        float y = 20.0f;

        // ü���� ǥ��.
        GUI.Label(new Rect(x, y, 200.0f, 20.0f), "������:" + (this.satiety * 100.0f).ToString("000"), guistyle);
        x += 200;
        GUI.Label(new Rect(x, y, 200.0f, 20.0f), "ü��:" + (this.bodyTemperature * 100.0f).ToString("000"), guistyle);
        x += 200;
        // ���� ������ ǥ��.
        GUI.Label(new Rect(x, y, 200.0f, 20.0f), "���� :" + (this.repairment * 100.0f).ToString("000"), guistyle);
        x += 200;
        GUI.Label(new Rect(x, y, 200.0f, 20.0f), "��ں� :" + (this.temperature * 100.0f).ToString("000"), guistyle);
    }

    // ���ּ� ������ ����
    public void addRepairment(float add)
    {
        this.repairment = Mathf.Clamp01(this.repairment + add); // 0.0~1.0 ���� ����
    }

    // ü���� �ø��ų� ����
    public void addSatiety(float add)
    {
        this.satiety = Mathf.Clamp01(this.satiety + add);
    }

    public void addTemperature(float add)
    {
        this.temperature = Mathf.Clamp01(this.temperature + add);
    }

    public void addBodyTemperature(float add)
    {
        this.bodyTemperature = Mathf.Clamp01(this.bodyTemperature + add);
    }

    // ������ Ŭ�����ߴ��� �˻�
    public bool isGameClear()
    {
        bool is_clear = false;
        if (this.repairment >= 1.0f)
        { // ���� ������ 100% �̻��̸�.
            is_clear = true; // Ŭ�����ߴ�.
        }
        return (is_clear);
    }
    // ������ �������� �˻�
    public bool isGameOver()
    {
        bool is_over = false;
        if (this.satiety <= 0.0f)
        { // ü���� 0���϶��.
            is_over = true; // ���� ����.
        }
        return (is_over);
    }

    public bool isBonfireOver()
    {
        bool is_over = false;
        if (this.temperature <= 0.0f)
        { // ü���� 0���϶��.
            is_over = true; // ���� ����.
        }
        return (is_over);
    }

    // �踦 ������ �ϴ� �޼��� �߰�
    public void alwaysSatiety()
    {
        this.satiety = Mathf.Clamp01(this.satiety - CONSUME_SATIETY_ALWAYS * Time.deltaTime);
    }

    // ����߸��� �޼��� �߰�
    public void alwaysBodyTemperature()
    {
        this.bodyTemperature = Mathf.Clamp01(this.bodyTemperature - CONSUME_BODYTEMPERATURE_ALWAYS * Time.deltaTime);
    }

    public void addBodyTemperature()
    {
        this.bodyTemperature = Mathf.Clamp01(this.bodyTemperature + ENJOY_THE_FIRE * Time.deltaTime);
    }

    public void regulateBonfire()
    {
        var emission = bonfire.transform.GetChild(0).GetComponent<ParticleSystem>().emission;
        
        temperature -= 0.04f * Time.deltaTime;
        if (temperature < 0)
        {
            temperature = 0;
            CONSUME_BODYTEMPERATURE_ALWAYS = 0.05f;
        }
        else
            CONSUME_BODYTEMPERATURE_ALWAYS = 0.02f;

        emission.rateOverTime = temperature * 15;
    }

    public float GetTemperature()
    {
        return temperature;
    }
    
}
