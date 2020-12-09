using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void Alarm();

public class TimeKeeper : MonoBehaviour
{
    
    int battleTime;
    SortedList<int, Alarm> alarms = new SortedList<int, Alarm>();

    // Use this for initialization
    void Start()
    {
        
        //alarms.Add(5, delegate { Debug.Log("anon method success"); });
        //InvokeRepeating("BattleTimerTick", 1f, 1f);
    }

    void BattleTimerTick()
    {
        battleTime++;
        if (alarms.Count == 0) return;
        int nextAlarm = alarms.Keys[0];
        
        if(battleTime == nextAlarm)
        {
            alarms[nextAlarm] += EventTest;
            alarms[nextAlarm]();
            alarms.Remove(nextAlarm);
        }
        

    }
    void EventTest()
    {
        Debug.Log("Success");
    }
}

public class Timer
{
    public int elapsedTime;
    public SortedList<int, Alarm> alarms = new SortedList<int, Alarm>();
}
