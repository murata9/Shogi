using UnityEngine;
using System.Collections;

//透明なボタン
public class BoardButton : MonoBehaviour {

	//何もないところがクリックされた時
	public void Click()
	{
		//駒選択リセット
		PieceManager.GetInstance ().SetSelectPiece (null);
		Guide.AllGuideDelete ();
	}
}
