using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using System.Collections.Generic;
using MiniJSON;
using UnityEngine.Events;

//入室ボタン
public class login_button : MonoBehaviour {
	//Unityで設定
	public InputField inputFieldName;//名前入力欄
	public InputField inputFieldRoomNo;//部屋番号入力欄
	//WWWManagerで接続成功後のデータを受け取る関数
	//ログイン
	void ReceiveLogin(Dictionary<string,object> jsonData)
	{
		loginDataManager.user_id = System.Convert.ToInt32(jsonData ["user_id"]);
		loginDataManager.play_id = System.Convert.ToInt32(jsonData ["play_id"]);
		loginDataManager.watcher_flag = false;
		if((string)jsonData ["role"] == "watcher")
		{
			loginDataManager.watcher_flag = true;
		}
		loginDataManager.login_flag = true;
		Debug.Log (jsonData ["play_id"]);
		Debug.Log (jsonData ["user_id"]);
		Debug.Log (jsonData ["state"]);
		Debug.Log (jsonData ["role"]);
		//ゲーム画面へ移行
		Application.LoadLevel ("game_scene");
	}
	//ログインボタンが押された時の処理
	public void PushButtonLogin () {
		//ログイン
		WWWForm form = new WWWForm ();
		int room_no = int.Parse(inputFieldRoomNo.text);
		form.AddField ("name", inputFieldName.text);//ユーザ名
		form.AddField ("room_no", room_no);//部屋番号
		//ログイン
		string url = define.URL + "users/login";
		WWW www = new WWW (url, form);
		WWWManager.GetInstance().ConnectWWW(www, ReceiveLogin);
	}
}
