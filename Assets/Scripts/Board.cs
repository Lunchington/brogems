using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour {

	public GameObject[] gemTypes;
	public int[,] board;

	public int gridWidth = 6;
	public int gridHeight = 5;


	public static GameObject lastCollide= null;
	void Start () {
		board = new int[gridWidth, gridHeight];
		BuildLevel ();
	}
	
	void FixedUpdate () {


		if (Gem.select) {
			if (!getLastCollisionGem ())
				lastCollide = null;
			if (Gem.drag) {
				HandleDragging();

			} else {
				CheckMatch(Gem.select.GetComponent<Gem>());
					Gem.select.transform.position = Gem.dragtransform;
				Gem.select= null;

			}	
		} else {
			lastCollide = null;

		}
	}

	private GameObject getLastCollisionGem() {
			return Gem.select.GetComponent<Gem> ().gemCollide;

	}

	private void HandleDragging() {
		GameObject collisionGem = getLastCollisionGem ();
		if ((collisionGem) && (collisionGem != lastCollide)) {
			if (!CheckAdjacent(Gem.dragtransform, collisionGem.transform.position)) {
				return;
			}

			Gem gemColl =	collisionGem.GetComponent<Gem>();
			Gem gemSel 	= 	Gem.select.GetComponent<Gem>();


			//swap render position.
			Vector3 temp = collisionGem.transform.position;
			collisionGem.transform.position = Gem.dragtransform;
			Gem.dragtransform = temp;
			lastCollide = collisionGem;

			collisionGem.GetComponent<Gem> ().gemCollide = null;

			//swap Data
			int tempX =  gemSel.x;
			int tempY = gemSel.y;

			gemSel.x = gemColl.x;
			gemSel.y = gemColl.y;
		
			gemColl.x = tempX;
			gemColl.y = tempY;

			//change Board ID 
			board[gemColl.x, gemColl.y] = gemColl.id;
			board[gemSel.x, gemSel.y] 	= gemSel.id;

		}
	}

	private void swapGems(GameObject source, GameObject target) {

	}


	private void BuildLevel() {
		for (int y = 0; y< gridHeight; y++) {
			for (int x = 0; x< gridWidth; x++) {

				GameObject gemPrefab = getGemType(Random.Range(0,gemTypes.Length));

				GameObject g = Instantiate(gemPrefab,new Vector3(x,-y,0),Quaternion.identity)as GameObject;

				g.transform.parent = transform;

				g.GetComponent<Gem>().x = x;
				g.GetComponent<Gem>().y = y;

				board[x,y] = g.GetComponent<Gem>().id;


			}
		}
	}

	private GameObject getGemType(int i) {
		return gemTypes [i];
	}


	public bool CheckAdjacent(Vector3 sourcePos, Vector3 targetPos) {

		if ((sourcePos.x +1 == targetPos.x) && (sourcePos.y == targetPos.y)) //left
			return true;
		if ((sourcePos.x -1 == targetPos.x) && (sourcePos.y == targetPos.y)) //right
			return true;

		if ((sourcePos.x == targetPos.x) && (sourcePos.y +1 == targetPos.y)) //up
			return true;
		if ((sourcePos.x == targetPos.x) && (sourcePos.y -1 == targetPos.y)) //down
			return true;

		if ((sourcePos.x +1 == targetPos.x) && (sourcePos.y +1 == targetPos.y)) //left/top
			return true;
		if ((sourcePos.x -1 == targetPos.x) && (sourcePos.y-1 == targetPos.y)) //right/top
			return true;
		
		if ((sourcePos.x+1 == targetPos.x) && (sourcePos.y -1 == targetPos.y)) //up
			return true;
		if ((sourcePos.x-1 == targetPos.x) && (sourcePos.y +1 == targetPos.y)) //down
			return true;

		return false;

	}
	bool CheckMatch(Gem gemSelected){
			Gem[] gems = FindObjectsOfType(typeof(Gem)) as Gem[];

			int countUp = 0;
			int countDown = 0;
			int countRight = 0;
			int CountLeft = 0;

			//LEFT
			for(int l = gemSelected.x-1; l>=0; l--){
				if(board[l,gemSelected.y]==gemSelected.id){//If block have same ID
					CountLeft++;
				}
				if(board[l,gemSelected.y]!=gemSelected.id){//If block doesn't have same ID
					//Stop
					break;
				}
			}

			//Right
			for(int r = gemSelected.x; r<board.GetLength(0); r++){
				if(board[r,gemSelected.y]==gemSelected.id){
					countRight++;
				}
				if(board[r,gemSelected.y]!=gemSelected.id){
					break;
				}
			}
			//Down
			for(int d = gemSelected.y-1; d>=0; d--){
				if(board[gemSelected.x,d]==gemSelected.id){
						countDown++;
					}
				if(board[gemSelected.x,d]!=gemSelected.id){
						break;
				}
			}
			
			//Up
			for(int u = gemSelected.y; u<board.GetLength(1); u++){
				if(board[gemSelected.x,u]==gemSelected.id){
						countUp++;
					}
					
				if(board[gemSelected.x,u]!=gemSelected.id){
						break;
					}
				}
		//Check if there is 3+ match 
		if(CountLeft+countRight>=3 || countDown+countUp>=3){
			if(CountLeft+countRight>=3){
				//Destroy and mark empty block
				for(int cl = 0; cl<=CountLeft; cl++){
					foreach(Gem b in gems){
						if(b.x == gemSelected.x-cl && b.y == gemSelected.y){
							b.StartCoroutine("destroyBlock");;
							board[b.x,b.y] = 500; //To mark empty block
						}
					}
				}
				for(int cr = 0; cr<countRight; cr++){
					foreach(Gem b in gems){
						if(b.x == gemSelected.x+cr && b.y == gemSelected.y){
							b.StartCoroutine("destroyBlock");;
							board[b.x,b.y] = 500; 
						}
					}
				}
			}
			if(countDown+countUp>=3){
				for(int cd = 0; cd<=countDown; cd++){
					foreach(Gem blc in gems){
						if(blc.x == gemSelected.x && blc.y == gemSelected.y - cd){
							board[blc.x, blc.y] = 500;
							blc.StartCoroutine("destroyBlock");;
						}
					}
				}
				for(int cu = 0; cu<countUp; cu++){
					foreach(Gem blc in gems){
						if(blc.x == gemSelected.x && blc.y == gemSelected.y+cu){
							board[blc.x, blc.y] = 500;
							blc.StartCoroutine("destroyBlock");;
						}
					}
				}
			}
			MoveY();
			return true;
			
		}
		return false;
	}
	//Move blocks down
	void MoveY(){
		Gem[] gems = FindObjectsOfType(typeof(Gem)) as Gem[];
		int moveDownBy = 0; //How many times we need to move them down?

		for (int x=0; x<gridWidth; x++) {
			for (int y=0; y<gridHeight; y++) {
				if (board[x,y] == 500) {
					foreach(Gem g in gems) {
						if (g.x == x && g.y < y) {
							g.readyToMove = true;
							g.y += 1;
						}
					}
					moveDownBy++;

				}
			}
			foreach(Gem b in gems){
				if(b.readyToMove){
					b.StartCoroutine(b.moveDown(moveDownBy)); //Fall down effect
					b.readyToMove = false;//They will not fall again, now
					board[b.x,b.y] = b.id;//New ID in board
				}
			}
			MarkEmpty (x,moveDownBy);

			//Reset
			moveDownBy = 0;
		}

		Respawn ();
	}

	void MarkEmpty(int x, int num){
		for(int i=0; i<num; i++){
			board[x,i] = 500;
		}
		
	}

	void Respawn(){			

		for(int y=0; y<gridHeight; y++){
			for(int x=0; x<gridWidth; x++){
				if(board[x,y]==500){ //Spawn it only on destroyed block

					GameObject gemPrefab = getGemType(Random.Range(0,gemTypes.Length));

					GameObject g = Instantiate(gemPrefab,new Vector3(x,-y,0),Quaternion.identity)as GameObject;

					g.transform.parent = transform;

					g.GetComponent<Gem>().x = x;
					g.GetComponent<Gem>().y = y;
					g.GetComponent<Gem>().fallEffect = true;

					board[x,y] = g.GetComponent<Gem>().id;

				}
			}
		}
	}

}
