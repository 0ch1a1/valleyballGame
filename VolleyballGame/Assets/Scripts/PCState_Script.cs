using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PCState_Script : MonoBehaviour
{
    public bool MoveChara; //true = 移動する, false  = 待機する
    [SerializeField] private string Name;
    
    [SerializeField] private GameObject SelfObj;
    [SerializeField] private GameObject OtherObj;
    [SerializeField] private GameObject BallObj;
    [SerializeField] private WholeState_Script WholeState;
    [SerializeField] private BallMove_Script BallMove_Scp;
    [SerializeField] private PCState_Script OtherPCState;
    [SerializeField] private Marker_Script Marker_Script;
    public Transform BlockTrans;
    [Header("0:Right, 1:Left")]
    [SerializeField] private Transform[] NetPoleTrans;
    [Header("0:Right, 1:Left")]
    [SerializeField] private Transform[] CourtCornerTrans;
    public Vector3 BallLandPos;
    private Vector3 CharaInitPos;
    private Vector3 InitPos;

    private float XZ_moveSpeed;
    private float X_moveSpeed;
    private float Y_moveSpeed;
    private float Z_moveSpeed;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float rotateSpeed;
    public float JampHigh;
    private float GRAVITY = 9.8f;

    private float JampMaxHigh;

    public float AttackMoveTime;

    private float ElapsedTime; //経過時間
    private float JampElapsedTime;
    private float EndElapsedTime; //経過時間
    public bool TouchFlag;

    public bool AttackStartMoveFlag;
    private bool AttackStartFlag;
    public bool AttackEndFlag;
    private bool AttackJampFlag;
    public bool ServeStartFlag;


    private bool BallLandPosUpdateFlag;

    [SerializeField] private CharaAnima_Script CharaAnima;



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == BallObj)
        {
            WholeState.EnemyBallMoveFlag = false;
            WholeState.PlayerBallMoveFlag =  true;
            BallMove_Scp.ServeFlag = false;
            WholeState.EnterFlag = false;
            TouchFlag = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == BallObj)
        {
            TouchFlag = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Initializer();
        //StartDebug();
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateDebug();
        BallMovement();
        Movement();
        //StateDebug();

    }


    void Initializer()
    {
        TouchFlag = false;
        ServeStartFlag = true;
        CharaInitPos = SelfObj.transform.position;
    }

    void Pass()
    {
        float distance = Vector3.Distance(BallObj.transform.position, SelfObj.transform.position);

        if (Input.GetMouseButtonUp(0) && TouchFlag && distance < 2.5f)
        {
            if (MoveChara)
            {
                if(WholeState.TouchCount == 0)
                {
                    BallMove_Scp.ballStatus.Power = 0;
                    BallMove_Scp.ballStatus.Impact = 1;
                }

                WholeState.TouchCount++;
                if (distance > 0.8 && distance < 2.3f)
                {
                    BallMove_Scp.ballStatus.accuracy = BallStatus.Accuracy.Good;
                    WholeState.PlayerUrtGauge++;
                }
                else
                {
                    BallMove_Scp.ballStatus.accuracy = BallStatus.Accuracy.Bad;
                }

                WholeState.BallState_CS = BallBehaviorStates.Pass;
                BallMove_Scp.TossProjectionMoveFlag = false;
                BallMove_Scp.HorizonMoveFlag = false;
                BallMove_Scp.ThrowUpFlag = false;
                BallMove_Scp.ProjectileMoveFlag = true;
                //CharaAnima.HandAnimaStartFlag = true;
                if (Marker_Script.MarkerInObj == WholeState.PlayerCourt || Marker_Script.MarkerInObj != WholeState.EnemyCourt && WholeState.AttackSide_CS == AttackSideState.Player)
                {
                    BallLandPos = BallMove_Scp.BallLandTrans.position;
                    OtherPCState.BallLandPos = BallMove_Scp.BallLandTrans.position;
                }
                else
                {
                    BallLandPos = BlockTrans.position;
                    OtherPCState.BallLandPos = OtherPCState.BlockTrans.position;
                }
                MovementADM();
            }
            TouchFlag = false;
        }

    }

    void Toss()
    {
        float distance = Vector3.Distance(BallObj.transform.position, SelfObj.transform.position);

        if (Input.GetMouseButtonUp(0) && TouchFlag && distance < 2.5f)
        {
            if (MoveChara)
            {
                WholeState.TouchCount++;
                if (distance > 1f && distance < 2f)
                {
                    BallMove_Scp.ballStatus.accuracy = BallStatus.Accuracy.Good;
                    WholeState.PlayerUrtGauge++;
                }
                else
                {
                    BallMove_Scp.ballStatus.accuracy = BallStatus.Accuracy.Bad;
                }

                WholeState.BallState_CS = BallBehaviorStates.Toss;
                BallMove_Scp.HorizonMoveFlag = false;
                BallMove_Scp.ProjectileMoveFlag = false;
                BallMove_Scp.ThrowUpFlag = false;
                BallMove_Scp.TossProjectionMoveFlag = true;
                if(Marker_Script.MarkerInObj == WholeState.PlayerCourt || Marker_Script.MarkerInObj != WholeState.EnemyCourt && WholeState.AttackSide_CS == AttackSideState.Player)
                {
                    BallLandPos = BallMove_Scp.BallLandTrans.position;
                    OtherPCState.BallLandPos = BallMove_Scp.BallLandTrans.position;
                }
                else
                {
                    BallLandPos = BlockTrans.position;
                    OtherPCState.BallLandPos = OtherPCState.BlockTrans.position;
                }
                MovementADM();
                BallLandPosUpdateFlag = false;

                WholeState.ActionStates_CS = ActionStates.Attack;
            }

            TouchFlag = false;
        }
    }

    void Attack()
    {
        float distance = Vector3.Distance(BallObj.transform.position, SelfObj.transform.position);

        if (Input.GetMouseButtonUp(0) && TouchFlag && distance < 2.5f)
        {
            if (MoveChara)
            {
                WholeState.TouchCount++;
                if (distance > 0.9f && distance < 2.1f)
                {
                    BallMove_Scp.ballStatus.accuracy = BallStatus.Accuracy.Good;
                    WholeState.PlayerUrtGauge += 3;
                }
                else
                {
                    BallMove_Scp.ballStatus.accuracy = BallStatus.Accuracy.Bad;
                }

                WholeState.BallState_CS = BallBehaviorStates.Attack;
                BallMove_Scp.TossProjectionMoveFlag = false;
                BallMove_Scp.ProjectileMoveFlag = false;
                BallMove_Scp.ThrowUpFlag = false;
                BallMove_Scp.HorizonMoveFlag = true;
            }


            TouchFlag = false;
        }

    }

    void Serve()
    {
        float distance = Vector3.Distance(BallObj.transform.position, SelfObj.transform.position);

        if (Input.GetMouseButtonDown(0) && TouchFlag && distance <2.5f)
        {
            if (ServeStartFlag)
            {
                WholeState.BallState_CS = BallBehaviorStates.Serve;
                BallMove_Scp.HorizonMoveFlag = false;
                BallMove_Scp.ProjectileMoveFlag = false;
                BallMove_Scp.TossProjectionMoveFlag = false;
                BallMove_Scp.ThrowUpFlag = true;
                ServeStartFlag = false;
            }
            TouchFlag = false;
        }

        if(Input.GetMouseButtonUp(0) && TouchFlag && distance < 2.5f) 
        { 
            if (!ServeStartFlag)
            {
                if (distance > 1f && distance < 2f)
                {
                    BallMove_Scp.ballStatus.accuracy = BallStatus.Accuracy.Good;
                    BallMove_Scp.ballStatus.Impact = 3;
                    WholeState.PlayerUrtGauge++;
                }
                else
                {
                    BallMove_Scp.ballStatus.accuracy = BallStatus.Accuracy.Bad;
                    BallMove_Scp.ballStatus.Impact = 1;
                }
                WholeState.BallState_CS = BallBehaviorStates.Pass;
                BallMove_Scp.TossProjectionMoveFlag = false;
                BallMove_Scp.HorizonMoveFlag = false;
                BallMove_Scp.ThrowUpFlag = false;
                BallMove_Scp.ProjectileMoveFlag = true;
                TouchFlag = false;
            }

        }
    }

    void BallMovement()
    {
        if(WholeState.AttackSide_CS == AttackSideState.Player)
        {
            if (WholeState.TouchCount == 1 && TwoDistanceCheck(NetPoleTrans, OtherObj.transform,2.5f) && TwoDistanceCheck(NetPoleTrans, BallMove_Scp.BallLandTrans, 2.5f) && WholeState.ActionStates_CS != ActionStates.Attack && WholeState.ActionStates_CS != ActionStates.Serve && WholeState.BallState_CS == BallBehaviorStates.Pass)
            {
                WholeState.ActionStates_CS = ActionStates.Toss;
                BallLandPosUpdateFlag = true;
            }

            else if (!TwoDistanceCheck(NetPoleTrans, BallMove_Scp.BallLandTrans, 2.5f) && WholeState.ActionStates_CS != ActionStates.Attack && WholeState.ActionStates_CS != ActionStates.Serve)
            {
                WholeState.ActionStates_CS = ActionStates.Receive;
                BallLandPosUpdateFlag = false;
            }

            switch (WholeState.ActionStates_CS)
            {
                case ActionStates.Receive:
                    Pass();
                    break;

                case ActionStates.Toss:
                    Toss();
                    break;

                case ActionStates.Attack:
                    Attack();
                    break;

                case ActionStates.Serve:
                    Serve();
                    break;
            }

        }
    }


    void Movement()
    {
        switch (WholeState.AttackSide_CS)
        {
            case AttackSideState.Player:
                if (MoveChara)
                {
                    switch (WholeState.ActionStates_CS)
                    {
                        case ActionStates.Receive:
                            ChaseTarget(BallLandPos, SelfObj.transform, MoveSpeed);
                            RotateToTarget(BallObj.transform.position, SelfObj.transform, rotateSpeed);
                            break;

                        case ActionStates.Toss:
                            ChaseTarget(BallLandPos, SelfObj.transform, MoveSpeed);
                            RotateToTarget(BallObj.transform.position, SelfObj.transform, rotateSpeed);
                            break;


                        case ActionStates.Attack:
                            if (AttackEndFlag)
                            {
                                Landing(SelfObj.transform);
                            }
                            else
                            {
                                TossChaseTarget(BallLandPos, SelfObj.transform, AttackMoveTime, JampHigh);
                                RotateToTarget(BallLandPos, SelfObj.transform, rotateSpeed);
                            }
                            break;

                    }
                    break;

                }
                else
                {
                    switch (WholeState.ActionStates_CS)
                    {
                        case ActionStates.Receive:
                            BackAway(BallLandPos, SelfObj.transform, 5f);
                            RotateToTarget(BallObj.transform.position, SelfObj.transform, rotateSpeed);
                            break;

                        case ActionStates.Toss:
                            if (BallLandPosUpdateFlag)
                            {
                                BallLandPos = BallMove_Scp.BallLandTrans.position;
                            }

                            TossWaitMove(BallLandPos, SelfObj.transform, CourtCornerTrans[0], CourtCornerTrans[1]);
                            RotateToTarget(BallObj.transform.position, SelfObj.transform, rotateSpeed);
                            break;

                        case ActionStates.Attack:
                            BackAway(BallLandPos, SelfObj.transform, 5f);
                            RotateToTarget(BallObj.transform.position, SelfObj.transform, rotateSpeed);
                            break;


                    }

                    break;
                }

            case AttackSideState.Enemy:
                if (AttackEndFlag)
                {
                    Landing(SelfObj.transform);
                }
                else 
                {
                    ChaseTarget(BlockTrans.position, SelfObj.transform, MoveSpeed);
                    RotateToTarget(BallObj.transform.position, SelfObj.transform, rotateSpeed);
                }
                break;
        }
    }

    void ChaseTarget(Vector3 TargetPos, Transform SelfTrans, float MoveSpeed)
    {


        // 目的との距離を計算

        float Distance = Vector3.Distance(TargetPos, SelfTrans.position);

        Vector3 MoveTowardPos = new Vector3(TargetPos.x, CharaInitPos.y, TargetPos.z);


        if (Distance > 0.2f)
        {
            // 現在位置から目的へ移動
            SelfTrans.position = Vector3.MoveTowards(SelfTrans.position, MoveTowardPos, MoveSpeed * Time.deltaTime);
        }
    }

    void TossChaseTarget(Vector3 TargetPos, Transform SelfTrans, float MoveTime, float MaxHigh)
    {
        TargetPos = new Vector3(TargetPos.x, SelfTrans.position.y, TargetPos.z);

        float Distance = Vector3.Distance(TargetPos, SelfTrans.position);


        if (AttackStartMoveFlag)
        {
            ElapsedTime = 0f;

            float X_distance = TargetPos.x - SelfTrans.position.x;
            float Z_distance = TargetPos.z - SelfTrans.position.z;

            float phi = Mathf.Atan(Mathf.Abs(Z_distance / X_distance));

            XZ_moveSpeed = Distance / MoveTime;

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

            InitPos = SelfTrans.position;

            AttackStartFlag = true;
            AttackStartMoveFlag = false;
        }

        if (AttackStartFlag && Distance < 2 && Distance > 0.5)
        {
            JampElapsedTime = 0f;
            AttackJampFlag = true;
            AttackStartFlag = false;
        }

        if(AttackJampFlag && Distance < 0.5)
        {
            EndElapsedTime = 0f;
            JampMaxHigh = SelfTrans.position.y;
            AttackEndFlag = true;
            AttackJampFlag = false;
        }


        ElapsedTime += Time.deltaTime;
        JampElapsedTime += Time.deltaTime;
        EndElapsedTime += Time.deltaTime;


        if (AttackStartFlag)
        {
            if (Distance > 2)
            {
                float X_Pos = X_moveSpeed * ElapsedTime + InitPos.x;
                float Y_Pos = InitPos.y;
                float Z_Pos = Z_moveSpeed * ElapsedTime + InitPos.z;
                SelfTrans.position = new Vector3(X_Pos, Y_Pos, Z_Pos);
            }

        }

        if (AttackJampFlag)
        {
            if (Distance < 2 && Distance > 0.5)
            {
                Y_moveSpeed = (MaxHigh * XZ_moveSpeed) / 2f;

                float X_Pos = X_moveSpeed * ElapsedTime + InitPos.x;
                float Y_Pos = Y_moveSpeed * JampElapsedTime + InitPos.y;
                float Z_Pos = Z_moveSpeed * ElapsedTime + InitPos.z;
                SelfTrans.position = new Vector3(X_Pos, Y_Pos, Z_Pos);
            }
        }


    }

    void Landing(Transform SelfTrans)
    {
        if (AttackEndFlag)
        {
            EndElapsedTime += Time.deltaTime;
            ElapsedTime += Time.deltaTime;

            float k = 3f;
            float phi = Mathf.Atan(Z_moveSpeed / X_moveSpeed);
            float X_Pos = 0f;
            float Z_Pos = 0f;

            X_Pos = (X_moveSpeed * ElapsedTime) - (k * EndElapsedTime * Mathf.Cos(phi)) + InitPos.x;

            if (Mathf.Abs((X_moveSpeed * ElapsedTime) - (k * EndElapsedTime * Mathf.Cos(phi))) < 0.2f)
            {
                X_Pos = SelfTrans.position.x;
            }

            float Y_Pos = -(GRAVITY * EndElapsedTime * EndElapsedTime / 2f) + InitPos.y + JampMaxHigh;
            Z_Pos = (Z_moveSpeed * ElapsedTime) - (k * EndElapsedTime * Mathf.Sin(phi)) + InitPos.z;

            if (Mathf.Abs((Z_moveSpeed * ElapsedTime) - (k * EndElapsedTime * Mathf.Sin(phi))) < 0.2f)
            {
                Z_Pos = SelfTrans.position.z;
            }


            SelfTrans.position = new Vector3(X_Pos, Y_Pos, Z_Pos);

            if (SelfTrans.position.y < InitPos.y)
            {
                SelfTrans.position = new Vector3(SelfTrans.position.x, InitPos.y, SelfTrans.position.z);
                AttackEndFlag = false;
            }
        }

    }

    void TossWaitMove(Vector3 TargetPos, Transform SelfTrans, Transform EndCornerRight, Transform EndCornerLeft)
    {
        TargetPos = new Vector3(TargetPos.x, 0f, TargetPos.z);
        Vector3 EndCornerAPos = new Vector3(EndCornerRight.position.x, 0f, EndCornerRight.position.z);
        Vector3 EndCornerBPos = new Vector3(EndCornerLeft.position.x, 0f, EndCornerLeft.position.z);
        Vector3 EndSideVec = EndCornerBPos - EndCornerAPos;
        EndSideVec.y = 0f;
        Vector3 TargetFromCornerVec = TargetPos - EndCornerAPos;
        TargetFromCornerVec.y = 0f;
        float TargetFromCornerDis = Mathf.Sqrt((TargetFromCornerVec.x * TargetFromCornerVec.x) + (TargetFromCornerVec.z * TargetFromCornerVec.z));
        float Angle = Vector3.SignedAngle(TargetFromCornerVec, EndSideVec, SelfTrans.up) * Mathf.Deg2Rad;
        float θ = Mathf.Atan(Mathf.Abs(EndSideVec.x / EndSideVec.z));
        float MoveTowardDis = TargetFromCornerDis * Mathf.Cos(Angle);
        Vector3 MoveTowardPos = new Vector3(MoveTowardDis * Mathf.Sin(θ) + EndCornerAPos.x, 0f, MoveTowardDis * Mathf.Cos(θ) + EndCornerAPos.z);
        MoveTowardPos.y = CharaInitPos.y;
        
        SelfTrans.position = Vector3.MoveTowards(SelfTrans.position, MoveTowardPos, MoveSpeed * Time.deltaTime);
    }


    void RotateToTarget(Vector3 TargetPos, Transform SelfTras, float rotateSpeed)
    {
        Quaternion targetRot = Quaternion.identity;
        float angle = 0f;
        Vector3 dir = (TargetPos - SelfTras.position);
        dir.y = 0;

        if (dir.magnitude != 0f)
        {
            targetRot = Quaternion.LookRotation(dir);
            angle = Quaternion.Angle(SelfTras.rotation, targetRot);        // 現在の方向と目標の方向がなす角度
        }

        if (angle > 0f)
        {
            // 現在の回転から目的の回転へ、滑らかに補間
            SelfTras.rotation = Quaternion.Slerp(SelfTras.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }


    }

    void BackAway(Vector3 TargetPos, Transform SelfTrans, float NeedDis)
    {
        if (Marker_Script.MarkerInObj == WholeState.PlayerCourt)
        {
            Vector3 SelfPos = SelfTrans.position;
            SelfPos.y = 0f;

            Vector3 TargetVec = new Vector3(TargetPos.x, 0f, TargetPos.z) - SelfPos;
            TargetVec = Vector3.Normalize(TargetVec);
            float Distance = Vector3.Distance(new Vector3(TargetPos.x, 0f, TargetPos.z), SelfPos);

            if (Distance < NeedDis)
            {
                SelfTrans.position = Vector3.MoveTowards(SelfTrans.position, SelfTrans.position - 2 * TargetVec, MoveSpeed * Time.deltaTime);
            }
        }

    }



    void MovementADM()　//キャラクターの動きに関するflagを制御する関数
    {


        if (WholeState.AttackSide_CS == AttackSideState.Player)
        {

            if (MoveChara)
            {
                MoveChara = false;
            }
            else if (!MoveChara)
            {
                MoveChara = true;
            }
            else
            {
                MoveChara = false;
            }

            if (OtherPCState.MoveChara)
            {
                OtherPCState.MoveChara = false;
            }
            else if (!OtherPCState.MoveChara)
            {
                OtherPCState.MoveChara = true;
            }
            else
            {
                OtherPCState.MoveChara = false;
            }


        }
    }

    bool TwoDistanceCheck(Transform[] TargetTrans, Transform SelfTrans, float NeedDistance)
    {
        TargetTrans[0].position = new Vector3(TargetTrans[0].position.x, 0f, TargetTrans[0].position.z);
        TargetTrans[1].position = new Vector3(TargetTrans[1].position.x, 0f, TargetTrans[1].position.z);
        Vector3 TargetVec = TargetTrans[1].position - TargetTrans[0].position;
        TargetVec.y = 0f;
        Vector3 TargetToCharaVec = SelfTrans.position - TargetTrans[0].position;
        TargetToCharaVec.y = 0f;

        float TargetToCharaDis = Mathf.Sqrt((TargetToCharaVec.x * TargetToCharaVec.x) + (TargetToCharaVec.z * TargetToCharaVec.z));
        float Angle = Vector3.SignedAngle(TargetToCharaVec, TargetVec, SelfTrans.up) * Mathf.Deg2Rad;


        float distance = TargetToCharaDis * Mathf.Sin(Angle);

        if (distance < NeedDistance)
        {
            return true;
        }
        else if(distance > NeedDistance)
        {
            return false;
        }
        else
        {
            return false;   
        }
    }

    void StartDebug()
    {
        WholeState.ActionStates_CS = ActionStates.Receive;
        BallLandPos = BallObj.transform.position;
    }

    void UpdateDebug()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BallLandPos = BallMove_Scp.BallLandTrans.position;
            OtherPCState.BallLandPos = BallMove_Scp.BallLandTrans.position;
        }

        TossChaseTarget(BallLandPos, SelfObj.transform, AttackMoveTime, JampHigh);   
    }
}
