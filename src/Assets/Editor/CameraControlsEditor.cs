﻿namespace AssemblyCSharp
{
	using System;
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(CameraControls))]
	public class CameraControlsEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			CameraControls controls = (CameraControls)target;
			controls.InverseZoom = EditorGUILayout.FloatField("InverseZoom", controls.InverseZoom);
		}
	}

}