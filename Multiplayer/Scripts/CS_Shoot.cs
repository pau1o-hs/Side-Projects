using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CS_Shoot : NetworkBehaviour {

    private CS_WeaponManager m_WeaponManager;
    private CS_Weapon currentWeapon;
    AudioSource m_AudioSource;
    CS_PlayerSetup m_Setup;
    CS_Player m_Player;
    Vector3 hitPos, hitNormal;

    float fireTime, recoilMultiplier;
    // Use this for initialization
    void Start()
    {
        m_Setup = GetComponent<CS_PlayerSetup>();
        m_Player = GetComponent<CS_Player>();
        m_WeaponManager = GetComponent<CS_WeaponManager>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        currentWeapon = m_WeaponManager.GetCurrentWeapon();

        if (CS_Pause.isOn) {
            CancelInvoke("Fire");
            return;
        }

        Fire();
    }


    [Command]
    void CmdOnFire(Vector3 _hitPoint, Vector3 _normal)
    {
        RpcFireEffects(_hitPoint, _normal); //Go to all Clients
    }

    [ClientRpc]
    void RpcFireEffects(Vector3 _hitPoint, Vector3 _normal)
    {
        if (isLocalPlayer)
            return;

        //EFFECTS
        GameObject bulletRayInst = Instantiate(m_WeaponManager.GetCurrentGraphics().bulletRay);
        bulletRayInst.GetComponent<LineRenderer>().SetPosition(0, m_WeaponManager.GetCurrentWeapon().bulletSpawn.position);
        bulletRayInst.GetComponent<LineRenderer>().SetPosition(1, _hitPoint);
        Destroy(bulletRayInst, 2f);
        m_AudioSource.PlayOneShot(m_WeaponManager.GetCurrentGraphics().shotSound);

        //HIT SOMETHING
        if (_hitPoint != m_Setup.m_Camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 40))) {
            m_WeaponManager.GetCurrentGraphics().muzzleFlash.Play();
            GameObject hitEffect = Instantiate(m_WeaponManager.GetCurrentGraphics().hitEffect, _hitPoint, Quaternion.LookRotation(_normal), null);
            Destroy(hitEffect, 2f);
        }
    }

    void DdFireEffects(Vector3 _hitPoint, Vector3 _normal)
    {
        //EFFECTS
        GameObject bulletRayInst = Instantiate(m_WeaponManager.GetCurrentGraphics().bulletRay);
        bulletRayInst.GetComponent<LineRenderer>().SetPosition(0, m_WeaponManager.GetCurrentWeapon().bulletSpawn.position);
        bulletRayInst.GetComponent<LineRenderer>().SetPosition(1, _hitPoint);
        Destroy(bulletRayInst, 2f);
        m_AudioSource.PlayOneShot(m_WeaponManager.GetCurrentGraphics().shotSound);

        //HIT SOMETHING
        if (_hitPoint != m_Setup.m_Camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 40))) {
            m_WeaponManager.GetCurrentGraphics().muzzleFlash.Play();
            GameObject hitEffect = Instantiate(m_WeaponManager.GetCurrentGraphics().hitEffect, _hitPoint, Quaternion.LookRotation(_normal), null);
            Destroy(hitEffect, 2f);
        }
    }

    [Client]
    public void Fire()
    {
        if (!isLocalPlayer)
            return;

        if (recoilMultiplier >= 0 && !Input.GetMouseButton(0))
            recoilMultiplier -= 20f * Time.deltaTime;

        recoilMultiplier = Mathf.Clamp(recoilMultiplier, 0, 1f);

        Vector3 recoil = ((Vector3.up * Random.Range(.01f, .025f)) + (Vector3.right * Random.Range(-.025f, .025f))) * recoilMultiplier;

        //CAMERA POSITION TO HIT
        RaycastHit hit, _hit;
        if (Physics.Raycast(m_Setup.m_Camera.transform.position, m_Setup.m_Camera.transform.forward + recoil, out hit, currentWeapon.fireRange)) {

            //GUN POSITION TO HIT
            if (Physics.Raycast(currentWeapon.bulletSpawn.position, hit.point - currentWeapon.bulletSpawn.position + recoil, out _hit, currentWeapon.fireRange)) {

                //if (_hit.collider.tag == "Player" || _hit.collider.tag == "Bot")
                    //m_Setup.m_Canvas._crosshair.color = Color.red;
                //else m_Setup.m_Canvas._crosshair.color = Color.white;

                //FIRE INPUT
                if (Input.GetMouseButton(0) && Time.time > fireTime) {
                    if (hit.point != _hit.point)
                        CheckHit(_hit);
                    else
                        CheckHit(hit);

                    recoilMultiplier += .5f;
                    m_Setup.m_OrbitCam.Recoil(currentWeapon.recoil);
                    fireTime = Time.time + 1 / currentWeapon.fireRate;
                    CallFireEffects(_hit.transform.name);
                }
            }
        }
        else if (Physics.Raycast(currentWeapon.bulletSpawn.position, m_Setup.m_Camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 40)) - currentWeapon.bulletSpawn.position + recoil, out _hit, currentWeapon.fireRange)) {

            //if (_hit.collider.tag == "Player" || _hit.collider.tag == "Bot") 
                //m_Setup.m_Canvas._crosshair.color = Color.red;           
            //else m_Setup.m_Canvas._crosshair.color = Color.white;

            if (Input.GetMouseButton(0) && Time.time > fireTime) {

                CheckHit(_hit);

                recoilMultiplier += .5f;
                m_Setup.m_OrbitCam.Recoil(currentWeapon.recoil);
                fireTime = Time.time + 1 / currentWeapon.fireRate;
                CallFireEffects(_hit.transform.name);
            }
        }
        else {
            if (Input.GetMouseButton(0) && Time.time > fireTime) {

                recoilMultiplier += .5f;
                hitPos = m_Setup.m_Camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 40));
                m_Setup.m_OrbitCam.Recoil(currentWeapon.recoil);
                fireTime = Time.time + 1 / currentWeapon.fireRate;
                //m_Setup.m_Canvas._crosshair.color = Color.white;
                CallFireEffects("");
            }
        }
    }

    void CheckHit (RaycastHit hit)
    {
        CmdHitTarget(hit.transform.name, currentWeapon.damage, isServer);

        if (hit.collider.tag == "Player")
        {
            hit.transform.GetComponent<CS_Player>().DdTakeDamage(currentWeapon.damage, m_Player.GetUsername(), m_Player.m_Color);
        }
        if (hit.collider.tag == "Bot")
            hit.transform.GetComponent<CS_Bot>().DdTakeDamage(transform.name, currentWeapon.damage);

        if (hit.collider.tag == "Player" || hit.collider.tag == "Bot")
            m_Setup.m_Canvas.HitEffect();

        hitPos = hit.point;
        hitNormal = hit.normal;
    }

    void CallFireEffects (string hittedName)
    {
        CS_Player playerHitted = CS_GameManager.GetPlayer(hittedName);
        CS_Bot botHitted = CS_GameManager.GetBot(hittedName);

        currentWeapon.bulletSpawn.LookAt(hitPos);
        CmdOnFire(hitPos, hitNormal); //Go to Server
        DdFireEffects(hitPos, hitNormal); // DIBRA DELAY
        m_Setup.m_OrbitCam.m_Animator.SetTrigger("Shake");

        if (!isServer && hittedName != "") {
            if (playerHitted.GetComponent<CS_Player>().GetHealthAmount() - currentWeapon.damage <= 0)
                m_Setup.m_Canvas.KillEffect();

            if (botHitted.GetComponent<CS_Bot>().GetHealthAmount() - currentWeapon.damage <= 0)
                m_Setup.m_Canvas.KillEffect();
        }
    }

    [Command]
    void CmdHitTarget(string hittedName, int damage, bool isHost)
    {
        CS_Player playerHitted = CS_GameManager.GetPlayer(hittedName);
        CS_Bot botHitted = CS_GameManager.GetBot(hittedName);

        if (playerHitted != null){
            playerHitted.GetComponent<CS_Player>().RpcTakeDamage(damage, m_Player.GetUsername(), m_Player.m_Color);

            if (playerHitted.GetComponent<CS_Player>().GetHealthAmount() - damage <= 0)
                m_Setup.m_Canvas.KillEffect();
        }

        if (botHitted != null) {
            botHitted.GetComponent<CS_Bot>().RpcTakeDamage(transform.name, damage, isHost);

            if (botHitted.GetComponent<CS_Bot>().GetHealthAmount() - damage <= 0)
                m_Setup.m_Canvas.KillEffect();
        }
    }
}
