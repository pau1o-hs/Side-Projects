using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cinematic : MonoBehaviour
{
	[SerializeField] Rigidbody2D planet;

    [SerializeField] Vector3 initialPosition;

    [SerializeField] float initialVelocity;
    [SerializeField] float decceleration;
    [SerializeField] float startAngle;
    [SerializeField] float angle;

	[SerializeField] float t;
	[SerializeField] float tSpeed;
    [SerializeField] float tMax;

	Rigidbody2D rb;
	Vector2 currAcc;

	float distance;

	// Start is called before the first frame update
	void Start()
    {
		rb = GetComponent<Rigidbody2D>();

        transform.position = initialPosition;
		currAcc = Vector2.one * decceleration;
		rb.rotation = startAngle;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		if (t >= tMax)
		{
			transform.position = initialPosition;
			rb.rotation = startAngle;
			t = 0;
		}

		t = Mathf.MoveTowards(t, tMax, tSpeed * Time.realtimeSinceStartup);

		//if (Tragetory(t).magnitude > GravityForce(t).magnitude && distance >= 0)
			rb.position = Tragetory(t) + GravityForce(t);
		//else t = tMax;

		rb.rotation += angle * Time.deltaTime;
	}

	Vector2 Tragetory(float t)
	{
		Vector2 r = initialPosition + ((initialVelocity * t) + decceleration * t * t / 2) * transform.up;
		return r;
	}

	Vector2 GravityForce(float t)
	{
		Vector2 direction = planet.position - Tragetory(t);
		distance = direction.magnitude /*- planet.GetComponent<CircleCollider2D>().radius*/;

		//if (distance <= 0 || distance > planet.GetComponent<Attractor>().attractRadius) return 0;

		float forceMagnitude = planet.GetComponent<Attractor>().G * (planet.mass * rb.mass) / Mathf.Pow(distance, 5);
		Vector2 force = direction.normalized * forceMagnitude;

		return force * t * t / 2;
	}
}
