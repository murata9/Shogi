using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using MiniJSON;

public class test_www : MonoBehaviour {
	public GameObject sphere;
	// Use this for initialization
	void Start () {
		//StartCoroutine (Download());
		//StartCoroutine (Post());
	}

	//GET
	IEnumerator Download()
	{
		//ゲームの状態を取得					/plays/対戦ID/state
		//WWW www = new WWW ("192.168.3.83:3000/plays/2/state");

		//駒の状態を取得							/plays/対戦ID/pieces
		WWW www = new WWW ("192.168.3.83:3000/plays/6/pieces");
		//接続待ち
		yield return www;

		if (www.error != null) {
			Debug.Log ("Error!");
		}
		/* else {
			//接続成功
			Debug.Log("DOWNLOAD Success");
			//JSON
			//1~40 駒のID
			var jsonAllPieceData = MiniJSON.Json.Deserialize(www.text) as Dictionary<string,object>;
			//各駒の情報
			for(int i=0;i<40;i++)
			{
				/var jsonOnePieceData = (Dictionary<string,object>)jsonAllPieceData[(i + 1).ToString()];
				//Debug.Log((string)jsonOnePieceData["name"]);
				//Debug.Log(jsonOnePieceData["owner"]);
				//Debug.Log(jsonOnePieceData["posx"]);
				//Debug.Log(jsonOnePieceData["posy"]);
				//Debug.Log(jsonOnePieceData["promote"]);
			}
		}
		*/
	}

	//POST
	IEnumerator Post()
	{
		WWWForm form = new WWWForm();
		form.AddField ("name", "test_user_mk");
		form.AddField ("room_no", define.room_no);//部屋番号101~200
		//ログイン
		WWW www = new WWW ("192.168.3.83:3000/users/login", form);
		//接続待ち
		yield return www;
		
		if (www.error != null) {
			Debug.Log ("Error!");
		} else {
			//接続成功
			Debug.Log("POST Success");
			var jsonData = MiniJSON.Json.Deserialize(www.text) as Dictionary<string,object>;
			Debug.Log(jsonData["user_id"]);
			Debug.Log(jsonData["play_id"]);
			Debug.Log(jsonData["state"]);
			Debug.Log(jsonData["role"]);
/*例
{
  "user_id": 1,
  "play_id": 1,
  "state"  : "waiting",
  "role"   : "player"
}*/
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
