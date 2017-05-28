using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ExampleInk : MonoBehaviour {

	public Dictionary<Transform,Vector3> buttonOriPos = new Dictionary<Transform, Vector3>();
	public Dictionary<Transform,Vector3> buttonOriScale = new Dictionary<Transform, Vector3>();
	// Use this for initialization
	public Graphic title;
	public CanvasGroup backButton;
	public Graphic menuBackground;
	public CanvasGroup menuButtons;
	public GameObject alert;
	public Graphic alertBackground;
	public Graphic alertButton;
	public CanvasGroup alertContent;

	private bool isInit ;

	void OnEnable(){
		if(!isInit){
			buttonOriPos.Add(title.transform,title.transform.localPosition);
			buttonOriPos.Add(menuBackground.transform,menuBackground.transform.localPosition);
			buttonOriPos.Add(alertButton.transform,alertButton.transform.localPosition);
			isInit = true;
		}
		alert.SetActive(false);
		//title.canvasRenderer.SetAlpha( 0.0f );
		//menuBackground.canvasRenderer.SetAlpha( 0.0f );
		//title.CrossFadeAlpha(1,2,false);
		//menuBackground.CrossFadeAlpha(1,2,false);

		backButton.alpha = 0;
		StartCoroutine(tweenAlpha(backButton,1,2));

		title.transform.localPosition = buttonOriPos[title.transform]+
			new Vector3(-title.rectTransform.rect.width,0,0);
		StartCoroutine(tweenMove(title.transform,buttonOriPos[title.transform],2));
		menuBackground.transform.localPosition = buttonOriPos[menuBackground.transform]+
			new Vector3(menuBackground.rectTransform.rect.width,0,0);
		StartCoroutine(tweenMove(menuBackground.transform,buttonOriPos[menuBackground.transform],2));
	}


	public void onButtonTap(){
		alert.SetActive(true);
		//alertBackground.canvasRenderer.SetAlpha( 0.0f );
		//alertBackground.CrossFadeAlpha(1,2,false);
		alertBackground.transform.localScale = Vector3.zero;
		StartCoroutine(tweenScale(alertBackground.transform,Vector3.one,2));
		//alertButton.canvasRenderer.SetAlpha( 0.0f );
		//alertButton.CrossFadeAlpha(1,2,false);
		alertButton.transform.localPosition = buttonOriPos[alertButton.transform]+
			new Vector3(alertButton.rectTransform.rect.width,0,0);
		StartCoroutine(tweenMove(alertButton.transform,buttonOriPos[alertButton.transform],2));

	}
	public void onCloseTap(){
		alert.SetActive(false);
	}
	IEnumerator tweenMove(Transform target ,Vector3 localPosition, float duraion = 1){
		float timer = 1;
		while(timer>0){
			yield return null;
			timer-=Time.deltaTime/duraion;
			if(timer>0)target.localPosition = Vector3.Lerp(target.localPosition,localPosition,Time.deltaTime/timer);
			else target.localPosition = localPosition;
		}
	}
	IEnumerator tweenScale(Transform target ,Vector3 localScale, float duraion = 1){
		float timer = 1;
		while(timer>0){
			yield return null;
			timer-=Time.deltaTime/duraion;
			if(timer>0)target.localScale = Vector3.Lerp(target.localScale,localScale,Time.deltaTime/timer);
			else target.localScale = localScale;
		}
	}
	IEnumerator tweenAlpha(CanvasGroup canvasGroup , float alpha, float duraion = 1){
		float timer = 1;
		while(timer>0){
			yield return null;
			timer-=Time.deltaTime/duraion;
			if(timer>0)canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha,alpha,Time.deltaTime/timer);
			else canvasGroup.alpha = alpha;
		}
	}
}
