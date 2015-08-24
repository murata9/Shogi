using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using System.Collections.Generic;
using MiniJSON;

//対戦中のユーザー情報を管理するクラス
public class UserNameManager : MonoBehaviour {
	public int first_player_user_ID;		//先手ID
	public string first_player_user_name;	//先手ユーザ名
	public int last_player_user_ID;			//後手ID
	public string last_player_user_name;	//後手ユーザ名
	public Text user1_text;//ユーザー名表示用テキスト1
	public Text user2_text;//ユーザー名表示用テキスト2
	public bool user_name_get_flag = false;//名前を取得したらtrue
	private static UserNameManager inst;//インスタンス
	//インスタンスの取得
	public static UserNameManager GetInst()
	{
		return inst;
	}
	public string GetUserNameForID(int id){
		string str = null;
		if(id == UserNameManager.GetInst().first_player_user_ID){
			//先手
			str = UserNameManager.GetInst().first_player_user_name;
		} else {
			str = UserNameManager.GetInst().last_player_user_name;
		}
		return str;
	}
	//ユーザー名の取得
	public void ReceiveUserName()
	{
		if (user_name_get_flag == false) {
			user_name_get_flag = true;
			StartCoroutine (DownloadPlayUserName ());
		}
	}
	//GET
	//ユーザー名の取得
	IEnumerator DownloadPlayUserName()
	{
		if (loginDataManager.play_id == -1) {
			yield break;
		} else {
			//ユーザ名を取得						/plays/対戦ID/users
			string url = define.URL +  "plays/" + loginDataManager.play_id + "/users";
			WWW www = new WWW (url);
			//接続待ち
			yield return www;
			
			if (www.error != null) {
				Debug.Log ("Error!");
			} else {
				//接続成功
				Debug.Log ("DownloadPlayUserName Success");
				//JSON
				var jsonData = MiniJSON.Json.Deserialize (www.text) as Dictionary<string,object>;
				var firstPlayerData = (Dictionary<string,object>)jsonData["first_player"];
				first_player_user_ID = System.Convert.ToInt32(firstPlayerData["user_id"]);
				first_player_user_name = (string)(firstPlayerData["name"]);
				user1_text.text = first_player_user_name;
				var lastPlayerData = (Dictionary<string,object>)jsonData["last_player"];
				last_player_user_ID = System.Convert.ToInt32(lastPlayerData["user_id"]);
				last_player_user_name = (string)(lastPlayerData["name"]);
				user2_text.text = last_player_user_name;
				//先手判定
				GameManager.GetInstance().first_player_flag = false;
				if(first_player_user_ID == loginDataManager.user_id || loginDataManager.watcher_flag == true)
				{
					GameManager.GetInstance().first_player_flag = true;
					//先手なら表示を逆にする
					user1_text.text = last_player_user_name;
					user2_text.text = first_player_user_name;
				}
			}
		}
	}
	// Use this for initialization
	void Start () {
		inst = this;//作成時にインスタンスをスタティック変数に保持
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
