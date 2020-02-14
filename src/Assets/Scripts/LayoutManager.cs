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
		public Toggle leftHideSidebarToggle;
		public Toggle rightHideSidebarToggle;

		private float leftSidebarInitialWidth = 0;
		private float rightSidebarInitialWidth = 0;
		private float leftSidebarScrollbarInitialWidth = 0;
		private float rightSidebarScrollbarInitialWidth = 0;

		private bool isLeftSidebarVisible = true;
		private bool isRightSidebarVisible = true;

		public void Start()
		{
			leftSidebarInitialWidth = leftSidebar.preferredWidth;
			rightSidebarInitialWidth = rightSidebar.preferredWidth;
			leftSidebarScrollbarInitialWidth = leftSidebarScrollbar.preferredWidth;
			rightSidebarScrollbarInitialWidth = rightSidebarScrollbar.preferredWidth;

			leftHideSidebarToggle.onValueChanged.AddListener(OnSetLeftSidebarVisible);
			rightHideSidebarToggle.onValueChanged.AddListener(OnSetRightSidebarVisible);

			if (Screen.width < 600)
			{
				// Hide Sidebars on small screens
				leftHideSidebarToggle.isOn = false;
				rightHideSidebarToggle.isOn = false;
			}
		}

		private void OnSetLeftSidebarVisible(bool visible)
		{
			if (visible)
			{
				leftSidebar.preferredWidth = leftSidebarInitialWidth;
				leftSidebarScrollbar.preferredWidth = leftSidebarScrollbarInitialWidth;
			}
			else
			{
				leftSidebar.preferredWidth = 0;
				leftSidebarScrollbar.preferredWidth = 0;
			}

			isLeftSidebarVisible = visible;
			UpdateSelectionAreaOffsets();
		}

		private void OnSetRightSidebarVisible(bool visible)
		{
			if (visible)
			{
				rightSidebar.preferredWidth = rightSidebarInitialWidth;
				rightSidebarScrollbar.preferredWidth = rightSidebarScrollbarInitialWidth;
			}
			else
			{
				rightSidebar.preferredWidth = 0;
				rightSidebarScrollbar.preferredWidth = 0;
			}

			isRightSidebarVisible = visible;
			UpdateSelectionAreaOffsets();
		}

		private void UpdateSelectionAreaOffsets()
		{
			if (isLeftSidebarVisible)
				selectionArea.offsetMin = new Vector2(leftSidebarInitialWidth + leftSidebarScrollbarInitialWidth,
					selectionArea.offsetMin.y);
			else
				selectionArea.offsetMin = new Vector2(0, selectionArea.offsetMin.y);

			if (isRightSidebarVisible)
				selectionArea.offsetMax = new Vector2(-rightSidebarInitialWidth - rightSidebarScrollbarInitialWidth,
					selectionArea.offsetMax.y);
			else
				selectionArea.offsetMax = new Vector2(0, selectionArea.offsetMax.y);
		}
	}
}