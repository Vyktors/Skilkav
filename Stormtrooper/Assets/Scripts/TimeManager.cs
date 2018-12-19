using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

	public float slowDownFactor = 0.05f;

    private float fixedDeltaTimeReset;

    void Start()
    {
        fixedDeltaTimeReset = Time.fixedDeltaTime;
        Debug.Log(fixedDeltaTimeReset); //0.0167f
    }

    void Update()
	{     
		Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
	}	

	public void DoSlowMotion(){
		Time.timeScale = slowDownFactor;	
		Time.fixedDeltaTime = Time.timeScale * .02f;	
	}

    public IEnumerator DoSlowMotion(float timeInSlowMo)
    {
        
        DoSlowMotion();
        yield return new WaitForSeconds(timeInSlowMo * slowDownFactor);
        ExitSlowMotion();
        
    }

    public void ExitSlowMotion()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = fixedDeltaTimeReset;
    }


}
