using UnityEngine;
using System.Collections;

public class HoldBlock : MonoBehaviour {

	public GameObject[] blocks;
	public GameObject[] heldBlocks;
	public GameObject block;
	public GameObject temp;
	public int[] blockID;
	public int tempID;

	public int switchBlock(int i, int current){
		if (block != null)
			Destroy (block);
		switch (i) {
			case 0:
				FindObjectOfType<Block1> ().showBlock (current);
				break;
			case 1:
				FindObjectOfType<Block2> ().showBlock (current);
				break;
			case 2:
				FindObjectOfType<Block3> ().showBlock (current);
				break;
		}
		if (heldBlocks [i] == null) {
			heldBlocks [i] = blocks [current];
			blockID[i] = current;
			return -1;
		}
		else {
			temp = heldBlocks[i];
			heldBlocks[i] = blocks[current];
			tempID = blockID[i];
			blockID[i] = current;
			return tempID;
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
