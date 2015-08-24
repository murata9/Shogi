using UnityEngine;
using System.Collections;

//駒の基本情報とクリック時の処理
public class PieceBase : MonoBehaviour {
	public int ID;//駒ID
	public int board_pos_x;//盤上の座標x
	public int board_pos_y;//盤上の座標y
	public int kind;//種類
	public bool promote;//成っているかどうか
	public bool enemy_flag;//ユーザから見て敵かどうか
	public bool have_flag;//持ち駒かどうか
	public bool last_move_flag;//最後に動いた駒ならtrue
	public Vector3 move_targer_pos;//移動先
	public bool move_anim_flag = false;//アニメーション移動中か
	public int owner_ID;//所持者のユーザID
	public PieceMoveBase move = null;//移動管理
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
	//ボード位置を設定する
	public void SetBoardPos(int ix, int iy)
	{
		board_pos_x = ix;
		board_pos_y = iy;
	}
	public Vector3 CalcPiecePosVec3(int board_x, int board_y)
	{
		float fx = BoardManager.GetPieceXFromBoardPosX (board_x);
		float fy = BoardManager.GetPieceYFromBoardPosY (board_y);
		Vector3 pos = new Vector3 (fx, fy, 0);
		return pos;
	}
	//ボード位置からローカル座標を設定する
	public void SetLocalPos(int ix, int iy)
	{
		SetBoardPos (ix, iy);
		Vector3 pos = CalcPiecePosVec3(ix, iy);
		this.gameObject.transform.localPosition = pos;
	}
	//ボード位置から移動目標座標を設定する
	public void SetMoveTargetPos(int ix, int iy)
	{
		SetBoardPos (ix, iy);
		Vector3 pos = CalcPiecePosVec3(ix, iy);
		move_targer_pos = pos;
	}
	//持ち駒の位置を設定する
	public void SetPosForHavePiece(int index){
		int tx = -1;
		int ty = index;
		//縦に5つ並んだら横にずらす
		while (ty > 4) {
			tx--;
			ty-=5;
		}
		//相手の持ち駒は反対の位置に置く
		if (enemy_flag == false && GameManager.GetInstance().first_player_flag == true) {
			tx = BoardManager.ReversePosX(tx);
			ty = BoardManager.ReversePosY(ty);
		}
		if (enemy_flag == true && GameManager.GetInstance().first_player_flag == false) {
			tx = BoardManager.ReversePosX(tx);
			ty = BoardManager.ReversePosY(ty);
		}
		//位置設定
		float fx = BoardManager.GetPieceXFromBoardPosX (tx);
		float fy = BoardManager.GetPieceYFromBoardPosY (ty);
		Vector3 pos = new Vector3 (fx, fy, 0);
		move_targer_pos = pos;//移動先
		//this.gameObject.transform.localPosition = pos;
	}
	public void SetID(int i){
		ID = i;
	}
	public void SetPromote(bool b){
		promote = b;
	}
	public void SetEnemyFlag(bool b){
		enemy_flag = b;
	}
	public void SetHaveFlag(bool b){
		have_flag = b;
	}
	//種類を設定し画像を設定する
	public void SetKind(int k)
	{
		kind = k;
		UnityEngine.UI.Image image;
		image = gameObject.GetComponent<UnityEngine.UI.Image> ();
		image.sprite = PieceManager.GetInstance().GetSprite(k, enemy_flag, promote);
		//移動設定
		SetMove ();
	}
	//種類から移動設定を行う
	public void SetMove()
	{
		if (promote == true) {
			if(kind == PieceKind.HISHA) move = new MoveRyuoh();
			else if(kind == PieceKind.KAKU) move = new MoveRyuma();
			else move = new MoveNariKin();//ほかはすべて成金とする
			return;
		}
		switch (kind) {
		case PieceKind.OH:
			move = new MoveOh(); break;
		case PieceKind.HISHA:
			move = new MoveHisha(); break;
		case PieceKind.KAKU:
			move = new MoveKaku(); break;
		case PieceKind.KIN:
			move = new MoveKin(); break;
		case PieceKind.GIN:
			move = new MoveGin(); break;
		case PieceKind.KEIMA:
			move = new MoveKeima(); break;
		case PieceKind.KYOSHA:
			move = new MoveKyosha(); break;
		case PieceKind.FU:
			move = new MoveFu(); break;
		}
	}
	//クリックされた時
	public void ClickPiece()
	{
		//自分のターンか確認
		if (GameManager.GetInstance().GetIsTurnPlayerMe() == false)return;
		if (enemy_flag == true)return;
		//ピースマネージャーに選択されたことを伝える
		PieceManager pieceManager = PieceManager.GetInstance ();
		pieceManager.SetSelectPiece (this.gameObject);
		//ガイドリセット
		Guide.AllGuideDelete ();
		if (have_flag == true) {
			//持ち駒なら配置可能判定
			Guide.CreateGuideForHavePiece(kind);
		} else {
			//自分の移動可能な場所にガイドを設置
			move.CreateMoveGuide (board_pos_x, board_pos_y);
		}
	}
}
