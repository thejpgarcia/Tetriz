using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	// Blocks
	public GameObject[] blocks;
	public GameObject block;
	public GameObject temp;
	public int tempID;
	public int falling;
	public int current;
	public int next;

	public void holdBlock(int i){
		
		Destroy (block);
		tempID = FindObjectOfType<HoldBlock>().switchBlock(i, falling);
		if (tempID != -1) {
			falling = tempID;
			block = Instantiate (blocks[tempID],
			          	transform.position,
            	        Quaternion.identity) as GameObject;
		} 
		else {
			spawnNext ();
		}
	}

	public void spawnNext() {
		// Random Index
		do {
			next = Random.Range(0, blocks.Length);
		} while(next == current);
		// Spawn Group at current Position
	
		block = Instantiate(blocks[current],
		            transform.position,
		            Quaternion.identity) as GameObject;
		falling = current;
		current = next;
		// Spawn next Block
		FindObjectOfType<NextBlock>().showNext(next);
	}

	// Use this for initialization
	void Start () {
		current = Random.Range(0, blocks.Length);
		spawnNext ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
