namespace AssemblyCSharp
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

		void Update()
		{
			if (Input.GetButtonDown("Del"))
			{
				List<NodeUi> selected = GetSelectedNodesList();
				ClearSelection();
				for (int i = selected.Count - 1; i >= 0; i--)
					Destroy(selected[i].gameObject);
			}
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

		public List<NodeUi> GetSelectedNodesList()
		{
			List<NodeUi> list = new List<NodeUi>();
			foreach (NodeUi node in GetSelectedNodes())
				list.Add(node);
			return list;
		}

		public void ClearSelection()
		{
			foreach (NodeUi node in UiManager.GetNodes())
			{
				node.selected = false;
				node.preSelected = false;
			}
			OnSelectionChange.Invoke(box.GetAllSelected());
		}
	}
}