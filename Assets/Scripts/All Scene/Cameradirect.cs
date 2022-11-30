using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameradirect : MonoBehaviour
{
    //このスクリプトは操作キャラにつける

    float cameraAngle1;　//カメラの角度を代入する変数
    float cameraAngle2;
    float cameraAngle3;
    float cameraAngle4;

    Vector3 cameradirect;

    public new GameObject camera; //カメラオブジェクトを格納
    public GameObject target1;  //キャラ
    public GameObject target2;
    public GameObject target3;
    public GameObject target4;

    public static int flag = 0;

    //キャラの数だけフラグを用意
    public static int flag1 = 0;
    public static int flag2 = 0;
    public static int flag3 = 0;
    public static int flag4 = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log(this.target1.transform.position);
    }

    // Update is called once per frame
    void Update()
    {

        //target1
        //カメラの方向を取得
        cameradirect = camera.transform.forward;
        //Debug.Log("camaraangle:" + cameraAngle);
        //Debug.Log("camaradirect:" + cameradirect);

        //target1
        Vector3 aim1 = this.target1.transform.position - camera.transform.position;　　//ターゲットまでの方向を取得
        aim1.y = aim1.y + 1.50f;  //キャラの顔への方向調整
                                  // var aim1_n = aim1.normalized;

        cameraAngle1 = Vector3.Angle(cameradirect, aim1);

                

        if(0 < cameraAngle1 && cameraAngle1 < 15)
        {
            flag1 = 1;
           // Debug.Log("カメラ角度OK 1 ");
        }
        else
        {
            flag1 = 0;
        }

        //target2
        Vector3 aim2 = this.target2.transform.position - camera.transform.position;
        aim2.y = aim2.y + 1.50f;
        //var aim2_n = aim2.normalized;

        cameraAngle2 = Vector3.Angle(cameradirect, aim2);

        if (0 < cameraAngle2 && cameraAngle2 < 15)
        {
            flag2 = 1;
           // Debug.Log("カメラ角度OK 2 ");
        }
        else
        {
            flag2 = 0;
        }



        //target3
        var aim3 = this.target3.transform.position - camera.transform.position;
        aim3.y = aim3.y + 1.50f;

        cameraAngle3 = Vector3.Angle(cameradirect, aim3);

        if (0 < cameraAngle3 && cameraAngle3 < 15)
        {
            flag3 = 1;
           // Debug.Log("カメラ角度OK 3 ");
        }
        else
        {
            flag3 = 0;
        }


        //target4
        var aim4 = this.target4.transform.position - camera.transform.position;
        aim4.y = aim4.y + 1.50f;
        var aim4_n = aim4.normalized;

        cameraAngle4 = Vector3.Angle(cameradirect, aim4);

        if (0 < cameraAngle4 && cameraAngle4 < 15)
        {
            flag4 = 1;
            //Debug.Log("カメラ角度OK 4 ");
        }
        else
        {
            flag4 = 0;
        }



        //var look = Quaternion.LookRotation(aim, Vector3.up);
        // Debug.Log("aim:" + aim_n);
        //this.transform.localRotation = look;

        //Quaternion lookRotation = Quaternion.LookRotation(-target1.transform.position + transform.position, Vector3.up);

        //lookRotation.z = 0;
        //lookRotation.x = 0;

        //transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.1f);

    }
}
