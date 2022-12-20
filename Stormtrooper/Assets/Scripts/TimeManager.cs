using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

	public const float SLOW_FACTOR = 0.3f;

    public static bool isSlowedDown = false;

	public static void DoSlowMotion(){
        if (isSlowedDown)
            return;
        Time.timeScale *= SLOW_FACTOR;
        Time.fixedDeltaTime *= SLOW_FACTOR;
        isSlowedDown = true;
    }

    public static IEnumerator DoSlowMotion(float timeInSlowMo)
    {
        DoSlowMotion();
        yield return new WaitForSeconds(timeInSlowMo * SLOW_FACTOR);
        ExitSlowMotion();
    }

    public static void ExitSlowMotion()
    {
        if (!isSlowedDown)
            return;

        Time.timeScale /= SLOW_FACTOR;
        Time.fixedDeltaTime /= SLOW_FACTOR;
        isSlowedDown = false;
    }
}
