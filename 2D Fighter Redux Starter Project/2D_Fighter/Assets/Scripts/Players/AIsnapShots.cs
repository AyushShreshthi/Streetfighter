using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIsnapShots : MonoBehaviour
{
    public List<AIStats> ai_stats = new List<AIStats>();

    private void Start()
    {
        if (ai_stats.Count < 1)
        {
            ai_stats.Add(new AIStats());
        }
    }
    public void RequestAISnapShot(AIcharacter t)
    {
        int index = MySceneManager.GetInstance().progIndex;

        if (index > ai_stats.Count - 1)
        {
            index = ai_stats.Count - 1;
        }
        SetAIStats(index, t);
    }

    public void SetAIStats(int i,AIcharacter t)
    {
        AIStats a = ai_stats[i];
        t.changeStateTolerance = Ran(a.changeStateTolerance_min, a.changeStateTolerance_max);
        t.normalRate = Ran(a.normalRate_min, a.normalRate_max);
        t.closeRate = Ran(a.closeRate_min, a.closeRate_max);
        t.blockingRate = Ran(a.blockingRate_min, a.blockingRate_max);
        t.aiStateLife = Ran(a.aiStateLife_min, a.aiStateLife_max);
        t.JumpRate = Ran(a.JumpRate_min, a.jumpRate_max);


    }

    float Ran(float min,float max)
    {
        return Random.Range(min, max);
    }
    public static AIsnapShots instance;
    public static AIsnapShots GetInstance()
    {
        return instance;
    }
    private void Awake()
    {
        instance = this;
    }
}

[System.Serializable]
public class AIStats
{
    public float changeStateTolerance_max = 3;
    public float changeStateTolerance_min = 2;
    public float normalRate_max = 1;
    public float normalRate_min = 0.8f;
    public float closeRate_max = 0.5f;
    public float closeRate_min = 0.4f;
    public float blockingRate_max = 1.5f;
    public float blockingRate_min = 1.5f;
    public float aiStateLife_max = 1;
    public float aiStateLife_min = 1;
    public float jumpRate_max = 1;
    public float JumpRate_min = 1;
}
