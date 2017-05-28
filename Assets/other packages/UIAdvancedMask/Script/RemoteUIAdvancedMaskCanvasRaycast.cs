using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace sunny.mask{
public class RemoteUIAdvancedMaskCanvasRaycast : MonoBehaviour, ICanvasRaycastFilter {
	public RemoteUIAdvancedMask target;
	// Use this for initialization

	public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
	{
		if (target == null)
			return true;
		RectTransform rectTransform = target.GetComponent<RectTransform>();
		Vector2 localPositionPivotRelative;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, sp, eventCamera, out localPositionPivotRelative);

		float x = 0;
		float y = 0;

		x = localPositionPivotRelative.x / rectTransform.rect.width;
		y = localPositionPivotRelative.y / rectTransform.rect.height;

		x += rectTransform.pivot.x;
		y += rectTransform.pivot.y;

		if (target.forceClampX)
			x = Mathf.Clamp01 (x);
		if (target.forceClampY)
			y = Mathf.Clamp01 (y);


		Color result =UIAdvancedMaskBase.getColorFromPixel(target.texture,x,y);
		float a = target.UseRedAsAlpha ? result.r : result.a;
		if (target.flippedAlpha)
			a = 1 - a;

		a*=target.alpha;
		return a > .5f;
	}
}
}
