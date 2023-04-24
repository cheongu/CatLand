using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private CountDown countDown;
    [SerializeField]
    private MoleSpawner moleSpawner;
    private int score;
    private int combo;
    private float currentTime;

    public int Score
    {
        set => score = Mathf.Max(0, value);
        get => score;
    }
    
    public int Combo
    {
        // set => combo = Mathf.Max(0, value);
        set
        {
            combo = Mathf.Max(0, value);
            // 70 이상일 땐 MaxSpawnMole이 5 고정되기 때문에 70까지만 체크   
            if ( combo <= 70)
            {
                // 콤보에 따라 생성되는 최대 두더지 숫자
                moleSpawner.MaxSpawnMole = 1 + (combo + 10) / 20;
            }
            if ( combo > MaxCombo)
            {
                MaxCombo = combo;
            }
        }
        get => combo;
    }
    public int MaxCombo { private set; get; }

    public int NormalMoleHitCount { set; get; }
    public int RedMoleHitCount { set; get; }
    public int BlueMoleHitCount { set; get; }

    [field: SerializeField]
    public float MaxTime { private set; get; }

    //public float CurrentTIme { private set; get; }
    public float CurrentTime
    {
        set => currentTime = Mathf.Clamp(value, 0, MaxTime);
        get => currentTime;
    }

    // Start is called before the first frame update
    private void Start()
    {
        countDown.StartCountDown(GameStart);
    }

    private void GameStart()
    {
        moleSpawner.Setup();

        StartCoroutine("OnTimeCount");
    }

    private IEnumerator OnTimeCount()
    {
        CurrentTime = MaxTime;

        while ( CurrentTime > 0 )
        {
            CurrentTime -= Time.deltaTime;

            yield return null;
        }

        // CurrentTime이 0이 되면 GameOver() 메소드를 호출해 게임오버 처리
        GameOver();

    }

    private void GameOver()
    {
        // 현재 스테이지에서 획득한 여러 정보 저장
        PlayerPrefs.SetInt("CurrentScore", Score);
        PlayerPrefs.SetInt("CurrentMaxCombo", MaxCombo);
        // PlayerPrefs.SetInt("CurrentNormalMoleHitCount", NormalMoleHitCount);
        //PlayerPrefs.SetInt("CurrentRedMoleHitCount", RedMoleHitCount);
        // PlayerPrefs.SetInt("CurrentBlueMoleHitCount", BlueMoleHitCount);

        // "GameOver" 씬으로 이동
        SceneManager.LoadScene("GameOver");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
