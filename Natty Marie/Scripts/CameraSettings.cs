using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraSettings : MonoBehaviour {

    public Transform[] target;
    public float damp;

    Vector3 targetPos;
    int P2, P3, P4;
    float maxDist;
    float minCamSize; 
    public float maxDistance = 10;

    PostProcessingProfile ppProfile;
    public enum Light {Dark, Lantern, Lamp};
    public Light m_Light;

    ColorGradingModel.Settings colorGrad;
    VignetteModel.Settings vignetteSet;
    public Vector3 lightIntensity;
    public Vector3 vignetteIntensity;

    // Use this for initialization
    void Start () {

        transform.SetParent(null);
        minCamSize = GetComponent<Camera>().orthographicSize;
        ppProfile = GetComponent<PostProcessingBehaviour>().profile;
        //StartCoroutine(StartGame());
	}

    //IEnumerator StartGame()
    //{
    //    target[0].GetComponent<CharacterScript>().enabled = true;
    //    if (target[1] != null) target[1].GetComponent<CharacterScript>().enabled = true;
    //    if (target[2] != null) target[2].GetComponent<CharacterScript>().enabled = true;
    //    if (target[3] != null) target[3].GetComponent<CharacterScript>().enabled = true;

    //}

    // Update is called once per frame
    void FixedUpdate () {

        //CamSet();
        LightSet();
        transform.position = new Vector3(target[0].position.x, target[0].position.y + .8f, transform.position.z);
	}

    void CamSet()
    {
        maxDist = 0;

        //CALCULATE CENTER POSITION OF PLAYERS
        target[0].GetComponent<CharacterScript>().transform.position = new Vector2(Mathf.Clamp(target[0].GetComponent<CharacterScript>().transform.position.x,
                                                                            Camera.main.transform.position.x - ((minCamSize + maxDistance * .25f) * 2 - 3),
                                                                            Camera.main.transform.position.x + ((minCamSize + maxDistance * .25f) * 2 - 3)),
                                                                            target[0].GetComponent<CharacterScript>().transform.position.y);

        if (target[1] != null) {

            if (Vector3.Distance(target[0].position, target[1].position) > maxDist) {
                maxDist = Vector3.Distance(target[0].position, target[1].position);
                targetPos = target[1].position;
            }

            target[1].GetComponent<CharacterScript>().transform.position = new Vector2(Mathf.Clamp(target[1].GetComponent<CharacterScript>().transform.position.x,
                                                                                Camera.main.transform.position.x - ((minCamSize + maxDistance * .25f) * 2 - 3),
                                                                                Camera.main.transform.position.x + ((minCamSize + maxDistance * .25f) * 2 - 3)),
                                                                                target[1].GetComponent<CharacterScript>().transform.position.y);
            if (target[2] != null) {

                if (Vector3.Distance(target[0].position, target[2].position) > maxDist) {
                    maxDist = Vector3.Distance(target[0].position, target[2].position);
                    targetPos = target[2].position;
                }

                if (Vector3.Distance(target[1].position, target[2].position) > maxDist) {
                    maxDist = Vector3.Distance(target[1].position, target[2].position);
                    targetPos = target[2].position;
                }

                target[2].GetComponent<CharacterScript>().transform.position = new Vector2(Mathf.Clamp(target[2].GetComponent<CharacterScript>().transform.position.x,
                                                                    Camera.main.transform.position.x - ((minCamSize + maxDistance * .25f) * 2 - 3),
                                                                    Camera.main.transform.position.x + ((minCamSize + maxDistance * .25f) * 2 - 3)),
                                                                    target[2].GetComponent<CharacterScript>().transform.position.y);

                if (target[3] != null) {

                    if (Vector3.Distance(target[0].position, target[3].position) > maxDist) {
                        maxDist = Vector3.Distance(target[0].position, target[3].position);
                        targetPos = target[3].position;
                    }
                    if (Vector3.Distance(target[1].position, target[3].position) > maxDist) {
                        maxDist = Vector3.Distance(target[1].position, target[3].position);
                        targetPos = target[3].position;
                    }
                    if (Vector3.Distance(target[2].position, target[3].position) > maxDist) {
                        maxDist = Vector3.Distance(target[2].position, target[3].position);
                        targetPos = target[3].position;
                    }

                    target[3].GetComponent<CharacterScript>().transform.position = new Vector2(Mathf.Clamp(target[3].GetComponent<CharacterScript>().transform.position.x,
                                                                    Camera.main.transform.position.x - ((minCamSize + maxDistance * .25f) * 2 - 3),
                                                                    Camera.main.transform.position.x + ((minCamSize + maxDistance * .25f) * 2 - 3)),
                                                                    target[3].GetComponent<CharacterScript>().transform.position.y);

                    //targetPos /= 4;
                }
                //else targetPos /= 3;
            }

            targetPos += target[0].position;
            targetPos /= 2;
        }
        else targetPos = target[0].position;

        targetPos.y += 0.25f * Camera.main.orthographicSize;
        targetPos.z = -10;

        maxDist = Mathf.Clamp(maxDist, 0, maxDistance);

        Camera.main.orthographicSize = minCamSize + maxDist * .25f;

        transform.position = Vector3.Lerp(transform.position, targetPos, damp * Time.deltaTime);
    }
    
    void LightSet()
    {
        switch (m_Light) {

            case Light.Dark:
                colorGrad = ppProfile.colorGrading.settings;
                colorGrad.basic.postExposure = Mathf.Lerp(colorGrad.basic.postExposure, lightIntensity.x, 2 * Time.deltaTime);
                ppProfile.colorGrading.settings = colorGrad;

                vignetteSet = ppProfile.vignette.settings;
                vignetteSet.intensity = Mathf.Lerp(vignetteSet.intensity, vignetteIntensity.x, 2 * Time.deltaTime);
                ppProfile.vignette.settings = vignetteSet;
                break;

            case Light.Lantern:
                colorGrad = ppProfile.colorGrading.settings;
                colorGrad.basic.postExposure = Mathf.Lerp(colorGrad.basic.postExposure, lightIntensity.y, 2 * Time.deltaTime);
                ppProfile.colorGrading.settings = colorGrad;

                vignetteSet = ppProfile.vignette.settings;
                vignetteSet.intensity = Mathf.Lerp(vignetteSet.intensity, vignetteIntensity.y, 2 * Time.deltaTime);
                ppProfile.vignette.settings = vignetteSet;
                break;

            case Light.Lamp:
                colorGrad = ppProfile.colorGrading.settings;
                colorGrad.basic.postExposure = Mathf.Lerp(colorGrad.basic.postExposure, lightIntensity.z, 2 * Time.deltaTime);
                ppProfile.colorGrading.settings = colorGrad;

                vignetteSet = ppProfile.vignette.settings;
                vignetteSet.intensity = Mathf.Lerp(vignetteSet.intensity, vignetteIntensity.z, 2 * Time.deltaTime);
                ppProfile.vignette.settings = vignetteSet;
                break;
        }
    }
}