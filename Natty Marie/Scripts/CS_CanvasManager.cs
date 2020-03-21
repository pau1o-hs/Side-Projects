using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CS_CanvasManager : MonoBehaviour {

    [System.NonSerialized] public CharacterScript m_Character;

    bool isPaused, isDead;
    public GameObject pauseMenu, GOMenu, panel_Damage, itemPrefab;
    int healthAmount;
    public Image[] IMG_Health;
   
    // Use this for initialization
    void Start () {

        transform.SetParent(null);
        UpdateItemStatus();
    }

    // Update is called once per frame
    void Update () {

        HealthDisplay();
        bool pauseInput = Input.GetKeyDown(KeyCode.Escape);

        if (pauseInput && !isDead)
            isPaused = !isPaused;

        if (pauseInput) {

            pauseMenu.SetActive(!pauseMenu.activeSelf);

            CS_GameManager.isPaused = pauseMenu.activeSelf;

            if (pauseMenu.activeSelf) {
                Time.timeScale = 0;
                UpdateItemStatus();
            }
            else Time.timeScale = 1;
        }

        if (isDead) {

            //Game Over
            GOMenu.SetActive(true);
            Cursor.visible = true;
        }

        panel_Damage.GetComponent<Image>().color = Color.Lerp(panel_Damage.GetComponent<Image>().color, Color.clear, 2 * Time.deltaTime);
    }

    public void Continue()
    {
        isPaused = false;

        pauseMenu.SetActive(false);
        CS_GameManager.isPaused = false;
        Time.timeScale = 1;
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(PlayerPrefs.GetString("Save"));
    }

    public void Quit()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void HealthDisplay()
    {
        healthAmount = m_Character.currentHealth;
        if (healthAmount >= 1) IMG_Health[0].color = Color.red;
        else IMG_Health[0].color = new Color(1, 0, 0, 0.1f);

        if (healthAmount >= 2) IMG_Health[1].color = Color.red;
        else IMG_Health[1].color = new Color(1, 0, 0, 0.1f);

        if (healthAmount >= 3) IMG_Health[2].color = Color.red;
        else IMG_Health[2].color = new Color(1, 0, 0, 0.1f);

        if (healthAmount <= 0)
            isDead = true;
    }

    public void DamageDisplay()
    {
        panel_Damage.GetComponent<Image>().color = new Color(1, 0, 0, .3f);
    }

    public void UpdateItemStatus()
    {
        for(int i = 0; i < transform.Find("Panel_PauseMenu").Find("Inventory").childCount; i++) {

            Destroy(transform.Find("Panel_PauseMenu").Find("Inventory").GetChild(0).gameObject);
        }

        for (int i = 0; i < CS_GameManager.instance.events.Length; i++) {

            for (int j = 0; j < CS_GameManager.instance.events[i].item.Length; j++) {

                GameObject item = Instantiate(itemPrefab, transform.Find("Panel_PauseMenu").Find("Inventory"), false);
                item.GetComponent<Image>().sprite = CS_GameManager.instance.events[i].item[j].GetComponent<SpriteRenderer>().sprite;

                if (CS_GameManager.instance.events[i].item[j].gameObject.activeSelf == false)
                    item.GetComponent<Image>().color = Color.white;
                else
                    item.GetComponent<Image>().color = Color.black;
            }
        }
    }
}
