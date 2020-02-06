using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class Master_Node : MonoBehaviour
{
    public GameObject Main_Object;
    public GameObject Sub_Objects_N;
    public GameObject Sub_Objects_P;
    public Master_Node Next_Node;

    //private Sprite img;

    public Master_Node Set_Node(GameObject M_Object, Sprite sprite)
    {
        Main_Object = M_Object;
        //img = sprite;
        //Sub_Objects_N = new GameObject();
        //Sub_Objects_P = new GameObject();
        return this;
    }
    
    public void ITSME(string n, Vector3 v)
    {
        SetTarget(n);
        M_Target(v);
    }
     
    public void Set_Next_Node(Master_Node qwe)
    {
        Next_Node = qwe;
    }

    private GameObject Origin_Target;
    private GameObject Move_Target;

    // Update is called once per frame
    void Update()
    {
       
    }


    private void M_Target(Vector3 v)
    {
        Move_Target.GetComponent<Sub_Node_N>().SetDir(v);
    }


    private void SetTarget(string name)
    {
        if(name == "N")
        {
            Origin_Target = Sub_Objects_N;
            Move_Target = Sub_Objects_P;
        }
        else
        {
            Origin_Target = Sub_Objects_P;
            Move_Target = Sub_Objects_N;
        }
    }
}
