using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp
{
	public class ObjectToggle : ImageToggle
	{
		public GameObject[] ObjectsToToggle;

		public Toggle.ToggleEvent onObjectToggled = new Toggle.ToggleEvent();

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

			StartCoroutine(CallbackAfterLayoutRebuild(value));
		}

		private IEnumerator CallbackAfterLayoutRebuild(bool value)
		{
			yield return new WaitForEndOfFrame();
			onObjectToggled.Invoke(value);
		}
	}
}