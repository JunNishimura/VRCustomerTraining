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
using System.IO;
using System.Text;

public class Scenario2_controller : MonoBehaviour
{
    private bool hasRecognized;

    //音声認識
    public Speech_Recognition_Manager speech_rec;

    //頭の位置
    //public GameObject CameraPos;

    //カメラの取得
    public new GameObject camera;
    Vector3 cameradirect;

    //各テーブルに配置されたプレイヤーが入出を判定するcube
    public GameObject table1_cube;
    public GameObject table2_cube;
    public GameObject table3_cube;
    public GameObject table4_cube;
    public GameObject table5_cube;
    public GameObject table6_cube;
    public GameObject entrance_cube;

    //テーブルの顧客
    public GameObject table1_customer1;
    public GameObject table2_customer1;
    public GameObject table2_customer2;
    public GameObject table3_customer1;
    public GameObject table3_customer2;
    public GameObject table4_customer1;
    public GameObject table4_customer2;
    public GameObject table5_customer1;
    public GameObject entrance_customer;


    //途中で出現させる空のお皿
    public GameObject plate1;
    public GameObject plate2;
    public GameObject plate3;

    //途中で消す料理
    public GameObject food1;
    public GameObject food2;
    public GameObject food3;

    //皿をカウンターへ帰したときの判定
    public GameObject cube;


    Animator animator_table1_customer1;
    Animator animator_table2_customer1;
    Animator animator_table2_customer2;
    Animator animator_table3_customer1;
    Animator animator_table3_customer2;
    Animator animator_table4_customer1;
    Animator animator_table4_customer2;
    Animator animator_table5_customer1;
    Animator animator_entrance_customer;


    //テーブルに近づいたかの判定
    EnterCube entercube1;
    EnterCube entercube2;
    EnterCube entercube3;
    EnterCube entercube4;
    EnterCube entercube5;
    EnterCube entercube6;
    EnterCube entercube_entrance;

    //皿の判定
    Cleanup_Dish cd;

    //エージェント
    private NavMeshAgent agent;
    public Transform target;

    //発言認識内容書き出し
    public Dictionary<string, string> speechRecognitionTable = new Dictionary<string, string>();
    public static string speechContent;

    //時間書き出し
    List<double> timelist = new List<double>();
    List<bool> t = new List<bool>() { false };

    private double t1 = 0.0;
    private double t2 = 0.0;

    //料理名保存
    //List<GameObject> food = new List<GameObject>();

    //シナリオカウンタ
    private int counter = 0;
    private int dialogue_counter = 0;

    //シナリオ正誤判定
    private bool jobcompleted = false;
    private bool scenariocorrect = false;
    private bool finished = false;

    //経過時間を記録
    private double time = 0.0;

    private string ScenarioID = "";

    //シナリオ終了判定
    //public static bool Senariofinished = false;


    ScenarioList scenarioList;


    void Start()
    {
        //各テーブルのcubeへの入出判定を行うスクリプトを取得、フラグ(入：１、出：０)
        entercube1 = table1_cube.GetComponent<EnterCube>();
        entercube2 = table2_cube.GetComponent<EnterCube>();
        entercube3 = table3_cube.GetComponent<EnterCube>();
        entercube4 = table4_cube.GetComponent<EnterCube>();
        entercube5 = table5_cube.GetComponent<EnterCube>();
        entercube6 = table6_cube.GetComponent<EnterCube>();
        entercube_entrance = entrance_cube.GetComponent<EnterCube>();

        //各アバターのアニメーションを取得
        animator_table1_customer1 = table1_customer1.GetComponent<Animator>();
        animator_table2_customer1 = table2_customer1.GetComponent<Animator>();
        animator_table2_customer2 = table2_customer2.GetComponent<Animator>();
        animator_table3_customer1 = table3_customer1.GetComponent<Animator>();
        animator_table3_customer2 = table3_customer2.GetComponent<Animator>();
        animator_table4_customer1 = table4_customer1.GetComponent<Animator>();
        animator_table4_customer2 = table4_customer2.GetComponent<Animator>();
        animator_table5_customer1 = table5_customer1.GetComponent<Animator>();
        animator_entrance_customer = entrance_customer.GetComponent<Animator>();

        agent = entrance_customer.GetComponent<NavMeshAgent>();
        cd = cube.GetComponent<Cleanup_Dish>();

        // 音声認識クラスの起動
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("インターネット未接続: 音声認識エンジンを使わずに起動します。");
            speech_rec = null;
        }
        else
        {
            speech_rec = new Speech_Recognition_Manager();
            speech_rec.startRecognition();
            Debug.Log("音声認識を起動します。");
        }

        scenarioList = Resources.Load("Scenario_Sheet") as ScenarioList;

        //シングルトン：クラス名.Instance.関数()or変数

        // Debug.Log("a--------------------------------------" + scenarioList.sheets[1].list[0].guide);
        //Debug.Log("a--------------------------------------" + scenarioList.sheets.Count);

        //ガイド・ヒントパネルの内容記述
        UIManager.Instance.guide.text = scenarioList.sheets[2].list[0].guide;
        UIManager.Instance.hint1.text =
            "接客での注意点\n" +
            "・接客順序は、早く処理できる業務から行いましょう\n" +
            "・複数のお客様から呼び出された場合に、先に呼ばれたお客様から対応しましょう\n" +
            "・配膳はオーダー表の順に配膳するようにしましょう\n" +
            "・済んでいる皿を見かけたら声をかけ積極的に片付けましょう\n" +
            "・デザートがある場合、食事済みを確認したらすぐに配膳しましょう\n";

        UIManager.Instance.hint2.text =
           "・注文した料理はお客さんの前に置くようにしましょう\n" +
           "・済んだ皿はカウンターへ戻しましょう\n" +
           "・持ってきた料理名をお客さんに伝えましょう\n" +
           "・お客様を待たせてしまう場合には、「少々お待ちください」と声かけを行いましょう\n" +
           "・注文を取り終えた際には、注文を復唱してお客様に確認しましょう。\n" +
           "・配膳後には注文が以上であるかを確認しましょう\n";

        UIManager.Instance.hint3.text =
       "操作方法\n" +
        "・右Touchの下のボタン(Aボタン)：発話した直後に押すことでシナリオが進行\n" +
        "・右Touchの上のボタン(Bボタン)：ポーズ\n" +
        "・左Touchの下のボタン(Xボタン)：ガイドパネルの表示非表示の切り替え\n" +
        "・左Touchの上のボタン(Yボタン)：ヒントパネルの表示非表示の切り替え(全3ページ)\n" +
        "・中指にあるボタン(グリップ)：手を料理・皿の方へ向けて持つ\n" +
        "・左スティック：テレポート移動\n" +
        "・右スティック：テレポート移動後のキャラクターの向きを変更する";


        UIManager.Instance.ordertable.text = "";

        //フラグの初期化
        FlagManager.Instance.ResetFlags();


        //テーブル初期化
        for (int i = 1; i < 5; i++)
        {
            speechRecognitionTable.Add($"2_{i}", "");
        }

        AudioManager.Instance.PlayBGM("Special To Me-Slow Edit");
        AudioManager.Instance.AttachBGMSource.loop = true;
        AudioManager.Instance.ChangeVolume(0.2f, 1f, 1f);

        //4番挙手
        RaiseHands(animator_table4_customer2, true);

    }



    void Update()
    {

        //ポーズ中の処理
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }

        time = Time.time;

        Debug.Log("音声認識終了:------------------------" + Speech_Recognition_Manager.rec_complete);
        // Debug.Log("経過時間：" + (int)time / 60 + "分" + (int)time % 60 + "秒");
        //Debug.Log("音声認識終了:------------------------" + entercube2.GetObjectName().[0]);

        // Debug.Log("音声認識の状態：" + speech_rec.getSystemStatus());
        // Debug.Log("音声認識：" + speech_rec);
        
        Debug.Log("音声認識-------------------------：" + scenarioList.sheets[2].list[0].guide);
        //デバック用
        if (Input.GetKeyDown(KeyCode.J))
        {
            UIManager.Instance.recognition.text = "音声認識完了";
            Speech_Recognition_Manager.rec_complete = true;
        }



        /*------アバターとの会話判定------------------
         * テーブルごとに処理を行う
         * アバターとの会話するためには、
         * 「アバターを見ること」と「テーブルにあるcubeに入ること」が必要
         * それぞれ、Cameradirectedとentercube{n}.GetEnter()で確認
         * 会話イベントは音声認識とコントローラー（oculus）のAボタンで進行
         * 会話進行の内部処理は「dialogue_counter」で処理
         * 今回のシナリオは訓練者自ら話しかけることで進行する仕組み
         * -----------------------------------------*/

        Debug.Log("a--------------------------------------" + scenarioList.sheets[2].list[14].guide);

        //table4-----------------------------------------------------------

        if (entercube4.GetEnter() == 1)
        {
            if (CameraDirected(table4_customer1) == true || CameraDirected(table4_customer2) == true)
            {
                if (Speech_Recognition_Manager.rec_complete == true) 
                {
                    //Aボタン表示：
                    if (FlagManager.Instance.flags[0] == false)
                    {
                        AUpdate(true);
                    }
                    else
                    {
                        //Aボタン非表示・Bボタン表示
                        AUpdate(false);
                        BUpdate(true);
                       
                    }
                    if (OVRInput.GetDown(OVRInput.RawButton.A))
                    {
                        if (FlagManager.Instance.flags[0] == false)
                        {
                            AUpdate(false);
                            BUpdate(true);
                            switch (dialogue_counter)
                            {
                                case 0:
                                    RaiseHands(animator_table4_customer2, false);
                                    AudioManager.Instance.PlayVoice("2-1");
                                    GuidePanelUpdate(2, 1);
                                    dialogue_counter += 1;
                                    Speech_Recognition_Manager.rec_complete = false;

                                    //1番挙手
                                    RaiseHands(animator_table1_customer1, true);
                                    t1 = time;
                                    break;

                                case 1:
                                    AudioManager.Instance.PlayVoice("2-2");
                                    GuidePanelUpdate(2,2);
                                    dialogue_counter += 1;
                                    Speech_Recognition_Manager.rec_complete = false;

                                    break;



                                case 2:
                                    AudioManager.Instance.PlayVoice("2-3");
                                    FlagManager.Instance.flags[0] = true;
                                    counter += 1;
                                    dialogue_counter = 0;
                                    Speech_Recognition_Manager.rec_complete = false;
                                    GuidePanelUpdate(2,3) ;

                                    //入店客出現
                                    entrance_customer.SetActive(true);



                                    //認識内容の保存、リセット
                                    ScenarioID = "2_1";
                                    speechRecognitionTable[ScenarioID] = speechContent;
                                    if (speech_rec != null) speech_rec.resetDictationResult();

                                    break;
                            }
                        }
                    }
                }
                
            }
            else
            {
                AUpdate(false);
            }
        }


        //entrance
        //エントランスのcubeとアバターを見ているか判定
        if (entercube_entrance.GetEnter() == 1)
        {
            
            if (FlagManager.Instance.flags[0] == true && FlagManager.Instance.flags[1] == false && jobcompleted == false)
            {
                GuidePanelUpdate(2,15);
                jobcompleted = true;
            }
            
            if(CameraDirected(entrance_customer) == true)
            {
                //音声認識完了、Aボタン入力
                if (Speech_Recognition_Manager.rec_complete == true) 
                {
                    if (FlagManager.Instance.flags[1] ==　false)
                    {
                        AUpdate(true);
                    }
                    else
                    {
                        AUpdate(false);
                        BUpdate(true);
                       
                    }
                    if (OVRInput.GetDown(OVRInput.RawButton.A))
                    {
                        if (FlagManager.Instance.flags[1] == false)
                        {
                            AUpdate(false);
                            BUpdate(true);
                            //会話の進行チェック
                            switch (dialogue_counter)
                            {
                                case 0:
                                    AudioManager.Instance.PlayVoice("2-4");
                                    GuidePanelUpdate(2,5);
                                    dialogue_counter += 1;
                                    Speech_Recognition_Manager.rec_complete = false;

                                    //customer raise hands
                                    RaiseHands(animator_table5_customer1, true);
                                    t2 = time;
                                    break;

                                case 1:
                                    AudioManager.Instance.PlayVoice("2-5");
                                    GuidePanelUpdate(2,6) ;
                                    dialogue_counter = 0;
                                    counter++;
                                    Speech_Recognition_Manager.rec_complete = false;
                                    FlagManager.Instance.flags[1] = true;
                                    jobcompleted = false;


                                    //認識内容の保存、リセット
                                    ScenarioID = "2_2";
                                    speechRecognitionTable[ScenarioID] = speechContent;
                                    if (speech_rec != null) speech_rec.resetDictationResult();

                                    break;

                            }
                        }


                    }
                }
                
            }
            else
            {
                AUpdate(false);
            }

        }

        //お客様の移動
        if (FlagManager.Instance.flags[1] == true)
        {
            if (target != null)
            {

                agent.SetDestination(target.position);
                animator_entrance_customer.SetBool("walk", true);
                entrance_customer.GetComponent<LookAtController>().enabled = false;

            }
            else
            {
                Debug.LogWarning("targetがありません!!!");
            }

            if (Vector3.Distance(target.position, entrance_customer.transform.position) < 1)
            {
                entrance_customer.transform.rotation = Quaternion.Slerp(entrance_customer.transform.rotation, Quaternion.Euler(0f, 0f, 0f), 0.2f);
            }

            if (Vector3.Distance(target.position, entrance_customer.transform.position) < 0.51)
            {
                animator_entrance_customer.SetBool("sitting", true);
            }
        }

        //table1---------------------------------
        /*シナリオ：サンドウィッチを持ってくるのが正解
         * 
         */

        //テーブルのcubeとアバターを見ているか判定
        if (entercube1.GetEnter() == 1)
        {
            if (FlagManager.Instance.flags[2] == false && FlagManager.Instance.flags[1] == false && jobcompleted == false)
            {
                //お待たせする声掛け判定
                GuidePanelUpdate(2,14);
                jobcompleted = true;
            }

            if (CameraDirected(table1_customer1) == true)
            {
                //音声認識完了、Aボタン入力
                if (Speech_Recognition_Manager.rec_complete == true) 
                {
                    if (FlagManager.Instance.flags[2] == false && FlagManager.Instance.flags[1] == false && FlagManager.Instance.flags[6] == false)
                    {
                        AUpdate(true);
                    }
                    else if(FlagManager.Instance.flags[2] == false && FlagManager.Instance.flags[1] == true)
                    {
                        AUpdate(true);
                    }
                    else
                    {
                        AUpdate(false);
                        BUpdate(true);
                      
                    }

                    if (OVRInput.GetDown(OVRInput.RawButton.A))
                    {

                        if (FlagManager.Instance.flags[2] == false && FlagManager.Instance.flags[1] == false)
                        {
                            AUpdate(false);
                            BUpdate(true);
                            //お待たせする声掛け判定
                            RaiseHands(animator_table1_customer1, false);
                            GuidePanelUpdate(2,15);
                            AudioManager.Instance.PlayVoice("2-8");
                            FlagManager.Instance.flags[6] = true;
                        }
                        else if (FlagManager.Instance.flags[2] == false)
                        {
                            AUpdate(false);
                            BUpdate(true);
                            //会話の進行チェック
                            switch (dialogue_counter)
                            {
                                case 0:

                                    //costomer down hands
                                    RaiseHands(animator_table1_customer1, false);
                                    t1 = time - t1;

                                    //顧客ボイス再生
                                    AudioManager.Instance.PlayVoice("2-6");
                                    GuidePanelUpdate(2,7) ;
                                    dialogue_counter += 1;

                                    //音声認識フラグ
                                    Speech_Recognition_Manager.rec_complete = false;


                                    break;

                                case 1:
                                    AudioManager.Instance.PlayVoice("2-7");
                                    GuidePanelUpdate(2,8) ;
                                    Speech_Recognition_Manager.rec_complete = false;

                                    dialogue_counter += 1;


                                    break;

                                case 2:
                                    AudioManager.Instance.PlayVoice("2-8");
                                    GuidePanelUpdate(2,9) ;
                                    dialogue_counter = 0;
                                    counter += 1;
                                    Speech_Recognition_Manager.rec_complete = false;

                                    jobcompleted = false;

                                    //フラグ更新
                                    FlagManager.Instance.flags[2] = true;


                                    //認識内容の保存、リセット
                                    ScenarioID = "2_3";
                                    speechRecognitionTable[ScenarioID] = speechContent;
                                    if (speech_rec != null) speech_rec.resetDictationResult();

                                    break;

                            }

                        }
                        else
                        {
                            Debug.Log("接客することはありません");
                        }

                    }
                }
               
            }
            else
            {
                AUpdate(false);
            }

        }

        //table5
        /*
         
         */
        if (entercube5.GetEnter() == 1)
        {
            if (FlagManager.Instance.flags[1] == true && FlagManager.Instance.flags[2] == false && jobcompleted == false)
            {
                //お待たせする声掛け判定
                GuidePanelUpdate(2,16);
                jobcompleted = true;
            }

            if (CameraDirected(table5_customer1) == true)
            {
                if (Speech_Recognition_Manager.rec_complete == true) 
                {
                    if (FlagManager.Instance.flags[3] == false && FlagManager.Instance.flags[2] == false && FlagManager.Instance.flags[7] == false)
                    {
                        AUpdate(true);
                    }
                    else if (FlagManager.Instance.flags[3] == false && FlagManager.Instance.flags[2] == true)
                    {
                        AUpdate(true);
                    }
                    else
                    {
                        AUpdate(false);
                        BUpdate(true);

                    }
                    if (OVRInput.GetDown(OVRInput.RawButton.A))
                    {
                        if (FlagManager.Instance.flags[3] == false && FlagManager.Instance.flags[2] == false)
                        {
                            AUpdate(false);
                            BUpdate(true);
                            RaiseHands(animator_table5_customer1, false);
                            AudioManager.Instance.PlayVoice("2-11");
                            FlagManager.Instance.flags[7] = true;
                            GuidePanelUpdate(2,17) ;
                        }
                        else if (FlagManager.Instance.flags[3] == false)
                        {
                            AUpdate(false);
                            BUpdate(true);
                            switch (dialogue_counter)
                            {
                                case 0:

                                    //costomer down hands
                                    RaiseHands(animator_table5_customer1, false);
                                    t2 = time - t2;

                                    AudioManager.Instance.PlayVoice("2-9");
                                    GuidePanelUpdate(2,10);
                                    dialogue_counter += 1;
                                    Speech_Recognition_Manager.rec_complete = false;

                                    break;

                                case 1:
                                    AudioManager.Instance.PlayVoice("2-10");
                                    GuidePanelUpdate(2,11);
                                    Speech_Recognition_Manager.rec_complete = false;

                                    dialogue_counter += 1;

                                    break;

                                case 2:
                                    FlagManager.Instance.flags[3] = true;
                                    AudioManager.Instance.PlayVoice("2-11");
                                    GuidePanelUpdate(2,12) ;
                                    dialogue_counter = 0;
                                    counter += 1;
                                    Speech_Recognition_Manager.rec_complete = false;


                                    //認識内容の保存、リセット
                                    ScenarioID = "2_4";
                                    speechRecognitionTable[ScenarioID] = speechContent;
                                    if (speech_rec != null) speech_rec.resetDictationResult();


                                    break;

                            }


                        }
                        else
                        {
                            Debug.Log("接客する必要がありません");
                        }


                    }
                }
               
            }
            else
            {
                AUpdate(false);
            }
        }

        //table2-----------------------------------------------------------
        /*
         
        */
        if (entercube2.GetEnter() == 1)
        {
            if (CameraDirected(table2_customer1) || CameraDirected(table2_customer2) == true)
            {
                if (Speech_Recognition_Manager.rec_complete == true && OVRInput.GetDown(OVRInput.RawButton.A))
                {
                    // AudioManager.Instance.PlayVoice("2-11");
                }
            }
        }
            

        //table3
        /*
        
        */

        if (entercube3.GetEnter() == 1)
        {
            if (CameraDirected(table3_customer1) == true || CameraDirected(table3_customer2) == true)
            {
                if (Speech_Recognition_Manager.rec_complete == true && OVRInput.GetDown(OVRInput.RawButton.A))
                {
                    // AudioManager.Instance.PlayVoice("2-11");
                }
            }
        }

 

        //接客順序が正しいかの判定
        SenarioOrderJudge();
        ScenarioCompletedJudge();

        //接客完了時間の記録
        recordtime();

        //接客終了した場合の処理
        if (FlagManager.Instance.flags[4] == true && finished == false)
        {
            //GuidePanelUpdate(2,12);
            UIManager.Instance.resultPanel.SetActive(true);
            UIManager.Instance.evaluationPanel.SetActive(true);
            UIManager.Instance.recognitionPanel.SetActive(false);
            UIManager.Instance.APanel.SetActive(false);
            UIManager.Instance.BPanel.SetActive(false);
            UIManager.Instance.guidePanel.SetActive(false);
            
            UIManager.Instance.result.text = "接客タイム：\n";
            for (int i = 0; i < timelist.Count; i++)
            {
                UIManager.Instance.result.text +=  (i + 1) + "番目終了タイム：" + (int)timelist[i] / 60 + "分" + (int)timelist[i] % 60 + "秒     ";
                if (i % 2 != 0)
                {
                    UIManager.Instance.result.text += "\n";
                }
            }
            UIManager.Instance.result.text += "1番のお客様を待たせた時間：" + (int)t1 / 60 + "分" + (int)t1 % 60 + "秒\n";
            UIManager.Instance.result.text += "5番のお客様を待たせた時間：" + (int)t2 / 60 + "分" + (int)t2 % 60 + "秒\n";

            if (FlagManager.Instance.flags[10] == true)
            {
                UIManager.Instance.evaluation.color = new Color(1.0f, 0.27f, 0.27f, 1.0f);
                UIManager.Instance.evaluation.text += "接客順序を間違えています。\n" +
                    "業務は素早くこなせるものからこなし、呼び出しは呼び出された順番にこなしましょう。\n";
            }

            if (FlagManager.Instance.flags[6] == false && FlagManager.Instance.flags[7] == false)
            {
                UIManager.Instance.evaluation.color = new Color(1.0f, 0.27f, 0.27f, 1.0f);
                UIManager.Instance.evaluation.text += "1番テーブルと５番テーブルへの声掛けを忘れています。\n" +
                   "お客さんを順番待ちさせてしまう場合には、お待たせすることを伝えましょう。\n";
            }
            else if (FlagManager.Instance.flags[6] == false)
            {
                UIManager.Instance.evaluation.color = new Color(1.0f, 0.27f, 0.27f, 1.0f);
                UIManager.Instance.evaluation.text += "1番テーブルへの声掛けを忘れています。\n" +
                   "お客さんを順番待ちさせてしまう場合には、お待たせすることを伝えましょう。\n";
            }          
            else if (FlagManager.Instance.flags[7] == false)
            {
                UIManager.Instance.evaluation.color = new Color(1.0f, 0.27f, 0.27f, 1.0f);
                UIManager.Instance.evaluation.text += "5番テーブルへの声掛けを忘れています。\n" +
                   "お客さんを順番待ちさせてしまう場合には、お待たせすることを伝えましょう。\n";
            }
            
            if (FlagManager.Instance.flags[10] == false &&
                FlagManager.Instance.flags[6] == true &&
                FlagManager.Instance.flags[7] == true)
            {
                UIManager.Instance.evaluation.color = Color.green;
                UIManager.Instance.evaluation.text += "正確に接客ができました。\n";
            }

            //UIManager.Scenariofinished = false;
            finished = true;
        }


        // 動作確認用: エスケープが押されたら音声認識のON/OFFを切り替える
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("インターネット未接続: 音声認識エンジンを使えません。");
                speech_rec = null;
            }
            else if (speech_rec == null)
            {
                Debug.Log("音声認識を起動します。+");
                speech_rec = new Speech_Recognition_Manager();
                speech_rec.startRecognition();
            }
            //

            else if (speech_rec.dispose == true)
            {
                Debug.Log("音声認識を起動します。(ver.dis)");
                speech_rec = new Speech_Recognition_Manager();
                speech_rec.startRecognition();
                //speech_rec.dispose = false;
            }

            else
            {
                if (speech_rec.getSystemStatus())
                {
                    Debug.Log("音声認識を停止します。");
                    speech_rec.stopRecognition();
                }
                else
                {
                    Debug.Log("音声認識を起動します。");
                    speech_rec.startRecognition();
                }
            }
        }

    }

    
    //終了時に行う処理
    private void OnApplicationQuit()
    {
        //発言内容書き出し
        Encoding enc = Encoding.GetEncoding("Shift_JIS");
        StreamWriter speechFile = new StreamWriter("D:/Documents/experiment/SpeechRecognition2.csv", true, enc);
        speechFile.WriteLine("シナリオID, 認識内容");
        foreach (KeyValuePair<string, string> item in speechRecognitionTable)
        {
            speechFile.WriteLine($"{item.Key}, {item.Value}");
        }
        speechFile.Close();
    }
    

    //接客順序が正しいかの判定
    private void SenarioOrderJudge()
    {
        //接客順序が正しいかの判定
        for (int i = 3; i >= 0; i--)
        {
            if (FlagManager.Instance.flags[10] == false)
            {
                if (FlagManager.Instance.flags[i] == true)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (FlagManager.Instance.flags[j] == false)
                        {
                            FlagManager.Instance.flags[10] = true;
                            Debug.Log("接客順序を間違えています");
                            break;
                        }

                    }
                }
            }

        }


    }

    //接客がすべて完了しているかの判定
    private void ScenarioCompletedJudge()
    {

        if (FlagManager.Instance.flags[4] == false)
        {
            for (int i = 0; i <= 3; i++)
            {
                if (FlagManager.Instance.flags[i] == false)
                {
                    Debug.Log("接客がすべて完了していません");
                    break;

                }

                if (i == 3 && FlagManager.Instance.flags[i] == true)
                {
                    FlagManager.Instance.flags[4] = true;
                }

            }

        }

    }


    //カメラの向きの判定
    private bool CameraDirected(GameObject target)
    {
        float angle;

        //カメラの方向を取得
        cameradirect = camera.transform.forward;

        Vector3 aim = target.transform.position - camera.transform.position;　　//ターゲットまでの方向を取得
        aim.y = aim.y + 1.50f;  //キャラの顔への方向調整

        angle = Vector3.Angle(cameradirect, aim);

        if (0 < angle && angle < 30)
        {
            Debug.Log("カメラ角度OK ターゲットを見ています：" + target.name);
            return true;
        }
        else
        {
            return false;
        }
    }

    //顧客アバターの挙手させる関数
    private void RaiseHands(Animator animator, bool up)
    {
        if (up == true)
        {
            animator.SetBool("updown", true);
        }
        else
        {
            animator.SetBool("updown", false);
        }

    }


    private void recordtime()
    {

        if (t.Count - timelist.Count < 1)
        {
            t.Add(false);
        }


        for (int i = 0; i < t.Count; i++)
        {

            if (FlagManager.Instance.flags[i] == true && t[i] == false)
            {
                timelist.Add(time);
                t[i] = true;

            }

        }
    }

    private void AUpdate(bool a)
    {
        if (a == true)
        {
            UIManager.Instance.a.text = "A";


        }
        else
        {
            UIManager.Instance.a.text = "";

        }

    }

    private void BUpdate(bool b)
    {
        if (b == true)
        {
            UIManager.Instance.b.text = "B";


        }
        else
        {
            UIManager.Instance.b.text = "";

        }


    }

    //ガイドパネルの更新の際に、SEをつける際に使用
    private void GuidePanelUpdate(int a, int b)
    {
        //UIManager.Instance.guidePanel.color = 
        AudioManager.Instance.PlaySE("button");
        UIManager.Instance.guide.text = scenarioList.sheets[a].list[b].guide;
    }

}
