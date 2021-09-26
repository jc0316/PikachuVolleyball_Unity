using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddShadow : MonoBehaviour
{
    public GameObject[] ShadowyObjects;
    public GameObject Shadow;
    public GameObject[] Shadows;

    public float groundSurface = -7.68f;

    // Start is called before the first frame update
    void Start()
    {
        Shadows = new GameObject[ShadowyObjects.Length];
        for (int i = 0; i < ShadowyObjects.Length; i++)
        {
            GameObject shadow = Instantiate(Shadow, new Vector3(0, groundSurface, 0), Quaternion.identity);
            Shadows[i] = shadow;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for(int i = 0; i < ShadowyObjects.Length; i++)
        {
            Shadows[i].transform.localPosition = new Vector3(ShadowyObjects[i].transform.position.x, groundSurface, 0);
        }
    }
}
