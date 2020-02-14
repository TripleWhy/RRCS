using System;
using UnityEngine.UI;

namespace AssemblyCSharp
{
	using UnityEngine;

	public class LayoutManager : MonoBehaviour
	{
		public LayoutElement leftSidebar;
		public LayoutElement rightSidebar;
		public LayoutElement leftSidebarScrollbar;
		public LayoutElement rightSidebarScrollbar;
		public RectTransform selectionArea;
		public Toggle sidebarToggle;

		private float leftSidebarInitialWidth = 0;
		private float rightSidebarInitialWidth = 0;
		private float leftSidebarScrollbarInitialWidth = 0;
		private float rightSidebarScrollbarInitialWidth = 0;

		public void Start()
		{
			leftSidebarInitialWidth = leftSidebar.preferredWidth;
			rightSidebarInitialWidth = rightSidebar.preferredWidth;
			leftSidebarScrollbarInitialWidth = leftSidebarScrollbar.preferredWidth;
			rightSidebarScrollbarInitialWidth = rightSidebarScrollbar.preferredWidth;

			if (Screen.width < 600)
			{
				// Hide Sidebars on small screens
				sidebarToggle.isOn = false;
			}
		}

		public void SetSidebarsVisible(bool visible)
		{
			if (visible)
			{
				leftSidebar.preferredWidth = leftSidebarInitialWidth;
				rightSidebar.preferredWidth = rightSidebarInitialWidth;
				leftSidebarScrollbar.preferredWidth = leftSidebarScrollbarInitialWidth;
				rightSidebarScrollbar.preferredWidth = rightSidebarScrollbarInitialWidth;
				
				selectionArea.offsetMin = new Vector2(leftSidebarInitialWidth, selectionArea.offsetMin.y);
				selectionArea.offsetMax = new Vector2(rightSidebarInitialWidth, selectionArea.offsetMax.y);
			}
			else
			{
				leftSidebar.preferredWidth = 0;
				rightSidebar.preferredWidth = 0;
				leftSidebarScrollbar.preferredWidth = 0;
				rightSidebarScrollbar.preferredWidth = 0;
				
				selectionArea.offsetMin = new Vector2(0, selectionArea.offsetMin.y);
				selectionArea.offsetMax = new Vector2(0, selectionArea.offsetMax.y);
			}
		}
	}
}