using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class ObjectToggle : ImageToggle
	{
		public GameObject[] ObjectsToToggle;
		
		protected override void Start()
		{
			base.Start();
			toggle.onValueChanged.AddListener(OnToggleValueChanged);
		}

		private void OnToggleValueChanged(bool value)
		{
			foreach (var obj in ObjectsToToggle)
			{
				obj.SetActive(value);
			}
		}
	}
}