using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp
{
	public class ObjectWidthToggle : ImageToggle
	{
		public LayoutElement[] ObjectsToToggle;

		private float[] initialWidths;

		public Toggle.ToggleEvent onObjectToggled = new Toggle.ToggleEvent();

		protected override void Start()
		{
			base.Start();
			toggle.onValueChanged.AddListener(OnToggleValueChanged);

			initialWidths = ObjectsToToggle.Select(layout => layout.preferredWidth).ToArray();
		}

		private void OnToggleValueChanged(bool value)
		{
			for (int i = 0; i < ObjectsToToggle.Length; i++)
			{
				ObjectsToToggle[i].preferredWidth = value ? initialWidths[i] : 0;
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