using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CS_Player : NetworkBehaviour {

    [SerializeField] [SyncVar] string username;
    [HideInInspector] [SyncVar] public Color m_Color;
    [SerializeField] public float maxHealth = 100;
    [SerializeField] float healthRegen = 1f;
    [SerializeField] private Behaviour[] disableOnDeath;
    [SerializeField] private GameObject[] disableGameObjectsOnDeath;
    [HideInInspector] public CS_Canvas m_Canvas;

    [SerializeField] GameObject deathEffect;

    private float healthAmount;
    [SyncVar] private bool _isDead;
    public bool isDead {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    private bool[] wasEnabled;

    [ClientRpc]
    public void RpcGetPlayerInfo (string _name, Color _color)
    {
        username = _name;
        m_Color = _color;
    }

    public string GetUsername()
    {
        return username;
    }

    public float GetHealthAmount ()
    {
        return healthAmount;
    }

    public void Setup()
    {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
            wasEnabled[i] = disableOnDeath[i].enabled;

        SetDefaults();   
    }

    void Update()
    {
        if (healthAmount > 0)
            healthAmount += healthRegen * Time.deltaTime;
        healthAmount = Mathf.Clamp(healthAmount, 0, maxHealth);

        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.K))
            RpcTakeDamage(9999, null, Color.white);
    }

    [ClientRpc]
    public void RpcTakeDamage (int damage, string _source, Color sourceColor)
    {
        if (isDead || (!isLocalPlayer && _source != "BOT"))
            return;

        healthAmount -= damage;

        if (isLocalPlayer)
            m_Canvas.HittedEffect();

        if (healthAmount <= 0)
            Die(_source, sourceColor);
    }

    
    public void DdTakeDamage(int damage, string _source, Color sourceColor)
    {
        healthAmount -= damage;

        if (healthAmount <= 0)
            Die(_source, sourceColor);
    }

    private void Die (string _source, Color sourceColor)
    {
        isDead = true;

        if (_source != null)
            CS_GameManager.instance.onPlayerKilledCallback.Invoke(username, m_Color, _source, sourceColor);

        //DISABLE COMPONENTS
        for (int i = 0; i < disableOnDeath.Length; i++)
            disableOnDeath[i].enabled = false;

        //DISABLE GAME OBJECTS
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
            disableGameObjectsOnDeath[i].SetActive(false);

        //DISABLE COLLIDER
        transform.Find("Colliders").gameObject.SetActive(false);
        GetComponent<Rigidbody>().useGravity = false;


        if (isLocalPlayer) {
            GetComponent<CS_Shoot>().CancelInvoke("Fire");
            GetComponent<Rigidbody>().velocity = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
        }

        //DEATH EFFECT
        GameObject _deathEffect = Instantiate(deathEffect, transform.position, transform.rotation, null);
        Destroy(_deathEffect, 5f);

        //RESPAWN
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(CS_GameManager.instance.matchSettings.respawnTime);

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;

        SetDefaults();
    }

    void SetDefaults ()
    {
        isDead = false;
        healthAmount = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
            disableOnDeath[i].enabled = wasEnabled[i];

        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
            disableGameObjectsOnDeath[i].SetActive(true);

        transform.Find("Colliders").gameObject.SetActive(true);
        GetComponent<Rigidbody>().useGravity = true;
    }
}
