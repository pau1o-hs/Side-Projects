using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw_Circle : MonoBehaviour
{
    public GameObject quad;
    public List<GameObject> vertices;
    public int numVertex = 8;

    int count = 0;
    float angle;

    // Start is called before the first frame update
    void Start()
    {
        vertices = new List<GameObject>(Resources.LoadAll<GameObject>("quad"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Draw(float radius)
    {
        for (int i = 0; i < numVertex; i++, count++)
        {
            angle += 2 * 3.14f / numVertex;

            vertices.Add(GameObject.Instantiate(quad, transform, false));
            vertices[i].transform.position = new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
            vertices[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);
        }
    }
}
