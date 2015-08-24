using UnityEngine;
using System.Collections;

//ログイン情報を保持するクラス
public class loginDataManager : MonoBehaviour {
	public static int user_id = -1;//ユーザID
	public static int play_id = -1;//対戦ID
	public static bool login_flag = false;//ログイン出来ているか
	public static bool watcher_flag = false;//観戦者ならtrue
	// Use this for initialization
	void Start () {
		//シーンを切り替えても破棄されないようにする
		DontDestroyOnLoad (this);
	}
	//読み込み完了時
	void Awake(){
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
