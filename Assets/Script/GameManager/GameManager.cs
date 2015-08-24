using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using System.Collections.Generic;
using MiniJSON;
public class GameManager : MonoBehaviour {
	//Unity側で設定する
	public Canvas canvas;//変換用キャンバス
	public Text turn_text;//ターン数表示用テキスト
	public Text turn_player_text;//ターンプレイヤー表示用テキスト
	public Text winner_text;//ターンプレイヤー表示用テキスト
	//ターン情報
	public int last_turn = -1;//最後に取得した時のターン数
	public int turn_player_id = -1;//現在のターンプレイヤーID
	public bool first_player_flag;//自分が先手ならtrue
	private bool is_turn_plyer_me = false;//自分がターンプレイヤーかどうか
	//対戦情報
	public int winner_id = -1;//勝者のID
	public bool battle_flag = false;//対戦中かどうか
	public bool end_flag = false;//対戦終了かどうか
	private static GameManager inst;//インスタンス
	public int frame;//フレーム数

	//インスタンスの取得
	public static GameManager GetInstance()
	{
		return inst;
	}
	// Use this for initialization
	void Start () {
		inst = this;//作成時にインスタンスをスタティック変数に保持
	}
	// Update is called once per frame
	void Update () {
		frame++;
		if (end_flag == true) {
			return;
		}
		if (frame > 60) {
			frame = 0;
			//対戦相手が来ているかチェック
			StartCoroutine (DownloadPlayState());
			if (battle_flag == true) {
				//ターンの変化をチェック
				if (is_turn_plyer_me == false) {
					DownloadCheckChengeTurn ();
				}
			}
		}
	}
	//WWWManagerで接続成功後のデータを受け取る関数
	//ゲーム状態(ターン数など)の取得
	void ReceiveGameState(Dictionary<string,object> jsonData)
	{
		int turn_count = System.Convert.ToInt32 (jsonData ["turn_count"]);
		if (last_turn != turn_count) {
			//ターンが変化した
			Debug.Log ("ターンの変化を受信");
			last_turn = turn_count;//ターン数を保存
			turn_player_id = System.Convert.ToInt32 (jsonData ["turn_player"]);
			//盤の情報を更新
			PieceManager.GetInstance ().ReceivePiecesDate ();
			if (end_flag == false) {//ゲームが終了していない
				if (turn_player_id == loginDataManager.user_id) {
					SetIsTurnPlayerMe (true);
				} else {
					SetIsTurnPlayerMe (false);
				}
				//UI更新
				turn_text.text = "ターン " + turn_count.ToString ();
			}
		}
	}
	//ターンが変化していないか調べる関数
	void DownloadCheckChengeTurn()
	{
		//駒の状態を取得						/plays/対戦ID
		string url = define.URL + "plays/" + loginDataManager.play_id;
		WWW www = new WWW (url);
		WWWManager.GetInstance ().ConnectWWW (www, ReceiveGameState);
		return;
	}
	
	//部屋の状態を取得
	IEnumerator DownloadPlayState()
	{
		if (loginDataManager.play_id == -1) {
			yield break;
		} else {
			//駒の状態を取得						/plays/対戦ID/state
			string url = define.URL + "plays/" + loginDataManager.play_id + "/state";
			WWW www = new WWW (url);
			//接続待ち
			yield return www;
			
			if (www.error != null) {
				Debug.Log ("Error!");
			} else {
				//接続成功
				Debug.Log ("DownloadPlayData Success");
				//JSON
				var jsonData = MiniJSON.Json.Deserialize (www.text) as Dictionary<string,object>;
				string state = (string)(jsonData ["state"]);
				if(state == "playing") {
					battle_flag = true;
					//名前の取得
					UserNameManager.GetInst().ReceiveUserName();
				} else {
					turn_player_text.text = state;
					battle_flag = false;
					if(state == "exit")
					{
						//相手が退室
						turn_player_text.text = "相手が退室しました";
						if(loginDataManager.watcher_flag == true){
							string lose_user_name = null;
							if(winner_id == UserNameManager.GetInst().first_player_user_ID) {
								lose_user_name = UserNameManager.GetInst().last_player_user_name;
							} else if(winner_id == UserNameManager.GetInst().last_player_user_ID) {
								lose_user_name = UserNameManager.GetInst().first_player_user_name;
							}
							turn_player_text.text = lose_user_name + "が退室しました";
						}
						BattleEnd(state);
					}
					if(state == "finish")
					{
						//ゲーム終了
						turn_player_text.text = "ゲーム終了です";
						BattleEnd(state);
					}
				}
			}
		}
	}

	//ターンプレイヤー
	public bool GetIsTurnPlayerMe(){
		return is_turn_plyer_me;
	}
	public void SetIsTurnPlayerMe(bool b){
		is_turn_plyer_me = b;
		//UIの更新
		if (b == true) {
			turn_player_text.text = "あなたのターンです";
		} else {
			turn_player_text.text = "相手のターンです";
		}
		if (loginDataManager.watcher_flag == true) {
				turn_player_text.text = UserNameManager.GetInst().GetUserNameForID(turn_player_id) + "のターンです";
		}
	}

	//ゲーム終了時に呼ばれる関数
	void BattleEnd(string state)
	{
		//勝ったプレイヤー名を取得
		end_flag = true;
		StartCoroutine (DownloadWinner(state));
	}

	//勝者を取得
	IEnumerator DownloadWinner(string state)
	{
		//勝者を取得
		if (loginDataManager.play_id == -1) {
			yield break;
		} else {
			//駒の状態を取得						/plays/対戦ID/state
			string url = define.URL + "plays/" + loginDataManager.play_id + "/winner";
			WWW www = new WWW (url);
			//接続待ち
			yield return www;
			
			if (www.error != null) {
				Debug.Log ("Error!");
			} else {
				//接続成功
				Debug.Log ("DownloadWinner Success");
				//JSON
				var jsonData = MiniJSON.Json.Deserialize (www.text) as Dictionary<string,object>;
				winner_id = System.Convert.ToInt32 (jsonData ["winner"]);
				if(winner_id == loginDataManager.user_id) {
					winner_text.text = "あなたの勝利です";
				} 
				else
				{
					winner_text.text = UserNameManager.GetInst().GetUserNameForID(winner_id) + "の勝利です";
				}
				if(state == "exit"){
					turn_player_text.text = "相手が退室しました";
					if(loginDataManager.watcher_flag == true){
						//相手が退室
						int loser_id = -1;//退室したプレイヤーのID
						if(winner_id == UserNameManager.GetInst().first_player_user_ID){
							loser_id = UserNameManager.GetInst().last_player_user_ID;
						} else if(winner_id == UserNameManager.GetInst().last_player_user_ID){
							loser_id = UserNameManager.GetInst().first_player_user_ID;
						}
						turn_player_text.text = UserNameManager.GetInst().GetUserNameForID(loser_id) + "が退室しました";
					}
				}
			}
		}
	}
}
