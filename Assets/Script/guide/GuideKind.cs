using UnityEngine;
using System.Collections;

//ガイドの種類の定義
public class GuideKind{
	public const int MOVE = 0;		//移動先ガイド
	public const int PROMOTE = 1;	//成り選択ガイド
	public const int LAST_MOVER = 2;//最後に移動した駒
}
