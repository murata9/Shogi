using UnityEngine;
using System.Collections;

//駒の移動ベースクラス（継承される）
public class PieceMoveBase{
	public int[,] CanMoveArray = new int [3,3] { {1,1,1},{1,0,1},{1,1,1} };//周囲の移動可能判定
	public bool CanPromote = true;//成ることが出来る駒か
	//移動可能な場所にガイド配置を配置
	public virtual void CreateMoveGuide(int pos_x, int pos_y){
		//自分の移動可能な場所にガイドを設置
		int y_mul = 1;//後手ならy方向を逆にする
		if (GameManager.GetInstance ().first_player_flag == false) {
			y_mul = -1;
		}
		for (int y = 0; y < 3; y ++) {
			for (int x = 0; x < 3; x++) {
				int move_x = x - 1;//-1 ~ 1に変換
				int move_y = y - 1;
				int move_power = CanMoveArray[y,x];//移動力
				if(move_power == 1)//1マス移動
				{
					int tx = pos_x + move_x;
					int ty = pos_y + move_y * y_mul;
					Guide.CreateGuide (tx, ty, GuideKind.MOVE);
				}
				else if(move_power > 1)//複数マス進める
				{
					for(int i = 1; i < move_power; i++)
					{
						int tx = pos_x + move_x * i;
						int ty = pos_y + move_y * y_mul * i;
						if(Guide.CreateGuide (tx, ty, GuideKind.MOVE) == false) {
							//移動不可能になれば終了
							break;
						}
						GameObject obj = PieceManager.GetInstance().BoardPosArray [ty - 1, tx - 1];//移動先にいる別の駒
						if (obj != null) {
							break;//ほかの駒があれば終了
						}
					}
				}
			}
		}
	}
}

//王.玉
public class MoveOh : PieceMoveBase{
	public MoveOh(){
		CanMoveArray = new int[3, 3] {
			{1,1,1},
			{1,0,1},
			{1,1,1}
		};
		CanPromote = false;
	}
}

//飛車
public class MoveHisha : PieceMoveBase{
	public MoveHisha(){
		CanMoveArray = new int[3, 3] {
			{0,9,0},
			{9,0,9},
			{0,9,0}
		};
		CanPromote = true;
	}
}

//竜王
public class MoveRyuoh : PieceMoveBase{
	public MoveRyuoh(){
		CanMoveArray = new int[3, 3] {
			{1,9,1},
			{9,0,9},
			{1,9,1}
		};
		CanPromote = false;
	}
}

//角
public class MoveKaku : PieceMoveBase{
	public MoveKaku(){
		CanMoveArray = new int[3, 3] {
			{9,0,9},
			{0,0,0},
			{9,0,9}
		};
		CanPromote = true;
	}
}

//竜馬
public class MoveRyuma: PieceMoveBase{
	public MoveRyuma(){
		CanMoveArray = new int[3, 3] {
			{9,1,9},
			{1,0,1},
			{9,1,9}
		};
		CanPromote = false;
	}
}
//金
public class MoveKin : PieceMoveBase{
	public MoveKin(){
		CanMoveArray = new int[3, 3] {
			{1,1,1},
			{1,0,1},
			{0,1,0}
		};
		CanPromote = false;
	}
}

//成金
public class MoveNariKin : PieceMoveBase{
	public MoveNariKin(){
		CanMoveArray = new int[3, 3] {
			{1,1,1},
			{1,0,1},
			{0,1,0}
		};
		CanPromote = false;
	}
}

//銀
public class MoveGin : PieceMoveBase{
	public MoveGin(){
		CanMoveArray = new int[3, 3] {
			{1,1,1},
			{0,0,0},
			{1,0,1}
		};
		CanPromote = true;
	}
}

//桂馬
public class MoveKeima : PieceMoveBase{
	public MoveKeima(){
		CanMoveArray = new int[3, 3] {
			{0,0,0},
			{0,0,0},
			{0,0,0}
		};
		CanPromote = true;
	}
	//桂馬特有の移動
	public override void CreateMoveGuide (int pos_x, int pos_y)
	{
		int y_mul = 2;
		if (GameManager.GetInstance ().first_player_flag == false) {
			y_mul = -2;//後手ならy方向を逆にする
		}
		Guide.CreateGuide (pos_x - 1, pos_y - y_mul, GuideKind.MOVE);
		Guide.CreateGuide (pos_x + 1, pos_y - y_mul, GuideKind.MOVE);
	}
}

//香車
public class MoveKyosha : PieceMoveBase{
	public MoveKyosha(){
		CanMoveArray = new int[3, 3] {
			{0,9,0},
			{0,0,0},
			{0,0,0}
		};
		CanPromote = true;
	}
}

//歩
public class MoveFu : PieceMoveBase{
	public MoveFu(){
		CanMoveArray = new int[3, 3] {
			{0,1,0},
			{0,0,0},
			{0,0,0}
		};
		CanPromote = true;
	}
}