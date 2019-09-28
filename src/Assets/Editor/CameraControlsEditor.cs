namespace AssemblyCSharp
{
	using UnityEditor;

	[CustomEditor(typeof(CameraControls))]
	public class CameraControlsEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			CameraControls controls = (CameraControls)target;
			controls.ZoomLevel = EditorGUILayout.IntField("ZoomLevel", controls.ZoomLevel);
			EditorGUILayout.FloatField("InverseZoom", controls.InverseZoom);
		}
	}

}