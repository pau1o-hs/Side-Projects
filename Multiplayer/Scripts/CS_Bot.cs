using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CS_BotController))]
public class CS_Bot : NetworkBehaviour {

    bool isDead;
    [SerializeField] float maxHealth;
    float healthAmount;

    // Use this for initialization
    void Start () {

        healthAmount = maxHealth;
    }

    public float GetHealthAmount()
    {
        return healthAmount;
    }

    private void Update()
    {
        if (isLocalPlayer)
            GetComponent<CS_BotController>().enabled = false;
    }

    public override void OnStartClient()
    {
        string _netId = GetComponent<NetworkIdentity>().netId.ToString();
        CS_GameManager.RegisterBot(_netId, this);
        GetComponent<CS_BotController>().Setup();
    }

    [Command]
    public void CmdSendDamage()
    {
        if (hasAuthority)
            CS_GameManager.instance.GetClosestPlayer().GetComponent<CS_Player>().RpcTakeDamage(5, "BOT", Color.white);
    }

    [ClientRpc]
    public void RpcTakeDamage (string source, int damage, bool hostShooted)
    {
        if ((!hostShooted && !isServer) || isDead)
            return;

        healthAmount -= damage;

        if (healthAmount <= 0)
            Die(source); 
    }

    public void DdTakeDamage (string source, int damage)
    {
        if (isServer || isDead)
            return;

        healthAmount -= damage;

        if (healthAmount <= 0)
            Die(source);
    }

    void Die(string _source)
    {
        isDead = true;

        GetComponent<CS_BotController>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        StartCoroutine(DestroyObj());
    }

    IEnumerator DestroyObj()
    {
        yield return new WaitForSeconds(10f);
        CS_GameManager.UnRegisterBot(transform.name);
        Destroy(gameObject);
    }
}
