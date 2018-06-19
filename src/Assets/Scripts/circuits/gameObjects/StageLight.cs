namespace AssemblyCSharp
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
				int alpha = InValue(2);
				int color = ((InValue(1) % 12) + 12) % 12;
				switch (color)
				{
					default:
					case 0:
						SetColor(255, 222, 197, (byte)(InValue(2) * 169 / 100));
						break;
					case 1:
						SetColor(255,  53,  58, (byte)(InValue(2) * 241 / 100));
						break;
					case 2:
						SetColor(255, 127,  66, (byte)(InValue(2) * 241 / 100));
						break;
					case 3:
						SetColor(255, 252,  60, (byte)(InValue(2) * 239 / 100));
						break;
					case 4:
						SetColor(207, 255, 106, (byte)(InValue(2) * 222 / 100));
						break;
					case 5:
						SetColor( 52, 143, 255, (byte)(InValue(2) * 233 / 100));
						break;
					case 6:
						SetColor( 33, 235, 255, (byte)(InValue(2) * 230 / 100));
						break;
					case 7:
						SetColor(  8, 255, 209, (byte)(InValue(2) * 191 / 100));
						break;
					case 8:
						SetColor(255, 158, 233, (byte)(InValue(2) * 242 / 100));
						break;
					case 9:
						SetColor(255,  65, 104, (byte)(InValue(2) * 242 / 100));
						break;
					case 10:
						SetColor(255, 177, 119, (byte)(InValue(2) * 241 / 100));
						break;
					case 11:
						//SetColor(255, 251, 247, (byte)(InValue(2) * 241 / 100)); //Closer to original, but almost invisible on the white background.
						SetColor(245, 245, 237, (byte)(InValue(2) * 255 / 100));
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
