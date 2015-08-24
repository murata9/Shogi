using UnityEngine;
using System.Collections;

using UnityEngine.UI;
//部屋一覧リスト内のボタン
public class CanVisitRoomButton : MonoBehaviour {
	string room_no;//部屋番号
	//prefabから作成時に部屋番号を設定する
	public void SetRoomNo(string no){
		room_no = no;
	}
	//入室可能な部屋ボタンが押された時
	public void PushCanVisitRoomButton(){
		//部屋番号を入力
		InputField inputFieldRoomNo = GameObject.Find("InputFieldRoomNo").GetComponent<InputField>();
		inputFieldRoomNo.text = room_no;
	}
}
