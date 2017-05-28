using UnityEngine;
using System.Collections;


namespace sunny.mask{
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
public class MaskTextureRenderer : MonoBehaviour {
	public RenderTexture renderTarget;
	public bool renderEveryFrame = true;
	public Shader renderShader;
	private Material renderMaterial;
	public bool debugDisplay;
	// Use this for initialization
	void Start () {
	
	}
	
	void LateUpdate(){
		if(renderEveryFrame || !Application.isPlaying){
			render();
		}
	}
	public void render(){
		if(renderMaterial == null){
			if(renderShader) renderShader = Shader.Find("Particles/Alpha Blended");
			renderMaterial = new Material(renderShader);
		}
		if(renderTarget){
			Matrix4x4 m = UIAdvancedMaskBase.getMatrixOnWorld(GetComponent<RectTransform>());
			RenderTexture.active = renderTarget;
			GL.PushMatrix();
			GL.LoadOrtho();
			GL.Clear(true,true,Color.black);
			foreach(MaskTextureNode node in GetComponentsInChildren<MaskTextureNode>()){
				
				Vector2[] uv = new Vector2[]{
					new Vector2(0,0),
					new Vector2(0,1),
					new Vector2(1,1),
					new Vector2(1,0)
				};
				if( node.sprite){
					
					Rect r = node.sprite.rect;
					float w = node.sprite.texture.width;
					float h = node.sprite.texture.height;
					r.xMin /= w;
					r.yMin/= h;
					r.xMax/= w;
					r.yMax/= h;
					renderMaterial.mainTexture = node.sprite.texture;
					uv=node.sprite.uv;
					uv = new Vector2[]{
						new Vector2(r.xMin,r.yMin),
						new Vector2(r.xMin,r.yMax),
						new Vector2(r.xMax,r.yMax),
						new Vector2(r.xMax,r.yMin)
					};
					/*
					print(node.sprite.uv.Length);
					print(uv[0].x+","+uv[0].y+"\n"+
						uv[1].x+","+uv[1].y+"\n"+
						uv[2].x+","+uv[2].y+"\n"+
						uv[3].x+","+uv[3].y
					);
					*/
				}else if( node.texture){
					renderMaterial.mainTexture = node.texture;
				}else {
					continue;
				}
				renderMaterial.SetPass(0);
				RectTransform rT = node.GetComponent<RectTransform>();
				Vector3[] corners = new Vector3[4];
				rT.GetWorldCorners(corners);
				for(int i = 0 ; i< 4 ; i ++){
					corners[i] = m.MultiplyPoint(corners[i]);
					corners[i].z -=50;
				}
				GL.Begin(GL.TRIANGLES);
				/*
				GL.TexCoord2(0,0);
				GL.Vertex(corners[0]);
				GL.TexCoord2(0,1);
				GL.Vertex(corners[1]);
				GL.TexCoord2(1,1);
				GL.Vertex(corners[2]);
				GL.TexCoord2(0,0);
				GL.Vertex(corners[0]);
				GL.TexCoord2(1,1);
				GL.Vertex(corners[2]);
				GL.TexCoord2(1,0);
				GL.Vertex(corners[3]);
				*/
				GL.TexCoord(uv[0]);
				GL.Vertex(corners[0]);
				GL.TexCoord(uv[1]);
				GL.Vertex(corners[1]);
				GL.TexCoord(uv[2]);
				GL.Vertex(corners[2]);
				GL.TexCoord(uv[0]);
				GL.Vertex(corners[0]);
				GL.TexCoord(uv[2]);
				GL.Vertex(corners[2]);
				GL.TexCoord(uv[3]);
				GL.Vertex(corners[3]);
				GL.End();
			}
			GL.PopMatrix();
			RenderTexture.active = null;

		}
	}
	void OnDrawGizmos(){
		RectTransform rT = GetComponent<RectTransform>();
		Vector3[] corners = new Vector3[4];
		rT.GetWorldCorners(corners);
		Gizmos.DrawLine(corners[0],corners[1]);
		Gizmos.DrawLine(corners[1],corners[2]);
		Gizmos.DrawLine(corners[2],corners[3]);
		Gizmos.DrawLine(corners[3],corners[0]);
		if(renderMaterial == null){
			if(renderShader) renderShader = Shader.Find("Unlit/Transparent");
			renderMaterial = new Material(renderShader);
		}
		if(debugDisplay && renderTarget ){
			renderMaterial.mainTexture = renderTarget;
			renderMaterial.SetPass(0);
			GL.Begin(GL.TRIANGLES);
			GL.TexCoord2(0,0);
			GL.Vertex(corners[0]);
			GL.TexCoord2(0,1);
			GL.Vertex(corners[1]);
			GL.TexCoord2(1,1);
			GL.Vertex(corners[2]);
			GL.TexCoord2(0,0);
			GL.Vertex(corners[0]);
			GL.TexCoord2(1,1);
			GL.Vertex(corners[2]);
			GL.TexCoord2(1,0);
			GL.Vertex(corners[3]);
			GL.End();
		}
	}
}
}
