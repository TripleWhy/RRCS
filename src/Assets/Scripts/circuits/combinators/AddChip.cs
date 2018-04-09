﻿namespace AssemblyCSharp
{
	public class AddChip : Chip
	{
		public AddChip() : base(3, 1, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 9;
			}
		}

		override public void Evaluate()
		{
			if (ToBool(inputPorts[4]))
				outputPorts[0].Value = 0;
			else
				outputPorts[0].Value = ToInt(inputPorts[0]) + ToInt(inputPorts[1]) + ToInt(inputPorts[2]);
		}
	}
}