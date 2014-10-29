using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

	//score
	public static int score = 0;

	// rows deleted
	int rows;

	int totalRows = 50;
	// level
	int level = 1;
	// Time since last gravity tick
	float lastFall = 0;

	bool isValidGridPos() {        
		foreach (Transform child in transform) {
			Vector2 v = Grid.roundVec2(child.position);
			
			// Not inside Border?
			if (!Grid.insideBorder(v))
				return false;
			
			// Block in grid cell (and not part of same group)?
			if (Grid.grid[(int)v.x, (int)v.y] != null &&
			    Grid.grid[(int)v.x, (int)v.y].parent != transform)
				return false;
		}
		return true;
	}

	void updateGrid() {
		// Remove old children from grid
		for (int y = 0; y < Grid.h; ++y)
			for (int x = 0; x < Grid.w; ++x)
				if (Grid.grid[x, y] != null)
					if (Grid.grid[x, y].parent == transform)
						Grid.grid[x, y] = null;
		
		// Add new children to grid
		foreach (Transform child in transform) {
			Vector2 v = Grid.roundVec2(child.position);
			Grid.grid[(int)v.x, (int)v.y] = child;
		}        
	}

	// Use this for initialization
	void Start() {
		// Default position not valid? Then its game over
		if (!isValidGridPos()) {
			Application.LoadLevel ("gameover_c");
		}
	}
	
	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown (KeyCode.Z)) {
			FindObjectOfType<Spawner>().holdBlock(0);
		}
		else if (Input.GetKeyDown (KeyCode.X)) {
			FindObjectOfType<Spawner>().holdBlock(1);
		}
		else if (Input.GetKeyDown (KeyCode.C)) {
			FindObjectOfType<Spawner>().holdBlock(2);
		}
		// Hard Drop
		else if (Input.GetKeyDown(KeyCode.Space)) {
			while (isValidGridPos()) {
				// Modify position
				transform.position += new Vector3 (0, -1, 0);
			}
			// Revert one row
			transform.position += new Vector3 (0, 1, 0);
			// Update grid
			updateGrid();
			
			// Clear filled horizontal lines
			rows = Grid.deleteFullRows ();
			if (rows == 1) {
				score += 40 * level;
			} else if (rows == 2) {
				score += 100 * level;
			} else if (rows == 3) {
				score += 300 * level;
			} else if (rows == 4) {
				score += 1200 * level;
			}
			
			totalRows += rows;

			if(totalRows % 10 == 0 && totalRows != 0){
				level++;
			}
			
			// Spawn next Group
			FindObjectOfType<Spawner> ().spawnNext();
			
			// Disable script
			enabled = false;
		}
		// Move Left
		else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			// Modify position
			transform.position += new Vector3(-1, 0, 0);
			
			// See if valid
			if (isValidGridPos())
				// Its valid. Update grid.
				updateGrid();
			else
				// Its not valid. revert.
				transform.position += new Vector3(1, 0, 0);
		}
		
		// Move Right
		else if (Input.GetKeyDown(KeyCode.RightArrow)) {
			// Modify position
			transform.position += new Vector3(1, 0, 0);
			
			// See if valid
			if (isValidGridPos())
				// Its valid. Update grid.
				updateGrid();
			else
				// Its not valid. revert.
				transform.position += new Vector3(-1, 0, 0);
		}
		
		// Rotate
		else if (Input.GetKeyDown(KeyCode.UpArrow)) {
			transform.Rotate(0, 0, -90);
			
			// See if valid
			if (isValidGridPos())
				// Its valid. Update grid.
				updateGrid();
			else
				// Its not valid. revert.
				transform.Rotate(0, 0, 90);
		}
		
		// Move Downwards and Fall
		else if (Input.GetKeyDown(KeyCode.DownArrow)) {
			// Modify position
			transform.position += new Vector3(0, -1, 0);
			
			// See if valid
			if (isValidGridPos()) {
				// Its valid. Update grid.
				updateGrid();
			} else {
				// Its not valid. revert.
				transform.position += new Vector3(0, 1, 0);
				
				// Clear filled horizontal lines
				rows = Grid.deleteFullRows();

				if (rows == 1) {
					score += 40 * level;
				} else if (rows == 2) {
					score += 100 * level;
				} else if (rows == 3) {
					score += 300 * level;
				} else if (rows == 4) {
					score += 1200 * level;
				}
				totalRows += rows;

				if(totalRows % 10 == 0 && totalRows != 0){
					level++;
				}

				// Spawn next Group
				FindObjectOfType<Spawner>().spawnNext();

				// Disable script
				enabled = false;
			}
		}
		else if (Time.time - lastFall >= 1) {
			// Modify position
			transform.position += new Vector3(0, -1, 0);
			
			// See if valid
			if (isValidGridPos()) {
				// Its valid. Update grid.
				updateGrid();
			} else {
				// Its not valid. revert.
				transform.position += new Vector3(0, 1, 0);
				
				// Clear filled horizontal lines
				rows = Grid.deleteFullRows();
				
				if (rows == 1) {
					score += 40 * level;
				} else if (rows == 2) {
					score += 100 * level;
				} else if (rows == 3) {
					score += 300 * level;
				} else if (rows == 4) {
					score += 1200 * level;
				}
				totalRows += rows;
				
				if(totalRows % 10 == 0 && totalRows != 0){
					level++;
				}
				
				// Spawn next Group
				FindObjectOfType<Spawner>().spawnNext();
				
				// Disable script
				enabled = false;
			}
			
			lastFall = Time.time;
		}
	}
}
