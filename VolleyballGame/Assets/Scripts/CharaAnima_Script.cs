using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaAnima_Script : MonoBehaviour
{
    [Header("Right: 0, Left :1")]
    [SerializeField] private Transform[] HandTrans;
    [Header("Right: 0, Left :1")]
    [SerializeField] private Transform[] LegTrans;
    [SerializeField] private PCState_Script SelfPCState;
    [SerializeField] private Transform BallTrans;
    [SerializeField] private Transform SelfTrans;

    private float HandElapsedTime;
    private float LegElapsedTime;

    private Vector3[] HandInitPos = new Vector3[2]; //Right: 0, Left :1
    private Vector3[] LegInitPos = new Vector3[2]; //Right: 0, Left :1
    private Vector3 HandTargetPos;
    [SerializeField] private float HandMoveSpeed = 3f;
    [SerializeField] private float HandHoldTime = 0.15f;
    private float HandHoldElapsedTime;

    public bool HandAnimaStartFlag;
    private bool HandAnimaReturnFlag;
    private bool HandAnimaMoveFlag;


    [SerializeField] private MeshRenderer WaveMesh;
    // Start is called before the first frame update
    void Start()
    {
        Initializer();
    }

    // Update is called once per frame
    void Update()
    {
        HandAnimation(HandTrans[0], HandTrans[1], BallTrans.InverseTransformPoint(SelfTrans.position));
    }

    void Initializer()
    {
        HandInitPos[0] = HandTrans[0].localPosition;
        HandInitPos[1] = HandTrans[1].localPosition;
        LegInitPos[0] = LegTrans[0].localPosition;
        LegInitPos[1] = LegTrans[1].localPosition;
        HandElapsedTime = 0f;
        HandHoldElapsedTime = 0f;

        HandAnimaStartFlag = false;
        HandAnimaReturnFlag = false;
        HandAnimaMoveFlag = false;
}

void HandAnimation(Transform RightTrans, Transform LeftTrans, Vector3 TargetPos)
    {

        if (HandAnimaStartFlag)
        {
            HandElapsedTime = 0f;
            HandHoldElapsedTime = 0f;
            HandTargetPos = TargetPos;
            HandAnimaMoveFlag = true;
            HandAnimaReturnFlag = false;
            HandAnimaStartFlag = false;
        }

        if (!HandAnimaMoveFlag)
        {
            return;
        }

        if (!HandAnimaReturnFlag)
        {
            HandElapsedTime += Time.deltaTime * HandMoveSpeed;
            float t = Mathf.Clamp01(HandElapsedTime);
            RightTrans.localPosition = Vector3.Lerp(HandInitPos[0], HandTargetPos, t);
            LeftTrans.localPosition = Vector3.Lerp(HandInitPos[1], HandTargetPos, t);

            if (t >= 1f)
            {
                HandHoldElapsedTime += Time.deltaTime;
                if (HandHoldElapsedTime >= HandHoldTime)
                {
                    HandAnimaReturnFlag = true;
                }
            }
        }
        else
        {
            HandElapsedTime -= Time.deltaTime * HandMoveSpeed;
            float t = Mathf.Clamp01(HandElapsedTime);
            RightTrans.localPosition = Vector3.Lerp(HandInitPos[0], HandTargetPos, t);
            LeftTrans.localPosition = Vector3.Lerp(HandInitPos[1], HandTargetPos, t);

            if (t <= 0f)
            {
                RightTrans.localPosition = HandInitPos[0];
                LeftTrans.localPosition = HandInitPos[1];
                HandAnimaMoveFlag = false;
                HandAnimaReturnFlag = false;
                HandHoldElapsedTime = 0f;
            }
        }
    }

    void LegAnimation(Transform RightTrans, Transform LeftTrans, Transform TargetTrans)
    {
        LegElapsedTime += Time.deltaTime;
    }

    void WaveAnimation(MeshRenderer WaveMesh)
    {
        
    }
}
