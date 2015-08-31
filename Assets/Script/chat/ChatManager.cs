using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
//チャット管理
public class ChatManager : MonoBehaviour {
	GameObject ChatNodePrefab;//チャットテキストのプレハブ
	public GameObject ScrollViewContent;//クローンの親にするためのパネル
	List<string> chat_log_list = new List<string>();
	float last_update_time;//最後に更新したtime 1秒ごとに更新
	// Use this for initialization
	void Start () {
		last_update_time = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		//Time.time Unityの計測している時間 秒単位のfloat値
		if (last_update_time + 1.0f < Time.time ) {
			GetChatLog();
			last_update_time = Time.time;
		}
	}

	void GetChatLog(){
		if (loginDataManager.play_id == -1) {
			Debug.LogError("Must Login");
			//return;
			loginDataManager.play_id = 1;
			loginDataManager.user_id = 1;
		}
		string url = define.MyServerURL + "chats/" + loginDataManager.play_id + "/chat_get";
		WWW www = new WWW (url);
		WWWManager.GetInstance ().ConnectWWW (www, ReceiveChatLog);
	}
	//WWWManagerで接続成功後のデータを受け取る関数
	//チャット内容の取得
	void ReceiveChatLog(Dictionary<string,object> jsonData)
	{
		//チャット内容をリセット
		//Guide.AllDeleteFromTag (define.ChatLogTag);
		int chat_num = System.Convert.ToInt32(jsonData ["chat_num"]);
		if (chat_log_list.Count != chat_num) {
			//内容が更新された
			Debug.Log("UpdateChat");
			//chat_log_list.Clear();
			string comment = null;
			for(int i = chat_log_list.Count; i < chat_num; i++)
			{
				comment = "";
				var log = (Dictionary<string,object>)jsonData[(i + 1).ToString()];
				comment += log["user_name"] + " < " + log["comment"];
				chat_log_list.Add(comment);
				//ノード作成
				CreateChatNode(comment);
			}
			//スクロール
			StartCoroutine(Scroll(chat_num));
		}
	}
	IEnumerator Scroll(int chat_num){
		yield return new WaitForSeconds (0.1f);//待機
		//スクロールさせる
		for (int i=0; i<100; i++) {
			Vector3 scroll_pos = ScrollViewContent.transform.localPosition;
			scroll_pos.y = 28;
			int scroll_num = chat_num - 3;//チャット数が３を超えたらスクロールさせる
			if (scroll_num > 0) {
				const int node_height = 18;
				scroll_pos.y += node_height * scroll_num;
			}
			Debug.Log ("scroll_pos_y:" + scroll_pos.y);
			Vector3 old_pos = ScrollViewContent.transform.localPosition;
			Vector3 move_vec = scroll_pos - old_pos;
			ScrollViewContent.transform.localPosition += move_vec * 0.5f;
			Debug.Log ("move_vec:" + move_vec);
			if(move_vec.y < 0.5f){
				//十分にスクロールした
				Debug.Log ("EndScroll_scroll_pos:" + scroll_pos);
				ScrollViewContent.transform.localPosition = scroll_pos;
				yield break;
			}
			yield return new WaitForSeconds (0.1f);//待機
		}
	}

	//チャットテキストをプレハブから作成
	public GameObject CreateChatNode(string comment)
	{
		if (ChatNodePrefab == null) {
			ChatNodePrefab = (GameObject)Resources.Load ("prefab/ChatNode");
		}
		var obj = Instantiate(ChatNodePrefab) as GameObject;
		//スクロールビューに表示するパネルを親に設定
		obj.transform.SetParent(ScrollViewContent.transform);
		obj.transform.localPosition = new Vector3 (0, 0, 1);//位置はUIのElementから設定される
		obj.transform.localScale = new Vector3 (1, 1, 1);
		Text node_text = obj.GetComponentInChildren<Text> ();
		node_text.text = comment;
		obj.tag = define.ChatLogTag;
		return obj;
	}
}
