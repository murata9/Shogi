using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using System.Collections.Generic;
using MiniJSON;
public class PieceManager : MonoBehaviour {
	private const int SpriteArrayMax = 4;//画像配列の最大数
	private static PieceManager inst;//インスタンス
	public Text ohte_text;//王手表示用テキスト 	不透明度0
	public GameObject board;//ボード 回転用に取得
	public GameObject SelectPiece;//選択中の駒
	public GameObject PiecePrefab;//駒のプレハブ
	public GameObject[] PiecesArrayID = new GameObject[define.PieceNum];//全駒の配列ID順
	public GameObject[,] BoardPosArray = new GameObject[define.BoardSizeY, define.BoardSizeX];//盤面配列の駒の配列 y,xの順
	public List<GameObject> HavePiecesList;//自分の持っている駒のリスト
	public List<GameObject> HavePiecesListEnemy;//敵の持っている駒のリスト
	public Sprite[,] sprite = new Sprite[SpriteArrayMax,(int)PieceKind.PIECE_KIND_MAX];//画像配列
	//駒をプレハブから作成
	public GameObject CreatePiece()
	{
		if (PiecePrefab == null) {
			PiecePrefab = (GameObject)Resources.Load ("prefab/piece");
		}
		var obj = Instantiate(PiecePrefab) as GameObject;
		//キャンバスを親に設定
		obj.transform.SetParent(UnityEngine.Object.FindObjectOfType<Canvas>().transform);
		obj.transform.localPosition = new Vector3 (0, 0, 1);;//ローカル位置を設定
		obj.transform.localScale = new Vector3 (1, 1, 1);
		return obj;
	}
	//駒の位置配列をnullで初期化
	public void InitBoardPosArray()
	{
		for (int y = 0; y < define.BoardSizeY; y++) {
			for (int x = 0; x < define.BoardSizeX; x++) {
				BoardPosArray[y,x] = null;
			}
		}
	}
	//種類などから画像を取得
	public Sprite GetSprite(int kind,bool enemy, bool promote)
	{
		int type = 0;//0:自軍 1:自軍成り 2:敵軍 3:敵軍成り
		if (promote == true) {
			type++;
		}
		if (enemy == true) {
			type += 2;
		}
		Sprite sp = sprite[type,kind];
		return sp;
	}
	//画像読み込み
	void LoadSprite()
	{
		//4種類 0:自軍 1:自軍成り 2:敵軍 3:敵軍成り
		for (int n = 0; n < SpriteArrayMax; n++) {
			Sprite[] load;
			string name = "piece/" + (n + 1).ToString();//フォルダ名
			load = Resources.LoadAll<Sprite> (name);//フォルダ内の画像をすべて読み込む
/*
			System.Array.Find<Sprite>(allSprites
			                          ,(sprite) => sprite.name.Equals(spName));
			//ロードしたスプライトを名前で検索
			Sprite  sp = System.Array.Find<Sprite>(allSprites
			,(sprite) => sprite.name.Equals(spName));
*/
			//２次元配列に代入
			for (int k=0; k<(int)PieceKind.PIECE_KIND_MAX; k++) {
				//Sprite temp = System.Array.Find<Sprite>(load,(sprite2) => sprite2.name.Equals("sgl01"));
				//sprite [n, k] = temp;
				sprite [n, k] = load [k];
			}
		}
	}
	//王手判定
	bool CheckOhte(){
		Guide.AllGuideDelete ();//ガイドリセット
		//ターンプレイヤーの王の位置を取得
		int oh_x = -1;
		int oh_y = -1;
		foreach (var p in PiecesArrayID) {
			PieceBase piece = p.GetComponent<PieceBase>();
			if(piece.kind == PieceKind.OH){
				if(GameManager.GetInstance().turn_player_id == piece.owner_ID){
					oh_x = piece.board_pos_x;
					oh_y = piece.board_pos_y;
					break;
				}
			}
		}
		Guide.ohte_check_flag = true;//ガイド制作時に味方駒の扱いを変更する
		//非ターンプレイヤーの全駒の移動可能範囲の取得
		foreach (var p in PiecesArrayID) {
			PieceBase piece = p.GetComponent<PieceBase>();
			if(GameManager.GetInstance().turn_player_id == piece.owner_ID)continue;//非ターンプレイヤー
			if(piece.have_flag == true)continue;//持ち駒
			piece.move.CreateMoveGuide(piece.board_pos_x,piece.board_pos_y);
		}
		Guide.ohte_check_flag = false;//リセット
		GameObject[] guides = GameObject.FindGameObjectsWithTag (define.GuideTag);
		foreach (var g in guides) {
			Guide guide = g.GetComponent<Guide>();
			if(guide.pos_x == oh_x && guide.pos_y == oh_y){
				//王手
				Debug.Log("王手");
				StartCoroutine(DrawOhte());
				return true;
			}
		}
		return false;
	}
	//王手テキストの表示
	IEnumerator DrawOhte()
	{
		int alpah = 240;//アルファ値 表示フレーム
		while(alpah > 0)
		{
			alpah -= 2;
			Color color = ohte_text.color;
			if(alpah > 100) {
				color.a = 1.0f;
			} else {
				color.a = (float)alpah / 100.0f;
			}
			ohte_text.color = color;
			yield return null;
		}
	}
	//持ち駒の更新
	void UpdateHavePiece(bool anim_flag)
	{
		//持ち駒を並べる
		int index = 0;
		foreach(var p in HavePiecesList){
			if(anim_flag == true){
				SetPieceMoveAnimStart(p);//移動アニメーション
			}
			p.GetComponent<PieceBase>().SetPosForHavePiece(index);
			index++;
		}
		index = 0;
		foreach(var p in HavePiecesListEnemy){
			if(anim_flag == true){
				SetPieceMoveAnimStart(p);//移動アニメーション
			}
			p.GetComponent<PieceBase>().SetPosForHavePiece(index);
			index++;
		}
	}
	//駒のアニメーション移動を開始させる
	void SetPieceMoveAnimStart(GameObject obj){
		StartCoroutine (PieceMoveAnim(obj));
	}
	//駒をアニメーション移動させる	事前にmove_target_posを決定しておくこと
	IEnumerator PieceMoveAnim(GameObject obj){
		PieceBase comp = obj.GetComponent<PieceBase> ();
		if (comp.move_anim_flag == true) {
			yield break;//すでにアニメーション移動中
		}
		while (true) {
			yield return null;//1フレーム待つ
			Vector3 MyPos = obj.transform.localPosition;//現在地
			Vector3 DiffPos = comp.move_targer_pos - MyPos;//位置差
			float DiffDist = DiffPos.x * DiffPos.x + DiffPos.y * DiffPos.y;//三平方の定理から距離差を計算
			if(DiffDist < 1)
			{
				//終了
				obj.transform.localPosition = comp.move_targer_pos;
				comp.move_anim_flag = false;
				if(comp.have_flag == false){
					//持ち駒でなければ持ち駒のアニメーション移動を行う
					UpdateHavePiece(true);
				}
				break;
			}
			Vector3 pos = MyPos + DiffPos * 0.2f;//差を２０％ずつ詰めて滑らかに移動
			obj.transform.localPosition = pos;
		}
	}
	//GET
	//駒の状態を取得
	IEnumerator DownloadPiecesData()
	{
		string url = define.URL;
		if (loginDataManager.login_flag == false) {
			url = (define.URL + "get_pieces.json");//test用
		} else {
			url += "plays/" + loginDataManager.play_id.ToString() + "/pieces";
		}
		WWW www = new WWW (url);
		//接続待ち
		yield return www;
		
		if (www.error != null) {
			Debug.Log ("Error!");
		} else {
			//接続成功
			Debug.Log("DOWNLOAD Piece Success");
			Debug.Log(www.text);
			Debug.Log(www.url);
			//JSON
			//1~40 駒のID
			var jsonAllPieceData = MiniJSON.Json.Deserialize(www.text) as Dictionary<string,object>;
			//初期化
			InitBoardPosArray();
			HavePiecesList.Clear();
			HavePiecesListEnemy.Clear();
			Guide.AllDeleteFromTag(define.LastMoverTag);//最後に移動した駒表示ガイドを削除
			//各駒の情報
			for(int i = 0; i < define.PieceNum; i++)
			{
				var jsonOnePieceData = (Dictionary<string,object>)jsonAllPieceData[(i + 1).ToString()];
				bool create_flag = false;
				if(PiecesArrayID[i] == null)//初めて呼ばれた時は作成する
				{
					GameObject piece = CreatePiece();
					PiecesArrayID[i] = piece;
					create_flag = true;
				}
				//コンポーネントの取得
				PieceBase comp = PiecesArrayID[i].GetComponent<PieceBase> ();
				//ID設定
				comp.SetID(i + 1);
				//位置設定
				int pos_x = System.Convert.ToInt32(jsonOnePieceData["posx"]);
				int pos_y = System.Convert.ToInt32(jsonOnePieceData["posy"]);
				if(create_flag == false)//初期化時ではない
				{
					//このターン移動した駒を判定
					if(comp.board_pos_x != pos_x || comp.board_pos_y != pos_y){
						comp.last_move_flag = true;
					}
					if(comp.last_move_flag == true){
						Guide.CreateGuide(pos_x, pos_y, GuideKind.LAST_MOVER);
						comp.SetBoardPos(pos_x, pos_y);
						//駒をアニメーション移動
						//持ち駒ではなく盤面にあるかチェック
						if(BoardManager.CheckOutRangeBoardPointXY(pos_x, pos_y) == false)
						{
							comp.SetMoveTargetPos(pos_x, pos_y);
							SetPieceMoveAnimStart(PiecesArrayID[i]);
						}
					}
				}else{
					comp.SetLocalPos (pos_x, pos_y);
				}
				comp.last_move_flag = false;
				int user_id = loginDataManager.user_id;
				if(loginDataManager.login_flag == false){
					//test用
					user_id = 5;
				}
				if(loginDataManager.watcher_flag == true)
				{
					//観戦者なら先手視点
					user_id = UserNameManager.GetInst().first_player_user_ID;
				}
				int owner_ID = System.Convert.ToInt32(jsonOnePieceData["owner"]);
				comp.owner_ID = owner_ID;
				if(owner_ID == user_id) {
					//自軍
					comp.SetEnemyFlag (false);
				} else {
					//敵
					comp.SetEnemyFlag (true);
				}
				//成り
				bool promote_flag = System.Convert.ToBoolean(jsonOnePieceData["promote"]);
				comp.SetPromote (promote_flag);
				//駒の名前と敵判定と成りの情報から種類を決定
				int kind = PieceKind.ChangeKindStringToint((string)jsonOnePieceData["name"]);
				comp.SetKind (kind);
				//盤面配列に記録
				if(BoardManager.CheckOutRangeBoardPointXY(pos_x,pos_y) == false)//盤面上の駒なら 取った駒は座標0,0
				{
					int tx = pos_x - 1;
					int ty = pos_y - 1;
					BoardPosArray[ty ,tx] = PiecesArrayID[i];
					comp.SetHaveFlag(false);//持ち駒ではなくす
				}
				else
				{
					//盤面に無い駒は持ち駒とする
					comp.SetHaveFlag(true);
					if(comp.enemy_flag == true)	{
						HavePiecesList.Add(PiecesArrayID[i]);
					} else {
						HavePiecesListEnemy.Add(PiecesArrayID[i]);
					}
				}
			}
			//王手判定
			CheckOhte();
			Guide.AllGuideDelete ();//ガイド削除
			//持ち駒を並べる
			UpdateHavePiece(false);
			//盤
			if(GameManager.GetInstance().first_player_flag == false)
			{
				//後手なら盤を180度回転させる
				board.transform.localEulerAngles = new Vector3(0,0,180.0f);
			}
			else
			{
				board.transform.localEulerAngles = new Vector3(0,0,0.0f);
			}
		}
	}
	//盤面の情報を受信し盤面を更新する
	public void ReceivePiecesDate()
	{
		StartCoroutine (DownloadPiecesData());
	}
	//POST
	//駒の移動情報を送る
	IEnumerator PostPieceMove()
	{
		if (loginDataManager.login_flag == false) {
			Debug.Log ("Piece Update Error!");
			Debug.Log("ログインできていない");
			yield break;
		}
		//コンポーネントの取得
		PieceBase comp = SelectPiece.GetComponent<PieceBase> ();
		WWWForm form = new WWWForm ();
		//対戦IDとユーザID
		form.AddField ("play_id", loginDataManager.play_id);
		form.AddField ("user_id", loginDataManager.user_id);
		//動かした駒情報
		int move_id = comp.ID;
		form.AddField ("move_id", move_id);//駒ID
		//駒の新しい位置
		int pos_x = comp.board_pos_x;
		int pos_y = comp.board_pos_y;
		form.AddField ("posx", pos_x);
		form.AddField ("posy", pos_y);
		//成り
		bool promote = comp.promote;
		form.AddField ("promote", promote.ToString());
		//取った駒のID
		int get_id = -1;
		var get_obj = BoardPosArray [pos_y - 1, pos_x - 1];
		if (get_obj != null) {
			//移動先にほかの駒がある
			PieceBase get_obj_comp = get_obj.GetComponent<PieceBase> ();
			get_id  = get_obj_comp.ID; 
		}
		//取得した駒
		form.AddField ("get_id", get_id);//とりあえずなし
		//駒更新
		string url = define.URL + "plays/update";
		WWW www = new WWW (url, form);
		//接続待ち
		yield return www;
		
		if (www.error != null) {
			Debug.Log ("Piece Update Error!" + www.url);
			Debug.Log (www.error);
		} else {
			//接続成功
			Debug.Log ("Piece Update Success");
			Debug.Log (www.text);
			//移動フラグ
			comp.last_move_flag = true;
		}
	}
	//インスタンスの取得
	public static PieceManager GetInstance()
	{
		return inst;
	}
	//動ける場所があるか調べる なければfalse
	public bool CheckMovePos(int kind, int pos_y)
	{
		//後手なら上下反転
		if (GameManager.GetInstance ().first_player_flag == false) {
			pos_y = BoardManager.ReversePosY(pos_y);
		}
		//歩と香車
		if (kind == PieceKind.FU || kind == PieceKind.KYOSHA) {
			if(pos_y <= 1){
				return false;
			}
		}
		//桂馬
		if (kind == PieceKind.KEIMA) {
			if(pos_y <= 2){
				return false;
			}
		}
		return true;
	}
	//指定した座標が敵の陣地か調べる
	bool CheckEnemyTerritory(int pos_y)
	{
		//後手なら上下反転
		if (GameManager.GetInstance ().first_player_flag == false) {
			pos_y = BoardManager.ReversePosY(pos_y);
		}
		if (pos_y <= 3) {
			return true;
		}
		return false;
	}
	//成り判定 返り値がtrueならガイドを設置したので移動処理を中断
	bool PromoteCheck(Guide gui, PieceBase piece)
	{
		int pos_x = gui.pos_x;
		int pos_y = gui.pos_y;
		//ガイドが移動用なら成り用ガイドを設置するか判定
		if (piece.move.CanPromote == true && piece.have_flag == false) {//成ることが出来る駒で配置ではない
			bool check = false;
			//移動先が相手陣地なら
			if (CheckEnemyTerritory(pos_y)) {
				check = true;
			}
			//元の場所が相手陣地だった場合
			if (CheckEnemyTerritory(piece.board_pos_y)) {
				check = true;
			}
			if(check == true)
			{
				//成ることが出来る
				//成らないと動ける場所がなくなる場合を調べる
				if(CheckMovePos(piece.kind,pos_y) == true){
					Guide.CreateGuidePromote (pos_x, pos_y, piece);//成るかどうか選択させる
					return true;
				} else {
					//動ける場所がない
					Debug.Log ("成り");
					piece.SetPromote (true);//強制的に成る
					return false;
				}
			}
		}
		//成ることが出来ない
		return false;
	}
	//選択
	public void SetSelectPiece(GameObject obj)
	{
		SelectPiece = obj;
	}
	//選択
	public void MoveSelectPiece(Guide guide)
	{
		if (SelectPiece == null) {
			Debug.LogError("Error MoveSelectPiece");
			return;
		}
		//移動
		PieceBase comp = SelectPiece.GetComponent<PieceBase>();
		//成り判定
		if (guide.kind != GuideKind.PROMOTE) {
			if(PromoteCheck(guide,comp) == true)return;//成りガイドを新たに設置
		} else {
			//成り用ガイドならガイドの情報を使用
			comp.SetPromote(guide.promote);
		}
		//移動させる
		comp.SetBoardPos (guide.pos_x, guide.pos_y);
		comp.SetMoveTargetPos(guide.pos_x, guide.pos_y);
		SetPieceMoveAnimStart (SelectPiece);
		//自分のターンか確認
		if(GameManager.GetInstance().GetIsTurnPlayerMe() == true){
			//移動情報の送信
			StartCoroutine (PostPieceMove ());
			GameManager.GetInstance().SetIsTurnPlayerMe(false);
		} else {
			Debug.Log("自分のターンではない");
			//仮
			loginDataManager.user_id = GameManager.GetInstance ().turn_player_id;
			Debug.Log("UserIDを変更しました");
			GameManager.GetInstance().last_turn = -1;
		}
		//選択解除
		SelectPiece = null;
		//ガイドをすべて削除
		Guide.AllGuideDelete ();
	}
	// Use this for initialization
	void Start () {
		inst = this;//作成時にインスタンスをスタティック変数に保持
		LoadSprite ();//画像を読み込む
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
