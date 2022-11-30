using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{

	//ボリューム保存用のkeyとデフォルト値
	private const string BGM_VOLUME_KEY = "BGM_VOLUME_KEY";
	private const string SE_VOLUME_KEY = "SE_VOLUME_KEY";
	private const string VOICE_VOLUME_KEY = "VOICE_VOLUE_KEY";

	private const float BGM_VOLUME_DEFAULT = 1.0f;
	private const float SE_VOLUME_DEFAULT = 1.0f;
	private const float VOICE_VOLUME_DEFAULT = 1.0f; 

	//BGMがフェードするのにかかる時間
	public const float BGM_FADE_SPEED_RATE_HIGH = 0.9f;
	public const float BGM_FADE_SPEED_RATE_LOW = 0.3f;
	private float _bgmFadeSpeedRate = BGM_FADE_SPEED_RATE_HIGH;

	//次流すBGM名、SE名
	private string _nextBGMName;
	private string _nextSEName;
	private string _nextVOICEName;

	//BGMをフェードアウト中か
	private bool _isFadeOut = false;

	//BGM用、SE用に分けてオーディオソースを持つ
	public AudioSource AttachBGMSource, AttachSESource, AttachVOICESource;

	//全Audioを保持
	private Dictionary<string, AudioClip> _bgmDic, _seDic, _voDic;

	//=================================================================================
	//初期化
	//=================================================================================

	private void Awake()
	{
		if (this != Instance)
		{
			Destroy(this);
			return;
		}

		DontDestroyOnLoad(this.gameObject);

		//リソースフォルダから全SE&BGMのファイルを読み込みセット
		_bgmDic = new Dictionary<string, AudioClip>();
		_seDic = new Dictionary<string, AudioClip>();
		_voDic = new Dictionary<string, AudioClip>();

		object[] bgmList = Resources.LoadAll("Audio/BGM");
		object[] seList = Resources.LoadAll("Audio/SE");
		object[] voList = Resources.LoadAll("Voice");

		foreach (AudioClip bgm in bgmList)
		{
			_bgmDic[bgm.name] = bgm;
		}
		foreach (AudioClip se in seList)
		{
			_seDic[se.name] = se;
		}
		foreach (AudioClip vo in voList)
		{
			_voDic[vo.name] = vo;
		}
	}

	private void Start()
	{
		AttachBGMSource.volume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, BGM_VOLUME_DEFAULT);
		AttachSESource.volume = PlayerPrefs.GetFloat(SE_VOLUME_KEY, SE_VOLUME_DEFAULT);
		AttachVOICESource.volume = PlayerPrefs.GetFloat(VOICE_VOLUME_KEY, VOICE_VOLUME_DEFAULT);
	}

	//=================================================================================
	//SE
	//=================================================================================

	/// <summary>
	/// 指定したファイル名のSEを流す。第二引数のdelayに指定した時間だけ再生までの間隔を空ける
	/// </summary>
	public void PlaySE(string seName, float delay = 0.0f)
	{
		if (!_seDic.ContainsKey(seName))
		{
			Debug.Log(seName + "という名前のSEがありません");
			return;
		}

		_nextSEName = seName;
		Invoke("DelayPlaySE", delay);
	}

	private void DelayPlaySE()
	{
		AttachSESource.PlayOneShot(_seDic[_nextSEName] as AudioClip);
	}

	//=================================================================================
	//BGM
	//=================================================================================

	/// <summary>
	/// 指定したファイル名のBGMを流す。ただし既に流れている場合は前の曲をフェードアウトさせてから。
	/// 第二引数のfadeSpeedRateに指定した割合でフェードアウトするスピードが変わる
	/// </summary>
	public void PlayBGM(string bgmName, float fadeSpeedRate = BGM_FADE_SPEED_RATE_HIGH)
	{
		if (!_bgmDic.ContainsKey(bgmName))
		{
			Debug.Log(bgmName + "という名前のBGMがありません");
			return;
		}

		//現在BGMが流れていない時はそのまま流す
		if (!AttachBGMSource.isPlaying)
		{
			_nextBGMName = "";
			AttachBGMSource.clip = _bgmDic[bgmName] as AudioClip;
			AttachBGMSource.Play();
		}
		//違うBGMが流れている時は、流れているBGMをフェードアウトさせてから次を流す。同じBGMが流れている時はスルー
		else if (AttachBGMSource.clip.name != bgmName)
		{
			_nextBGMName = bgmName;
			FadeOutBGM(fadeSpeedRate);
		}

	}

	/// <summary>
	/// 現在流れている曲をフェードアウトさせる
	/// fadeSpeedRateに指定した割合でフェードアウトするスピードが変わる
	/// </summary>
	public void FadeOutBGM(float fadeSpeedRate = BGM_FADE_SPEED_RATE_LOW)
	{
		_bgmFadeSpeedRate = fadeSpeedRate;
		_isFadeOut = true;
	}

	private void Update()
	{
		if (!_isFadeOut)
		{
			return;
		}

		//徐々にボリュームを下げていき、ボリュームが0になったらボリュームを戻し次の曲を流す
		AttachBGMSource.volume -= Time.deltaTime * _bgmFadeSpeedRate;
		if (AttachBGMSource.volume <= 0)
		{
			AttachBGMSource.Stop();
			AttachBGMSource.volume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, BGM_VOLUME_DEFAULT);
			_isFadeOut = false;

			if (!string.IsNullOrEmpty(_nextBGMName))
			{
				PlayBGM(_nextBGMName);
			}
		}

	}

	//=================================================================================
	//Voice
	//=================================================================================

	/// <summary>
	/// 指定したファイル名のvoiceを流す。第二引数のdelayに指定した時間だけ再生までの間隔を空ける
	/// </summary>
	public void PlayVoice(string voName, float delay = 0.0f)
	{
		if (!_voDic.ContainsKey(voName))
		{
			Debug.Log(voName + "という名前のVoiceがありません");
			return;
		}

		_nextVOICEName = voName;
		Invoke("DelayPlayVOICE", delay);
	}

	private void DelayPlayVOICE()
	{
		AttachVOICESource.PlayOneShot(_voDic[_nextVOICEName] as AudioClip);
		Debug.Log("音声は流れました" + _nextVOICEName);
	}

	//=================================================================================
	//音量変更
	//=================================================================================

	/// <summary>
	/// BGM,SE,VOICEのボリュームを別々に変更&保存
	/// </summary>
	public void ChangeVolume(float BGMVolume, float SEVolume, float VOICEVolume)
	{
		AttachBGMSource.volume = BGMVolume;
		AttachSESource.volume = SEVolume;
		AttachVOICESource.volume = VOICEVolume;

		PlayerPrefs.SetFloat(BGM_VOLUME_KEY, BGMVolume);
		PlayerPrefs.SetFloat(SE_VOLUME_KEY, SEVolume);
		PlayerPrefs.SetFloat(VOICE_VOLUME_KEY, VOICEVolume);
	}
}
