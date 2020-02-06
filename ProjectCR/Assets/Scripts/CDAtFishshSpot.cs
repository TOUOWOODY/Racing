using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDAtFishshSpot : MonoBehaviour
{
    [SerializeField]
    private FinishSpotDetector fSD = null;
    
    private bool isOn = false;
    public bool Ison
    {
        get
        {
            //Debug.LogWarning(string.Format("{0}    {1}", this.gameObject.name, isOn));
            return isOn;
        }
    }

    public void Initialize()
    {
        isOn = false;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.LogWarning(string.Format("{0} enter trigger at {1}", collision.gameObject.tag, this.gameObject.name));
        //fSD.SetLayerFlag(this.gameObject.name,true);
        isOn = true;
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.LogWarning(string.Format("{0} exit trigger at {1}", collision.gameObject.tag, this.gameObject.name));
        //fSD.SetLayerFlag(this.gameObject.name, false);
        isOn = false;
    }
}