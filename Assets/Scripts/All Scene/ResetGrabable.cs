using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGrabable : MonoBehaviour
{
    //お皿
    public GameObject grabable;

    // Start is called before the first frame update
    void Start()
    {
        GetChildren(grabable);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit"); // ログを表示する
    }

    void GetChildren(GameObject obj)
    {
        Transform children = obj.GetComponentInChildren<Transform>();
        //子要素がいなければ終了
        if (children.childCount == 0)
        {
            return;
        }
        foreach (Transform ob in children)
        {
            //ここに何かしらの処理
            //例　ボーンについてる武器を取得する
            //if (ob.name == "Right Hand")
            //  {
            //      rightHandWeapon = ob.transform.GetChild(0).gameObject;
            //   }
            GetChildren(ob.gameObject);
        }
    }
}
