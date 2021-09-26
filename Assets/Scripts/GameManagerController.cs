using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerController : MonoBehaviour
{
    public GameObject p1ScoreBoard;
    public GameObject p2ScoreBoard;
    public GameObject Player1;
    public GameObject Player2;
    public GameObject Ball;
    public UIManager uimanager;


    Player1Controller Player1_Controller;
    Player2Controller Player2_Controller;
    BallController Ball_Controller;

    public bool roundOver = false;
    public bool practiceMode = true;
    public int p1Score = 0;
    public int p2Score = 0;
    public int whoScored = 0;
    public float RoundendSlowmoTime = 1f;
    public float RoundstartSlowmoTime = 0.7f;

    // Start is called before the first frame update
    float width = 26.5f;
    float height = 19f;
    void Start()
    {
        StartCoroutine(SlowmoStart(RoundstartSlowmoTime));
        if (!practiceMode)
        {
            Player1_Controller = Player1.GetComponent<Player1Controller>();
            Player2_Controller = Player2.GetComponent<Player2Controller>();
            Ball_Controller = Ball.GetComponent<BallController>();
            initializeForNewRound();
            FindObjectOfType<AudioManager>().Play("bgm");
        }
        else if(practiceMode)
        {
            initializeForNewRound();
            uimanager.enabled = false;
        }
    }

    public void initializeForNewRound()
    {
        if(!practiceMode)
        {
            p1ScoreBoard.GetComponent<ScoreBoard>().changeScore(p1Score);
            p2ScoreBoard.GetComponent<ScoreBoard>().changeScore(p2Score);
            roundOver = false;
            Player1.GetComponent<Player1Controller>().initializeForNewRound();
            Player2.GetComponent<Player2Controller>().initializeForNewRound();

        }
        //spawnManyCourts();
    }
    

    public void addScore(int player)
    {
        if (!roundOver)
        {
            roundOver = true;
            if (player == 1)
            {
                p1Score = p1Score + 1;
                p1ScoreBoard.GetComponent<ScoreBoard>().changeScore(p1Score);
                whoScored = 1;
            }
            else if (player == 2)
            {
                p2Score = p2Score + 1;
                p2ScoreBoard.GetComponent<ScoreBoard>().changeScore(p2Score);
                whoScored = 2;
            }
        }
        StartCoroutine(SlowmoEnd(RoundendSlowmoTime));
    }

    private IEnumerator SlowmoEnd(float duration)
    {
        Time.timeScale = 0.4f;
        yield return new WaitForSeconds(duration);
        StartCoroutine(SlowmoStart(RoundstartSlowmoTime));
    }
    private IEnumerator SlowmoStart(float duration)
    {
        uimanager.BlackFadein(1f, 0.3f);
        
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < duration)
        {
            yield return null;
        }
        if (whoScored == 1 || whoScored == 0)
        {
            Ball.GetComponent<BallController>().initializeForNewRound(false);
        }
        else if (whoScored == 2)
        {
            Ball.GetComponent<BallController>().initializeForNewRound(true);
        }
        initializeForNewRound();
        uimanager.BlackFadeout(0.1f);
        Time.timeScale = 1f;
        StartCoroutine(FreezeBall(1f));
    }
    private IEnumerator FreezeBall(float duration)
    {
        Ball_Controller.gravity_amount = 0f;
        yield return new WaitForSeconds(duration);
        Ball_Controller.gravity_amount = -1f;
        roundOver = false;
    }
}
