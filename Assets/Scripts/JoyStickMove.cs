using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class JoyStickMove : MonoBehaviour
{
    void Update()
    {
        Move();
    }

    void Move()
    {
        //右ジョイスティックの情報取得
        Vector2 stickL = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
        Vector3 changePosition = new Vector3((stickL.x), 0, (stickL.y));
        //HMDのY軸の角度取得
        Vector3 changeRotation = new Vector3(0, InputTracking.GetLocalRotation(XRNode.Head).eulerAngles.y, 0);
        //OVRCameraRigの位置変更
        this.transform.position += this.transform.rotation * (Quaternion.Euler(changeRotation) * changePosition);
    }
}