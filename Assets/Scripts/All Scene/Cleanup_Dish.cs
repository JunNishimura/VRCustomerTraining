using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleanup_Dish : MonoBehaviour
{
    //フラグ
    private static int dish_enter = 0;
    string layerName = null;
    List<string> plateList = new List<string>();

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

        layerName = LayerMask.LayerToName(other.gameObject.layer);
        if (layerName == "grabbable")
        {
            dish_enter = 1;
            Debug.Log(other.name + "Enter" + dish_enter);
        }

        if (other.gameObject.CompareTag("plate") && !plateList.Contains(other.name)) 
        {
            plateList.Add(other.name);
            Debug.Log(other.name + ":list in");
        } 

    }

    // 重なり離脱の判定
    void OnTriggerExit(Collider other)
    {
        layerName = LayerMask.LayerToName(other.gameObject.layer);
        if (layerName == "grabbable")
        {
            dish_enter = 0;
            Debug.Log(other.name + "Enter" + dish_enter);
        }
        if (other.gameObject.CompareTag("plate") && plateList.Contains(other.name))
        {
            plateList.Remove(other.name);
            Debug.Log(other.name + ":out of list");
        }

    }

    public int GetPlateEnter()
    {
        int counter;
        counter = plateList.Count;
        //Debug.Log("counter ==================" + counter);
        return counter;
    }

}
