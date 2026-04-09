using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameDraw : MonoBehaviour
{
    [SerializeField] private Mesh TargetMesh;
    [SerializeField] private GameObject Target;
    [SerializeField] private float TargetTY ;
    private Color gizmoColor = Color.yellow;


    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireMesh(TargetMesh, Target.transform.position, Target.transform.rotation, Target.transform.lossyScale);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
