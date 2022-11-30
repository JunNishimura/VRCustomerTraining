using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flag : MonoBehaviour
{

    //会話の選択肢に関する宣言
    public Text guide;
    public Text subtitle;

    public Text choice1;
    public Text choice2;
    public Text choice3;


    // Start is called before the first frame update
    void Start()
    {
        FlagManager.Instance.ResetFlags();
        Debug.Log("Stage Start");

        //ガイドパネルと選択肢の初期状態
        guide.text = "テーブルの男性にメニューを聞きに行きましょう";
        subtitle.text = "";
        choice1.text = "May I take your order?";
        choice2.text = "お待たせしました。\nご注文はお決まりでしょうか";
        choice3.text = "Ready to Order?";
    }

    // Update is called once per frame
    void Update()
    {
        if(FlagManager.Instance.flags[0] == true)
        {
            subtitle.text = "Two lunch sets , please.\n and one coffee"; 
        }

        if(FlagManager.Instance.flags[1] == true)
        {
            //subtitle.text = "Two lunch sets , please.\n and one coffee.";
            
        }

        if (FlagManager.Instance.flags[2] == true)
        {
            guide.text = "注文を繰り返しましょう";
            choice1.text = "";
            choice2.text = "Two lunch sets and one with coffee, right?";
            choice3.text = "";
        }

        if (FlagManager.Instance.flags[3] == true)
        {
            subtitle.text = "Yes";
            guide.text = "女性にコーヒーは食前か食後どちらがいいか聞きましょう";
            choice1.text = "";
            choice2.text = "Would you like your coffee before or after meal?";
            choice3.text = "";
        }

        if (FlagManager.Instance.flags[4] == true)
        {
            subtitle.text = "After meal";
            guide.text = "「わかりました」と伝えましょう";
            choice1.text = "Sure";
            choice2.text = "Of course";
            choice3.text = "Certainly";
        }

        if (FlagManager.Instance.flags[5] == true)
        {
            subtitle.text = "";
            guide.text = "男性が料理を食べ終えています。お皿を下げましょう。";
            choice1.text = "May I take your plate";
            choice2.text = "Are you done?";
            choice3.text = "Are you still working on it?";
        }

        if (FlagManager.Instance.flags[6] == true)
        {
            subtitle.text = "Yes";
            guide.text = "カウンターにお皿を持っていきましょう。";
            choice1.text = "";
            choice2.text = "";
            choice3.text = "";
        }

        if(FlagManager.Instance.flags[7] == true)
        {
            subtitle.text = "";
            guide.text = "シーン1は終了です";
        }
    }
}
