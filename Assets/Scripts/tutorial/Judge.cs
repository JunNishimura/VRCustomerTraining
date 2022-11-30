using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 重なり瞬間判定
    void OnTriggerEnter(Collider other)
    {

        string layerName = LayerMask.LayerToName(other.gameObject.layer);
        if (layerName == "grabbable" && FlagManager.Instance.flags[6] == true)
        {
            FlagManager.Instance.flags[7] = true;
           // Debug.Log(other.name + "Enter" + flag);
        }

    }

    // 重なり離脱の判定
    void OnTriggerExit(Collider other)
    {
       
        //Debug.Log(other.name + "Exit" + flag);
    }
}
