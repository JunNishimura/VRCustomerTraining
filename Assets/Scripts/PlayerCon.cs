using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    //Rigidbody型のrbという変数を作る
    private Rigidbody rb;

    //float型のpowerという変数を作り、3fを代入する
    private float power = 3f;

    private void Start()
    {
        //リジットボディを取得しrbに代入する
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //キー入力による値を代入(解説は後で
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        //↑で代入された値をVector3型のIdouHoukouという変数に代入する
        Vector3 IdouHoukou = new Vector3(x, 0, z);

        //rbに力を加える
        //velocityは質量などの物理演算を無視するときに使う
        rb.velocity = IdouHoukou * power;

        //rbの回転をさせない様にする
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        //移動範囲の制限
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -5f, 5f),
            Mathf.Clamp(transform.position.y, 1f, 1f),
         Mathf.Clamp(transform.position.z, 5f, 5f));
    }
}