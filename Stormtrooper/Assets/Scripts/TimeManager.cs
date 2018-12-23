using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager{

	public const float SLOW_FACTOR = 0.05f;
    private const float DELTA_SLOW_FACTOR = 0.02f;

    private float fixedDeltaTimeReset;

    private static TimeManager instance;
    
    void Start()
    {
        fixedDeltaTimeReset = Time.fixedDeltaTime;
        Debug.Log(fixedDeltaTimeReset); 
    }

    void Update()
	{     
		Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
	}	

	public void DoSlowMotion(){
		Time.timeScale = SLOW_FACTOR;	
		Time.fixedDeltaTime = SLOW_FACTOR * DELTA_SLOW_FACTOR;
	}

    public IEnumerator DoSlowMotion(float timeInSlowMo)
    {        
        DoSlowMotion();
        yield return new WaitForSeconds(timeInSlowMo * SLOW_FACTOR);
        ExitSlowMotion();        
    }

    public void ExitSlowMotion()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = fixedDeltaTimeReset;
    }

    public static TimeManager GetInstance()
    {
        if(instance == null)
        {
            instance = new TimeManager();
        }
        return instance;
    }


}
