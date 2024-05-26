using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp5.Objects
{
	class GreenCircle : BaseObject
	{
		public GreenCircle(float x, float y, float angle) : base(x, y, angle)
		{
		}

		public override void Render(Graphics g)
		{
			g.FillEllipse(new SolidBrush(Color.Green), -25, -25, 50, 50);
			g.DrawEllipse(new Pen(Color.Black, 2), -25, -25, 50, 50);
		}

		public override GraphicsPath GetGraphicsPath()
		{
			var path = base.GetGraphicsPath();
			path.AddEllipse(-25, -25, 50, 50);
			return path;
		}
	}
}