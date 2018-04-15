namespace AssemblyCSharp
{
	using UnityEngine;
	using UnityEngine.UI;

	public class Accordion : MonoBehaviour
	{
		public AccordionSection[] sections;

		// Use this for initialization
		void Start()
		{
			foreach (AccordionSection section in sections)
				section.sectionButton.onClick.AddListener(delegate { ExpandSection(section); });
			if (sections.Length > 0)
				ExpandSection(sections[0]);
		}

		private void ExpandSection(AccordionSection section)
		{
			foreach (AccordionSection s in sections)
				s.content.SetActive(object.ReferenceEquals(s, section));
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
		}
	}
}
