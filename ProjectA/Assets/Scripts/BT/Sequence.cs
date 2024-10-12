using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sequence : Node 
{
    /** Chiildren nodes that belong to this sequence */
    private List<Node> m_nodes = new List<Node>();

    /** Must provide an initial set of children nodes to work */
    public Sequence(List<Node> nodes) 
    {
        m_nodes = nodes;
    }

    /* If any child node returns a failure, the entire node fails. Whence all 
     * nodes return a success, the node reports a success. */
    public override NodeStates Evaluate()
    {
        bool anyChildRunning = false;
        
        //FAILURE가 반환될때까지 노드를 순회하며 반환값을 읽음
        //SUCCESS가 반환되면 다음 노드를 탐색
        //RUNNING이 반환되면 행동중임을 표시, 종료될때까지 행동트리 유지
        foreach(Node node in m_nodes) 
        {
            switch (node.Evaluate()) 
            {

                case NodeStates.FAILURE:
                    m_nodeState = NodeStates.FAILURE;
                    return m_nodeState;                    
                case NodeStates.SUCCESS:
                    continue;
                case NodeStates.RUNNING:
                    anyChildRunning = true;
                    continue;
                default:
                    m_nodeState = NodeStates.SUCCESS;
                    return m_nodeState;
            }
        }
        m_nodeState = anyChildRunning ? NodeStates.RUNNING : NodeStates.SUCCESS;
        return m_nodeState;
    }
}
