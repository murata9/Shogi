using UnityEngine;
using System.Collections;

//将棋盤のマス関係の関数まとめ
public class BoardManager : MonoBehaviour {
	public const int BoardSizeHalf = 5;//盤面の大きさの半分
	//範囲外チェック 範囲外ならtrue
	public static bool CheckOutRangeBoardPos(int pos)
	{
		if (pos < 1 || pos > define.BoardSizeX/*XとYが同じである前提とする*/)
			return true;
		else
			return false;
	}
	//範囲外チェック 範囲外ならtrue
	public static bool CheckOutRangeBoardPointXY (int x, int y)
	{
		if (CheckOutRangeBoardPos(x) == true || CheckOutRangeBoardPos(y) == true)
			return true;
		else
			return false;
	}
	//座標変換関数1
	//将棋盤の位置を駒のローカル位置に変換
	public static float GetPieceXFromBoardPosX (int board_x)
	{
		float x;
		int temp = board_x;
		if (GameManager.GetInstance ().first_player_flag == false && loginDataManager.watcher_flag == false) {
			temp = BoardManager.ReversePosX(temp);
		}
		temp -= BoardSizeHalf;
		x = (-(temp) * (define.PIECE_SIZE_X / 2));
		return x;
	}
	
	public static float GetPieceYFromBoardPosY (int board_y)
	{
		int temp = board_y;
		if (GameManager.GetInstance ().first_player_flag == false && loginDataManager.watcher_flag == false) {
			temp = BoardManager.ReversePosY(temp);
		}
		temp -= BoardSizeHalf;
		float y = (-(temp) * (define.PIECE_SIZE_Y / 2));
		return y;
	}
	//座標変換関数2
	//駒のローカル位置を将棋盤の位置に変換
	public static int GetBoardPosXFromLocalX (float local_x)
	{
		float temp = (local_x / (define.PIECE_SIZE_X / 2));
		if (temp < 0)temp -= 0.5f;//誤差補正
		int board_x = -(int)temp;
		board_x += BoardSizeHalf;
		return board_x;
	}
	public static int GetBoardPosYFromLocalY (float local_y)
	{
		float temp = (local_y / (define.PIECE_SIZE_Y / 2));
		if (temp < 0)temp -= 0.5f;//誤差補正
		int board_y = -(int)temp;
		board_y += BoardSizeHalf;
		return board_y;
	}
	//駒の位置を反転させる
	public static int ReversePosX (int x)
	{
		return define.BoardSizeX + 1 - x;
	}
	public static int ReversePosY (int y)
	{
		return define.BoardSizeY + 1 - y;
	}
}
