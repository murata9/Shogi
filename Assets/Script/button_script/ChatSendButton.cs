using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

using System.Text;

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
		//utf-8に変換する
		string comment = ChatInputField.text;
		//comment = StringExtensions.ToUtf8(comment);
		form.AddField ("comment", comment, Encoding.GetEncoding("utf-8"));
		Debug.Log (comment);
		//発言送信
		string url = define.MyServerURL + "plays/chat_post";
		WWW www = new WWW (url, form);
		WWWManager.GetInstance().ConnectWWW(www, ReceiveChat);
		//入力欄を空にする
		ChatInputField.text = "";
	}

	//WWWManagerで接続成功後のデータを受け取る関数
	//チャット
	void ReceiveChat(Dictionary<string,object> jsonData)
	{
		Debug.Log (jsonData ["chat"]);
	}
}

//エンコード変換サンプル
public static class StringExtensions
{
	public static string ToShiftJis( this string unicodeStrings )
	{
		var unicode = Encoding.Unicode;
		var unicodeByte = unicode.GetBytes( unicodeStrings );
		var s_jis = Encoding.GetEncoding( "shift_jis" );
		var s_jisByte = Encoding.Convert( unicode, s_jis, unicodeByte );
		var s_jisChars = new char[ s_jis.GetCharCount( s_jisByte, 0, s_jisByte.Length ) ];
		s_jis.GetChars( s_jisByte, 0, s_jisByte.Length, s_jisChars, 0 );
		return new string( s_jisChars );
	}
	public static string ToUtf8( this string ShiftJisStrings )
	{
		var ShiftJis = Encoding.GetEncoding(932);//SHIFT_JIS
		var ShiftJisByte = ShiftJis.GetBytes( ShiftJisStrings );
		var s_utf = Encoding.GetEncoding( "utf-8" );
		var s_utfByte = Encoding.Convert( ShiftJis, s_utf, ShiftJisByte );
		var s_utfChars = new char[ s_utf.GetCharCount( s_utfByte, 0, s_utfByte.Length ) ];
		s_utf.GetChars( s_utfByte, 0, s_utfByte.Length, s_utfChars, 0 );
		return new string( s_utfChars );
	}
}