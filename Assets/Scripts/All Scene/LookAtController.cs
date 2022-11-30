using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtController : MonoBehaviour
{

    Animator animator;

    //振り向く方向の取得
    Vector3 targetPos;
    Vector3 touchPos;
   
    public GameObject avator;
    public GameObject cube;
   
    int enter = 0;

    //public static bool jobcompleted = false;


    void Start()
    {
        this.animator = GetComponent<Animator>();
        this.targetPos = avator.transform.position;
    }

    void Update()
    {
        touchPos = avator.transform.position;
        //touchPos.z = -0.5f;
        targetPos = touchPos;
        enter = cube.GetComponent<EnterCube>().GetEnter();

        if (enter == 1)
        {
            this.animator.SetBool("idle", true);
        }
        else
        {
            this.animator.SetBool("idle", false);
        }
        
    }

    //アバター振り向き
    private void OnAnimatorIK(int layerIndex)
    {
        //cube内にアバターが入ったら
        if (enter == 1 )
        {       
            this.animator.SetLookAtWeight(1.0f, 0.2f, 0.6f, 0.0f, 0f);
            this.animator.SetLookAtPosition(targetPos);
        }

    }
}