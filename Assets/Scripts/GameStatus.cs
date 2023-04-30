using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatus : MonoBehaviour
{
    // ö����, �Ĺ��� ������� �� ������ ���� ����.
    public static float GAIN_REPAIRMENT_ROCK = 0.30f;
    public static float GAIN_REPAIRMENT_PLANT = 0.10f;

    // ö����, ���, �Ĺ��� ������� �� ������ ü�� �Ҹ� ����.
    public static float CONSUME_SATIETY_ROCK = 0.20f;
    public static float CONSUME_SATIETY_APPLE = 0.1f;
    public static float CONSUME_SATIETY_PLANT = 0.1f;

    // ���, �Ĺ��� �Ծ��� �� ������ ü�� ȸ�� ����.
    public static float REGAIN_SATIETY_APPLE = 0.7f;
    public static float REGAIN_SATIETY_PLANT = 0.3f;

    public float repairment = 0.0f; // ���ּ��� ���� ����(0.0f~1.0f).
    public float satiety = 1.0f; // �����,ü��(0.0f~1.0f).

    public GUIStyle guistyle; // ��Ʈ ��Ÿ��.

    public static float CONSUME_SATIETY_ALWAYS = 0.03f;


    void Start()
    {
        this.guistyle.fontSize = 24; // ��Ʈ ũ�⸦ 24��.
    }

    void OnGUI()
    {
        float x = Screen.width * 0.2f;
        float y = 20.0f;

        // ü���� ǥ��.
        GUI.Label(new Rect(x, y, 200.0f, 20.0f), "ü��:" + (this.satiety * 100.0f).ToString("000"), guistyle);
        x += 200;
        // ���� ������ ǥ��.
        GUI.Label(new Rect(x, y, 200.0f, 20.0f), "���� :" + (this.repairment * 100.0f).ToString("000"), guistyle);
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

    // �踦 ������ �ϴ� �޼��� �߰�
    public void alwaysSatiety()
    {
        this.satiety = Mathf.Clamp01(this.satiety - CONSUME_SATIETY_ALWAYS * Time.deltaTime);
    }
}
