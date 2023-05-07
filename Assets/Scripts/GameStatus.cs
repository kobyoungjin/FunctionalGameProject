using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatus : MonoBehaviour
{
    // 철광석, 식물을 사용했을 때 각각의 수리 정도.
    public static float GAIN_REPAIRMENT_ROCK = 0.30f;
    public static float GAIN_REPAIRMENT_PLANT = 0.10f;

    // 철광석, 사과, 식물을 운반했을 때 각각의 체력 소모 정도.
    public static float CONSUME_SATIETY_ROCK = 0.06f;
    public static float CONSUME_SATIETY_APPLE = 0.03f;
    public static float CONSUME_SATIETY_PLANT = 0.03f;

    // 사과, 식물을 먹었을 때 각각의 체력 회복 정도.
    public static float REGAIN_SATIETY_APPLE = 0.7f;
    public static float REGAIN_SATIETY_PLANT = 0.3f;  // 공복 회복
    public static float REGAIN_TEMPERATURE_PLANT = 0.3f;  // 모닥불 회복
    public static float REGAIN_TEMPERATURE_APPLE = 0.1f;


    public float repairment = 0.0f; // 우주선의 수리 정도(0.0f~1.0f).
    public float satiety = 1.0f; // 배고픔,체력(0.0f~1.0f).
    public float bodyTemperature = 1.0f;
    public float temperature = 1.0f;
    
    public GUIStyle guistyle; // 폰트 스타일.
    public GameObject bonfire = null;

    public static float CONSUME_SATIETY_ALWAYS = 0.03f;
    public static float CONSUME_BODYTEMPERATURE_ALWAYS = 0.02f;
    public static float ENJOY_THE_FIRE = 0.06f; 

    void Start()
    {
        this.guistyle.fontSize = 24; // 폰트 크기를 24로.
        bonfire = GameObject.Find("Bonfire");
    }

    void OnGUI()
    {
        float x = Screen.width * 0.2f;
        float y = 20.0f;

        // 체력을 표시.
        GUI.Label(new Rect(x, y, 200.0f, 20.0f), "포만감:" + (this.satiety * 100.0f).ToString("000"), guistyle);
        x += 200;
        GUI.Label(new Rect(x, y, 200.0f, 20.0f), "체온:" + (this.bodyTemperature * 100.0f).ToString("000"), guistyle);
        x += 200;
        // 수리 정도를 표시.
        GUI.Label(new Rect(x, y, 200.0f, 20.0f), "로켓 :" + (this.repairment * 100.0f).ToString("000"), guistyle);
        x += 200;
        GUI.Label(new Rect(x, y, 200.0f, 20.0f), "모닥불 :" + (this.temperature * 100.0f).ToString("000"), guistyle);
    }

    // 우주선 수리를 진행
    public void addRepairment(float add)
    {
        this.repairment = Mathf.Clamp01(this.repairment + add); // 0.0~1.0 강제 지정
    }

    // 체력을 늘리거나 줄임
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

    // 게임을 클리어했는지 검사
    public bool isGameClear()
    {
        bool is_clear = false;
        if (this.repairment >= 1.0f)
        { // 수리 정도가 100% 이상이면.
            is_clear = true; // 클리어했다.
        }
        return (is_clear);
    }
    // 게임이 끝났는지 검사
    public bool isGameOver()
    {
        bool is_over = false;
        if (this.satiety <= 0.0f)
        { // 체력이 0이하라면.
            is_over = true; // 게임 오버.
        }
        return (is_over);
    }

    public bool isBonfireOver()
    {
        bool is_over = false;
        if (this.temperature <= 0.0f)
        { // 체력이 0이하라면.
            is_over = true; // 게임 오버.
        }
        return (is_over);
    }

    // 배를 고프게 하는 메서드 추가
    public void alwaysSatiety()
    {
        this.satiety = Mathf.Clamp01(this.satiety - CONSUME_SATIETY_ALWAYS * Time.deltaTime);
    }

    // 떨어뜨리는 메서드 추가
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
