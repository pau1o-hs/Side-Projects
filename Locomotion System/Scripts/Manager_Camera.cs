using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Camera : MonoBehaviour {

    public Transform target;
    public Vector2 sensetivity;
    public float followSpeed;
	public bool showCursor = true;

	[Header("Collider Settings")]
	public float dampIn = 10;
	public float dampOut = 10;

	Transform pivot;
	Camera cam;

	float t;

	float fov;
	float camDist;
	float pivotDist;

    float yaw;
    float pitch;

	float shakeIntensity;
	float shakeDuration;

    // Start is called before the first frame update
    void Start()
    {
		if (showCursor)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		else
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		pivot = transform.Find("Pivot");
		cam = pivot.GetComponentInChildren<Camera>();

		fov = cam.fieldOfView;
		camDist = -cam.transform.localPosition.z;
		pivotDist = pivot.localPosition.x;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		// INPUT
        yaw += Input.GetAxis("Mouse X") * sensetivity.x;
        pitch -= Input.GetAxis("Mouse Y") * sensetivity.y;
		pitch = Mathf.Clamp(pitch, -70f, 80f);

		// TRANSLATE AND ROTATE
		transform.eulerAngles = new Vector3(0, yaw);
        pivot.eulerAngles = new Vector3(pitch, transform.eulerAngles.y);
        transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);

		// CAMERA SHAKE
		if (shakeDuration != 0 && t < 1)
		{
			t += Time.deltaTime / shakeDuration;
			cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov + shakeIntensity, t);
			//print(t);
		}
		else {
			t = shakeDuration = 0;
			cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, 5 * Time.deltaTime);
		}

		// CAMERA COLLISION
		RaycastHit hitInfo;
		if (Physics.Raycast(pivot.position, cam.transform.position - pivot.position, out hitInfo, camDist))
		{
			if (!hitInfo.collider.CompareTag("Player"))
				cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, Mathf.Lerp(cam.transform.localPosition.z, -hitInfo.distance + .5f, dampIn * Time.deltaTime));
		}
		else {
			cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, Mathf.Lerp(cam.transform.localPosition.z, -camDist + .5f, dampOut * Time.deltaTime));
		}

		// PIVOT COLLISION
		Vector3 pivotOrigin = new Vector3(transform.position.x, pivot.position.y, transform.position.z);
		if (Physics.Raycast(pivotOrigin, pivot.position - pivotOrigin, out hitInfo, pivotDist))
		{
			if (!hitInfo.collider.CompareTag("Player"))
				pivot.localPosition = new Vector3(Mathf.Lerp(pivot.localPosition.x, hitInfo.distance - .2f, dampIn * Time.deltaTime), pivot.localPosition.y, pivot.localPosition.z);
			//print("Hitting " + hitInfo.collider.name);
		}
		else {
			pivot.localPosition = new Vector3(Mathf.Lerp(pivot.localPosition.x, pivotDist, dampOut * Time.deltaTime), pivot.localPosition.y, pivot.localPosition.z);
		}
	}
	public void Shake(float intensity, float duration)
	{
		shakeIntensity = intensity;
		shakeDuration = duration;
	}
}
