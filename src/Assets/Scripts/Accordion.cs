using System.Collections;

namespace AssemblyCSharp
{
    using UnityEngine;
    using UnityEngine.UI;

    public class Accordion : MonoBehaviour
    {
        public AccordionSection[] sections;

        void Start()
        {
            int i = 0;
            foreach (AccordionSection section in sections)
            {
                section.sectionButton.onClick.AddListener(delegate { ExpandSection(section); });
                section.transform.SetSiblingIndex(i++);
            }

            if (sections.Length > 0)
                ExpandSection(sections[0]);
        }

        private void ExpandSection(AccordionSection section)
        {
            foreach (AccordionSection s in sections)
            {
                if (ReferenceEquals(s, section))
                {
                    s.contentLayout.transform.localScale = new Vector3(1, 1, 1);
                    s.contentLayout.minHeight = -1;
                    s.contentLayout.preferredHeight = -1;
                    s.GetComponent<VerticalLayoutGroup>().spacing = 3;
                }
                else
                {
                    s.contentLayout.transform.localScale = new Vector3(0, 0, 0);
                    s.contentLayout.minHeight = 0;
                    s.contentLayout.preferredHeight = 0;
                    s.GetComponent<VerticalLayoutGroup>().spacing = 0;
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) transform);
        }
    }
}