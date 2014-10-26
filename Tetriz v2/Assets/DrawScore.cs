using UnityEngine;
using System.Collections;

public class DrawScore : MonoBehaviour {

	GUIText text;

	void OnGUI(){
		guiText.text = "" + Block.score;
	}
}
