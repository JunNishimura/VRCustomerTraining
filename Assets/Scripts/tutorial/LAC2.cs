using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LAC2 : MonoBehaviour
{

    Animator animator;
    //Animator animator1;
    //Animator animator2;

    Vector3 targetPos;
    public GameObject avator;
    public bool judge = false;

    //public GameObject customer1;
   // public GameObject customer2;

    private static int flag = 0;

    //public GameObject talkable;   //話しかけられる範囲を取得

    void Start()
    {
        this.animator = GetComponent<Animator>();
        this.targetPos = avator.transform.position;
        //Trigger trigger = talkable.GetComponent<Trigger>();

        //animator1 = customer1.GetComponent<Animator>();
       // animator2 = customer2.GetComponent<Animator>();

       // Debug.Log(animator1 + "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
        //Debug.Log(customer2.name + "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
    }

    void Update()
    {
        Vector3 touchPos = avator.transform.position;
        //touchPos.z = -0.5f;
        targetPos = touchPos;

        // Debug.Log("flag:" + flag);
    }


    public void conversation(bool conversation)
    {
        if (conversation == true)
        {
            judge = true;

        }
        else
        {
            judge = false;
            Debug.Log("tmp:" + targetPos);
        }

    }


    // 重なり瞬間判定
    void OnTriggerEnter(Collider other)
    {
        if (other.name == "OVRPlayerController")
        {
            flag = 1;
            Debug.Log(other.name + "Enter" + flag);

        }

    }

    // 重なり離脱の判定
    void OnTriggerExit(Collider other)
    {
        flag = 0;
        //Debug.Log(other.name + "Exit" + flag);
    }


    private void OnAnimatorIK(int layerIndex)
    {


        if (Trigger2.flag == 1)
        {
            this.animator.SetLookAtWeight(1.0f, 0.2f, 0.6f, 0.0f, 0f);
            this.animator.SetLookAtPosition(targetPos);

           

            /*
            animator1.SetLookAtWeight(1.0f, 0.2f, 0.6f, 0.0f, 0f);
            animator1.SetLookAtPosition(targetPos);

            animator2.SetLookAtWeight(1.0f, 0.2f, 0.6f, 0.0f, 0f);
            animator2.SetLookAtPosition(targetPos);
            */
        }

    }
}