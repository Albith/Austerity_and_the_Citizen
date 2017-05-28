using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*
 * 
 * This only works with RawImage and Image with simple ImageType
 * If you want something that can work with other ImageType, you can try some plugin like this (not done by me):
 * https://www.assetstore.unity3d.com/en/#!/content/28601
 * 
 */

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Graphic))]
public class SimpleRaycastButtonMask : MonoBehaviour, ICanvasRaycastFilter
{
	private static RenderTexture testRT ;
	private static Texture2D testTex;

	public static Color currentColor;

	[Range(0,1)]
	public float alphaGreatThan = 0.5f;

	private Image image;
	private RawImage rawImage;

	void OnEnable(){
		image = GetComponent<Image>();
		rawImage = GetComponent<RawImage>();
	}

	public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
	{
		var rectTransform = (RectTransform)transform;
		Vector2 localPositionPivotRelative;
		RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) transform, sp, eventCamera, out localPositionPivotRelative);

		float x = 0;
		float y = 0;
		Color result = Color.white;
		//get uv
		x = localPositionPivotRelative.x / rectTransform.rect.width;
		y = localPositionPivotRelative.y / rectTransform.rect.height;

		x += rectTransform.pivot.x;
		y += rectTransform.pivot.y;

		Texture tex = null;
		bool isDone = false;
		if(image){

			tex = image.sprite.texture;
			if (tex == null) return true;
			Rect r = image.sprite.rect;
			if(image.preserveAspect && image.type == Image.Type.Simple ||  image.type == Image.Type.Filled){
				if(rectTransform.rect.height/(float)r.height< rectTransform.rect.width/(float)r.width){

					x -=.5f;
					x *=(rectTransform.rect.width/(float)rectTransform.rect.height)/(r.width/(float)r.height);
					x +=.5f;

				}else if(rectTransform.rect.height/(float)r.height> rectTransform.rect.width/(float)r.width){
					y -=.5f;
					y *=(rectTransform.rect.height/(float)rectTransform.rect.width)/(r.height/(float)r.width);
					y +=.5f;

				}
				x = Mathf.Clamp(x,0,1);
				y = Mathf.Clamp(y,0,1);
			}

			x  = (x*r.width+r.x)/image.sprite.texture.width;
			y  = (y*r.height+r.y)/image.sprite.texture.height;



		}else if(rawImage){
			tex = rawImage.texture;
			if (tex == null) return true;
			Rect r = rawImage.uvRect;
			x  = (x*r.width+r.x);
			y  = (y*r.height+r.y);
		}
		if(tex && tex is Texture2D){
			try{
				result = (tex as Texture2D).GetPixelBilinear (x, y);
				isDone = true;
			}catch(System.Exception e){
			}
		}
		if(!isDone){
			if(testRT == null)testRT = RenderTexture.GetTemporary(1,1,0);
			if(testTex == null)testTex = new Texture2D(1,1);
			RenderTexture.active = testRT;
			GL.Clear (true, true, Color.clear);
			GL.LoadIdentity();
			GL.LoadProjectionMatrix(Matrix4x4.Ortho(0,1,1,0,-1,1));
			Graphics.DrawTexture(new Rect(1-x*1024,y*1024-(1024-1),1024,1024), tex);
			testTex.ReadPixels (new Rect (0, 0, 1, 1), 0, 0);
			result = testTex.GetPixel (0, 0);
			RenderTexture.ReleaseTemporary(testRT);
			RenderTexture.active = null;
		}


		currentColor = result;
		return result.a > alphaGreatThan;

	}
	public float AngleDir(float x1, float y1, float x2, float y2, bool clockwise)
	{
			float dot = x1*x2 + y1*y2 ;
		float det = x1*y2 - y1*x2  ;
		float angle = Mathf.Atan2(det, dot);
		if(clockwise)angle = -angle;
		if(angle<0)angle+=Mathf.PI*2;

		return angle;
	}  
}
