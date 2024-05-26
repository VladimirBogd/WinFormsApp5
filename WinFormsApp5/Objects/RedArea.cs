using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp5.Objects
{
	class RedArea : BaseObject
	{
		private int size = 0;
		public RedArea(float x, float y, float angle, int size) : base(x, y, angle)
		{
			this.size = size;
        }
        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public override void Render(Graphics g)
		{
			g.FillEllipse(new SolidBrush(Color.LightPink), -size/2, -size/2, size, size);
		}

		public override GraphicsPath GetGraphicsPath()
		{
			var path = base.GetGraphicsPath();
			path.AddEllipse(-size / 2, -size / 2, size, size);
			return path;
		}
	}
}
