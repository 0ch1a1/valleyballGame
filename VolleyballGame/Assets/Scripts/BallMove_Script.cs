using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallMove_Script : MonoBehaviour
{
    [SerializeField] private PCState_Script PCState_PC1;
    [SerializeField] private PCState_Script PCState_PC2;
    [SerializeField] private EnemyState_Script EnemyState_E1;
    [SerializeField] private EnemyState_Script EnemyState_E2;
    [SerializeField] private Transform BallTrans;
    [SerializeField] private Transform PlayerCenterTrans;
    [SerializeField] private Transform EnemyCenterTrans;
    [SerializeField] private Transform EnemyEndCornerRightTrans;
    [SerializeField] private Transform EnemyNetRightTrans;
    [SerializeField] private Transform[] EnemyTossLandTrans;
    public Transform BallLandTrans;
    [SerializeField] private WholeState_Script WholeState;

    private float GRAVITY = 9.8f;
    public float ElapsedTime; //経過時間
    public float MoveSpeed; //三次元の速さ
    private float XZ_moveSpeed; //XZ面の速さ
    private float X_moveSpeed; //X面の速さ
    private float Z_moveSpeed; //Z面の速さ
    private float Y_MoveSpeed; //Y面の速さ
    private float Th; //XZ面とY軸の角度
    public Vector3 InitPos;

    private bool ProjectileStartFlag;
    private bool HorizontalStartFlag;
    private bool TossProjectionStartFlag;
    private bool ThrowUpStartFlag;
    public bool ProjectileMoveFlag;
    public bool HorizonMoveFlag;
    public bool TossProjectionMoveFlag;
    public bool ThrowUpFlag;
    public bool ServeFlag;

    public int TossLandNum;

    [SerializeField] private GameObject Court;

    public BallStatus ballStatus;

    private bool UrtFrag;

    [SerializeField] private TrailRenderer Trail;
    [SerializeField] private Light Light;

    // Start is called before the first frame update
    void Start()
    {
        Initializer();
        //DebugStart();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug();
        TrailColor();
    }

    private void FixedUpdate()
    {
        Motion();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Court")
        {
            Bounding(Y_MoveSpeed);
        }
    }

    void Debug()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TossProjectionMoveFlag = true;
        }
    }

    void DebugStart() 
    {
        WholeState.BallState_CS = BallBehaviorStates.Serve;
    }

    void TrailColor()
    {
        switch (ballStatus.Power)
        {
            case 0:
                Trail.startColor = Color.gray;
                break;

            case 1:
                Trail.startColor = Color.green;
                break;

            case 2:
                Trail.startColor = Color.yellow;
                break;

            case 3:
                Trail.startColor = Color.magenta;
                break;

            case 4:
                Trail.startColor = Color.white;
                break;
        }
    }

    void Initializer()
    {
        ProjectileStartFlag = false;
        HorizontalStartFlag = false;
        TossProjectionStartFlag = false;
        ThrowUpStartFlag = false;
        ProjectileMoveFlag = false;
        HorizonMoveFlag = false;
        TossProjectionMoveFlag = false;
        //ThrowUpFlag = true;
        UrtFrag = false;

        ballStatus.Impact = 1;
        ballStatus.accuracy = BallStatus.Accuracy.Good;
        ballStatus.Power = 0;
        InitPos = transform.position;
    }

    void Motion() 
    {
        if (WholeState.PlayerBallMoveFlag)
        {
            switch (WholeState.BallState_CS)
            {
                case BallBehaviorStates.Pass:
                    ProjectileMotion(BallTrans, BallLandTrans.position);
                    break;

                case BallBehaviorStates.Toss:
                    TossProjection(BallTrans, BallLandTrans.position, 5f, 3f);
                    break;

                case BallBehaviorStates.Attack:
                    HorizontalProjection(BallTrans, BallLandTrans.position);
                    break;

                case BallBehaviorStates.Serve:
                    ThrowUp(BallTrans, 4f, 3f);
                    break;
            }
        }
        else if (WholeState.EnemyBallMoveFlag)
        {
            switch (WholeState.BallState_CS)
            {
                case BallBehaviorStates.Pass:
                    if (ServeFlag || WholeState.TouchCount >= 3)
                    {
                        ProjectileMotion(BallTrans, EnemyBallLandPos(PlayerCenterTrans.position, EnemyEndCornerRightTrans, EnemyNetRightTrans, -3f, 3f));
                    }
                    else
                    {
                        ProjectileMotion(BallTrans, EnemyBallLandPos(EnemyCenterTrans.position, EnemyEndCornerRightTrans, EnemyNetRightTrans, -3f, 3f));
                    }
                    break;

                case BallBehaviorStates.Toss:
                    TossProjection(BallTrans, EnemyTossLandTrans[TossLandNum].position, 5f, 3f);
                    break;

                case BallBehaviorStates.Attack:
                    HorizontalProjection(BallTrans, EnemyBallLandPos(PlayerCenterTrans.position, EnemyEndCornerRightTrans, EnemyNetRightTrans, -3f, 3f));
                    break;

                case BallBehaviorStates.Serve:
                    ThrowUp(BallTrans, 7f, 2f);
                    break;
            }
        }
    }

    public void ProjectileMotion(Transform StartTras, Vector3 TargetPos)　//射法投射
    {

        if (ProjectileMoveFlag) //必要な変数の決定
        {
            switch (ballStatus.accuracy)
            {
                case BallStatus.Accuracy.Good:
                    ballStatus.Power++;
                    break;

                case BallStatus.Accuracy.Bad:

                    break;
            }


            if (WholeState.AttackSide_CS == AttackSideState.Enemy)
            {
                PCState_PC1.BallLandPos = TargetPos;
                PCState_PC2.BallLandPos = TargetPos;
            }

            if(WholeState.TouchCount < 3 && WholeState.AttackSide_CS == AttackSideState.Enemy || WholeState.AttackSide_CS == AttackSideState.Player)
            {
                EnemyState_E1.BallLandPos = TargetPos;
                EnemyState_E2.BallLandPos = TargetPos;
                EnemyState_E1.TossLandPos = EnemyTossLandTrans[TossLandNum].position;
                EnemyState_E2.TossLandPos = EnemyTossLandTrans[TossLandNum].position;
            }
            else if(WholeState.TouchCount >= 3 && WholeState.AttackSide_CS == AttackSideState.Enemy)
            {
                EnemyState_E1.BallLandPos = EnemyState_E1.BlockTrans.position;
                EnemyState_E2.BallLandPos = EnemyState_E2.BlockTrans.position;

            }

            ProjectileStartFlag = false;
            ElapsedTime = 0f;

            float s;
            float RequiredTime = 0f;

            Vector3 selfPos = new Vector3(StartTras.position.x, 0f, StartTras.position.z);

            TargetPos = new Vector3(TargetPos.x, 0f, TargetPos.z);

            float distance = Vector3.Distance(selfPos, TargetPos);

            float X_distance = TargetPos.x - StartTras.position.x;
            float Z_distance = TargetPos.z - StartTras.position.z;
            float Y_distanse = StartTras.position.y - TargetPos.y;

            float phi = Mathf.Atan(Mathf.Abs(Z_distance / X_distance));

            if (distance > 0 && distance < 8)
            {
                RequiredTime = 2f;
            }
            else if (distance > 8)
            {
                float overdistance = distance - 8f;
                RequiredTime = 2f + (0.45f / 8) * overdistance;
            }

            if(WholeState.ActionStates_CS == ActionStates.Serve)
            {
                if(ballStatus.accuracy == BallStatus.Accuracy.Good)
                {
                    if (distance > 0 && distance < 8)
                    {
                        RequiredTime = 1.5f;
                    }
                    else if (distance > 8)
                    {
                        float overdistance = distance - 8f;
                        RequiredTime = 1.5f + (0.5f / 8) * overdistance;
                    }

                }
                else
                {
                    if (distance > 0 && distance < 8)
                    {
                        RequiredTime = 2f;
                    }
                    else if (distance > 8)
                    {
                        float overdistance = distance - 8f;
                        RequiredTime = 2f + (0.45f / 8) * overdistance;
                    }

                }
            }

            MoveSpeed = Mathf.Sqrt((distance / RequiredTime) * (distance / RequiredTime) + (GRAVITY * RequiredTime / 2) * (GRAVITY * RequiredTime / 2));
            Th = Mathf.Atan(GRAVITY * RequiredTime * RequiredTime / (2 * distance));
            s = Y_distanse / Mathf.Tan(Th);



            //XZ_moveSpeed = distance / (RequiredTime);

            XZ_moveSpeed = distance * distance / ((distance + s) * RequiredTime);

            if (X_distance > 0)
            {
                X_moveSpeed = XZ_moveSpeed * Mathf.Cos(phi);
            }
            else
            {
                X_moveSpeed = -XZ_moveSpeed * Mathf.Cos(phi);
            }
            if (Z_distance > 0)
            {
                Z_moveSpeed = XZ_moveSpeed * Mathf.Sin(phi);
            }
            else
            {
                Z_moveSpeed = -XZ_moveSpeed * Mathf.Sin(phi);
            }


            InitPos = StartTras.position;

            ProjectileStartFlag = true;
            ProjectileMoveFlag = false;
        }

        if (ProjectileStartFlag)
        {
            ElapsedTime += Time.deltaTime;

            float X_Pos = X_moveSpeed * ElapsedTime + InitPos.x;
            float Y_Pos = -(GRAVITY * ElapsedTime * ElapsedTime / 2.0f) + (MoveSpeed * ElapsedTime * Mathf.Sin(Th)) + InitPos.y;
            float Z_Pos = Z_moveSpeed * ElapsedTime + InitPos.z;
            Vector3 XYZ_Vector = new Vector3(X_Pos, Y_Pos, Z_Pos);

            StartTras.position = XYZ_Vector;
        }

    }

    public void TossProjection(Transform StartTras, Vector3 TargetPos, float MaxHigh, float LandHigh)
    {
        if (TossProjectionMoveFlag) //必要な変数の決定
        {
            switch (ballStatus.accuracy)
            {
                case BallStatus.Accuracy.Good:
                    ballStatus.Power++;
                    break;

                case BallStatus.Accuracy.Bad:

                    break;
            }


            if (WholeState.AttackSide_CS == AttackSideState.Enemy)
            {
                PCState_PC1.BallLandPos = TargetPos;
                PCState_PC2.BallLandPos = TargetPos;
            }

            if (WholeState.TouchCount < 3 && WholeState.AttackSide_CS == AttackSideState.Enemy || WholeState.AttackSide_CS == AttackSideState.Player)
            {
                EnemyState_E1.BallLandPos = TargetPos;
                EnemyState_E2.BallLandPos = TargetPos;
            }

            TossProjectionStartFlag = false;
            ElapsedTime = 0f;

            switch (WholeState.AttackSide_CS)
            {
                case AttackSideState.Player:
                    PCState_PC1.JampHigh = LandHigh;
                    PCState_PC2.JampHigh = LandHigh;
                    break;

                case AttackSideState.Enemy:

                    EnemyState_E1.JampHigh = LandHigh;
                    EnemyState_E2.JampHigh = LandHigh;
                    break;
            }

            //float s;
            float RequiredTime = 0f;

            Vector3 selfPos = new Vector3(StartTras.position.x, 0f, StartTras.position.z);

            TargetPos = new Vector3(TargetPos.x, 0f, TargetPos.z);

            float distance = Vector3.Distance(selfPos, TargetPos);

            float X_distance = TargetPos.x - StartTras.position.x;
            float Z_distance = TargetPos.z - StartTras.position.z;
            //float Y_distanse = StartTras.position.y - TargetTras.position.y;

            float phi = Mathf.Atan(Mathf.Abs(Z_distance / X_distance));

            LandHigh = - LandHigh + StartTras.transform.position.y;
            RequiredTime = (Mathf.Sqrt(2f * GRAVITY * (MaxHigh - LandHigh)) + Mathf.Sqrt(2f * GRAVITY * MaxHigh)) / GRAVITY;
            MoveSpeed = Mathf.Sqrt((distance / RequiredTime) * (distance / RequiredTime) + 2f * GRAVITY * (MaxHigh - LandHigh));
            Th = Mathf.Atan(RequiredTime * Mathf.Sqrt(2f * GRAVITY * (MaxHigh - LandHigh)) / distance);
            //s = Y_distanse / Mathf.Tan(θ);


            switch (WholeState.AttackSide_CS)
            {
                case AttackSideState.Player:
                    PCState_PC1.AttackMoveTime = RequiredTime;
                    PCState_PC2.AttackMoveTime = RequiredTime;

                    break;

                case AttackSideState.Enemy:
                    EnemyState_E1.AttackMoveTime = RequiredTime;
                    EnemyState_E2.AttackMoveTime = RequiredTime;

                    break;
            }


            XZ_moveSpeed = distance / (RequiredTime);

            //XZ_moveSpeed = distance * distance / ((distance + s) * RequiredTime);

            if (X_distance > 0)
            {
                X_moveSpeed = XZ_moveSpeed * Mathf.Cos(phi);
            }
            else
            {
                X_moveSpeed = -XZ_moveSpeed * Mathf.Cos(phi);
            }
            if (Z_distance > 0)
            {
                Z_moveSpeed = XZ_moveSpeed * Mathf.Sin(phi);
            }
            else
            {
                Z_moveSpeed = -XZ_moveSpeed * Mathf.Sin(phi);
            }


            InitPos = StartTras.position;

            switch (WholeState.AttackSide_CS)
            {
                case AttackSideState.Player:
                    PCState_PC1.AttackStartMoveFlag = true;
                    PCState_PC2.AttackStartMoveFlag = true;

                    break;

                case AttackSideState.Enemy:
                    EnemyState_E1.AttackStartMoveFlag = true;
                    EnemyState_E2.AttackStartMoveFlag = true;

                    break;
            }

            TossProjectionStartFlag = true;
            TossProjectionMoveFlag = false;
        }

        if (TossProjectionStartFlag)
        {
            ElapsedTime += Time.deltaTime;

            float X_Pos = X_moveSpeed * ElapsedTime + InitPos.x;
            float Y_Pos = -(GRAVITY * ElapsedTime * ElapsedTime / 2.0f) + (MoveSpeed * ElapsedTime * Mathf.Sin(Th)) + InitPos.y;
            float Z_Pos = Z_moveSpeed * ElapsedTime + InitPos.z;
            Vector3 XYZ_Vector = new Vector3(X_Pos, Y_Pos, Z_Pos);

            StartTras.position = XYZ_Vector;
        }
    }


    void HorizontalProjection(Transform StartTras, Vector3 TargetPos) //水平投射
    {

        if (HorizonMoveFlag) //必要な変数の決定
        {
            switch (ballStatus.accuracy)
            {
                case BallStatus.Accuracy.Good:
                    ballStatus.Power++;
                    break;

                case BallStatus.Accuracy.Bad:

                    break;
            }
            switch (ballStatus.Power)
            {
                case 1:
                    ballStatus.Impact++;
                    break;

                case 2:
                    ballStatus.Impact += 5;
                    break;

                case 3:
                    ballStatus.Impact += 7;
                    break;
            }

            if (UrtFrag)
            {
                ballStatus.Power = 4;
                ballStatus.Impact = 10;
                StartCoroutine(HitStop(0.2f));
                WholeState.PlayerUrtGauge = 0;
                UrtFrag = false;
            }

            switch (WholeState.AttackSide_CS)
            {
                case AttackSideState.Player:
                    if (WholeState.PlayerUrtGauge >= WholeState.GaugeMaxValue)
                    {
                        UrtFrag = true;
                    }
                    break;

                case AttackSideState.Enemy:
                    break;
            }



            if (WholeState.AttackSide_CS == AttackSideState.Enemy)
            {
                PCState_PC1.BallLandPos = TargetPos;
                PCState_PC2.BallLandPos = TargetPos;
            }

            if(WholeState.AttackSide_CS == AttackSideState.Player)
            {
                EnemyState_E1.BallLandPos = TargetPos;
                EnemyState_E2.BallLandPos = TargetPos;
            }

            HorizontalStartFlag = false;
            ElapsedTime = 0f;
            float RequiredTime = 0f;

            Vector3 selfPos = new Vector3(StartTras.position.x, 0f, StartTras.position.z);

            TargetPos = new Vector3(TargetPos.x, 0f, TargetPos.z);

            float distance = Vector3.Distance(selfPos, TargetPos);

            float X_distance = TargetPos.x - StartTras.position.x;
            float Z_distance = TargetPos.z - StartTras.position.z;
            float Y_distanse = StartTras.position.y - TargetPos.y;

            float phi = Mathf.Atan(Mathf.Abs(Z_distance / X_distance));

            float NormalTime = 1.2f;
            switch (ballStatus.Power)
            {
                case 0:
                    NormalTime = 1.5f;
                    break;
                case 1:
                    NormalTime = 1.2f;
                    break;
                case 2:
                    NormalTime = 0.8f;
                    break;
                case 3:
                    NormalTime = 0.4f;
                    break;
                case 4:
                    NormalTime = 0.3f;
                    break;
            }

            if (distance > 0  &&  distance < 2)
            {
                float LessDistance = 2f - distance;
                RequiredTime = NormalTime - 0.1f * LessDistance;
            }
            else if (distance > 2 && distance < 8)
            {
                RequiredTime = NormalTime;
            }
            else if (distance > 8 && distance < 16)
            {
                float overDistance = distance - 8f;
                RequiredTime = NormalTime + (overDistance / 16f);
            }
            else if(distance > 16)
            {
                RequiredTime = NormalTime * 2;
            }
            else
            {
                RequiredTime = 0f;
            }

            Th = Mathf.Atan(Y_distanse / distance);
            MoveSpeed = (Y_distanse - (GRAVITY * RequiredTime * RequiredTime / 2f)) / (Mathf.Sin(Th) * RequiredTime);

            XZ_moveSpeed = distance / RequiredTime;

            if (X_distance > 0)
            {
                X_moveSpeed = XZ_moveSpeed * Mathf.Cos(phi);
            }
            else
            {
                X_moveSpeed = -XZ_moveSpeed * Mathf.Cos(phi);
            }
            if (Z_distance > 0)
            {
                Z_moveSpeed = XZ_moveSpeed * Mathf.Sin(phi);
            }
            else
            {
                Z_moveSpeed = -XZ_moveSpeed * Mathf.Sin(phi);
            }


            InitPos = StartTras.position;

            HorizontalStartFlag = true;
            HorizonMoveFlag = false;
        }

        if (HorizontalStartFlag)
        {
            ElapsedTime += Time.deltaTime;

            float X_Pos = X_moveSpeed * ElapsedTime + InitPos.x;
            float Y_Pos = -(GRAVITY * ElapsedTime * ElapsedTime / 2.0f) - (MoveSpeed * ElapsedTime * Mathf.Sin(Th)) + InitPos.y;
            float Z_Pos = Z_moveSpeed * ElapsedTime + InitPos.z;
            Vector3 XYZ_Vector = new Vector3(X_Pos, Y_Pos, Z_Pos);

            StartTras.position = XYZ_Vector;
        }

    }

    void ThrowUp(Transform StartTras, float MaxHigh, float RequiredTime) //投げ上げ
    {
        if (ThrowUpFlag) //必要な変数の決定
        {
            ThrowUpStartFlag = false;
            ElapsedTime = 0f;
            InitPos = StartTras.position;

            Y_MoveSpeed = (2 * (MaxHigh - InitPos.y) / RequiredTime) + (GRAVITY * RequiredTime / 4f) ;

            InitPos = StartTras.position;

            ThrowUpStartFlag = true;
            ThrowUpFlag = false;
        }

        if (ThrowUpStartFlag)
        {
            ElapsedTime += Time.deltaTime;

            float X_Pos = InitPos.x;
            float Y_Pos = -(GRAVITY * ElapsedTime * ElapsedTime / 2.0f) + (Y_MoveSpeed * ElapsedTime) + InitPos.y;
            float Z_Pos = InitPos.z;
            Vector3 XYZ_Vector = new Vector3(X_Pos, Y_Pos, Z_Pos);

            StartTras.position = XYZ_Vector;
        }
    }

    Vector3 EnemyBallLandPos(Vector3 TargetPos, Transform EndCornerRightTrans, Transform NetRightTrans, float MinRange, float MaxRange)
    {
        float TargetPos_X = 0f;
        float TargetPos_Y = 0f;
        float TargetPos_Z = 0f;

        Vector3 CenterToNetRight = TargetPos - NetRightTrans.position;
        CenterToNetRight.y = 0f;
        Vector3 CenterToEndRight = TargetPos - EndCornerRightTrans.position;
        CenterToEndRight.y = 0f; 
        Vector3 SideRightVec = NetRightTrans.position - EndCornerRightTrans.position;
        SideRightVec.y = 0f;

        float Th = Vector3.Angle(SideRightVec, CenterToNetRight) * Mathf.Deg2Rad;
        float X_rand = Random.Range(MinRange, MaxRange);
        float Z_rand = Random.Range(MinRange, MaxRange);
        if (Mathf.Abs(SideRightVec.x) >= Mathf.Abs(SideRightVec.z))
        {
            TargetPos_X = TargetPos.x + (X_rand * Mathf.Sin(Th)) + (Z_rand * Mathf.Sin(Th));
            TargetPos_Y = TargetPos.y;
            TargetPos_Z = TargetPos.z + (X_rand * Mathf.Cos(Th)) - (Z_rand * Mathf.Cos(Th));
        }
        else
        {
            TargetPos_X = TargetPos.x + (X_rand * Mathf.Cos(Th)) - (Z_rand * Mathf.Cos(Th));
            TargetPos_Y = TargetPos.y;
            TargetPos_Z = TargetPos.z + (X_rand * Mathf.Sin(Th)) + (Z_rand * Mathf.Sin(Th));
        }
        Vector3 ReturnPos = new Vector3(TargetPos_X, TargetPos_Y, TargetPos_Z);
        return ReturnPos;
    }
    void Bounding(float Speed)
    {
        Speed = -Speed;
    }

    IEnumerator HitStop(float stoptime)
    {
        Color PreColor = Light.color;
        Time.timeScale = 0f;
        Light.color = new Color(0, 0, 0);
        yield return new WaitForSecondsRealtime(stoptime);
        Time.timeScale = 1f;
        Light.color = PreColor;
    }
}

public struct BallStatus 
{
    public int Impact; // 最小1、最大10

    public int Power;
    public enum Accuracy
    {
        Bad,
        Good
    }
    public Accuracy accuracy;
}

