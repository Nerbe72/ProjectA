using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionNode : Node
{
    protected Func<bool> condition;
    protected Node successNode;
    protected Node failureNode;

    public ConditionNode(Func<bool> _condition, Node _success = null, Node _failure = null)
    {
        condition = _condition;
        successNode = _success;
        failureNode = _failure;
    }

    public override NodeStates Evaluate()
    {
        if (condition())
        {
            if (successNode == null) return NodeStates.FAILURE;
            return successNode.Evaluate();
        }
        else
        {
            if (failureNode == null) return NodeStates.FAILURE;
            return failureNode.Evaluate();
        }
    }
}
