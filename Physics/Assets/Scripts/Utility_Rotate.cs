using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility_Rotate : MonoBehaviour
{
    [SerializeField]
    float speed = 0;

    [SerializeField]
    float degree = 0;
    //float radian = 0;

    //Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        degree = speed * Time.deltaTime;
        transform.Rotate(0, 0, degree);
    }
}
