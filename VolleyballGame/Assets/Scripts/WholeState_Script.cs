using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WholeState_Script : MonoBehaviour
{
    [SerializeField] private PCState_Script PCState_PC1;
    [SerializeField] private PCState_Script PCState_PC2;
    [SerializeField] private EnemyState_Script EnemyState_E1;
    [SerializeField] private EnemyState_Script EnemyState_E2;
    [SerializeField] private BallMove_Script BallMove_Scp;
    [SerializeField] private GameObject ballobj;
    [SerializeField] private GameObject playerCharaFirst;
    [SerializeField] private GameObject playerCharaSecond;
    [SerializeField] private GameObject enemyCharaFirst;
    [SerializeField] private GameObject enemyCharaSecond;
    [SerializeField] private GameObject sideBorder;
    [SerializeField] private Transform PlayerServeTrans;
    [SerializeField] private Transform EnemyServeTrans;
    [SerializeField] private GameObject Net;
    public GameObject PlayerCourt;
    public GameObject EnemyCourt;
    public GameObject Court;

    public BallBehaviorStates BallState_CS;
    public AttackSideState AttackSide_CS;
    public ActionStates ActionStates_CS;
    public int TouchCount;

    public bool PlayerBallMoveFlag;
    public bool EnemyBallMoveFlag;

    public ServeValue playerServe;
    public ServeValue enemyServe;


    private AttackSideState PrePointGetter;


    private int PlayerPoint;
    private int EnemyPoint;

    private bool GetPointFlag;
    private bool LandFlag;

    public bool EnterFlag;

    [SerializeField] private TextMeshProUGUI PointCounterText;

    [SerializeField] private GameObject[] canvas;
    private int nowActiveCanvas;
    [SerializeField] private TextMeshProUGUI resultText;

    private bool OverTouchFlag;

    public float PlayerUrtGauge;
    [SerializeField] private Slider PlayerGaugeSlider;
    [SerializeField] private Image PlayerFill;

    public float GaugeMaxValue;


    // Start is called before the first frame update
    void Start()
    {
        //DebugStart();
        Initializer();
        GaugeInitialize(PlayerGaugeSlider);
    }

    // Update is called once per frame
    void Update()
    {
        //DebugUpDate();
        GaugeADM(PlayerGaugeSlider, PlayerUrtGauge, PlayerFill, Color.red);
        OverTouch();
        Poze();
    }

    void DebugStart()
    {
        AttackSide_CS = AttackSideState.Enemy;

        FirstMovementADM();

    }

    void DebugUpDate()
    {
        Debug.Log("Ball_PC1: " + PCState_PC1.MoveChara + "   Ball_PC2: " + PCState_PC2.MoveChara);
    }


    void GaugeInitialize(Slider slider)
    {
        slider.minValue = 0;
        slider.maxValue = GaugeMaxValue;
        slider.value = 0;
    }

    void GaugeADM(Slider slider, float value, Image fill, Color MaxColor)
    {
        slider.value = value;
        if(value >= GaugeMaxValue)
        {
            fill.color = MaxColor;
        }
        else
        {
            fill.color = Color.white;
        }
    }


    void Initializer()
    {
        for (int i = 0; i < canvas.Length; i++)
        {
            if (i == 0)
            {
                canvas[i].SetActive(true);
                nowActiveCanvas = i;
            }
            else
            {
                canvas[i].SetActive(false);
            }
        }
        PlayerPoint = 0;
        EnemyPoint = 0;

        PlayerUrtGauge = 0;
        GaugeMaxValue = 30;

        OverTouchFlag = false;
        playerServe.Flag = true;
        playerServe.Counter = 0;
        enemyServe.Flag = false;
        enemyServe.Counter = 0;
        PrePointGetter = AttackSideState.Player;
        LandFlag = false;
        GetPointFlag = false;
        EnterFlag = false;
        TouchCount = 0;
        AttackSide_CS = AttackSideState.Player;
        ActionStates_CS = ActionStates.Serve;
        BallState_CS = BallBehaviorStates.Stop;
        PointCounterText.text = "Player: " + PlayerPoint + "  VS  " + EnemyPoint + " :Enemy";

        playerCharaFirst.transform.position = PlayerServeTrans.position;
        playerCharaSecond.transform.position = PCState_PC2.BlockTrans.position;
        enemyCharaFirst.transform.position = EnemyState_E1.BlockTrans.position;
        enemyCharaSecond.transform.position = EnemyState_E2.BlockTrans.position;
        ballobj.transform.position = PlayerServeTrans.position;

    }

    void BallStateADM() //ボールのStateを制御する関数
    {

        switch (BallState_CS)
        {
            case BallBehaviorStates.Pass:
                BallState_CS = BallBehaviorStates.Attack;
                break;

            case BallBehaviorStates.Attack:
                BallState_CS = BallBehaviorStates.Pass;
                break;
            default:
                break;
        }
    }

    public void AttackSideADM() //手番の切り替え
    {
        switch (AttackSide_CS)
        {
            case AttackSideState.Player:
                PCState_PC1.MoveChara = false;
                PCState_PC2.MoveChara = false;
                break;
            case AttackSideState.Enemy:
                EnemyState_E1.MoveChara = false;
                EnemyState_E2.MoveChara = false;
                break;
        }

        if (AttackSide_CS == AttackSideState.Player)
        {
            AttackSide_CS = AttackSideState.Enemy;
        }
        else if (AttackSide_CS == AttackSideState.Enemy)
        {
            AttackSide_CS = AttackSideState.Player;
        }
    }

    void FirstMovementADM() //どちらのキャラが動くのかを決定
    {


        switch (AttackSide_CS)
        {
            case AttackSideState.Player:
                if (ThreeDistanceCheck(ballobj.transform, playerCharaFirst.transform, playerCharaSecond.transform))
                {
                    PCState_PC1.MoveChara = true;
                    PCState_PC2.MoveChara = false;
                    break;
                }
                else if (ThreeDistanceCheck(ballobj.transform, playerCharaSecond.transform, playerCharaFirst.transform))
                {
                    PCState_PC1.MoveChara = false;
                    PCState_PC2.MoveChara = true;
                    break;
                }
                break;

            case AttackSideState.Enemy:
                if (ThreeDistanceCheck(ballobj.transform, enemyCharaFirst.transform, enemyCharaSecond.transform))
                {
                    EnemyState_E1.MoveChara = true;
                    EnemyState_E2.MoveChara = false;
                    break;
                }
                else if (ThreeDistanceCheck(ballobj.transform, enemyCharaSecond.transform, enemyCharaFirst.transform))
                {
                    EnemyState_E1.MoveChara = false;
                    EnemyState_E2.MoveChara = true;
                    break;
                }

                break;
        }
    }

    public bool ThreeDistanceCheck(Transform TargetPos, Transform SelfPos, Transform OtherPos) //２オブジェクトのどちらが近いのかを判定
    {
        float SelfDistance = Vector3.Distance(TargetPos.position, SelfPos.position);
        float OtherDistance = Vector3.Distance(TargetPos.position, OtherPos.position);

        if (SelfDistance < OtherDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OverTouch()
    {
        if(TouchCount > 3 && !OverTouchFlag)
        {
            BallState_CS = BallBehaviorStates.Stop;

            switch (AttackSide_CS)
            {
                case AttackSideState.Player:
                    PlayerUrtGauge--;
                    EnemyPoint++;
                    OverTouchFlag = true; 
                    LandFlag = true;
                    if (PrePointGetter == AttackSideState.Player)
                    {
                        playerServe.Flag = false;
                        enemyServe.Flag = true;
                        if (enemyServe.Counter == 0)
                        {
                            enemyServe.Counter = 1;
                        }
                        else
                        {
                            enemyServe.Counter = 0;
                        }
                        PrePointGetter = AttackSideState.Enemy;
                    }
                    EndGame(PlayerPoint, EnemyPoint);
                    Invoke(nameof(ResetField), 2f);
                    break;

                case AttackSideState.Enemy:
                    PlayerPoint++;
                    OverTouchFlag = true;
                    LandFlag = true;
                    if (PrePointGetter == AttackSideState.Enemy)
                    {
                        enemyServe.Flag = false;
                        playerServe.Flag = true;
                        if (playerServe.Counter == 0)
                        {
                            playerServe.Counter = 1;
                        }
                        else
                        {
                            playerServe.Counter = 0;
                        }
                        PrePointGetter = AttackSideState.Player;

                    }
                    EndGame(PlayerPoint, EnemyPoint);
                    Invoke(nameof(ResetField), 2f);
                    break;
            }
        }
    }

    void PointAdder(Collider collider)
    {

        switch (GetPointFlag)
        {
            case true:
                if (collider.gameObject == PlayerCourt && !LandFlag)
                {
                    BallState_CS = BallBehaviorStates.Stop;

                    EnemyPoint++;
                    LandFlag = true;
                    if (PrePointGetter == AttackSideState.Player)
                    {
                        playerServe.Flag = false;
                        enemyServe.Flag = true;
                        if (enemyServe.Counter == 0)
                        {
                            enemyServe.Counter = 1;
                        }
                        else
                        {
                            enemyServe.Counter = 0;
                        }
                        PrePointGetter = AttackSideState.Enemy;
                    }
                    EndGame(PlayerPoint, EnemyPoint);
                    Invoke(nameof(ResetField), 2f);
                    EnterFlag = true;
                }
                else if (collider.gameObject == EnemyCourt && !LandFlag)
                {
                    BallState_CS = BallBehaviorStates.Stop;

                    PlayerPoint++;
                    LandFlag = true;
                    if (PrePointGetter == AttackSideState.Enemy)
                    {
                        enemyServe.Flag = false;
                        playerServe.Flag = true;
                        if (playerServe.Counter == 0)
                        {
                            playerServe.Counter = 1;
                        }
                        else
                        {
                            playerServe.Counter = 0;
                        }
                        PrePointGetter = AttackSideState.Player;

                    }
                    EndGame(PlayerPoint, EnemyPoint);
                    Invoke(nameof(ResetField), 2f);
                    EnterFlag = true;

                }
                break;

            case false:
                if (collider.gameObject == PlayerCourt && !LandFlag || collider.gameObject == EnemyCourt && !LandFlag)
                {
                    BallState_CS = BallBehaviorStates.Stop;
                    switch (AttackSide_CS)
                    {
                        case AttackSideState.Player:
                            EnemyPoint++;
                            LandFlag = true;
                            if (PrePointGetter == AttackSideState.Player)
                            {
                                playerServe.Flag = false;
                                enemyServe.Flag = true;
                                if (enemyServe.Counter == 0)
                                {
                                    enemyServe.Counter = 1;
                                }
                                else
                                {
                                    enemyServe.Counter = 0;
                                }
                                PrePointGetter = AttackSideState.Enemy;
                            }
                            EndGame(PlayerPoint, EnemyPoint);
                            Invoke(nameof(ResetField), 2f);
                            break;

                        case AttackSideState.Enemy:
                            PlayerPoint++;
                            LandFlag = true;
                            if (PrePointGetter == AttackSideState.Enemy)
                            {
                                enemyServe.Flag = false;
                                playerServe.Flag = true;
                                if (playerServe.Counter == 0)
                                {
                                    playerServe.Counter = 1;
                                }
                                else
                                {
                                    playerServe.Counter = 0;
                                }
                                PrePointGetter = AttackSideState.Player;

                            }
                            EndGame(PlayerPoint, EnemyPoint);
                            Invoke(nameof(ResetField), 2f);
                            break;
                    }
                    EnterFlag = true;
                }
                break;
        }

        if (collider.gameObject.tag == "Court" && !LandFlag)
        {
            if (PlayerBallMoveFlag)
            {
                BallState_CS = BallBehaviorStates.Stop;
                EnemyPoint++;
                if (PrePointGetter == AttackSideState.Player)
                {
                    playerServe.Flag = false;
                    enemyServe.Flag = true;
                    if (enemyServe.Counter == 0)
                    {
                        enemyServe.Counter = 1;
                    }
                    else
                    {
                        enemyServe.Counter = 0;
                    }
                    PrePointGetter = AttackSideState.Enemy;
                }
                EndGame(PlayerPoint, EnemyPoint);
                Invoke(nameof(ResetField), 2f);
            }
            else if (EnemyBallMoveFlag)
            {
                BallState_CS = BallBehaviorStates.Stop;
                PlayerPoint++;
                if (PrePointGetter == AttackSideState.Enemy)
                {
                    enemyServe.Flag = false;
                    playerServe.Flag = true;
                    if (playerServe.Counter == 0)
                    {
                        playerServe.Counter = 1;
                    }
                    else
                    {
                        playerServe.Counter = 0;
                    }
                    PrePointGetter = AttackSideState.Player;
                }
                EndGame(PlayerPoint, EnemyPoint);
                Invoke(nameof(ResetField), 2f);

            }
            EnterFlag = true;
            LandFlag = true;
        }

        if (collider.gameObject == Net)
        {
            //BallMove_Scp.MoveSpeed = 0;
            //BallMove_Scp.InitPos = ballobj.transform.position;
            //BallMove_Scp.ElapsedTime = 0;

            BallState_CS = BallBehaviorStates.Stop;
            switch (AttackSide_CS)
            {
                case AttackSideState.Player:
                    EnemyPoint++;
                    LandFlag = true;
                    if (PrePointGetter == AttackSideState.Player)
                    {
                        playerServe.Flag = false;
                        enemyServe.Flag = true;
                        if (enemyServe.Counter == 0)
                        {
                            enemyServe.Counter = 1;
                        }
                        else
                        {
                            enemyServe.Counter = 0;
                        }
                        PrePointGetter = AttackSideState.Enemy;
                    }
                    EndGame(PlayerPoint, EnemyPoint);
                    Invoke(nameof(ResetField), 2f);
                    break;

                case AttackSideState.Enemy:
                    PlayerPoint++;
                    LandFlag = true;
                    if (PrePointGetter == AttackSideState.Enemy)
                    {
                        enemyServe.Flag = false;
                        playerServe.Flag = true;
                        if (playerServe.Counter == 0)
                        {
                            playerServe.Counter = 1;
                        }
                        else
                        {
                            playerServe.Counter = 0;
                        }
                        PrePointGetter = AttackSideState.Player;

                    }
                    EndGame(PlayerPoint, EnemyPoint);
                    Invoke(nameof(ResetField), 2f);
                    break;
            }
            EnterFlag = true;
        }

    }



    void ResetField()
    {
        PointCounterText.text = "Player: " + PlayerPoint + "  VS  " + EnemyPoint + " :Enemy";
        TouchCount = 0;
        BallMove_Scp.ballStatus.Power = 0;
        ActionStates_CS = ActionStates.Serve;
        BallState_CS = BallBehaviorStates.Stop;
        EnterFlag = false;

        if (playerServe.Flag)
        {
            AttackSide_CS = AttackSideState.Player;
            PCState_PC1.AttackEndFlag = false;
            PCState_PC2.AttackEndFlag = false;

            if (playerServe.Counter == 0)
            {
                playerCharaFirst.transform.position = PlayerServeTrans.position;
                playerCharaSecond.transform.position = PCState_PC2.BlockTrans.position;
                PCState_PC1.ServeStartFlag = true;
                PCState_PC1.TouchFlag = true;
                PCState_PC1.MoveChara = true;
                PCState_PC2.MoveChara = false;
            }
            else
            {
                playerCharaFirst.transform.position = PCState_PC1.BlockTrans.position;
                playerCharaSecond.transform.position = PlayerServeTrans.position;
                PCState_PC2.ServeStartFlag = true;
                PCState_PC2.TouchFlag = true;
                PCState_PC1.MoveChara = false;
                PCState_PC2.MoveChara = true;
            }
            enemyCharaFirst.transform.position = EnemyState_E1.BlockTrans.position;
            enemyCharaSecond.transform.position = EnemyState_E2.BlockTrans.position;
            ballobj.transform.position = PlayerServeTrans.position;
        }
        if (enemyServe.Flag)
        {
            AttackSide_CS = AttackSideState.Enemy;
            EnemyState_E1.AttackEndFlag = false;
            EnemyState_E2.AttackEndFlag = false;


            playerCharaFirst.transform.position = PCState_PC1.BlockTrans.position;
            playerCharaSecond.transform.position = PCState_PC2.BlockTrans.position;
            if (enemyServe.Counter == 0)
            {
                enemyCharaFirst.transform.position = EnemyServeTrans.position;
                enemyCharaSecond.transform.position = EnemyState_E2.BlockTrans.position;
                EnemyState_E1.ServeStartFlag = true;
                EnemyState_E1.TouchFlag = true;
                EnemyState_E1.MoveChara = true;
                EnemyState_E2.MoveChara = false;
            }
            else
            {
                enemyCharaFirst.transform.position = EnemyState_E1.BlockTrans.position;
                enemyCharaSecond.transform.position = EnemyServeTrans.position;
                EnemyState_E2.ServeStartFlag = true;
                EnemyState_E2.TouchFlag = true;
                EnemyState_E1.MoveChara = false;
                EnemyState_E2.MoveChara = true;
            }
            ballobj.transform.position = EnemyServeTrans.position;

        }
        OverTouchFlag = false;
        GetPointFlag = false;
        LandFlag = false;
    }

    void EndGame(int PlayerPoint, int EnemyPoint)
    {
        if (PlayerPoint >= 15 && PlayerPoint - EnemyPoint >= 2)
        {
            Time.timeScale = 0;
            resultText.text = "YOU WIN";
            resultText.color = Color.red;
            ChangeCanvas(2);
        }
        if (EnemyPoint >= 15 && EnemyPoint - PlayerPoint >= 2)
        {
            Time.timeScale = 0;
            resultText.text = "YOU LOSE";
            resultText.color = Color.blue;
            ChangeCanvas(2);
        }
    }

    void ChangeCanvas(int afterCanvas)
    {
        canvas[nowActiveCanvas].SetActive(false);
        nowActiveCanvas = afterCanvas - 1;
        canvas[afterCanvas - 1].SetActive(true);
    }


    public void ChangeScene(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }


    private void OnTriggerEnter(Collider collider)
    {
        //if (!EnterFlag)
        //{
        if (collider.gameObject == sideBorder)
        {
            TouchCount = 0;
            GetPointFlag = true;
            ActionStates_CS = ActionStates.Receive;

            AttackSideADM();
            FirstMovementADM();

        }
        PointAdder(collider);

        //}
    }


    void Poze()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            switch (nowActiveCanvas)
            {
                case 0:
                    ChangeCanvas(3);
                    Time.timeScale = 0;
                    break;

                case 2:
                    ChangeCanvas(1);
                    Time.timeScale = 1;
                    break;

            }
        }
    }


}
public enum ActionStates
{
    Receive,
    Toss,
    Attack,
    Serve
}

public enum BallBehaviorStates
{
    Pass,
    Toss,
    Attack,
    Serve,
    Stop
}

public enum AttackSideState
{
    Player,
    Enemy
}

public struct ServeValue
{
    public bool Flag;
    public int Counter;
}


