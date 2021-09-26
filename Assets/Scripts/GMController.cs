using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMController : MonoBehaviour
{
    public GameObject Court;

    // Start is called before the first frame update
    void Start()
    {
        //spawnManyCourts();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnManyCourts()
    {
        float width = 34f;
        float height = 20f;
        for (int x = 0; x < 13; x++)
        {
            for (int y = 0; y < 12; y++)
            {
                if (x + y != 0) Instantiate(Court, new Vector3(width * x, height * y, 0), Quaternion.identity);
            }
        }
    }
}
