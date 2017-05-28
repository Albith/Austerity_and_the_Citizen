using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class UIAdvancedMask3DGUI : ShaderGUI {

	public enum BlendMode
	{
		Normal,
		Additive
	}
	public static readonly string[] blendNames = Enum.GetNames (typeof (BlendMode));
	MaterialProperty mainTexture = null;
	MaterialProperty mainColor = null;
	MaterialProperty blendMode;
	MaterialProperty zWrite;
	MaterialEditor m_MaterialEditor;
	bool m_FirstTimeApply;

	public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] props)
	{
		FindProperties (props); 

		m_MaterialEditor = materialEditor;
		Material material = materialEditor.target as Material;

		if (m_FirstTimeApply)
		{
			MaterialChanged(material,(BlendMode)material.GetFloat("_Mode"));
			m_FirstTimeApply = false;
		}
		// Detect any changes to the material
		EditorGUI.BeginChangeCheck();
		{
			BlendModePopup();
			m_MaterialEditor.ColorProperty(mainColor,"Color");
			m_MaterialEditor.TextureProperty(mainTexture,"Texture") ;

			material.SetFloat("_ZWrite",EditorGUILayout.Toggle("ZWrite",material.GetFloat("_ZWrite") == 1)?1:0);
		}
		if (EditorGUI.EndChangeCheck())
		{
			MaterialChanged(material,(BlendMode)material.GetFloat("_Mode"));
		}
	}
	public void FindProperties (MaterialProperty[] props)
	{
		blendMode = FindProperty ("_Mode", props);
		mainTexture =  FindProperty ("_MainTex", props);
		mainColor =  FindProperty ("_Color", props);
		zWrite =  FindProperty ("_ZWrite", props);
		
	}
	public override void AssignNewShaderToMaterial (Material material, Shader oldShader, Shader newShader)
	{
		base.AssignNewShaderToMaterial(material, oldShader, newShader);
		BlendMode blendMode = BlendMode.Normal;
		material.SetFloat("_Mode", (float)blendMode);
	}
	static void MaterialChanged(Material material,BlendMode blendMode)
	{
		switch (blendMode)
		{
		case BlendMode.Normal:
			material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			break;
		case BlendMode.Additive:
			material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
			break;
		}
	}

	void BlendModePopup()
	{
		EditorGUI.showMixedValue = blendMode.hasMixedValue;
		var mode = (BlendMode)blendMode.floatValue;

		EditorGUI.BeginChangeCheck();
		mode = (BlendMode)EditorGUILayout.Popup("Blend Mode", (int)mode, blendNames);
		if (EditorGUI.EndChangeCheck())
		{
			m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
			blendMode.floatValue = (float)mode;
		}

		EditorGUI.showMixedValue = false;
	}
}
