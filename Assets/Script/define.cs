using UnityEngine;
using System.Collections;

public class define : MonoBehaviour {
	//定数
	public const int room_no = 101;//部屋番号101~200
	public const string URL = "http://192.168.3.83:3000/";//アドレス
	public const string MyServerURL = "http://192.168.3.83:3009/";//アドレス
	//駒
	public const int PieceNum = 40;//駒の数
	//将棋盤
	public const int BoardSizeX = 9;//マス目の最大数
	public const int BoardSizeY = 9;//マス目の最大数
	//盤面のマスのサイズ
	public const float chip_size_x_local = 30.0f;
	public const float chip_size_y_local = 32.0f;
	//駒画像のサイズ
	public const float PIECE_SIZE_X = 60;
	public const float PIECE_SIZE_Y = 64;
	//tag
	public const string GuideTag = "GuideTag";//移動ガイド
	public const string LastMoverTag = "LastMover";//最後に移動した駒のガイド
	public const string RoomListTag = "RoomListTag";//部屋一覧に表示されるボタン

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
