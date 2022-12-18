using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestScenario2Controller : MonoBehaviour
{
    public GameObject[] _foods; //  "サンドウィッチセット", "ピザ", "パンケーキ", "ハンバーガーセット", "チョコレートケーキ", "バゲットセット"の順番で
    public GameObject[] _emptyPlates; // 空き皿。テーブル1から6にかけて格納
    public GameObject[] _avatars; // アバター。テーブル1から6にかけて格納
    public GameObject[] _chairs; // 椅子1-6を順番に格納
    public EnterCube[] _enterCubes;
    public EnterCube _entranceEnterCube; // エントランスのenterCube
    public GameObject _entranceAvatar; // エントランスのアバター

    [SerializeField] CleanPlateJudge _cleanPlageJudge;
    [SerializeField] GameObject _playerEye; // playerの目

    Speech_Recognition_Manager _speechRecManager;
    Animator[] _animators = new Animator[6]; // animatorをアバター1から6にかけて順番に格納
    Animator _entranceAvatarAnimator; // 入店客のanimator
    List<double> _taskFinishedTimeList; // 各タスクの終了時間
    List<string> _taskFinishedList; // タスクの終了順序
    List<int> _dialogueProgressList; // 各タスクの会話の進行具合を格納したリスト // 0: 5番テーブル, 1: 1番テーブル, 2: 3番テーブル, 3: 2番テーブル, 4: 6番テーブル 5: 4番テーブル
    NavMeshAgent _navMeshAgent;
    double _elapsedTime; // 経過時間

    // シナリオ //
    // 5番テーブル入店　1番テーブル片付け
    // 3番テーブル注文　2番テーブル配膳
    // 6番テーブルクレーム応対　4番テーブル片付け

    void Start()
    {
        BGMActivate();

        // _animatorsの初期化
        for (int i = 0; i < _avatars.Length; i++)
        {
            _animators[i] = _avatars[i].GetComponent<Animator>();
        }
        _entranceAvatarAnimator = _entranceAvatar.GetComponent<Animator>();

        // ガイドパネルの初期化
        UIManager.Instance.guide.text = "";

        // 入店客の表示、1番テーブル空き皿表示
        _entranceAvatar.SetActive(true);
        _navMeshAgent = _entranceAvatar.GetComponent<NavMeshAgent>();
        _emptyPlates[0].SetActive(true);

        // ヒントパネルの初期化
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

        // オーダーテーブルの初期化
        UIManager.Instance.ordertable.text = "";

        // フラグマネージャーの初期化
        FlagManager.Instance.ResetFlags();

        // 音声認識クラスの起動
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            _speechRecManager = null;
        }
        else
        {
            _speechRecManager = new Speech_Recognition_Manager();
            _speechRecManager.startRecognition();
        }

        // 接客終了時間を格納するリストの初期化
        _taskFinishedTimeList = new List<double>();

        // 接客終了順序を格納するリスト
        _taskFinishedList = new List<string>();

        // 会話進行リストの初期化
        InitializeDialogueProgressList();
    }

    void Update()
    {
        // 経過時間の更新
        _elapsedTime = Time.time;

        // 入店客の対応（行先5番テーブル）
        if (_entranceEnterCube.GetEnter() == 1 && IsPlayerLookAtCustomer(_entranceAvatar))
        {
            // 音声認識完了のチェック
            if (Speech_Recognition_Manager.rec_complete)
            {
                if (!FlagManager.Instance.flags[0])
                {
                    ABbuttonUpdate(true); // Aボタン表示
                    if (OVRInput.GetDown(OVRInput.RawButton.A))
                    {
                        ABbuttonUpdate(false); // Bボタン表示
                        switch (_dialogueProgressList[0])
                        {
                            case 0:
                                AudioManager.Instance.PlayVoice("2-4"); // 音声「３名です。後から二人きます」

                                _dialogueProgressList[0] += 1;

                                break;
                            case 1:
                                AudioManager.Instance.PlayVoice("yes_man"); // 音声「はい」
                                FlagManager.Instance.flags[0] = true;

                                // 終了時間の格納
                                _taskFinishedTimeList.Add(_elapsedTime);

                                // 終了順序の格納
                                _taskFinishedList.Add("5番テーブル入店");

                                // 1番テーブル注文が終わっていたら次のタスク開始
                                if (FlagManager.Instance.flags[1])
                                {
                                    // オーダーパネルの更新
                                    UIManager.Instance.ordertable.text += "2番：ハンバーガーセット\n";
                                    // 「ピザ」の表示
                                    _foods[3].SetActive(true);

                                    // ３番テーブル挙手
                                    _animators[2].SetBool("updown", true);
                                }

                                break;
                            default:
                                break;
                        }
                        Speech_Recognition_Manager.rec_complete = false;
                    }
                }
            }
        }
        // 入店客の移動
        if (FlagManager.Instance.flags[0])
        {
            int tableNum = 4;
            _navMeshAgent.SetDestination(_chairs[tableNum].transform.position);
            _entranceAvatarAnimator.SetBool("walk", true);
            _entranceAvatar.GetComponent<LookAtController>().enabled = false;

            if (Vector3.Distance(_chairs[tableNum].transform.position, _entranceAvatar.transform.position) < 1)
            {
                if (tableNum > 1) // table 2, 3, 4, 5の場合
                {
                    _entranceAvatar.transform.rotation = Quaternion.Slerp(_entranceAvatar.transform.rotation, Quaternion.Euler(0f, 0f, 0f), 0.2f);
                }
                else // table 0, 1の場合
                {
                    _entranceAvatar.transform.rotation = Quaternion.Slerp(_entranceAvatar.transform.rotation, Quaternion.Euler(0f, 90f, 0f), 0.2f);
                }
            }

            if (Vector3.Distance(_chairs[tableNum].transform.position, _entranceAvatar.transform.position) < 0.71)
            {
                _entranceAvatarAnimator.SetBool("sitting", true);
            }
        }

        // 1番テーブル片付け
        // テーブルから皿が片づけられたかの判定
        if (_cleanPlageJudge.IsPlatePicked("Plate1") && !FlagManager.Instance.flags[1])
        {
            // 片付け完了
            FlagManager.Instance.flags[1] = true;
            // 終了時間の格納
            _taskFinishedTimeList.Add(_elapsedTime);
            // 終了順序の格納
            _taskFinishedList.Add("1番テーブル片付け");
            // 5番テーブル入店が終わっていたら次のタスク開始
            if (FlagManager.Instance.flags[0])
            {
                // オーダーパネルの更新
                UIManager.Instance.ordertable.text += "2番：ハンバーガーセット\n";
                // 「ピザ」の表示
                _foods[3].SetActive(true);

                // ３番テーブル挙手
                _animators[2].SetBool("updown", true);
            }
        }

        // 3番テーブル注文
        if (_enterCubes[2].GetEnter() == 1 && IsPlayerLookAtCustomer(_avatars[2]))
        {
            if (Speech_Recognition_Manager.rec_complete)
            {
                if (FlagManager.Instance.flags[2]) // タスクが完了していたら、Bボタン表示するだけ
                {
                    ABbuttonUpdate(false);
                }
                else
                {
                    ABbuttonUpdate(true); // Aボタン表示
                    if (OVRInput.GetDown(OVRInput.RawButton.A))
                    {
                        ABbuttonUpdate(false); // Bボタン表示
                        switch (_dialogueProgressList[2])
                        {
                            case 0:
                                _animators[2].SetBool("updown", false); // 手を下げる

                                // 音声「注文いいでしょうか」
                                AudioManager.Instance.PlayVoice("order_ok_woman");

                                _dialogueProgressList[2] += 1; // 会話の進行

                                break;
                            case 1:
                                AudioManager.Instance.PlayVoice("chocolatecake_woman"); // 音声。「チョコレートケーキ」

                                _dialogueProgressList[2] += 1; // 会話の進行

                                break;
                            case 2:
                                // 音声「はい」
                                AudioManager.Instance.PlayVoice("yes_woman");

                                FlagManager.Instance.flags[2] = true;

                                // 終了時間の格納
                                _taskFinishedTimeList.Add(_elapsedTime);

                                // 終了順序の格納
                                _taskFinishedList.Add("3番テーブル注文");

                                // 2番テーブル配膳が終わっていたら次のタスク開始
                                if (FlagManager.Instance.flags[3])
                                {
                                    // 4番テーブル空き皿表示
                                    _emptyPlates[3].SetActive(true);
                                    // 6番テーブル顧客挙手
                                    _animators[5].SetBool("updown", true);
                                }

                                break;
                            default:
                                break;
                        }
                        // 音声認識オフ
                        Speech_Recognition_Manager.rec_complete = false;
                    }
                }
            }
        }

        // 2番テーブル配膳
        // プレイヤーが顧客を見ているかの判定
        if (_enterCubes[1].GetEnter() == 1 && IsPlayerLookAtCustomer(_avatars[1]))
        {
            // 音声認識完了チェック
            if (Speech_Recognition_Manager.rec_complete)
            {
                if (FlagManager.Instance.flags[3]) // タスクが完了していたら、Bボタン表示するだけ
                {
                    ABbuttonUpdate(false);
                }
                else
                {
                    ABbuttonUpdate(true); // Aボタン表示
                    if (OVRInput.GetDown(OVRInput.RawButton.A))
                    {
                        ABbuttonUpdate(false); // Bボタン表示
                        switch (_dialogueProgressList[3])
                        {
                            case 0:
                                // 適当な料理が運ばれたかの判定
                                if (_enterCubes[1].GetObjectName().Contains("ハンバーガーセット"))
                                {
                                    // 適当な料理が運ばれたとき
                                    // 音声「ありがとうございます」
                                    AudioManager.Instance.PlayVoice("thankyou_man");

                                    _dialogueProgressList[3] += 1; // 会話進行具合の更新
                                }
                                else
                                {
                                    //間違えた料理を持ってきた場合
                                    // 音声「その料理は頼んでいません」
                                    AudioManager.Instance.PlayVoice("not_order_man");
                                }

                                break;
                            case 1:
                                // 音声「はい」
                                AudioManager.Instance.PlayVoice("yes_man");

                                FlagManager.Instance.flags[3] = true;

                                // 終了時間の格納
                                _taskFinishedTimeList.Add(_elapsedTime);

                                // 終了順序の格納
                                _taskFinishedList.Add("2番テーブル配膳");

                                // オーダーテーブルから削除
                                UIManager.Instance.Order_Remove("2番:ハンバーガーセット");

                                // 3番テーブル注文が終わっていたら次のタスク開始
                                if (FlagManager.Instance.flags[2])
                                {
                                    // 4番テーブル空き皿表示
                                    _emptyPlates[3].SetActive(true);
                                    // 6番テーブル顧客挙手
                                    _animators[5].SetBool("updown", true);
                                }

                                break;
                            default:
                                break;
                        }
                        // 音声認識オフ
                        Speech_Recognition_Manager.rec_complete = false;
                    }
                }
            }
        }

        // 6番テーブルクレーム応対
        // プレイヤーが見ているかの判定
        if (_enterCubes[5].GetEnter() == 1 && IsPlayerLookAtCustomer(_avatars[5]))
        {
            // 音声認識完了チェック
            if (Speech_Recognition_Manager.rec_complete)
            {
                if (FlagManager.Instance.flags[4]) // タスクが完了していたら、Bボタン表示するだけ
                {
                    ABbuttonUpdate(false);
                }
                else
                {
                    ABbuttonUpdate(true); // Aボタン表示
                    if (OVRInput.GetDown(OVRInput.RawButton.A))
                    {
                        ABbuttonUpdate(false); // Bボタン表示
                        switch (_dialogueProgressList[4])
                        {
                            case 0:
                                _animators[5].SetBool("updown", false); // 手を下げる

                                // 音声「すみません、ちょっといいですか」
                                AudioManager.Instance.PlayVoice("excuse_me_man");

                                _dialogueProgressList[4] += 1; // 会話の進行

                                break;
                            case 1:
                                // 音声「先程の店員の接客態度が悪かったのですが」
                                AudioManager.Instance.PlayVoice("bad_manner_man");

                                _dialogueProgressList[4] += 1; // 会話の進行

                                break;
                            case 2:
                                // 音声「はい、以後気を付けてください」
                                AudioManager.Instance.PlayVoice("be_careful_man");

                                FlagManager.Instance.flags[4] = true;

                                // 終了時間の格納
                                _taskFinishedTimeList.Add(_elapsedTime);

                                // 終了順序の格納
                                _taskFinishedList.Add("6番テーブルクレーム応対");

                                break;
                            default:
                                break;
                        }
                        Speech_Recognition_Manager.rec_complete = false;
                    }
                }
            }
        }


        // 4番テーブル片付け
        if (_cleanPlageJudge.IsPlatePicked("Plate4") && !FlagManager.Instance.flags[5])
        {
            // 片付け完了
            FlagManager.Instance.flags[5] = true;
            // 終了時間の格納
            _taskFinishedTimeList.Add(_elapsedTime);
            // 終了順序の格納
            _taskFinishedList.Add("4番テーブル片付け");
        }

        // 全タスク終了
        ScenarioCompletedJudge();

        if (FlagManager.Instance.flags[6])
        {
            // ガイドパネルの更新
            UIManager.Instance.resultPanel.SetActive(true);
            UIManager.Instance.evaluationPanel.SetActive(true);
            UIManager.Instance.recognitionPanel.SetActive(false);
            UIManager.Instance.APanel.SetActive(false);
            UIManager.Instance.BPanel.SetActive(false);
            UIManager.Instance.guidePanel.SetActive(false);

            // 接客終了時間の表示
            UIManager.Instance.result.text = "接客タイム：\n";
            for (int i = 0; i < _taskFinishedTimeList.Count; i++)
            {
                UIManager.Instance.result.text += (i + 1) + "番目終了タイム：" + (int)_taskFinishedTimeList[i] / 60 + "分" + (int)_taskFinishedTimeList[i] % 60 + "秒     ";
                if (i % 2 != 0)
                {
                    UIManager.Instance.result.text += " " + "\n";
                }
            }

            // 接客終了順序の表示
            UIManager.Instance.evaluation.text = "接客順序：\n";
            for (int i = 0; i < _taskFinishedList.Count; i++)
            {
                UIManager.Instance.evaluation.text += (i + 1) + "番目：" + _taskFinishedList[i] + "    ";
                if (i % 2 == 1)
                {
                    UIManager.Instance.evaluation.text += "\n";
                }
            }
        }
    }

    // 全シナリオが完了しているかの判定
    void ScenarioCompletedJudge()
    {
        bool _isCompleted = true;
        if (!FlagManager.Instance.flags[6])
        {
            for (int i = 0; i < 6; i++)
            {
                if (!FlagManager.Instance.flags[i])
                {
                    _isCompleted = false;
                }
            }

            // 完了している場合は、終了フラグを立てる
            if (_isCompleted)
            {
                FlagManager.Instance.flags[6] = true;
            }
        }
    }

    // ABボタンの更新
    void ABbuttonUpdate(bool isA)
    {
        if (isA)
        {
            UIManager.Instance.a.text = "A";
            UIManager.Instance.b.text = "";
        }
        else
        {
            UIManager.Instance.a.text = "";
            UIManager.Instance.b.text = "B";
        }
    }

    // BGMを流す
    void BGMActivate()
    {
        AudioManager.Instance.PlayBGM("Special To Me-Slow Edit");
        AudioManager.Instance.AttachBGMSource.loop = true;
        AudioManager.Instance.ChangeVolume(0.2f, 1f, 1f);
    }

    // プレイヤーが顧客を見ているかの判定
    bool IsPlayerLookAtCustomer(GameObject customer)
    {
        // カメラの方向を取得
        Vector3 cameraDirection = _playerEye.transform.forward;

        // ターゲットまでの方向を取得
        Vector3 targetDirection = customer.transform.position - _playerEye.transform.position;
        targetDirection.y = targetDirection.y + 1.50f;  //キャラの顔への方向調整

        float angle = Vector3.Angle(cameraDirection, targetDirection);

        return 0 < angle && angle < 30;
    }

    // _dialogueProgressListの初期化
    void InitializeDialogueProgressList()
    {
        _dialogueProgressList = new List<int>();

        // シナリオの数だけ0を追加
        for (int i = 0; i < 6; i++)
        {
            _dialogueProgressList.Add(0);
        }
    }
}
