﻿namespace AssemblyCSharp
{
	public class StageLight : CircuitNode
	{
		public class Color
		{
			public byte red;
			public byte green;
			public byte blue;
			public byte alpha;
		}

		public delegate void ColorChangedEventHandler(StageLight source, Color color);
		public event ColorChangedEventHandler ColorChanged = delegate { };

		public readonly Color color;

		public StageLight(CircuitManager manager) : base(manager, 3, 0, false)
		{
			color = new Color();
			color.red = 255;
			color.green = 255;
			color.blue = 255;
			color.alpha = 255;
		}

		protected override void EvaluateOutputs()
		{
			if (!InBool(0))
				SetColor(0, 0, 0, 0);
			else
			{
				//byte alpha = (byte)(InValue(2) * 255 / 100);
				int alpha = InInt(2);
				int color = ((InInt(1) % 12) + 12) % 12;
				switch (color)
				{
					default:
					case 0:
						SetColor(255, 222, 197, (byte)(alpha * 169 / 100));
						break;
					case 1:
						SetColor(255,  53,  58, (byte)(alpha * 241 / 100));
						break;
					case 2:
						SetColor(255, 127,  66, (byte)(alpha * 241 / 100));
						break;
					case 3:
						SetColor(255, 252,  60, (byte)(alpha * 239 / 100));
						break;
					case 4:
						SetColor(207, 255, 106, (byte)(alpha * 222 / 100));
						break;
					case 5:
						SetColor( 52, 143, 255, (byte)(alpha * 233 / 100));
						break;
					case 6:
						SetColor( 33, 235, 255, (byte)(alpha * 230 / 100));
						break;
					case 7:
						SetColor(  8, 255, 209, (byte)(alpha * 191 / 100));
						break;
					case 8:
						SetColor(255, 158, 233, (byte)(alpha * 242 / 100));
						break;
					case 9:
						SetColor(255,  65, 104, (byte)(alpha * 242 / 100));
						break;
					case 10:
						SetColor(255, 177, 119, (byte)(alpha * 241 / 100));
						break;
					case 11:
						//SetColor(255, 251, 247, (byte)(alpha * 241 / 100)); //Closer to original, but almost invisible on the white background.
						SetColor(245, 245, 237, (byte)(alpha * 255 / 100));
						break;
				}
			}
		}

		private void SetColor(byte red, byte green, byte blue, byte alpha)
		{
			if (color.red == red && color.green == green && color.blue == blue && color.alpha == alpha)
				return;
			color.red = red;
			color.green = green;
			color.blue = blue;
			color.alpha = alpha;
			ColorChanged(this, color);
		}
	}
}
