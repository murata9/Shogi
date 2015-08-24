using UnityEngine;
using System.Collections;

using System.Collections.Generic;
//退室ボタン
public class logout_button : MonoBehaviour {
	// Use this for initialization
	void Start () {
	
	}
	// Update is called once per frame
	void Update () {
	
	}
	//ボタンが押された時の処理
	public void PushButtonLogout () {
		if (loginDataManager.login_flag == true && loginDataManager.watcher_flag == false) {
			//投了
			WWWForm form = new WWWForm ();
			form.AddField ("play_id", loginDataManager.play_id);
			form.AddField ("user_id", loginDataManager.user_id);
			string url = define.URL + "users/logout";
			WWW www = new WWW (url, form);
			//ログアウトしログイン画面に移行
			WWWManager.GetInstance ().ConnectWWW (www, ReceiveLogout);
		} else {
			//ログイン画面へ移行
			Application.LoadLevel ("login_scene");
			return;
		}
	}
	//WWWManagerで接続成功後のデータを受け取る関数
	//ログアウト
	void ReceiveLogout(Dictionary<string,object> jsonData){
		//ログイン画面へ移行
		Application.LoadLevel ("login_scene");
	}
}
