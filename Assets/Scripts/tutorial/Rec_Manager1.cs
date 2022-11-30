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

public class Rec_Manager1 : MonoBehaviour
{
    private bool hasRecognized;
    KeywordRecognizer keywordRecognizer;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    public static bool ConversationStart = false;


    //頭の位置
    // public GameObject CameraPos;

    Vector3 FirstPos;

    public AudioClip Cus1;　//Two lunch sets, please
    public AudioClip Cus2;  //and coffee
    public AudioClip Cus3;  //yes_man
    public AudioClip Cus4;  //yes_woman
    public AudioClip Cus5;  //after meal
    
    AudioSource audioSource;


    //話かける判定
    public GameObject talkable;   //話しかけられる範囲を取得
    public GameObject player;  //操作キャラを取得
    //public Trigger trigger;

    private int ef = 0;
    

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



        //man1
        //反応するキーワードを辞書に登録
        keywords.Add("May I take your order?", () => {
            Debug.Log("「May I take your order ?」をキーワードに指定");
        });

        keywords.Add("Ready to Order? ", () => { });

        keywords.Add("Are you ready to order?", () => { });


       
        keywords.Add("All right", () => { });

        keywords.Add("Two lunch sets and one with coffee, right?", () => { });

        //woman
        keywords.Add("Would you like your coffee before or after meal?", () => {
            Debug.Log("「Would you like your coffee before or after meal?」をキーワードに指定");
        });

        keywords.Add("Sure", () => { });
        keywords.Add("Of course", () => { });
        keywords.Add("Certainly", () => { });


        //デバック用
        keywords.Add("Hello", () => { });

        //senario1　反応するキーワードを辞書に登録

        /*
        keywords.Add("Please take a seat anywhere you like", () => { });

        keywords.Add("Please be seated there", () => { });
        */

        //キーワードを渡す
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        //キーワードを認識したら反応するOnPhraseRecognizedに「KeywordRecognizer_OnPhraseRecognized」処理を渡す
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;

        //音声認識開始
        keywordRecognizer.Start();
        Debug.Log("音声認識開始");

        //TriggerとCameradirectにあるフラグを持ってくる
        //Trigger trigger = talkable.GetComponent<Trigger>();
        //int trigger = talkable.GetComponent<Trigger>().flag;
        Cameradirect direct = player.GetComponent<Cameradirect>();

    }



    void Update()
    {



        /*
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


        //男性アバターが話し終わったら
        if (FlagManager.Instance.flags[1] == true && !audioSource.isPlaying)
        {
            audioSource.clip = Cus2;
            audioSource.PlayOneShot(Cus2); //and one coffee
            
            FlagManager.Instance.flags[1] = false;

            //ユーザの音声認識へ
            FlagManager.Instance.flags[2] = true;

        }


        

        if (Trigger.flag == 1)
        {
            Debug.Log("Trigger.flag =  " + Trigger.flag);
            Debug.Log("会話範囲に入りました:table1");

            //アバター振り向き
            //LookAtController look = GetComponent<LookAtController>();
            //look.conversation(true);
                       

            if (Cameradirect.flag1 == 1)
            {
                Debug.Log("会話相手を見ています:man");
            }

            if (Cameradirect.flag2 == 1)
            {
                Debug.Log("会話相手を見ています:woman");
            }

        }

        //Debug.Log("Cameradirect1 = " + Cameradirect.flag);
        Debug.Log("Cameradirect1 = " + Cameradirect.flag1);




    }


    //キーワードを認識したときに反応する処理
    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        
        if (Trigger.flag == 1)
        {

            //man1の音声認識
            if (Cameradirect.flag1 == 1)
            {
          
             //デリゲート
            //イベントやコールバック処理の記述をシンプルにしてくれる。クラス・ライブラリを活用するには必須らしい
              System.Action keywordAction;//　keywordActionという処理を行う

                //認識されたキーワードが辞書に含まれている場合に、アクションを呼び出す。
                 if (keywords.TryGetValue(args.text, out keywordAction) && ef == 0)
                {
                    ef = 1;
                    // keywordAction.Invoke();
                    Debug.Log("認識した1");
                    hasRecognized = true;

                    FlagManager.Instance.flags[0] = true;
                    audioSource.PlayOneShot(Cus1); //Two lunch sets , please.

                    //女性アバター発話開始のフラグ
                    FlagManager.Instance.flags[1] = true;
                   

                }

                if (keywords.TryGetValue(args.text, out keywordAction) && FlagManager.Instance.flags[2] == true)
                {
                    Debug.Log("認識した2-1");
                    hasRecognized = true;

                    audioSource.clip = Cus3;
                    audioSource.PlayOneShot(Cus3);//yes
                    audioSource.clip = Cus4;
                    audioSource.PlayOneShot(Cus4);//yes

                    FlagManager.Instance.flags[2] = false;
                    FlagManager.Instance.flags[3] = true;

                }

            }

            //womanの方の認識部分
            if(Cameradirect.flag2 == 1)
            {
                
                System.Action keywordAction;//　keywordActionという処理を行う

                //認識されたキーワードが辞書に含まれている場合に、アクションを呼び出す。

                //womanをみて注文確認をした場合
                if (keywords.TryGetValue(args.text, out keywordAction) && FlagManager.Instance.flags[4] == true)
                {

                    FlagManager.Instance.flags[4] = false;
                    //audioSource.PlayOneShot(Cus3); //after meal

                    //別のお客へ
                    FlagManager.Instance.flags[5] = true;

                }
                
                if (keywords.TryGetValue(args.text, out keywordAction) && FlagManager.Instance.flags[3] == true)
                {

                    Debug.Log("認識した3");
                    hasRecognized = true;
                    audioSource.clip = Cus5;
                    audioSource.PlayOneShot(Cus5); //after meal

                    FlagManager.Instance.flags[3] = false;
                    //
                    FlagManager.Instance.flags[4] = true;

                }

                if (keywords.TryGetValue(args.text, out keywordAction) && FlagManager.Instance.flags[2] == true)
                {
                    Debug.Log("認識した2-2");
                    hasRecognized = true;
                    audioSource.clip = Cus3;
                    audioSource.PlayOneShot(Cus3);//yes
                    audioSource.clip = Cus4;
                    audioSource.PlayOneShot(Cus4);//yes

                    FlagManager.Instance.flags[2] = false;
                    //認識完了、シナリオ更新
                    FlagManager.Instance.flags[3] = true;

                }


            }
        }
    }

}