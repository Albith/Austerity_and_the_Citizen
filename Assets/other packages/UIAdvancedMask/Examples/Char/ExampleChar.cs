using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExampleChar : MonoBehaviour {
	public Text hpText;
	public RawImage hpWave;
	public RawImage hpBubbles;
	public Transform hpArea;
	private Vector3 hpAreaOriPos;
	public GameObject[] eyesClose;
	public GameObject[] eyesOpen;
	void Awake(){
		hpAreaOriPos = hpArea.localPosition;
		onSliderChange(1);
	}
	// Use this for initialization
	void Update () {
		Rect hpWaveRect = hpWave.uvRect;
		Rect hpBubblesRect = hpBubbles.uvRect;
		hpWaveRect.x-=Time.deltaTime;
		hpBubblesRect.y-=Time.deltaTime;
		hpWave.uvRect = hpWaveRect;
		hpBubbles.uvRect = hpBubblesRect;
		foreach (GameObject gObj in eyesClose){
			gObj.SetActive(Time.time%2>1.5f);
		}
		foreach (GameObject gObj in eyesOpen){
			gObj.SetActive(Time.time%2<1.5f);
		}
	}
	
	// Update is called once per frame
	public void onSliderChange (float value) {
		hpArea.localPosition  = hpAreaOriPos - new Vector3(0,(1-value)*360,0);
		hpText.text = "POWER: "+Mathf.RoundToInt(value*100)+"/100";
	}
}
