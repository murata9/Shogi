using UnityEngine;
using System.Collections;

//ガイド
public class Guide : MonoBehaviour {
	static public Sprite last_mover_sprite = Resources.Load<Sprite> ("guide/guide2");//黄色のガイド
	static public bool ohte_check_flag = false;//王手チェック中ならtrue
	//盤面上の位置
	public int pos_x;
	public int pos_y;
	public int kind;//種類 GuideKind.MOVEなら移動用 PROMOTEなら配置用
	public bool promote;//移動時に成るならtrue;
	//静的
	static GameObject loadObj = (GameObject)Resources.Load ("prefab/guide");//プレハブ
	//ガイドをすべて削除
	public static void AllGuideDelete()
	{
		AllDeleteFromTag (define.GuideTag);
	}
	
	//指定したタグをもつオブジェクトをすべて削除
	public static void AllDeleteFromTag(string tag)
	{
		GameObject[] guides = GameObject.FindGameObjectsWithTag (tag);
		foreach (var g in guides) {
			Destroy(g.gameObject);
		}
	}
	//ガイドをプレハブから作成
	static GameObject CreateGuideInstant(int board_x ,int board_y, int kind)
	{
		var clone = Instantiate (loadObj) as GameObject;
		//キャンバスを親に設定
		clone.transform.SetParent (UnityEngine.Object.FindObjectOfType<Canvas> ().transform);
		//タグを設定
		clone.tag = define.GuideTag;
		//スケール
		clone.transform.localScale = new Vector3 (1, 1, 1);
		Guide comp = clone.GetComponent<Guide> ();
		//種類設定
		comp.kind = kind;
		//成り設定
		comp.promote = false;
		//位置設定
		comp.SetPos (board_x, board_y);
		float x = BoardManager.GetPieceXFromBoardPosX (board_x);
		float y = BoardManager.GetPieceYFromBoardPosY (board_y);
		Vector3 pos = new Vector3 (x, y, 0);
		clone.transform.localPosition = pos;//ローカル位置を設定
		return clone;
	}
	//成り決定用ガイド
	public static void CreateGuidePromote(int board_x ,int board_y, PieceBase piece)
	{
		for (int i = 0; i < 2; i++) {
			GameObject clone = CreateGuideInstant(board_x ,board_y, GuideKind.PROMOTE);
			Guide comp = clone.GetComponent<Guide> ();
			//スケール2倍
			clone.transform.localScale = new Vector3 (2, 2, 1);
			//位置設定
			comp.SetPos (board_x, board_y);
			float x = BoardManager.GetPieceXFromBoardPosX (board_x);
			float y = BoardManager.GetPieceYFromBoardPosY (board_y);
			//選択用
			if(i == 0){
				comp.promote = true;//成る
				x -= define.chip_size_x_local * 2;//位置を左にずらす
			} else if(i == 1) {
				comp.promote = false;
				x += define.chip_size_x_local * 2;//位置を右にずらす
			}
			Vector3 pos = new Vector3 (x, y, 0);
			clone.transform.localPosition = pos;//ローカル位置を設定
			//画像設定
			Sprite sprite = PieceManager.GetInstance().GetSprite(piece.kind,piece.enemy_flag,comp.promote);
			UnityEngine.UI.Image image;
			image = clone.gameObject.GetComponent<UnityEngine.UI.Image> ();
			image.sprite = sprite;
			image.color = new Color(1.0f,1.0f,1.0f,1.0f);//透明度をリセットする
		}
	}

	//移動可能な場所設定
	public static bool CreateGuide(int board_x, int board_y, int guide_kind)
	{
		if (guide_kind == GuideKind.PROMOTE) {
			Debug.LogError("CreateGuideError");
			return false;
		}
		if (BoardManager.CheckOutRangeBoardPointXY (board_x, board_y) == true) {
			return false;//範囲外
		}
		if (ohte_check_flag == false)//王手チェックではない
		{
			GameObject obj = PieceManager.GetInstance ().BoardPosArray [board_y - 1, board_x - 1];//移動先にいる別の駒
			if (obj != null) {
				PieceBase piece = obj.GetComponent<PieceBase> ();
				if (piece.enemy_flag == false) {
					return false;//味方の駒がある
				}
			}
		}
		GameObject clone = CreateGuideInstant (board_x, board_y, guide_kind);
		if (guide_kind == GuideKind.LAST_MOVER) {
			clone.tag = define.LastMoverTag;
			//画像設定
			Sprite sprite = last_mover_sprite;
			UnityEngine.UI.Image image;
			image = clone.gameObject.GetComponent<UnityEngine.UI.Image> ();
			image.sprite = sprite;
		}
		return true;
	}
	//持ち駒用ガイド配置
	public static void CreateGuideForHavePiece(int kind){
		//全マスをループ
		for (int x = 0; x < define.BoardSizeX; x++) {
			if(kind == PieceKind.FU)//歩なら二歩にならないか調べる
			{
				bool continue_flag = false;
				//列に歩がいないか探す
				for (int y = 0; y < define.BoardSizeY; y++) {
					GameObject obj = PieceManager.GetInstance().BoardPosArray [y, x];//移動先にいる別の駒
					if(obj != null)
					{
						PieceBase Piece = obj.GetComponent<PieceBase>();
						if(Piece.kind == PieceKind.FU && Piece.promote == false && Piece.enemy_flag == false)//味方の成っていない歩があれば
						{
							//二歩なのでこの列に置けない
							continue_flag = true;
							break;
						}
					}
				}
				if(continue_flag == true)continue;
			}
			for (int y = 0; y < define.BoardSizeY; y++) {
				GameObject obj = PieceManager.GetInstance().BoardPosArray [y, x];//移動先にいる別の駒
				if (obj == null) {
					//駒のない場所なら
					if(PieceManager.GetInstance().CheckMovePos(kind,y+1) == true) {//移動先があるか
						CreateGuide(x + 1, y + 1, GuideKind.MOVE);
					}
				}
			}
		}
	}
	//座標設定
	void SetPos(int ix,int iy)
	{
		pos_x = ix;
		pos_y = iy;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//クリックされたとき
	public void ClickGuide()
	{
		if (kind == GuideKind.LAST_MOVER)
			return;
		//ピースマネージャーに移動命令を出させる
		PieceManager pieceManager = PieceManager.GetInstance ();
		pieceManager.MoveSelectPiece (this);
	}
}
