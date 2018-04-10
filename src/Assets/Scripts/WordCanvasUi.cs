namespace AssemblyCSharp
{
	using UnityEngine;

	public class WordCanvasUi : MonoBehaviour
	{
		public readonly CircuitManager manager = new CircuitManager();

		void FixedUpdate()
		{
			manager.EvaluateIfNecessary();
		}
	}
}