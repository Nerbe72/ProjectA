using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBT : MonoBehaviour
{
    private Sequence root;

    private bool isMoving = false;

    private Zombie agent;

    private void Start()
    {
        
    }

    private NodeStates DoSearching()
    {
        if (isMoving || agent.IsPlayerInSight())
        {
            return NodeStates.SUCCESS;
        }

        return NodeStates.FAILURE;
    }

    private NodeStates DoMove()
    {
        isMoving = true;
        agent.agent.isStopped = false;
        return NodeStates.SUCCESS;
    }

}
