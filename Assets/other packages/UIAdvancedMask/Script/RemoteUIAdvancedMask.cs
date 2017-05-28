using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace sunny.mask{
	[ExecuteInEditMode]
public class RemoteUIAdvancedMask : UIAdvancedMaskBase {

	private Material uiMaterial;
	private Material objectMaterial;

	public RectTransform[] targets;
	public bool addToChildren;
	private List<Material> uiMaterials = new List<Material>();
	private List<Material> worldMaterials = new List<Material>();


	private Matrix4x4 uiMatrix;
	private Matrix4x4 worldMatrix;
	// Use this for initialization
	void OnEnable () {

		updateMaskedObjects ();
		updateMaskMaterials();
	}
	
	// Update is called once per frame
	void Update () {
		if(Application.isPlaying){
			if (updateEveryFrame){
				updateMaskedObjects ();
				updateMaskMaterials();
			}
		}else{
			updateMaskedObjects ();
			updateMaskMaterials();
		}
	
	}

	public void updateMaskedObjects(){
		if (uiMaterial == null) {
			uiMaterial = new Material (Shader.Find ("UI/Advanced Masked"));
		}
		if (objectMaterial == null) {
			objectMaterial = new Material (Shader.Find ("Unlit/UIAdvancedMask-3D"));
		}
		uiMaterials.Clear();
		uiMaterials.Add (uiMaterial);
		if(targets != null&& targets.Length>0)foreach (RectTransform rT in targets) {
			if (addToChildren) {
				foreach (Graphic g in rT.GetComponentsInChildren<Graphic>(true)){
					Material ma = setMaterialToUI (g, uiMaterial);
					if(ma )uiMaterials.Add (ma);
				}
				foreach(Renderer t in GetComponentsInChildren<Renderer>()){

					worldMaterials.AddRange(t.sharedMaterials);
				}
			} else {
				if( GetComponent<Graphic>())
					setMaterialToUI (GetComponent<Graphic>(), uiMaterial);
			}
		}
	}
	public void updateMaskMaterials(){
		Canvas canvas = GetComponentInParent<Canvas> ();
		RectTransform rT = GetComponent<RectTransform> ();
		if(canvas){
			uiMatrix = UIAdvancedMaskBase.getMatrixOnCanvas (rT,canvas);
			insertDataToMaterial (uiMaterials,texture,uiMatrix);
		}
		worldMatrix = UIAdvancedMaskBase.getMatrixOnWorld (rT);
		insertDataToMaterial (worldMaterials,texture,worldMatrix);

	}
	void OnDisable () {
		disabledDataToMaterial(uiMaterials);
		disabledDataToMaterial(worldMaterials);
	}
}
}
