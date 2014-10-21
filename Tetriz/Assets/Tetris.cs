using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tetris : MonoBehaviour {
	
	//Board
	public int[,] board;
	
	//Block 
	public Transform block;
	
	//Spawn boolean
	public bool spawn;
	
	//Seconds before next block spawn
	public float nextBlockSpawnTime = 0.5f;
	
	//Block fall speed
	public float blockFallSpeed = 0.5f;
	
	//Game over level
	public int gameOverHeight = 22; //20 board + 2 edge 
	
	//Current spawned shapes
	private List<Transform> shapes = new List<Transform>();
	
	//Set true if  game over
	private bool gameOver;
	
	//Current rotation of an object
	private int currentRot = 0;
	
	//Current pivot of the shape
	private GameObject pivot;
	
	void Start(){
		//Deafult board is 10x16
		
		//1+10+1 - Side edge
		
		//+2 - Space for spawning
		//+1 - Top edge 
		//20 - Height
		//+1 - Down edge 
		
		board = new int[12,24];//Set board width and height
		GenBoard();//Generate board
		InvokeRepeating("moveDown",blockFallSpeed,blockFallSpeed); //move block down
	}
	
	
	
	
	void Update(){
		if(spawn && shapes.Count == 4){ //If there is block
			
			//Get spawned blocks positions
			Vector3 a = shapes[0].transform.position;
			Vector3 b = shapes[1].transform.position; 
			Vector3 d = shapes[2].transform.position;
			Vector3 c = shapes[3].transform.position;
			
			
			if(Input.GetKeyDown(KeyCode.LeftArrow)){//Move left
				if(CheckUserMove(a,b,c,d,true)){//Check if we can move it left
					a.x-=1;
					b.x-=1;
					c.x-=1;
					d.x-=1;
					
					pivot.transform.position = new Vector3(pivot.transform.position.x-1, pivot.transform.position.y, pivot.transform.position.z);
					
					shapes[0].transform.position = a;
					shapes[1].transform.position = b; 
					shapes[2].transform.position = c; 
					shapes[3].transform.position = d; 
				}
				
				
				
			}
			if(Input.GetKeyDown(KeyCode.RightArrow)){//Move right
				if(CheckUserMove(a,b,c,d,false)){//Check if we can move it right
					a.x+=1;
					b.x+=1;
					c.x+=1;
					d.x+=1;
					
					pivot.transform.position = new Vector3(pivot.transform.position.x+1, pivot.transform.position.y, pivot.transform.position.z);
					
					shapes[0].transform.position = a;
					shapes[1].transform.position = b; 
					shapes[2].transform.position = c; 
					shapes[3].transform.position = d; 
					
					
				}
			}
			
			if(Input.GetKey(KeyCode.DownArrow)){
				//Move down fast
				moveDown();
			}
			
			
			if(Input.GetKeyDown(KeyCode.Space)){
				//Rotate
				Rotate(shapes[0].transform,shapes[1].transform,shapes[2].transform,shapes[3].transform);
				
			}
		}
		
		
		if(!spawn && !gameOver){//If nothing spawned, if game over = false, then spawn
			StartCoroutine("Wait");
			spawn = true;
			//Reset rotation
			currentRot = 0;
		}
		
	}
	
	
	
	
	void moveDown(){
		//Spawned blocks positions
		if(shapes.Count!=4){
			return;
		}
		Vector3 a = shapes[0].transform.position;
		Vector3 b = shapes[1].transform.position; 
		Vector3 c = shapes[2].transform.position;
		Vector3 d = shapes[3].transform.position;
		
		if(CheckMove(a,b,c,d)==true){    // Will we hit anything if we move block down(true = we can move)
			//Move block down by 1
			a = new Vector3(Mathf.RoundToInt(a.x),Mathf.RoundToInt(a.y-1.0f),a.z);
			b = new Vector3(Mathf.RoundToInt(b.x),Mathf.RoundToInt(b.y-1.0f),b.z);
			c = new Vector3(Mathf.RoundToInt(c.x),Mathf.RoundToInt(c.y-1.0f),c.z);
			d = new Vector3(Mathf.RoundToInt(d.x),Mathf.RoundToInt(d.y-1.0f),d.z);
			
			pivot.transform.position = new Vector3(pivot.transform.position.x, pivot.transform.position.y-1, pivot.transform.position.z);
			
			shapes[0].transform.position = a;
			shapes[1].transform.position = b; 
			shapes[2].transform.position = c; 
			shapes[3].transform.position = d; 
			
		}
		else{
			//We hit something. Stop and mark current shape location as filled in board, also destroy last pivot gameobject
			
			
			Destroy(pivot.gameObject); //Destroy pivot
			
			//Set ID in board
			board[Mathf.RoundToInt(a.x),Mathf.RoundToInt(a.y)]=1;
			board[Mathf.RoundToInt(b.x),Mathf.RoundToInt(b.y)]=1;
			board[Mathf.RoundToInt(c.x),Mathf.RoundToInt(c.y)]=1;
			board[Mathf.RoundToInt(d.x),Mathf.RoundToInt(d.y)]=1;
			
			//****************************************************
			checkRow(1); //Check for any match
			checkRow(gameOverHeight); //Check for game over
			//****************************************************
			
			shapes.Clear(); //Clear spawned blocks from array
			spawn = false; //Spawn a new block
			
			
		}
	}
	
	//Wait time before next block spawn
	IEnumerator Wait(){
		
		yield return new WaitForSeconds(nextBlockSpawnTime);
		SpawnShape();
	}
	
	
	
	
	bool CheckMove(Vector3 a, Vector3 b, Vector3 c, Vector3 d){
		//Check, if we move a block down will it hit something
		if(board[Mathf.RoundToInt(a.x),Mathf.RoundToInt(a.y-1)]==1){
			return false;
		}
		if(board[Mathf.RoundToInt(b.x),Mathf.RoundToInt(b.y-1)]==1){
			return false;
		}
		if(board[Mathf.RoundToInt(c.x),Mathf.RoundToInt(c.y-1)]==1){
			return false;
		}
		if(board[Mathf.RoundToInt(d.x),Mathf.RoundToInt(d.y-1)]==1){
			return false;
		}
		
		return true;
		
	}
	
	
	bool CheckUserMove(Vector3 a, Vector3 b, Vector3 c, Vector3 d, bool dir){
		//Check, if we move a block left/right will it hit something
		if(dir){//Left
			if(board[Mathf.RoundToInt(a.x-1),Mathf.RoundToInt(a.y)]==1 || board[Mathf.RoundToInt(b.x-1),Mathf.RoundToInt(b.y)]==1 || board[Mathf.RoundToInt(c.x-1),Mathf.RoundToInt(c.y)]==1 || board[Mathf.RoundToInt(d.x-1),Mathf.RoundToInt(d.y)]==1){
				return false;
			}
		}
		else{//Right
			if(board[Mathf.RoundToInt(a.x+1),Mathf.RoundToInt(a.y)]==1 || board[Mathf.RoundToInt(b.x+1),Mathf.RoundToInt(b.y)]==1 || board[Mathf.RoundToInt(c.x+1),Mathf.RoundToInt(c.y)]==1 || board[Mathf.RoundToInt(d.x+1),Mathf.RoundToInt(d.y)]==1){
				return false;
			}
		}
		return true;
	}
	
	
	
	
	
	
	void GenBoard(){
		for(int x=0; x<board.GetLength(0);x++){
			for(int y=0; y<board.GetLength(1);y++){
				if(x<11 && x>0){
					if(y>0 && y<board.GetLength(1)-2){
						//Board
						board[x,y]=0;
						GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						cube.transform.position = new Vector3(x,y,1);
						Material material = new Material(Shader.Find("Diffuse"));
						material.color = Color.grey;
						cube.renderer.material = material;
						cube.transform.parent = transform;
					}
					else if(y<board.GetLength(1)-2){
						board[x,y]=1;
						GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						cube.transform.position = new Vector3(x,y,0);
						Material material = new Material(Shader.Find("Diffuse"));
						material.color = Color.black;
						cube.renderer.material = material;
						cube.transform.parent = transform;
						cube.collider.isTrigger = true;
						
					}
				}
				else if((y<board.GetLength(1)-2)){
					//Left and right edge
					board[x,y]=1;
					GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					cube.transform.position = new Vector3(x,y,0);
					Material material = new Material(Shader.Find("Diffuse"));
					material.color = Color.black;
					cube.renderer.material = material;
					cube.transform.parent = transform;
				}
			}
		}
	}
	
	
	
	
	
	void SpawnShape(){
		
		int shape = Random.Range(0,6);//Random shape
		int height = board.GetLength(1)-4;
		int xPos = board.GetLength(0)/2-1;
		
		//Create a new pivot
		
		pivot = new GameObject("RotateAround"); //Pivot of the shape
		
		
		//SShape
		if(shape==0){
			
			pivot.transform.position = new Vector3(xPos,height+1, 0);
			
			shapes.Add(GenBlock(new Vector3(xPos, height,0)));
			shapes.Add(GenBlock(new Vector3(xPos-1, height,0)));
			shapes.Add(GenBlock(new Vector3(xPos, height+1,0)));
			shapes.Add(GenBlock(new Vector3(xPos+1, height+1,0)));
			
			
			Debug.Log("Spawned SShape");
		}
		//IShape
		else if(shape==1){
			
			pivot.transform.position = new Vector3(xPos+0.5f,height+1.5f, 0);
			
			shapes.Add(GenBlock(new Vector3(xPos, height,0)));
			shapes.Add(GenBlock(new Vector3(xPos, height+1,0)));
			shapes.Add(GenBlock(new Vector3(xPos, height+2,0)));
			shapes.Add(GenBlock(new Vector3(xPos, height+3,0)));
			
			Debug.Log("Spawned IShape");
		}
		//OShape
		else if(shape==2){
			
			pivot.transform.position = new Vector3(xPos+0.5f,height+0.5f, 0);
			
			shapes.Add(GenBlock(new Vector3(xPos, height,0)));
			shapes.Add(GenBlock(new Vector3(xPos+1, height,0)));
			shapes.Add(GenBlock(new Vector3(xPos, height+1,0)));
			shapes.Add(GenBlock(new Vector3(xPos+1, height+1,0)));
			
			Debug.Log("Spawned OShape");
		}
		//JShape
		else if(shape==3){
			
			pivot.transform.position = new Vector3(xPos,height+2, 0);
			
			shapes.Add(GenBlock(new Vector3(xPos, height,0)));
			shapes.Add(GenBlock(new Vector3(xPos+1, height,0)));
			shapes.Add(GenBlock(new Vector3(xPos, height+1,0)));
			shapes.Add(GenBlock(new Vector3(xPos, height+2,0)));
			
			Debug.Log("Spawned JShape");
		}
		
		//TShape
		else if(shape==4){
			
			pivot.transform.position = new Vector3(xPos,height, 0);
			
			shapes.Add(GenBlock(new Vector3(xPos, height,0)));
			shapes.Add(GenBlock(new Vector3(xPos-1, height,0)));
			shapes.Add(GenBlock(new Vector3(xPos+1, height,0)));
			shapes.Add(GenBlock(new Vector3(xPos, height+1,0)));
			
			Debug.Log("Spawned TShape");
		}
		
		//LShape
		else if(shape==5){
			
			pivot.transform.position = new Vector3(xPos,height+1, 0);
			
			shapes.Add(GenBlock(new Vector3(xPos, height,0)));
			shapes.Add(GenBlock(new Vector3(xPos-1, height,0)));
			shapes.Add(GenBlock(new Vector3(xPos, height+1,0)));
			shapes.Add(GenBlock(new Vector3(xPos, height+2,0)));
			
			Debug.Log("Spawned LShape");
		}
		
		//ZShape
		else{
			
			pivot.transform.position = new Vector3(xPos,height+1, 0);
			
			shapes.Add(GenBlock(new Vector3(xPos, height,0)));
			shapes.Add(GenBlock(new Vector3(xPos+1, height,0)));
			shapes.Add(GenBlock(new Vector3(xPos, height+1,0)));
			shapes.Add(GenBlock(new Vector3(xPos-1, height+1,0)));
			
			Debug.Log("Spawned ZShape");        
			
		}
		
		
		
	}
	
	//Create a block at the position
	Transform GenBlock(Vector3 pos){
		
		Transform obj = (Transform)Instantiate(block.transform, pos, Quaternion.identity) as Transform;
		obj.tag = "Block";
		
		return obj;
	}
	
	
	
	//Check specific row for match
	void checkRow(int y){
		
		GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block"); //All blocks in the scene
		int count = 0; //Blocks found in a row
		
		for(int x=1; x<board.GetLength(0)-1; x++){//Go through each block on this height
			if(board[x,y]==1){//If there is any block at this position
				count++;//We found +1 block
			}
		}
		
		
		if(y==gameOverHeight && count>0){//If the current height is game over height, and there is more than 0 block, then game over
			Debug.LogWarning("Game over");
			gameOver = true;
		}
		
		if(count==10){//The row is full
			//Start from bottom of the board(withouth edge and block spawn space)
			for(int cy=y; cy<board.GetLength(1)-3; cy++){
				for(int cx=1; cx<board.GetLength(0)-1; cx++){
					foreach(GameObject go in blocks){
						
						int height = Mathf.RoundToInt(go.transform.position.y);
						int xPos = Mathf.RoundToInt(go.transform.position.x);
						
						if(xPos == cx && height == cy){
							
							if(height == y){//The row we need to destroy
								board[xPos,height] = 0;//Set empty space
								Destroy(go.gameObject);
							}
							else if(height > y){
								board[xPos,height] = 0;//Set old position to empty
								board[xPos,height-1] = 1;//Set new position 
								go.transform.position = new Vector3(xPos, height-1, go.transform.position.z);//Move block down
							}
						}
					}
				}
			}
			checkRow(y); //We moved blocks down, check again this row
		}
		else if(y+1<board.GetLength(1)-3){
			checkRow(y+1); //Check row above this
		}
	}
	
	
	
	
	void Rotate(Transform a, Transform b, Transform c, Transform d){
		
		
		//Set parent to pivot so we can rotate
		a.parent = pivot.transform;
		b.parent = pivot.transform;
		c.parent = pivot.transform;
		d.parent = pivot.transform;
		
		currentRot +=90;//Add rotation
		if(currentRot==360){ //Reset rotation
			currentRot = 0;
		}
		
		pivot.transform.localEulerAngles = new Vector3(0,0,currentRot);
		
		a.parent = null;
		b.parent = null;
		c.parent = null;
		d.parent = null;
		
		if(CheckRotate(a.position,b.position,c.position,d.position) == false){
			//Set parent to pivot so we can rotate
			a.parent = pivot.transform;
			b.parent = pivot.transform;
			c.parent = pivot.transform;
			d.parent = pivot.transform;
			
			currentRot-=90;
			pivot.transform.localEulerAngles = new Vector3(0,0,currentRot);
			
			a.parent = null;
			b.parent = null;
			c.parent = null;
			d.parent = null;
		}
	} 
	
	
	bool CheckRotate(Vector3 a, Vector3 b, Vector3 c, Vector3 d){
		if(Mathf.RoundToInt(a.x)<board.GetLength(0)-1){//Check if block is in board
			if(board[Mathf.RoundToInt(a.x),Mathf.RoundToInt(a.y)]==1){
				//If rotated block hit any other block or edge, after rotation
				return false; //Rotate in default position - previous
			}
		}
		else{//If the block is not in the board
			return false;//Do not rotate
		}
		if(Mathf.RoundToInt(b.x)<board.GetLength(0)-1){
			if(board[Mathf.RoundToInt(b.x),Mathf.RoundToInt(b.y)]==1){
				return false; 
			}
		}
		else{
			return false;
		}
		if(Mathf.RoundToInt(c.x)<board.GetLength(0)-1){
			if(board[Mathf.RoundToInt(c.x),Mathf.RoundToInt(c.y)]==1){
				
				return false; 
			}
		}
		else{
			return false;
		}
		if(Mathf.RoundToInt(d.x)<board.GetLength(0)-1){
			if(board[Mathf.RoundToInt(d.x),Mathf.RoundToInt(d.y)]==1){
				
				return false;
			}
		}
		else{
			return false;
		}
		
		return true; //We can rotate
	}
	
	
	
}