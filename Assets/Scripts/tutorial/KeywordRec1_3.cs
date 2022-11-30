using System.Collections;
using System.Collections.Generic;   //Windowsの音声認識で使用(最初から指定されてる）
using UnityEngine;
using UnityEngine.Windows.Speech;   //Windowsの音声認識で使用
using System.Linq;                  //Windowsの音声認識で使用
using UnityEngine.UI;
using UnityEngine.AI;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class KeywordRec1_3 : MonoBehaviour
{
    private bool hasRecognized;
    KeywordRecognizer keywordRecognizer;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();


    //頭の位置
    //public GameObject CameraPos;

    Vector3 FirstPos;



    public AudioClip Cus1;
    public AudioClip Cus2;
    public AudioClip Cus3;


    AudioSource audioSource;

    //話かける判定
    public GameObject talkable;   //話しかけられる範囲を取得
    public GameObject player;  //操作キャラを取得

    private void Awake()
    {
        //animator = Cman.GetComponent<Animator>();
    }

    void Start()
    {



        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = Cus1;




        //Cmanのアニメーションの設定
        //agent = GetComponent<NavMeshAgent>();



        //senario0
        //反応するキーワードを辞書に登録
        keywords.Add("May I take your plate?", () => {
            Debug.Log("「Would you like your coffee before or after meal?」をキーワードに指定");
        });

        keywords.Add("Are you done?", () => { });
        keywords.Add("Are you still working on it?", () => { });

        //デバック用
        keywords.Add("Hello", () => { });


        //キーワードを渡す
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        //キーワードを認識したら反応するOnPhraseRecognizedに「KeywordRecognizer_OnPhraseRecognized」処理を渡す
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;

        //音声認識開始
        keywordRecognizer.Start();
        //Debug.Log("音声認識開始");

        //TriggerとCameradirectにあるフラグを持ってくる
        Trigger trigger = talkable.GetComponent<Trigger>();
        Cameradirect direct = player.GetComponent<Cameradirect>();

    }



    void Update()
    {



        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("Order");
        }

        /*
        if (Input.GetKey(KeyCode.RightShift))
        {
            FirstPos = CameraPos.transform.position;

        }

        if ((FirstPos.y - CameraPos.transform.position.y) >= 0.22f)
        {
            Debug.Log("bow");
        }
        */

        if (Trigger2.flag == 1)
        {
            //振り向き
           // LookAtController look = GetComponent<LookAtController>();
           // look.conversation(true);

            Debug.Log("会話範囲に入りました:table2");
            if (Cameradirect.flag3 == 1)
            {
                Debug.Log("会話相手を見ています:oldman");
            }
        }

        Debug.Log("Cameradirect3 = " + Cameradirect.flag3);


    }


    //キーワードを認識したときに反応する処理
    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {

        if (Trigger2.flag == 1)
        {                    

            //Debug.Log("会話範囲に入りました");
            if (Cameradirect.flag3 == 1)
            {

                //デリゲート
                //イベントやコールバック処理の記述をシンプルにしてくれる。クラス・ライブラリを活用するには必須らしい
                System.Action keywordAction;//　keywordActionという処理を行う

                //認識されたキーワードが辞書に含まれている場合に、アクションを呼び出す。
                if (keywords.TryGetValue(args.text, out keywordAction) && FlagManager.Instance.flags[5] == true)
                {                                    

                    Debug.Log("認識した");
                    hasRecognized = true;
                    audioSource.PlayOneShot(Cus1); //yes

                    FlagManager.Instance.flags[5] = false;
                    //認識完了、シナリオ更新
                    FlagManager.Instance.flags[6] = true;


                }

                

            }
        }
    }

}