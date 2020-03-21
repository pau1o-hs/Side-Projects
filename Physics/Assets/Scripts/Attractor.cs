using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class Attractor : MonoBehaviour
{
    public float G = 0.6674f;

    public static List<Attractor> Attractors;

    [System.NonSerialized]
    public Rigidbody2D rb;
    public float attractRadius = 3.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {        
        foreach (Attractor attractor in Attractors)
        {
            if (attractor != this)
                Attract(attractor);
        }
    }

    void OnEnable()
    {
        if (Attractors == null) Attractors = new List<Attractor>();
        Attractors.Add(this);
    }

    private void OnDisable()
    {
        Attractors.Remove(this);
    }

    void Attract(Attractor objToAttract)
    {
        Rigidbody2D rbToAttract = objToAttract.rb;

        Vector2 direction = rb.position - rbToAttract.position;
        float distance = direction.magnitude;

        if (distance > attractRadius) return;

        float forceMagnitude = G * (rb.mass * rbToAttract.mass) / Mathf.Pow(distance, 2);
        Vector2 force = direction.normalized * forceMagnitude;

        rbToAttract.AddForce(force);
    }
}
