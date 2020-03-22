using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CS_BotController : MonoBehaviour {

    NavMeshAgent agent;

	// Use this for initialization
	public void Setup () {

        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(CS_GameManager.instance.GetClosestPlayer().position);
    }
	
	// Update is called once per frame
	void Update () {

        Vector3 target = CS_GameManager.instance.GetClosestPlayer().position;
        agent.destination = target;

        if (Vector3.Distance(transform.position, target) <= 2.5f)
            GetComponent<CS_Bot>().CmdSendDamage();
    }
}
