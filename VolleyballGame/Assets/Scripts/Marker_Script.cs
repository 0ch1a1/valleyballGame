using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker_Script : MonoBehaviour
{
    [SerializeField] private PCState_Script PCState_1;
    [SerializeField] private PCState_Script PCState_2;
    [SerializeField] private WholeState_Script WholeState;
    [SerializeField] private Transform BallTrans;
    [SerializeField] private Transform MarkerTrans;
    [SerializeField] private Transform CircleTrans;
    [SerializeField] private Transform PC1_CharaTrans;
    [SerializeField] private Transform PC2_CharaTrans;
    public GameObject MarkerInObj;
    
    private float preDis;
    // Start is called before the first frame update
    void Start()
    {
        preDis = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        MarkerMovement();
        //if(WholeState.AttackSide_CS == AttackSideState.Player)
        //{

        //}
        if(PCState_1.MoveChara == true || WholeState.ActionStates_CS == ActionStates.Serve && WholeState.playerServe.Counter == 0)
        {
            TimingVisualize(MarkerTrans.lossyScale, BallTrans, PC1_CharaTrans, CircleTrans);
        }
        else if(PCState_2.MoveChara == true || WholeState.ActionStates_CS == ActionStates.Serve && WholeState.playerServe.Counter == 1)
        {
            TimingVisualize(MarkerTrans.lossyScale, BallTrans, PC2_CharaTrans, CircleTrans);
        }
        else
        {
            CircleTrans.position = MarkerTrans.position;
            CircleTrans.localScale = MarkerTrans.lossyScale/3f;
        }
    }

    void MarkerMovement()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        //int layerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");

        if (Physics.Raycast(ray, out hit))
        {
            MarkerTrans.position = hit.point;
            //Debug.Log(target_Position);
            MarkerInObj = hit.collider.gameObject;
        }


    }

    void TimingVisualize(Vector3 MarkerScale, Transform BallTrans, Transform TargetTrans, Transform CircleTrans)
    {
        Vector3 TargetPos = TargetTrans.position;
        Vector3 BallPos = BallTrans.position;
        float distance = Vector3.Distance(TargetPos, BallPos);
        float Amount_Change = distance - preDis;
        if (Amount_Change > 0f)
        {
            CircleTrans.localScale = MarkerScale/3f;
        }
        else if (distance < 10f && distance > 0.1f && Amount_Change < 0f)
        {
            CircleTrans.localScale = (MarkerScale/3f - new Vector3(1, 1, 0.5f) * 0.75f) + new Vector3(1,1,1) * 0.5f *  Mathf.Abs(distance);
        }
        CircleTrans.position = MarkerTrans.position;
        preDis = distance;
    }
}
