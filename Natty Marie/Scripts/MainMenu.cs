using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public float timeToStart;

    [Header("LightChange")]
    ColorGradingModel.Settings colorGrad;
    PostProcessingProfile ppProfile;
    public Vector2 lightIntensity;
    public float speed;

    public Slider volume;
    public PostProcessingProfile pppMainMenu, pppGame;

    void Start()
    {
        ppProfile = Camera.main.GetComponent<PostProcessingBehaviour>().profile;

        Cursor.visible = true;
        Time.timeScale = 1;
    }

    private void Update()
    {
        ChangeLight();
    }

    bool going;
    void ChangeLight()
    {
        colorGrad = ppProfile.colorGrading.settings;
        ColorGradingModel.Settings game_ColorGrad = pppGame.colorGrading.settings;

        if (going) {

            going = colorGrad.basic.postExposure != lightIntensity.y;
            colorGrad.basic.postExposure = Mathf.MoveTowards(colorGrad.basic.postExposure, lightIntensity.y, speed * Time.deltaTime);
        }
        else {
            going = colorGrad.basic.postExposure == lightIntensity.x;
            colorGrad.basic.postExposure = Mathf.MoveTowards(colorGrad.basic.postExposure, lightIntensity.x, speed * Time.deltaTime);
        }

        AudioListener.volume = volume.value;
        ppProfile.colorGrading.settings = colorGrad;
        //pppGame.colorGrading.settings = colorGrad;
    }

    public GameObject OBJ_Flash;

    public void Flash()
    {
        OBJ_Flash.SetActive(true);
        StartCoroutine(TurnOff());
    }

    IEnumerator TurnOff()
    {
        yield return new WaitForSeconds(.2f);
        OBJ_Flash.SetActive(false);
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        StartCoroutine(Play("Level01"));
    }

    public void Continue()
    {
        StartCoroutine(Play(PlayerPrefs.GetString("Save")));
    }

    public void Tutorial()
    {
        StartCoroutine(Play("Tutorial"));
    }

    public IEnumerator Play(string _scene)
    {
        yield return new WaitForSeconds(timeToStart);
        SceneManager.LoadScene(_scene);
    }

    public void QuitGame ()
    {
        Application.Quit();
    }
}
