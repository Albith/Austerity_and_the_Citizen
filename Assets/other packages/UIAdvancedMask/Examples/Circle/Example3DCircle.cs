using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Example3DCircle : MonoBehaviour {
	public Graphic gradient1;
	public Graphic gradient2;
	public CanvasGroup menuA;
	public Color menuAColor1 =Color.white;
	public Color menuAColor2 =Color.white;
	public CanvasGroup menuB;
	public Color menuBColor1 =Color.white;
	public Color menuBColor2 =Color.white;
	public CanvasGroup menuC;
	public Color menuCColor1 =Color.white;
	public Color menuCColor2 =Color.white;
	// Use this for initialization
	public Transform[] leftButtons;
	public Transform[] rightButtons;
	public Dictionary<Transform,Vector3> buttonOriPos = new Dictionary<Transform, Vector3>();
	private CanvasGroup current;
	void Awake () {
		buttonOriPos = new Dictionary<Transform, Vector3>();
		foreach( Transform tr in leftButtons){
			buttonOriPos.Add(tr,tr.localPosition);
		}
		foreach( Transform tr in rightButtons){
			buttonOriPos.Add(tr,tr.localPosition);
		}
		
	}

	void OnEnable(){

		foreach( Transform tr in leftButtons){
			tr.localPosition = buttonOriPos[tr]+new Vector3(50,0,0);
			StartCoroutine(tweenMove(tr,buttonOriPos[tr],1));
		}
		foreach( Transform tr in rightButtons){
			tr.localPosition = buttonOriPos[tr]+new Vector3(-50,0,0);
			StartCoroutine(tweenMove(tr,buttonOriPos[tr],1));
		}
		onATap ();
	}
	
	// Update is called once per frame
	public void onATap () {
		menuA.gameObject.SetActive(true);
		menuB.gameObject.SetActive(false);
		menuC.gameObject.SetActive(false);
		current = menuA;
		gradient1.CrossFadeColor(menuAColor1,.5f,false,false);
		gradient2.CrossFadeColor(menuAColor2,.5f,false,false);
		StopCoroutine("tweenAlpha");
		StartCoroutine("tweenAlpha");

	}
	public void onBTap () {
		menuA.gameObject.SetActive(false);
		menuB.gameObject.SetActive(true);
		menuC.gameObject.SetActive(false);
		current = menuB;
		gradient1.CrossFadeColor(menuBColor1,.5f,false,false);
		gradient2.CrossFadeColor(menuBColor2,.5f,false,false);
		StopCoroutine("tweenAlpha");
		StartCoroutine("tweenAlpha");

	}
	public void onCTap () {
		menuA.gameObject.SetActive(false);
		menuB.gameObject.SetActive(false);
		menuC.gameObject.SetActive(true);
		current = menuC;
		gradient1.CrossFadeColor(menuCColor1,.5f,false,false);
		gradient2.CrossFadeColor(menuCColor2,.5f,false,false);
		StopCoroutine("tweenAlpha");
		StartCoroutine("tweenAlpha");
	}
	IEnumerator tweenAlpha(){
		float timer = 1;
		current.alpha = 0;
		while(timer>0){
			yield return null;
			timer-=Time.deltaTime;
			current.alpha = 1-timer;
		}
	}
	/*
	void Update(){
		Color c1;
		Color c2;
		if(current == menuA){
			c1 = menuAColor1;
			c2 = menuAColor2;
		}else if(current == menuA){
			c1 = menuBColor1;
			c2 = menuBColor2;
		}else {
			c1 = menuBColor1;
			c2 = menuBColor2;
		}
		gradient1.CrossFadeColor
	}
	*/

	IEnumerator tweenMove(Transform target ,Vector3 localPosition, float duraion = 1){
		float timer = 1;
		while(timer>0){
			yield return null;
			timer-=Time.deltaTime;
			if(timer>0)target.localPosition = Vector3.Lerp(target.localPosition,localPosition,Time.deltaTime/timer);
			else target.localPosition = localPosition;
		}
	}
}
