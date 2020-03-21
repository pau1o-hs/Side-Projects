using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_UI : MonoBehaviour
{
    float radius;

    // Start is called before the first frame update
    void Start()
    {
        radius = GetComponentInParent<Attractor>().attractRadius;
        BroadcastMessage("Draw", radius);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
