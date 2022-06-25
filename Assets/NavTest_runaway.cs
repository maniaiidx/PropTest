using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavTest_runaway : MonoBehaviour
{
    public GameObject TargetObject; /// 目標位置
    NavMeshAgent m_navMeshAgent; /// NavMeshAgent
    // Use this for initialization
    void Start()
    {
        // NavMeshAgentコンポーネントを取得
        m_navMeshAgent = GetComponent<NavMeshAgent>();
    }
    // Update is called once per frame
    void Update()
    {
        // NavMeshが準備できているなら
        if (m_navMeshAgent.pathStatus != NavMeshPathStatus.PathInvalid)
        {
            // NavMeshAgentに目的地をセット
            
            float disx;
            float disz;
            disx = this.transform.position.x - TargetObject.transform.position.x;
            disz = this.transform.position.z - TargetObject.transform.position.z;
            var awaypos = new Vector3(transform.position.x + disx, transform.position.y, transform.position.z + disz);
            //m_navMeshAgent.SetDestination(TargetObject.transform.position);
            m_navMeshAgent.SetDestination(awaypos);
            transform.position = new Vector3(transform.position.x,
           transform.position.y, transform.position.z);
        }
    }
}