using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class WarpMove : MonoBehaviour
{
    [SerializeField]
    GameObject hand;
    GameObject line;
    int positionCount = 10;
    float angle = 30;
    float groundPositionY = 0;
    float lazerStartDistance = 0.15f;
    float lazerStartPosition;
    float distance = 10;
    float minDistance = 2;
    float maxDistance = 100;
    float dropHeight = 5;
    float width = 0.1f;
    bool activeDraw = false;
    Vector3 hitPoint;
    LineRenderer linerenderer;

    void Start()
    {
        Line();
    }

    void Update()
    {
        ChangeDirection();
        Controller();
    }

    //トリガーを押しているかどうか
    void Controller()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            activeDraw = true;
            line.SetActive(true);
        }

        if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
        {
            activeDraw = false;
            line.SetActive(false);
            distance = 10;
            this.transform.position = new Vector3(hitPoint.x, this.transform.position.y, hitPoint.z);
        }

        if (activeDraw)
        {
            Ray();
        }
    }

    //OVRCameraRigの角度変更
    void ChangeDirection()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.LThumbstickLeft))
        {
            this.transform.Rotate(0, -angle, 0);
        }
        else if (OVRInput.GetDown(OVRInput.RawButton.LThumbstickRight))
        {
            this.transform.Rotate(0, angle, 0);
        }
    }

    //LineRendererコンポーネントがアタッチされたゲームオブジェクトを作成
    void Line()
    {
        line = new GameObject("Line");
        line.transform.parent = hand.transform;
        linerenderer = line.AddComponent<LineRenderer>();
        linerenderer.receiveShadows = false;
        linerenderer.shadowCastingMode = ShadowCastingMode.Off;
        linerenderer.loop = false;
        linerenderer.positionCount = positionCount;
        linerenderer.startWidth = width;
        linerenderer.endWidth = width;
    }

    public void Ray()
    {
        ChangeDistance();
        Vector3 rayStartPosition = hand.transform.position + hand.transform.forward * lazerStartDistance;
        Vector3 rayMiddlePosition = rayStartPosition + hand.transform.forward * (distance / 2);
        Vector3 rayFinishPosition = rayStartPosition + hand.transform.forward * distance;
        rayFinishPosition.y = groundPositionY;
        Vector3 _tmp3 = rayStartPosition;

        for (int i = 0; i < positionCount; i++)
        {
            float plotPosition = (i / (float)(positionCount - 1));
            Vector3 tmp1 = Vector3.Lerp(rayStartPosition, rayMiddlePosition, plotPosition);
            Vector3 tmp2 = Vector3.Lerp(rayMiddlePosition, rayFinishPosition, plotPosition);
            Vector3 tmp3 = Vector3.Lerp(tmp1, tmp2, plotPosition);

            RaycastHit hit;
            if (Physics.Linecast(_tmp3, tmp3, out hit))
            {
                Debug.Log("hit!!");
                hitPoint = hit.point;
                for (int j = i; j < positionCount; j++)
                {
                    linerenderer.SetPosition(j, hitPoint);
                }
                break;
            }
            else
            {
                linerenderer.SetPosition(i, tmp3);
                _tmp3 = tmp3;
            }
        }
    }

    //レーザーの長さ変更
    void ChangeDistance()
    {
        Vector2 stickR = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);
        distance = Mathf.Clamp(distance += stickR.y, minDistance, maxDistance);

    }
}