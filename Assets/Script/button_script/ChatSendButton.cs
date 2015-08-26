using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class ChatSendButton : MonoBehaviour {
	public InputField ChatInputField;//Unityで設定
	//チャット送信ボタンが押された時の処理
	public void PushButtonSendChat () {
		if (loginDataManager.login_flag == false) {
			Debug.LogError("Must Login");
			return;
		}
		WWWForm form = new WWWForm ();
		form.AddField ("user_id", loginDataManager.user_id);//ユーザID
		form.AddField ("play_id", loginDataManager.play_id);//対戦ID
		form.AddField ("comment", ChatInputField.text);
		//発言送信
		string url = define.MyServerURL + "plays/chat_post";
		WWW www = new WWW (url, form);
		WWWManager.GetInstance().ConnectWWW(www, ReceiveChat);
	}

	//WWWManagerで接続成功後のデータを受け取る関数
	//チャット
	void ReceiveChat(Dictionary<string,object> jsonData)
	{
		Debug.Log (jsonData ["chat"]);
	}
}
