using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp5.Objects
{
	class BaseObject
	{
		public float X;
		public float Y;
		public float Angle;
		// добавил поле делегат, к которому можно будет привязать реакцию на собыития
		public Action<BaseObject, BaseObject> OnOverlap;

		public BaseObject(float x, float y, float angle)
		{
			X = x;
			Y = y;
			Angle = angle;
		}

		public Matrix GetTransform()
		{
			var matrix = new Matrix();
			matrix.Translate(X, Y);
			matrix.Rotate(Angle);

			return matrix;
		}

		public virtual void Render(Graphics g) // виртуальный метод для отрисовки
		{
		}

		public virtual GraphicsPath GetGraphicsPath()
		{
			return new GraphicsPath();
		}
		public virtual bool Overlaps(BaseObject obj, Graphics g)
		{
			var path1 = this.GetGraphicsPath();
			var path2 = obj.GetGraphicsPath();

			path1.Transform(this.GetTransform());
			path2.Transform(obj.GetTransform());

			// используем класс Region, который позволяет определить пересечение объектов в данном графическом контексте
			var region = new Region(path1);
			region.Intersect(path2); // пересекаем формы
			return !region.IsEmpty(g); // если полученная форма не пуста то значит было пересечение
		}
		public virtual void Overlap(BaseObject obj)
		{
			if (this.OnOverlap != null) // если есть привязанные функции к полю
			{
				this.OnOverlap(this, obj); // то вызываем их
			}
		}
	}
}
