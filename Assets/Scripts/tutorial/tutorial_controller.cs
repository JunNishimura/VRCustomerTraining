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

public class tutorial_controller : MonoBehaviour
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

    //顧客
    public GameObject table1_customer1;
    public GameObject table3_customer1;
    public GameObject table3_customer2;
    public GameObject table5_customer1;
    public GameObject table5_customer2;
    public GameObject entrance_customer;

    //途中で出現させる空のお皿
    public GameObject plate1;
    public GameObject plate2;
    public GameObject plate3;

    //途中で消す料理
    public GameObject food1;
    public GameObject food2;
    public GameObject food3;

    public GameObject cube;


    Animator animator_table1_customer1;
    Animator animator_table2_customer1;
    Animator animator_table3_customer1;
    Animator animator_table3_customer2;
    Animator animator_table5_customer1;
    Animator animator_table5_customer2;
    Animator animator_entrance_customer;

    //テーブルに近づいたかの判定
    EnterCube entercube1;
    EnterCube entercube2;
    EnterCube entercube3;
    EnterCube entercube4;
    EnterCube entercube5;
    EnterCube entercube6;
    EnterCube enter_cube_entrance;

    //発言認識内容書き出し
    public Dictionary<string, string> speechRecognitionTable = new Dictionary<string, string>();
    public static string speechContent;


    //料理名保存
    //List<GameObject> food = new List<GameObject>();

    //シナリオカウンタ
    private int counter = 0;
    private int dialogue_counter = 0;

    //シナリオ正誤判定
    private bool jobcompleted = false;
    private bool scenariocorrect = false;
    private bool finished = false;

    //エージェント
    private NavMeshAgent agent;
    public Transform target;


    //経過時間を記録
    private double time = 0.0;

    //皿の判定
    Cleanup_Dish cd;

    private string ScenarioID = "";

    ScenarioList scenarioList;

    //シナリオ終了判定
    public static bool Senariofinished = false;

    void Start()
    {
        //各テーブルのcubeへの入出判定を行うスクリプトを取得、フラグ(入：１、出：０)
        entercube1 = table1_cube.GetComponent<EnterCube>();
        entercube2 = table2_cube.GetComponent<EnterCube>();
        entercube3 = table3_cube.GetComponent<EnterCube>();
        entercube4 = table4_cube.GetComponent<EnterCube>();
        entercube5 = table5_cube.GetComponent<EnterCube>();
        entercube6 = table6_cube.GetComponent<EnterCube>();
        enter_cube_entrance = entrance_cube.GetComponent<EnterCube>();


        //各アバターのアニメーションを取得
        animator_table1_customer1 = table1_customer1.GetComponent<Animator>();
        animator_table5_customer1 = table5_customer1.GetComponent<Animator>();
        animator_table5_customer2 = table5_customer2.GetComponent<Animator>();
        animator_entrance_customer = entrance_customer.GetComponent<Animator>();

        cd = cube.GetComponent<Cleanup_Dish>();

        agent = entrance_customer.GetComponent<NavMeshAgent>();

        // 音声認識クラスの起動
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("インターネット未接続: 音声認識エンジンを使わずに起動します。");
            Debug.LogError("音声認識動いていません。");
            speech_rec = null;
        }
        else
        {
            speech_rec = new Speech_Recognition_Manager();
            speech_rec.startRecognition();
            Debug.Log("音声認識を起動します。");
            //speech_rec.stopRecognition();
        }

        scenarioList = Resources.Load("Scenario_Sheet") as ScenarioList;

        //シングルトン：クラス名.Instance.関数()or変数

        //ガイド・ヒントパネルの内容記述
        UIManager.Instance.guide.text = scenarioList.sheets[0].list[0].guide; 
        UIManager.Instance.hint1.text =
            "ヒントパネルです。\n" +
            "・シナリオを完了する際に必要な知識やヒントが示されます。\n" +
            "・全3ページで、Yボタンを押すことで切り替えられます。\n" ;

        UIManager.Instance.hint2.text =
           "・2ページ目です。\n" +
           "料理やお皿は持つことができます。" ;

        UIManager.Instance.hint3.text =
        "操作方法\n" +
        "・右Touchの下のボタン(Aボタン)：発話した直後に押すことでシナリオが進行\n" +
        "・右Touchの上のボタン(Bボタン)：ポーズ\n" +
        "・左Touchの下のボタン(Xボタン)：ガイドパネルの表示非表示の切り替え\n" +
        "・左Touchの上のボタン(Yボタン)：ヒントパネルの表示非表示の切り替え(全3ページ)\n" +
        "・中指にあるボタン(グリップ)：手を料理・皿の方へ向けて持つ\n" +
        "・左スティック：テレポート移動\n" +
        "・右スティック：テレポート移動後のキャラクターの向きを変更する";


        UIManager.Instance.ordertable.text = "5番:バゲットセット、サンドウィッチセット\n";

        //フラグの初期化
        FlagManager.Instance.ResetFlags();



        //テーブル初期化
        for (int i = 1; i < 7; i++)
        {
            speechRecognitionTable.Add($"1_{i}", "");
        }


        AudioManager.Instance.PlayBGM("Special To Me-Slow Edit");
        AudioManager.Instance.AttachBGMSource.loop = true;
        AudioManager.Instance.ChangeVolume(0.2f, 1f, 1f);

    }



    void Update()
    {

        //ポーズ中の処理
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }

        time = Time.time;

        /*
        //音声認識onoff切り替え
        if (EnterCube.enterflag == true)
        {
            speech_rec.startRecognition();
            Debug.Log("音声認識を起動します。");
        }
        else
        {
            speech_rec.stopRecognition();
            Debug.Log("音声認識を停止します。");

        }
        */

        Debug.Log("音声認識終了:------------------------" + Speech_Recognition_Manager.rec_complete);


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

        //table5

        if (entercube5.GetEnter() == 1)
        {
            if (FlagManager.Instance.flags[0] == false && jobcompleted == false)
            {
                GuidePanelUpdate(0, 1);
                //UIManager.Instance.guide.text = scenarioList.sheets[0].list[1].guide;
                jobcompleted = true;
            }
           
            if (CameraDirected(table5_customer1) == true || CameraDirected(table5_customer2) == true)
            {
                if (Speech_Recognition_Manager.rec_complete == true)
                {
                    if(FlagManager.Instance.flags[0] == true)
                    {
                        AUpdate(false);
                        BUpdate(true);
                    }
                    else
                    {
                        AUpdate(true);
                    }
                    
                    if (OVRInput.GetDown(OVRInput.RawButton.A))
                    {
                        // AudioManager.Instance.PlayVoice("tutorial_man");

                        if (FlagManager.Instance.flags[0] == false)
                        {
                            AUpdate(false);
                            BUpdate(true);
                            switch (dialogue_counter)
                            {
                                case 0:
                                    AudioManager.Instance.PlayVoice("1-5");

                                    //UIManager.Instance.guide.text = scenarioList.sheets[0].list[2].guide;
                                    GuidePanelUpdate(0, 2);
                                    dialogue_counter += 1;
                                    Speech_Recognition_Manager.rec_complete = false;
                                    break;

                                case 1:
                                    AudioManager.Instance.PlayVoice("1-6");

                                    //UIManager.Instance.guide.text = scenarioList.sheets[0].list[3].guide;
                                    GuidePanelUpdate(0, 3);
                                    dialogue_counter = 0;
                                    Speech_Recognition_Manager.rec_complete = false;
                                    FlagManager.Instance.flags[0] = true;
                                    jobcompleted = false;
                                    UIManager.Instance.Order_Remove("5番:バゲットセット、サンドウィッチセット\n");
                                    RaiseHands(animator_table1_customer1, true);
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




        //table1---------------------------------
        /*シナリオ：サンドウィッチを持ってくるのが正解
         * 料理配膳
         */

        //テーブルのcubeとアバターを見ているか判定
        if (entercube1.GetEnter() == 1) 
        {
            if (CameraDirected(table1_customer1) == true)
            {
                if (FlagManager.Instance.flags[1] == false && jobcompleted == false)
                {
                   // UIManager.Instance.guide.text = scenarioList.sheets[0].list[4].guide;
                    GuidePanelUpdate(0, 4);
                    jobcompleted = true;
                }

                //音声認識完了、Aボタン入力
                if (Speech_Recognition_Manager.rec_complete == true)
                {
                    if (FlagManager.Instance.flags[1] == true)
                    {
                        AUpdate(false);
                        BUpdate(true);
                    }
                    else
                    {
                        AUpdate(true);
                    }

                    if (OVRInput.GetDown(OVRInput.RawButton.A))
                    {
                        //AudioManager.Instance.PlayVoice("tutorial_woman");
                        RaiseHands(animator_table1_customer1, false);

                        if (FlagManager.Instance.flags[1] == false)
                        {
                            AUpdate(false);
                            BUpdate(true);
                            //会話の進行チェック
                            switch (dialogue_counter)
                            {
                                case 0:
                                    AudioManager.Instance.PlayVoice("t-1");
                                    //UIManager.Instance.guide.text = scenarioList.sheets[0].list[5].guide;
                                    GuidePanelUpdate(0, 5);
                                    dialogue_counter += 1;
                                    Speech_Recognition_Manager.rec_complete = false;
                                    break;

                                case 1:
                                    AudioManager.Instance.PlayVoice("t-2");
                                    //UIManager.Instance.guide.text = scenarioList.sheets[0].list[6].guide;
                                    GuidePanelUpdate(0, 6);
                                    dialogue_counter += 1;
                                    Speech_Recognition_Manager.rec_complete = false;
                                    break;

                                case 2:
                                    AudioManager.Instance.PlayVoice("t-3");
                                    entrance_customer.SetActive(true);
                                    //UIManager.Instance.guide.text = scenarioList.sheets[0].list[7].guide;
                                    GuidePanelUpdate(0, 7);
                                    dialogue_counter = 0;
                                    Speech_Recognition_Manager.rec_complete = false;
                                    FlagManager.Instance.flags[1] = true;
                                    jobcompleted = false;
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
        if (enter_cube_entrance.GetEnter() == 1) 
        {
            if (CameraDirected(entrance_customer) == true)
            {
                if (FlagManager.Instance.flags[2] == false && jobcompleted == false)
                {
                    //UIManager.Instance.guide.text = scenarioList.sheets[0].list[4].guide;
                    //jobcompleted = true;
                }

                //音声認識完了、Aボタン入力
                if (Speech_Recognition_Manager.rec_complete == true)
                {
                    if (FlagManager.Instance.flags[2] == true)
                    {
                        AUpdate(false);
                        BUpdate(true);
                    }
                    else
                    {
                        AUpdate(true);
                    }
                    if (OVRInput.GetDown(OVRInput.RawButton.A))
                    {
                        if (FlagManager.Instance.flags[2] == false)
                        {
                            AUpdate(false);
                            BUpdate(true);
                            //会話の進行チェック
                            switch (dialogue_counter)
                            {
                                case 0:
                                    AudioManager.Instance.PlayVoice("t-4");
                                    //UIManager.Instance.guide.text = scenarioList.sheets[0].list[8].guide;
                                    GuidePanelUpdate(0, 8);
                                    dialogue_counter += 1;
                                    Speech_Recognition_Manager.rec_complete = false;
                                    break;

                                case 1:
                                    AudioManager.Instance.PlayVoice("t-5");
                                    //UIManager.Instance.guide.text = scenarioList.sheets[0].list[9].guide;
                                    GuidePanelUpdate(0, 9);
                                    dialogue_counter = 0;
                                    Speech_Recognition_Manager.rec_complete = false;
                                    FlagManager.Instance.flags[2] = true;
                                    //jobcompleted = false;
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
        if (FlagManager.Instance.flags[2] == true)
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
                entrance_customer.transform.rotation = Quaternion.Slerp(entrance_customer.transform.rotation, Quaternion.Euler(0f, -90f, 0f), 0.2f);
            }

            if (Vector3.Distance(target.position, entrance_customer.transform.position) < 0.51)
            {
                animator_entrance_customer.SetBool("sitting", true);
            }
        }
        

        //table3
        /*
       
        */


        if (entercube3.GetEnter() == 1)
        {
            if (CameraDirected(table3_customer1) == true || CameraDirected(table3_customer2) == true)
            {
                if (Speech_Recognition_Manager.rec_complete == true)
                {
                    if (FlagManager.Instance.flags[3] == true)
                    {
                        AUpdate(false);
                        BUpdate(true);
                    }
                    else
                    {
                        AUpdate(true);
                    }

                    if (OVRInput.GetDown(OVRInput.RawButton.A))
                    {
                        if (FlagManager.Instance.flags[3] == false)
                        {
                            AUpdate(false);
                            BUpdate(true);
                            switch (dialogue_counter)
                            {
                                case 0:

                                    AudioManager.Instance.PlayVoice("1-12");
                                    //UIManager.Instance.guide.text = scenarioList.sheets[0].list[10].guide;
                                    GuidePanelUpdate(0, 10);
                                    Speech_Recognition_Manager.rec_complete = false;

                                    FlagManager.Instance.flags[3] = true;

                                    break;

                                case 1:
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

       
        if (cd.GetPlateEnter() == 2)
        {
            FlagManager.Instance.flags[4] = true;
        }

        Debug.Log("接客することはありません");

        //接客順序が正しいかの判定
        //SenarioOrderJudge();
        ScenarioCompletedJudge();
        
        //接客終了した場合の処理
        if (FlagManager.Instance.flags[7] == true && finished == false)
        {
            UIManager.Instance.resultPanel.SetActive(true);
            UIManager.Instance.result.text = "プレイ時間：" + (int)time / 60 + "分" + (int)time % 60 + "秒\n";
            
            
            UIManager.Instance.result.text += "チュートリアル終了です。\n";
            

            UIManager.Scenariofinished = false;
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

    /*
    //終了時に行う処理
    private void OnApplicationQuit()
    {
        //発言内容書き出し
        Encoding enc = Encoding.GetEncoding("Shift_JIS");
        StreamWriter speechFile = new StreamWriter("C:/Users/星野/Documents/experiment/SpeechRecognition1.csv", false, enc);
        speechFile.WriteLine("シナリオID, 認識内容");
        foreach (KeyValuePair<string, string> item in speechRecognitionTable)
        {
            speechFile.WriteLine($"{item.Key}, {item.Value}");
        }
        speechFile.Close();
    }
    */

    //接客順序が正しいか・接客がすべて完了しているかの判定
    private void SenarioOrderJudge()
    {
        //接客順序が正しいかの判定
        for (int i = 5; i >= 0; i--)
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

        if (FlagManager.Instance.flags[7] == false)
        {
            for (int i = 0; i <= 4; i++)
            {
                if (FlagManager.Instance.flags[i] == false)
                {
                    Debug.Log("接客がすべて完了していません");
                    break;

                }

                if (i == 4 && FlagManager.Instance.flags[i] == true)
                {
                    FlagManager.Instance.flags[7] = true;
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
        if(b == true)
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
