using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace sunny.mask{
	[RequireComponent(typeof(RectTransform))]
public class UIAdvancedMaskBase : MonoBehaviour {
	public Texture texture;
	//public Rect rect;
	[Range(0,1)]
	public float alpha = 1;
	public bool updateEveryFrame;
	public bool UseRedAsAlpha;
	public bool flippedAlpha;
	public bool forceClampX;
	public bool forceClampY;
	public static Matrix4x4 getMatrixOnWorld(RectTransform target){
		Vector3[] corners = new Vector3[4];
		target.GetWorldCorners (corners);

		Matrix4x4 m = Matrix4x4.TRS (Vector3.zero, Quaternion.identity,new Vector3(Vector3.Distance(corners[0],corners[3]),Vector3.Distance(corners[0],corners[1]),1)).inverse*
			Matrix4x4.TRS (corners [0],target.rotation, Vector3.one).inverse
			;
		return m;

	}
	public static Matrix4x4 getMatrixOnCanvas(RectTransform target, Canvas canvas){

		Vector3[] corners = new Vector3[4];
		target.GetWorldCorners (corners);
		for(int i = 0 ; i<corners.Length ; i++){
			corners [i] = canvas.transform.InverseTransformPoint(corners[i]);
		}

		Matrix4x4 m = Matrix4x4.TRS (Vector3.zero, Quaternion.identity,new Vector3(Vector3.Distance(corners[0],corners[3]),Vector3.Distance(corners[0],corners[1]),1)).inverse*
			Matrix4x4.TRS (corners [0],Quaternion.Inverse(canvas.transform.rotation)*target.rotation, Vector3.one).inverse
			;
		return m;
	}

	protected void insertDataToMaterial(List<Material> materials,Texture mask,Matrix4x4 matrix ){
		
		foreach (Material mat in materials) {
			
				mat.SetMatrix ("_Projection", matrix);
				mat.SetTexture ("_MaskTex",mask);
				mat.SetFloat("_AlphaMultiply",alpha);

				if (UseRedAsAlpha)
					mat.EnableKeyword ("RED_AS_ALPHA");
				else
					mat.DisableKeyword ("RED_AS_ALPHA");
				if (flippedAlpha)
					mat.EnableKeyword ("ALPHA_FLIPPED");
				else
					mat.DisableKeyword ("ALPHA_FLIPPED");
				if (forceClampX)
					mat.EnableKeyword ("FORCE_CLAMP_X");
				else
					mat.DisableKeyword ("FORCE_CLAMP_X");
				if (forceClampY)
					mat.EnableKeyword ("FORCE_CLAMP_Y");
				else
					mat.DisableKeyword ("FORCE_CLAMP_Y");
			

		}
	}
	protected void disabledDataToMaterial(List<Material> materials ){
		foreach (Material mat in materials) {
			
				mat.SetTexture ("_MaskTex",null);
				mat.SetFloat("_AlphaMultiply",alpha);
				mat.DisableKeyword ("ALPHA_FLIPPED");

		}
	}


	public static Color getColorFromPixel(Texture texture , float x, float y){

		if (texture is Texture2D) {
			try {
				Color result = (texture as Texture2D).GetPixelBilinear (x, y);
				return result ;
			} catch (System.Exception e) {
			}
		}

		try {
			RenderTexture testRT = RenderTexture.GetTemporary(1,1,0);
			Texture2D testTex = new Texture2D(1,1);
			RenderTexture.active = testRT;
			GL.Clear (true, true, Color.clear);
			GL.LoadIdentity();
			GL.LoadProjectionMatrix(Matrix4x4.Ortho(0,1,1,0,-1,1));
			Graphics.DrawTexture(new Rect(1-x*1024,y*1024-(1024-1),1024,1024), texture);
			testTex.ReadPixels (new Rect (0, 0, 1, 1), 0, 0);
			RenderTexture.active = null;
			Color result = testTex.GetPixel (0, 0);
				RenderTexture.ReleaseTemporary(testRT);
			return result ;
		} catch (System.Exception e) {
		}
		return Color.white ;
	}

	protected static Material setMaterialToUI(Graphic graphic,Material material){
		graphic.material = material;

		return null;
		if(graphic.material == Graphic.defaultGraphicMaterial || graphic.material == material ){
			graphic.material = material;

			return null;
		}else{
			return material;
		}
	}
}
}
