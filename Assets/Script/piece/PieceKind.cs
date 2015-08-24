using UnityEngine;
using System.Collections;

//駒の種類の定義
public class PieceKind{
	public const int OH = 0;		//王
	public const int HISHA = 1;		//飛車
	public const int KAKU = 2;		//角
	public const int KIN = 3;		//金
	public const int GIN = 4;		//銀
	public const int KEIMA = 5;		//桂馬
	public const int KYOSHA = 6;	//香車
	public const int FU = 7;		//歩
	public const int PIECE_KIND_MAX = 8;	//最大数
	//駒の種類をstring型からint型に変換する関数
	public static int ChangeKindStringToint(string s)
	{
		if (s == "oh") {
			return OH;
		}
		if (s == "hisha" || s == "hisya") {
			return HISHA;
		}
		if (s == "kaku") {
			return KAKU;
		}
		if (s == "kin") {
			return KIN;
		}
		if (s == "gin") {
			return GIN;
		}
		if (s == "keima") {
			return KEIMA;
		}
		if (s == "kyosha" || s == "kyosya") {
			return KYOSHA;
		}
		if (s == "fu") {
			return FU;
		}
		Debug.LogError ("ChangeKindStringTointError");
		Debug.LogError (s);
		return -1;
	}
}
