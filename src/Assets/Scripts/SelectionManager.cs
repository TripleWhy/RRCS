﻿namespace AssemblyCSharp
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI.Extensions;

	public class SelectionManager : MonoBehaviour
	{
		private SelectionBox box;

		void Awake()
		{
			box = GetComponent<SelectionBox>();
			SetSelectables(UiManager.GetSelectables());
		}

		public SelectionBox.SelectionEvent OnSelectionChange
		{
			get
			{
				return box.onSelectionChange;
			}
		}

		public bool SelectionEnabled
		{
			get
			{
				return box.gameObject.activeSelf;
			}
			set
			{
				box.gameObject.SetActive(value);
			}
		}

		public void SetSelectables(IEnumerable<MonoBehaviour> behaviours)
		{
			if (box == null)
				return;
			box.SetSelectableGroup(behaviours);
		}

		public IEnumerable<NodeUi> GetSelectedNodes()
		{
			foreach (NodeUi node in UiManager.GetNodes())
				if (node.selected)
					yield return node;
		}
	}
}