using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;


// 音声認識を扱うクラス
public class Speech_Recognition_Manager
{
    // 音声認識
    public bool dispose;
    private bool recognition_flag;
    private string m_Recognitions;
    public static bool rec_complete;
    private DictationRecognizer m_DictationRecognizer;

    // イベント関数(デリゲート)
    private Action<string> dicatationResultEvent;
    private Action<string> dicatationHypothesysEvent;
    private Action<string> dicatationCompleteEvent;
    private Action<string> dicatationErrorEvent;

    //
    private bool Speech_End_Check;
    public static bool start_rec = false;

    public Speech_Recognition_Manager()
    {
        dicatationResultEvent = (text) => { };
        dicatationHypothesysEvent = (text) => { };
        dicatationCompleteEvent = (text) => { Debug.LogFormat(text); };
        dicatationErrorEvent = (text) => { Debug.LogErrorFormat(text); };

        recognition_flag = false;
        m_Recognitions = "";
        m_DictationRecognizer = new DictationRecognizer();
        m_DictationRecognizer.DictationResult += eventDictationResult;
        m_DictationRecognizer.DictationHypothesis += eventDictationHypothesis;
        m_DictationRecognizer.DictationComplete += eventDictationComplete;
        m_DictationRecognizer.DictationError += eventDictationError;
    }

    // 音声認識を開始/終了
    public void startRecognition() { m_DictationRecognizer.Start(); }
    public void stopRecognition()
    {
        m_DictationRecognizer.Stop();
        Debug.Log("Stop");
        this.disposeRecognition();
    }

    // 音声認識の開始状態/終了状態の確認
    public bool getSystemStatus()
    {
        if (m_DictationRecognizer.Status == SpeechSystemStatus.Running) return true;
        return false;
    }

    // 音声認識結果の取得/リセット
    public string getDictationResult() { return m_Recognitions; }
    public string resetDictationResult() { string temp = m_Recognitions; m_Recognitions = ""; return temp; }

    // 音声認識リソースの廃棄
    public void disposeRecognition()
    {
        m_DictationRecognizer.Dispose();
        Debug.Log("dispose");
        dispose = true;
    }


    // ========================================================================= 
    // ==== 以下、イベント関数 ==================================================
    // =========================================================================

    // 音声が認識されたタイミングで呼ぶ関数をセット（デリゲート）
    public void setDictationResultEvent(Action<string> calc) { this.dicatationResultEvent = calc; }

    // 音声が認識されているタイミングで呼ぶ関数をセット（デリゲート）
    public void setDictationHypothesysEvent(Action<string> calc) { this.dicatationHypothesysEvent = calc; }

    // 音声認識が停止したタイミングで呼ぶ関数をセット（デリゲート）
    public void setDictationCompleteEvent(Action<string> calc) { this.dicatationCompleteEvent += calc; }

    // 音声認識がエラーを吐いたタイミングに呼ぶ関数をセット（デリゲート）
    public void setDictationErrorEvent(Action<string> calc) { this.dicatationErrorEvent += calc; }

    // 音声が認識終了時に発生するイベント(フレーズが特定の認証制度で認識された時)
    private void eventDictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.LogFormat("検出された音声テキスト: {0}", text);
       

        m_Recognitions += text + ", ";
        this.dicatationResultEvent(text);
        //Debug.Log("音声認識終了: " + Gawara_Statics.mic_log);
        Debug.Log("音声認識完了: " + m_Recognitions);
       
        if (start_rec == true)
        {
            rec_complete = true;
            UIManager.Instance.recognition.text = "音声認識完了";
            start_rec = false;
        }
        

        Scenario1_controller.speechContent = m_Recognitions;
        Scenario2_controller.speechContent = m_Recognitions;
        Scenario3_controller.speechContent = m_Recognitions;
        Scenario4_controller.speechContent = m_Recognitions;
        //this.disposeRecognition();
    }

    // 音声を入力されているときに発生するイベント
    private void eventDictationHypothesis(string text)
    {
         if (!recognition_flag)
         {
             //Debug.Log("音声認識中...");
             recognition_flag = true;
             
         }

        if (start_rec == true)
        {
            Debug.Log("音声認識中...");
            //UIManager.Instance.recognition.color = Color.;
            UIManager.Instance.recognition.text = "音声認識中";
        } 
       
         this.dicatationHypothesysEvent(text);

    }

    // 音声認識セッションを終了したときに発生するイベント(stop等で音声認識が停止した時)
    private void eventDictationComplete(DictationCompletionCause cause)
    {
        switch (cause)
        {
            case DictationCompletionCause.TimeoutExceeded:
            case DictationCompletionCause.PauseLimitExceeded:
            case DictationCompletionCause.Canceled:
            case DictationCompletionCause.Complete:
            case DictationCompletionCause.UnknownError:
                // 無音状態などによって、音声認識処理が停止した場合の処理

                this.m_DictationRecognizer.Start();
                //this.dicatationCompleteEvent("Dictation is restarted.");
                Debug.Log("音声認識がリスタートしました");
                Debug.Log("Dictation completed successfully:" + cause);
                break;

            
            case DictationCompletionCause.AudioQualityFailure:
            case DictationCompletionCause.MicrophoneUnavailable:
            case DictationCompletionCause.NetworkFailure:
                // Error
                // その他の止まった原因に応じた処理
                this.dicatationCompleteEvent(cause.ToString());
                UIManager.Instance.recognition.text = "音声認識停止" + cause;
                Debug.Log("Dictation completed unsuccessfully:" + cause);
                Debug.Log("音声認識が止まりました");
                break;

        }
    }

    // 音声認識セッションがエラー検出したときに発生するイベント
    private void eventDictationError(string error, int hresult)
    {
        this.dicatationErrorEvent("Dictation error: " + error + "; HResult = " + hresult.ToString() + ".");
        Debug.Log("error");     //お試し　必要なくなったら消せ
        Debug.Log(error);       //お試し　必要なくなったら消せ
    }
}
