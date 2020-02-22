using System;
using UnityEngine.UI;

namespace AssemblyCSharp
{
	using UnityEngine;

	public class LayoutManager : MonoBehaviour
	{
		public RectTransform leftSidebarGroup;
		public RectTransform rightSidebarGroup;
		public Toggle leftHideSidebarToggle;
		public Toggle rightHideSidebarToggle;

		public void Start()
		{
			if (Screen.width < 600)
			{
				// Hide Sidebars on small screens
				leftHideSidebarToggle.isOn = false;
				rightHideSidebarToggle.isOn = false;
			}
		}

		/**
		 * Returns the width of the sidebar + scrollbar
		 */
		public float GetLeftSidebarWidth()
		{
			return leftSidebarGroup.rect.width;
		}
		
		
		/**
		 * Returns the width of the sidebar + scrollbar
		 */
		public float GetRightSidebarWidth()
		{
			return rightSidebarGroup.rect.width;
		}
	}
}