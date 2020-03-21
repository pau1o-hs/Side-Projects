using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_GameManager : MonoBehaviour {

    public static CS_GameManager instance = null;
    public float enableMovementIn;

    public GameObject player;
    CharacterScript m_Player;

    public string nextLevel;
    public static bool isPaused;

    public List<Platform> platform = new List<Platform>();

    [Header("Event")]
    public CS_Event[] events;

    private void Awake()
    {
        instance = this;

        GameObject.FindObjectOfType<Camera>().enabled = false;
        GameObject temp = Instantiate(player, transform.GetChild(0).position, transform.GetChild(0).rotation);

        m_Player = temp.GetComponent<CharacterScript>();
    }

    // Use this for initialization
    void Start() {

        if (!PlayerPrefs.HasKey("Save") || PlayerPrefs.GetString("Save") != SceneManager.GetActiveScene().name)
            PlayerPrefs.SetString("Save", SceneManager.GetActiveScene().name);

        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(enableMovementIn);
        m_Player.enabled = true;
    }

    private void Update()
    {
        if (platform.Count > 0) {
            for(int i = 0; i < platform.Count; i++) {

                if (platform[i]._object.position != platform[i].targetTransform.position || platform[i]._object.rotation != platform[i].targetTransform.rotation) {
                    platform[i]._object.position = Vector3.MoveTowards(platform[i]._object.position, platform[i].targetTransform.position, platform[i].moveSpeed * Time.deltaTime);
                    platform[i]._object.rotation = Quaternion.RotateTowards(platform[i]._object.rotation, platform[i].targetTransform.rotation, platform[i].rotateSpeed * Time.deltaTime);
                }
                else platform.Remove(platform[i]);
            }
        }
    }
    // Update is called once per frame
    public void UpdateEvent () {

        for (int i = 0; i < events.Length; i++) {

            bool allKeyActive = true;
            bool allButtonActive = true;
            bool allLeverActive = true;
            bool allPlateActive = true;

            bool keyNeeded = false;
            for (int j = 0; j < events[i].item.Length; j++) {

                if (!events[i].item[j].active) {
                    allKeyActive = false;
                    break;
                }

                keyNeeded = true;
            }

            for (int j = 0; j < events[i].buttons.Length; j++) {

                if (!events[i].buttons[j].active) {
                    allButtonActive = false;
                    break;
                }
            }

            for (int j = 0; j < events[i].levers.Length; j++) {

                if (events[i].levers[j].lever.active != events[i].levers[j].active) {
                    allLeverActive = false;
                    break;
                }
            }

            for (int j = 0; j < events[i].pressurePlates.Length; j++) {

                if (!events[i].pressurePlates[j].active) {
                    allPlateActive = false;
                    break;
                }
            }

            //CONSEQUENCE
            if (allKeyActive && allButtonActive && allLeverActive && allPlateActive) {

                for (int j = 0; j < events[i].dropObject.Length; j++)
                    events[i].dropObject[j].SetActive(true);

                for (int j = 0; j < events[i].door.Length; j++) {
                    if (!keyNeeded) {

                        if (events[i].door[j].toDoor != null)
                            player.transform.position = events[i].door[j].toDoor.position;

                        events[i].door[j].doorAnim.SetTrigger("Open");
                    }
                    else if (Vector3.Distance(m_Player.transform.position, events[i].door[j].doorAnim.transform.position) < 3) {
                        events[i].door[j].doorAnim.SetTrigger("Open");

                        StartCoroutine(EndLevel());
                        m_Player.enableMovements = false;
                    }
                }

                for (int j = 0; j < events[i].platform.Length; j++) {
                    platform.Add(events[i].platform[j]);

                    for(int k = 0; k < events.Length; k++) {
                        for(int l = 0; l < events[k].platform.Length; l++)
                            if (k != i && events[k].platform[l]._object == events[i].platform[j]._object)
                                platform.Remove(events[k].platform[l]);
                    }
                }
            }

        }
    }

    public IEnumerator EndLevel()
    {

        GetComponentInChildren<Animator>().Play("Anim_EndLevel");
        yield return new WaitForSeconds(enableMovementIn);
        SceneManager.LoadScene(nextLevel);
    }
}
