using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.Windows.Speech;
using System.Linq;

public class EnterCube : MonoBehaviour
{

   
    private int enter = 0;

    private int count = 0;

    //料理の名前を保存
    List<string> foodList = new List<string>();

    private void Update()
    {
        if(enter == 1 && OVRInput.GetDown(OVRInput.RawButton.B))
        {
            UIManager.Instance.b.text = "";
            Speech_Recognition_Manager.start_rec = true;
            UIManager.Instance.recognition.text = "音声認識可能";
            UIManager.Instance.recognition.color = Color.green;
        }
        //else if (enter == 1)
        //{
        //    UIManager.Instance.b.text = "B";
        //}



        //if (enter == 0)
        //{
        //    UIManager.Instance.b.text = "";
        //}
    }

    // 重なり瞬間判定
    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            enter = 1;
            //enterflag = true;
            UIManager.Instance.b.text = "B";
            count++;
            Debug.Log(other.name + "Enter" + enter);
        }
        if(other.gameObject.CompareTag("food") && !foodList.Contains(other.name))
        {
            foodList.Add(other.name);
            Debug.Log(other.name + ":list in" );
        }

    }

    // プレイヤーがテーブルの前（cube）に来たかの重なり判定
    void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            enter = 0;
            
            UIManager.Instance.b.text = "";
            UIManager.Instance.recognition.text = "";
            // enterflag = false;
            Debug.Log(other.name + "Exit" + enter);
            if (Speech_Recognition_Manager.rec_complete == true) Speech_Recognition_Manager.rec_complete = false;
        }
        if (other.gameObject.CompareTag("food") && foodList.Contains(other.name))
        {
            foodList.Remove(other.name);
            Debug.Log(other.name + ":out of list");
        }

        
    }

    //cube（テーブルの前）に入ったかの判定値、１：入、０：出
    public int GetEnter()
    {
        return enter;
    }

    //cubeに入った回数を記録
    public int GetEnterNumber()
    {
        return count;

    }


    //cubeに入ってきたobjectの名前をlistへ
    public List<string> GetObjectName()
    {
        List<string> food = new List<string>();

        if(foodList == null)
        {
            return food;
        }

        return foodList;
    }
}