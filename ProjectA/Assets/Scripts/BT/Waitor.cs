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
        //���� �� success ��ȯ
        return NodeStates.FAILURE;
        
    }
}
