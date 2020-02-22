using System;
using UnityEngine.UI;

namespace AssemblyCSharp
{
	using UnityEngine;

	public class LayoutManager : MonoBehaviour
	{
		public Canvas uiCanvas;
		public RectTransform leftSidebarGroup;
		public RectTransform rightSidebarGroup;
		public RectTransform selectionArea;
		public RectTransform collapseButtonRight;
		public Toggle leftHideSidebarToggle;
		public Toggle rightHideSidebarToggle;

		public void Start()
		{
			if (Screen.width < 600)
			{
				// Hide Sidebars on small screens
				CollapseSidebars(true, true);
			}
		}

		private float GetAdjustedWidth(RectTransform rect)
		{
			return RectTransformUtility.PixelAdjustRect(rect, uiCanvas).width * uiCanvas.transform.localScale.x;
		}

		/**
		 * Returns the width of the sidebar + scrollbar in adjusted screen-pixel coordinates
		 */
		public float GetAdjustedLeftSidebarWidth()
		{
			return GetAdjustedWidth(leftSidebarGroup);
		}


		/**
		 * Returns the width of the sidebar + scrollbar in adjusted screen-pixel coordinates
		 */
		public float GetAdjustedRightSidebarWidth()
		{
			return GetAdjustedWidth(rightSidebarGroup);
		}

		/**
		 * Returns the width of the inner selection area in adjusted screen-pixel coordinates
		 */
		public float GetAdjustedSelectionAreaWidth()
		{
			return GetAdjustedWidth(selectionArea);
		}

		public float GetCollapseButtonWidth()
		{
			return GetAdjustedWidth(collapseButtonRight);
		}

		public void OnRightSidebarExpandChanged(bool expanded)
		{
			if (expanded && GetAdjustedSelectionAreaWidth() <= 100)
			{
				CollapseSidebars(true, false);
			}
		}

		public void OnLeftSidebarExpandChanged(bool expanded)
		{
			if (expanded && GetAdjustedSelectionAreaWidth() <= 100)
			{
				CollapseSidebars(false, true);
			}
		}

		public void CollapseSidebars(bool left, bool right)
		{
			if (left)
				leftHideSidebarToggle.isOn = false;
			if (right)
				rightHideSidebarToggle.isOn = false;
		}
	}
}
