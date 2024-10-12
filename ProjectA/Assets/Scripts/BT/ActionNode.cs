using System;
using UnityEngine;
using System.Collections;

//액션을 설정할 리프 노드이다. 실행할 행동을 대리자delegate로 지정할 수 있다.
public class ActionNode : Node 
{
    //런타임에(실행중에) 실행할 함수를 지정한다.
    /* Method signature for the action. */
    public delegate NodeStates ActionNodeDelegate();

    /* The delegate that is called to evaluate this node */
    private ActionNodeDelegate m_action;

    /* Because this node contains no logic itself,
     * the logic must be passed in in the form of 
     * a delegate. As the signature states, the action
     * needs to return a NodeStates enum */
    public ActionNode(ActionNodeDelegate action) 
    {
        m_action = action;
    }

    /* Evaluates the node using the passed in delegate and 
     * reports the resulting state as appropriate */
    public override NodeStates Evaluate() 
    {
        switch (m_action()) 
        {
            case NodeStates.SUCCESS:
                m_nodeState = NodeStates.SUCCESS;
                break;
            case NodeStates.FAILURE:
                m_nodeState = NodeStates.FAILURE;
                break;
            case NodeStates.RUNNING:
                m_nodeState = NodeStates.RUNNING;
                break;
            default:
                m_nodeState = NodeStates.FAILURE;
                break;
        }
        return m_nodeState;
    }

}
