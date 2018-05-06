namespace AssemblyCSharp
{
	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof(IntEditor))]
	public class IntEditorEditor : Editor
	{
		public SerializedProperty onValueChangedProp;

		void OnEnable()
		{
			onValueChangedProp = serializedObject.FindProperty("onValueChanged");
		}

		public override void OnInspectorGUI()
		{
			IntEditor intEditor = (IntEditor)target;
			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(intEditor), typeof(IntEditor), false);
			GUI.enabled = true;
			intEditor.Minimum = EditorGUILayout.IntField("Minimum", intEditor.Minimum);
			intEditor.Maximum = EditorGUILayout.IntField("Maximum", intEditor.Maximum);
			intEditor.Value = EditorGUILayout.IntField("Value", intEditor.Value);

			EditorGUILayout.PropertyField(onValueChangedProp);
			if (GUI.changed)
				serializedObject.ApplyModifiedProperties();
		}
	}
}
