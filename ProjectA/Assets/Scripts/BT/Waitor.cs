using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waitor : Node
{
    float waitTime = 0f;
    float time = 0;

    public Waitor(float _t)
    {
        waitTime = _t;
    }

    public override NodeStates Evaluate()
    {
        time += Time.deltaTime;

        if (time >= waitTime)
        {
            time = 0f;
            return NodeStates.SUCCESS;
        } 
        //수초 뒤 success 반환
        return NodeStates.FAILURE;
        
    }
}
