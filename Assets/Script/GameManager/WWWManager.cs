using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using UnityEngine.Events;

//通信処理まとめ
public class WWWManager : MonoBehaviour {
	private static WWWManager inst;//インスタンス
	// Use this for initialization
	void Start () {
		if (inst == null) {
			inst = this;//作成時にインスタンスをスタティック変数に保持
		}
		DontDestroyOnLoad (this.gameObject);
	}
	//インスタンスの取得
	public static WWWManager GetInstance()
	{
		return inst;
	}
	public WWW ConnectWWW(WWW www,UnityAction<Dictionary<string,object>> func){
		StartCoroutine (WaitWWW(www, func));
		return www;
	}
	private IEnumerator WaitWWW(WWW www,UnityAction<Dictionary<string,object>> func) {
		//接続待ち
		yield return www;
		if (www.error != null) {
			Debug.Log ("Error!" + www.error);
		} else {
			//接続成功
			if(func != null)
			{
				Debug.Log("WWW:" + www.text);
				var jsonData = MiniJSON.Json.Deserialize (www.text) as Dictionary<string,object>;
				func(jsonData);
			}
		}
	}
}
