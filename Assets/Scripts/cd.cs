using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cd : MonoBehaviour
{
    //このスクリプトは操作キャラにつける

    float cameraAngle;　//カメラの角度を代入する変数
    Vector3 cameradirect;

    public new GameObject camera; //カメラオブジェクトを格納
    public GameObject target1;  //キャラ
    public GameObject target2;
    public GameObject target3;
    public GameObject target4;
    public static int flag = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log(this.target1.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        //カメラの方向を取得
        cameradirect = camera.transform.forward;
        //Debug.Log("camaraangle:" + cameraAngle);
        //Debug.Log("camaradirect:" + cameradirect);

        //target1
        Vector3 aim1 = this.target1.transform.position - camera.transform.position;　　//ターゲットまでの方向を取得
        aim1.y = aim1.y + 1.50f;  //キャラの顔への方向調整
                                  // var aim1_n = aim1.normalized;

        cameraAngle = Vector3.Angle(cameradirect, aim1);

        //Debug.Log(this.target1.transform.position);

        //Debug.Log("cameradirect = " + cameradirect);

        /*
        if ((aim1.y - 0.2f < cameradirect.y && aim1.y + 1.0f > cameradirect.y))
        {
            Debug.Log("y座標範囲はいりました");
            if (aim1.x - 0.1f < cameradirect.x && cameradirect.x < aim1.x + 0.1f)
            {
                Debug.Log("x座標範囲はいりました");
                flag = 1;
            }
            else
            {
                flag = 0;
            }

        }
        else
        {
            flag = 0;
        }
        */



        if (0 < cameraAngle && cameraAngle < 10)
        {
            flag = 1;
            Debug.Log("カメラ角度OK");
        }
        else
        {
            flag = 0;
        }

        //target2
        var aim2 = this.target2.transform.position - camera.transform.position;
        aim2.y = aim2.y + 1.25f;
        var aim2_n = aim2.normalized;


        if ((aim2.y - 0.2f < cameradirect.y && aim2.y + 1.0f > cameradirect.y))
        {
            //Debug.Log("y座標範囲はいりました");
            if (aim2.x - 0.1f < cameradirect.x && cameradirect.x < aim2.x + 0.1f)
            {
                // Debug.Log("x座標範囲はいりました");
                flag = 1;
            }
            else
            {
                flag = 0;
            }

        }
        else
        {
            flag = 0;
        }


        //target3
        var aim3 = this.target3.transform.position - camera.transform.position;
        aim3.y = aim3.y + 1.25f;
        var aim3_n = aim3.normalized;


        if ((aim3.y - 0.2f < cameradirect.y && aim3.y + 1.0f > cameradirect.y))
        {
            //Debug.Log("y座標範囲はいりました");
            if (aim3.x - 0.1f < cameradirect.x && cameradirect.x < aim3.x + 0.1f)
            {
                // Debug.Log("x座標範囲はいりました");
                flag = 1;
            }
            else
            {
                flag = 0;
            }

        }
        else
        {
            flag = 0;
        }



        //target4
        var aim4 = this.target4.transform.position - camera.transform.position;
        aim4.y = aim4.y + 1.25f;
        var aim4_n = aim4.normalized;


        if ((aim4.y - 0.2f < cameradirect.y && aim4.y + 1.0f > cameradirect.y))
        {
            //Debug.Log("y座標範囲はいりました");
            if (aim4.x - 0.1f < cameradirect.x && cameradirect.x < aim4.x + 0.1f)
            {
                // Debug.Log("x座標範囲はいりました");
                flag = 1;
            }
            else
            {
                flag = 0;
            }

        }
        else
        {
            flag = 0;
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
