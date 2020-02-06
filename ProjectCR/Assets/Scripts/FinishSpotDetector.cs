using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishSpotDetector : MonoBehaviour
{
    [SerializeField]
    private CDAtFishshSpot[] spotLayers = null;

    public CDAtFishshSpot mainSpot = null;
    
    public void Initialize()
    {
        mainSpot.Initialize();

        foreach (var s in spotLayers)
        {
            s.Initialize();
        }
    }
    
    public void ReSetflags()
    {
        Initialize();
    }

    public void ClacHowManyLayerOn()
    {
        DesignConstStorage.DoItList();
        //
    }
}