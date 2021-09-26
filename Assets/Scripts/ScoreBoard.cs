using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    public GameObject digit1;
    public GameObject digit2;

    // Start is called before the first frame update
    void Start()
    {
        digit1.GetComponent<ScoreChange>().changeNumber(0);
        digit2.GetComponent<ScoreChange>().changeNumber(0);
    }

    public void changeScore(int score)
    {
        int d1 = score % 10;
        int d2 = (score - d1)/10;
        digit1.GetComponent<ScoreChange>().changeNumber(d1);
        digit2.GetComponent<ScoreChange>().changeNumber(d2);
        if(d2 == 0)
        {
            digit2.GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            digit2.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
