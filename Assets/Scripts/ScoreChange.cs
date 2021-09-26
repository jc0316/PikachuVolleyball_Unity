using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreChange : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] scoreSpritesArray;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        changeNumber(0);
    }

    public void changeNumber(int num)
    {
        spriteRenderer.sprite = scoreSpritesArray[num];
    }
}
