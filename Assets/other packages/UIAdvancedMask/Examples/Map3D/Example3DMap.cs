using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Example3DMap : MonoBehaviour {
	public Transform camera;
	private Quaternion oriRotation;
	public Transform[] mapButtons;
	public Text buttonResult;
	public Transform[] leftButtons;
	public Transform[] rightButtons;

	void Awake () {
		oriRotation = camera.rotation;
	}

	void OnEnable () {
		StartCoroutine("start");
	}
	IEnumerator start(){
		float degree = 2;
		float duration = 3;
		while(true){

			Quaternion ori = camera.rotation;
			Quaternion goal = oriRotation*Quaternion.Euler(Random.Range(-degree,degree),Random.Range(-degree,degree),0);	
			float timer = 0;
			while(timer<1){
				timer+=Time.deltaTime/duration;
				yield return null;
				float p = Mathf.Cos(Mathf.PI*timer)*-.5f+.5f; 
				camera.rotation = Quaternion.Lerp(ori,goal,p);
			}
		}
	}

	void LateUpdate(){
		foreach(Transform btn in mapButtons){
			btn.transform.rotation = camera.rotation;
		}
	}

	public void changeText(string text){
		buttonResult.text = text;
	}
}
