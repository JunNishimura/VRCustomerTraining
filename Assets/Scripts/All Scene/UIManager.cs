using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    //会話の選択肢に関する宣言
    public GameObject guidePanel;
    public GameObject hintPanel1;
    public GameObject hintPanel2;
    public GameObject hintPanel3;
    public GameObject resultPanel;
    public GameObject evaluationPanel;
    public GameObject recognitionPanel;
    public GameObject APanel;
    public GameObject BPanel;

    public Text guide;
    public Text hint1;
    public Text hint2;
    public Text hint3;
    public Text result;
    public Text recognition;
    public Text a;
    public Text b;
    public Text time;
    public Text evaluation;

    public Text ordertable;

    int i;

    bool timestop = false;
    public static bool Scenariofinished = false;

  

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }
       

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Stage Start");
        guidePanel.SetActive(true);
        hintPanel1.SetActive(false);
        hintPanel2.SetActive(false);
        hintPanel3.SetActive(false);
        resultPanel.SetActive(false);
        evaluationPanel.SetActive(false);
        result.text = "";
        evaluation.text = "";
        a.text = "";
        b.text = "";
        recognition.text ="";

        i = 0;
    }

    // Update is called once per frame
    void Update()
    {

        //xボタンを押すとガイドパネルの表示・非表示の切り替え
        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            if(guidePanel.activeSelf == false)
            {
                guidePanel.SetActive(true);
            }
            else
            {
                guidePanel.SetActive(false);
            }                       
        }

        //yボタンを押すとヒントパネルの表示・非表示の切り替え
        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            switch (i)
            {
                case 0:
                    hintPanel1.SetActive(true);
                    i++;
                    break;

                case 1:
                    hintPanel1.SetActive(false);
                    hintPanel2.SetActive(true);
                    i++;
                    break;

                case 2:
                    hintPanel2.SetActive(false);
                    hintPanel3.SetActive(true);
                    i++;
                    break;

                default:
                    hintPanel1.SetActive(false);
                    hintPanel2.SetActive(false);
                    hintPanel3.SetActive(false);
                    i = 0;
                    break;

            }
                
        }

        /*
        //ポーズした場合
        if (timestop == true)
        {
            
            //終了する場合
            if (OVRInput.GetDown(OVRInput.RawButton.A))
            {
                Scenariofinished = true;
                Time.timeScale = 1f;
                timestop = false;
                //Debug.Log("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");

            }
           
        //終了しない場合
        if (OVRInput.GetDown(OVRInput.RawButton.B))
            {
                Restart();
                //Debug.Log("ccccccccccccccccccccccccccccccccccccccccccccccccccccccccc");

            }

        }
        else
        {
            //Bボタンでポーズ
            if (OVRInput.GetDown(OVRInput.RawButton.B))
            {
                Pause();
            }
        } */

    }

    //Bボタンにてボーズした際の処理
    private void Pause()
    {
        Time.timeScale = 0f;
        resultPanel.SetActive(true);
        hintPanel1.SetActive(false);
        hintPanel2.SetActive(false);
        hintPanel3.SetActive(false);

        result.text = "ポーズ中です。\n" +
        "操作方法\n" +
        "・右Touchの上のボタン(Aボタン)：発話した直後に押すことでシナリオが進行\n" +
        "・右Touchの下のボタン(Bボタン)：ポーズ\n" +
        "・左Touchの上のボタン(Xボタン)：ガイドパネルの表示非表示の切り替え\n" +
        "・左Touchの下のボタン(Yボタン)：ヒントパネルの表示非表示の切り替え(全3ページ)\n" +
        "・中指にあるボタン(グリップ)：手を料理・皿の方へ向けて持つ\n" +
        "・左スティック：テレポート移動\n" +
        "・右スティック：テレポート移動後のキャラクターの向きを変更する";
        timestop = true;
    }

    //ゲーム終了せずに継続した場合の処理
    private void Restart()
    {
        resultPanel.SetActive(false);
        Time.timeScale = 1f;
        timestop = false;
        i = 0;
        result.text = "";
    }

    //オーダー表の文字を削除する関数
    public void Order_Remove(string text)
    {
        ordertable.text = ordertable.text.Replace(text, "");
    }
}
