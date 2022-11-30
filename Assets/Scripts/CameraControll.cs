using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{

    public GameObject mainCamera;      //メインカメラ格納用
    public GameObject subCamera;       //サブカメラ格納用 


    //呼び出し時に実行される関数
    void Start()
    {
        //メインカメラとサブカメラをそれぞれ取得
        //mainCamera = GameObject.Find("MainCamera");
        //subCamera = GameObject.Find("Camera");

        //サブカメラを非アクティブにする
        subCamera.SetActive(false);
    }


    //単位時間ごとに実行される関数
    void Update()
    {
        //スペースキーが押されている間、サブカメラをアクティブにする
        if (Input.GetKey("space"))
        {
            //サブカメラをアクティブに設定
            mainCamera.SetActive(false);
            subCamera.SetActive(true);
        }
        else
        {
            //メインカメラをアクティブに設定
            subCamera.SetActive(false);
            mainCamera.SetActive(true);
        }
    }
}