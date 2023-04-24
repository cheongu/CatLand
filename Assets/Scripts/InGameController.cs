using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameController : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private TextMeshProUGUI textScore;
    [SerializeField]
    private TextMeshProUGUI textPlayTime;
    [SerializeField]
    private Slider sliderPalyTime;
    [SerializeField]
    private TextMeshProUGUI textCombo;


   
     // Start is called before the first frame update
    void Start()
    {
        
    }
   

    // Update is called once per frame
    void Update()
    {
        textScore.text = "Score  " + gameController.Score;

        textPlayTime.text = gameController.CurrentTime.ToString("F1");
        sliderPalyTime.value = gameController.CurrentTime / gameController.MaxTime;

        textCombo.text = "Combo " + gameController.Combo;
    }
}
