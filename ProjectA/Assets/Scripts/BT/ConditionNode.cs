using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionNode : Node
{
    protected Func<bool> condition;
    protected Node successNode;
    protected Node failureNode;

    public ConditionNode(Func<bool> _condition, Node _success, Node _failure)
    {
        condition = _condition;
        successNode = _success;
        failureNode = _failure;
    }

    public override NodeStates Evaluate()
    {
        if (condition())
            return successNode.Evaluate();
        else
            return failureNode.Evaluate();
    }
}
