namespace AssemblyCSharp
{
	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof(IntEditor))]
	public class IntEditorEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			IntEditor intEditor = (IntEditor)target;
			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(intEditor), typeof(IntEditor), false);
			GUI.enabled = true;
			intEditor.Minimum = EditorGUILayout.IntField("Minimum", intEditor.Minimum);
			intEditor.Maximum = EditorGUILayout.IntField("Maximum", intEditor.Maximum);
			intEditor.Value = EditorGUILayout.IntField("Value", intEditor.Value);
		}
	}

}