using UnityEngine;
using System.Collections;

public class prefab_test : MonoBehaviour {
	public Canvas canvas;//キャンバス
	public GameObject prefab;
	public Sprite sprite;
	// Use this for initialization
	void Start () {
		//var obj = Instantiate(prefab, new Vector3(0,-127,0), Quaternion.identity) as GameObject;
		//obj.transform.parent = (UnityEngine.Transform)canvas.transform;//キャンバスを親に設定
		//obj.transform.localPosition = new Vector3 (0, -127, 0);//ローカル位置を設定
		//var obj = PieceManager.GetInstance ().CreatePiece ();
		//PieceBase comp = obj.GetComponent<PieceBase> ();
		//comp.SetPos (5, 9);
		//comp.SetEnemyFlag (false);
		//comp.SetPromote (false);
		//comp.SetKind (PieceKind.FU);
		//return;
		/*
		//制作方法2 ファイルから読み込む
		GameObject loadObj = (GameObject)Resources.Load ("prefab/oh");
		var obj = Instantiate(loadObj) as GameObject;
		//キャンバスを親に設定
		obj.transform.SetParent(UnityEngine.Object.FindObjectOfType<Canvas>().transform);
		obj.transform.localPosition = new Vector3 (0, -127, 0);//ローカル位置を設定
		obj.transform.localScale = new Vector3 (1, 1, 1);

		obj = Instantiate(loadObj) as GameObject;
		//キャンバスを親に設定
		obj.transform.SetParent(canvas.transform);
		obj.transform.localPosition = new Vector3 (120, 127, 0);//ローカル位置を設定
		obj.transform.localScale = new Vector3 (1, 1, 1);

		//3イメージを指定する
		obj = Instantiate(loadObj) as GameObject;
		//キャンバスを親に設定
		obj.transform.SetParent(canvas.transform);
		obj.transform.localPosition = new Vector3 (0, 125, 0);//ローカル位置を設定
		obj.transform.localScale = new Vector3 (1, 1, 1);
		
		UnityEngine.UI.Image image;
		image = obj.gameObject.GetComponent<UnityEngine.UI.Image> ();
		image.sprite = sprite;
		*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
