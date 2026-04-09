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

        HandAnimaStartFlag = false;
        HandAnimaReturnFlag = false;
        HandAnimaMoveFlag = false;
}

void HandAnimation(Transform RightTrans, Transform LeftTrans, Vector3 TargetPos)
    {

        if (HandAnimaStartFlag)
        {
            HandElapsedTime = 0;
            HandTargetPos = TargetPos;
            HandAnimaMoveFlag = true;
            HandAnimaReturnFlag = false;
            HandAnimaStartFlag = false;
        }

        if (HandAnimaMoveFlag)
        {
            if (HandElapsedTime > 1f)
            {
                HandAnimaReturnFlag = true;
            }
            if (HandElapsedTime < -0.2f)
            {
                HandAnimaMoveFlag = false;
            }

            if (!HandAnimaReturnFlag)
            {
                HandElapsedTime += Time.deltaTime * 3f;
                RightTrans.localPosition = Vector3.Lerp(HandInitPos[0], HandTargetPos, HandElapsedTime);
                LeftTrans.localPosition = Vector3.Lerp(HandInitPos[1], HandTargetPos, HandElapsedTime);
            }
            else
            {
                HandElapsedTime -= Time.deltaTime * 3f;
                RightTrans.localPosition = Vector3.Lerp(HandInitPos[0], HandTargetPos, HandElapsedTime);
                LeftTrans.localPosition = Vector3.Lerp(HandInitPos[1], HandTargetPos, HandElapsedTime);
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
