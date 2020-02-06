using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Sub_Node_N : MonoBehaviour
{
    public Master_Node master_node;
    public Vector3 current;

    private float distance = 0;
    private Vector3 dir;

    [SerializeField]
    private string Name;

    void Start()
    {
        dir = (Vector3.zero - this.gameObject.transform.localPosition).normalized;
        distance = Vector3.Distance(Vector3.zero, this.gameObject.transform.localPosition);

        current = transform.localPosition;
    }
    
    void Update()
    {

        if (transform.localPosition != current)
        {
            dir = (Vector3.zero - this.gameObject.transform.localPosition).normalized;
            distance = Vector3.Distance(Vector3.zero, this.gameObject.transform.localPosition);

            master_node.ITSME(Name,dir);
        }

        current = transform.localPosition;
    }

    public void SetDir(Vector3 v)
    {
        transform.localPosition = v * distance;
        
        current = transform.localPosition;
    }
}