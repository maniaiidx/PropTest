using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavTest : MonoBehaviour
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
                m_navMeshAgent.SetDestination(TargetObject.transform.position);
            }
        }
    }