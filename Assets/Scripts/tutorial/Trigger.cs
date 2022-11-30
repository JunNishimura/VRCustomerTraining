using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.Windows.Speech;
using System.Linq;

public class Trigger : MonoBehaviour
{

    public static int flag = 0;

    public GameObject talkable;

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
        if(other.name == "OVRPlayerController")
        {
            flag = 1;
           // Debug.Log(other.name + "Enter" + enter);
        }
       
    }

    // 重なり離脱の判定
    void OnTriggerExit(Collider other)
    {
        if (other.name == "OVRPlayerController")
        {
            flag = 0;
            //Debug.Log(other.name + "Exit" + enter);
        }
    }

    public int GetEnter()
    {
        return flag;
    }

}
