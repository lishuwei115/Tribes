using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public struct MoveJob : IJob
{
	//Variables
	public float3 HumanPos;
	public float3 Dest;
	public float Timer;
    //Result Pointers
	public NativeArray<float3> result;
	public NativeArray<float> timerRes;
    /// <summary>
    /// 
    /// </summary>
    public void Execute()
    {
		result[0] = Vector3.Lerp(HumanPos, Dest, Timer);
		timerRes[0] = Timer + 0.02f * 2;
    }
}
