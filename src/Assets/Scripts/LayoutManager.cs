using System;
using UnityEngine.UI;

namespace AssemblyCSharp
{
	using UnityEngine;

	public class LayoutManager : MonoBehaviour
	{
		public LayoutElement leftSidebar;
		public LayoutElement rightSidebar;

		private float leftSidebarInitialWidth = 0;
		private float rightSidebarInitialWidth = 0;
		
		public void Start()
		{
			leftSidebarInitialWidth = leftSidebar.preferredWidth;
			rightSidebarInitialWidth = rightSidebar.preferredWidth;
		}

		public void SetSidebarsVisible(bool visible)
		{
			if (visible)
			{
				leftSidebar.preferredWidth = leftSidebarInitialWidth;
				rightSidebar.preferredWidth = rightSidebarInitialWidth;
			}
			else
			{
				leftSidebar.preferredWidth = 0;
				rightSidebar.preferredWidth = 0;
			}
		}
	}
}