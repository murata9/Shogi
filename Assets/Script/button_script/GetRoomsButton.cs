using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using MiniJSON;
using UnityEngine.UI;
public class GetRoomsButton : MonoBehaviour {
	private GameObject CanVisitRoomButtonPrefab;//ボタンのプレハブ
	public GameObject ScrollViewPanel;//クローンの親にするためのパネル
	public InputField inputFieldSearchUserName;//検索するユーザ名入力欄

	//部屋一覧ボタンが押された時に呼ばれる
	public void PushGetRoomsButton() {
		string url = define.MyServerURL + "plays/GetRoom";
		WWW www = new WWW (url);
		WWWManager.GetInstance().ConnectWWW(www, ReceiveRoomDataForJsonData);
	}
	//検索ボタンが押された時に呼ばれる
	public void PushSearchRoomsForUserNameButton() {
		string url = define.MyServerURL + "plays/SearchRoomForUserName";
		WWWForm form = new WWWForm();
		string Search_name = inputFieldSearchUserName.text;
		form.AddField ("search_name", Search_name);
		WWW www = new WWW (url, form);
		WWWManager.GetInstance().ConnectWWW(www, ReceiveRoomDataForJsonData);
	}

	//部屋番号をソートしたリストを作成する関数
	List<int> CreateSortRoomsList(Dictionary<string,object> jsonAllRoomsData) {
		List<int> list = new List<int>();
		foreach(var room in jsonAllRoomsData) {
			var roomData = (Dictionary<string,object>)room.Value;
			int room_no;
			//部屋番号が数字でなければ無視する
			if(int.TryParse((string)roomData["room_no"], out room_no)) {
				list.Add(room_no);
			}
		}
		list.Sort ();
		return list;
	}
	//部屋Noが一致する部屋を返す
	Dictionary<string,object> FindRoomForNo(int search_no, Dictionary<string,object> jsonAllRoomsData) {		
		foreach(var room in jsonAllRoomsData) {
			//一つの部屋の情報
			var roomData = (Dictionary<string,object>)room.Value;
			//部屋番号を取得
			int room_no;
			//部屋番号が数字でなければ無視する
			if(int.TryParse((string)roomData["room_no"], out room_no)){
				if(room_no == search_no){
					//部屋情報を返す
					return roomData;
				}
			}
		}
		return null;
	}
	//一部屋のデータからボタンを作成する
	void CreateRoomButtonForOneRoomData(Dictionary<string,object> roomData, int room_no)
	{
		string state = (string)roomData ["state"];
		int player_num = System.Convert.ToInt32 (roomData ["player_num"]);
		//プレイヤーの名前を取得
		string player_name1 = "";
		if (player_num > 0) {
			player_name1 = (string)roomData ["player1"];
		}
		string BackString;//プレイヤー名の後ろにつける文字列
		if (player_num > 1) {
			BackString = "VS " + (string)roomData ["player2"];// プレイヤーが二人いれば二人目のプレイヤー名
		} else {
			BackString = state;//プレイヤーが一人なら部屋の状態
		}
		//プレハブからボタンを作成
		GameObject clone = CreateCanVisitRoomButton ();
		Text button_text = clone.transform.GetComponentInChildren<Text> ();//ボタンの子のTextを取得する
		//文字設定
		button_text.text = room_no + " " + player_name1 + " " + BackString;
		//部屋番号設定
		CanVisitRoomButton comp = clone.GetComponent<CanVisitRoomButton> ();
		comp.SetRoomNo ((string)roomData ["room_no"]);
	}

	//WWWManagerで接続成功後のデータを受け取る関数
	//サーバから取得した部屋情報を処理する
	void ReceiveRoomDataForJsonData(Dictionary<string,object> jsonData)
	{
		//部屋一覧をリセット
		Guide.AllDeleteFromTag (define.RoomListTag);
		int room_num = System.Convert.ToInt32(jsonData["room_num"]);
		var jsonAllRoomsData = (Dictionary<string,object>)jsonData["room_data"];//取得した全部屋のDictionary
		//ソート
		//SortedDictionary<string,int> (new Comparer() )を使う？
		List<int> list = CreateSortRoomsList (jsonAllRoomsData);
		//ソートした順番に表示する
		int old_search_no = -1;//直前のno 重複をなくす
		foreach (int search_no in list) {
			if(search_no == old_search_no){
				continue;//既に表示している
			}
			old_search_no = search_no;
			//全データからソートされた値と一致するデータを探す
			var roomData = FindRoomForNo(search_no, jsonAllRoomsData);
			if(roomData != null)
			{
				//部屋情報からボタンを作成し一覧に追加
				CreateRoomButtonForOneRoomData(roomData, search_no);
			}
		}
	}
	//ボタンをプレハブから作成
	public GameObject CreateCanVisitRoomButton()
	{
		if (CanVisitRoomButtonPrefab == null) {
			CanVisitRoomButtonPrefab = (GameObject)Resources.Load ("prefab/LayoutCanVisitRoomButtonPrefab");
		}
		var obj = Instantiate(CanVisitRoomButtonPrefab) as GameObject;
		//スクロールビューに表示するパネルを親に設定
		obj.transform.SetParent(ScrollViewPanel.transform);
		obj.transform.localPosition = new Vector3 (0, 0, 1);//位置はUIのElementから設定される
		obj.transform.localScale = new Vector3 (1, 1, 1);
		obj.tag = define.RoomListTag;
		return obj;
	}
}
