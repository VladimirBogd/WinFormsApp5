using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Forms;
using WinFormsApp5.Objects;

namespace WinFormsApp5
{
	public partial class Form1 : Form
	{
		List<BaseObject> objects = new();
		Player player;
		Marker marker;
		GreenCircle firstCircle, secondCircle;
		RedArea area;

		int score = 0;
		Random random = new Random();

		int areaGrowthRate = 3; // скорость увеличения размера красной зоны

		public Form1()
		{
			InitializeComponent();

			player = new Player(pbMain.Width / 2, pbMain.Height / 2, 0);
			objects.Add(player);

			firstCircle = new GreenCircle(random.Next(25, pbMain.Width - 25), random.Next(25, pbMain.Height - 25), 0);
			secondCircle = new GreenCircle(random.Next(25, pbMain.Width - 25), random.Next(25, pbMain.Height - 25), 0);
			objects.Add(firstCircle);
			objects.Add(secondCircle);

			area = new RedArea(random.Next(0, pbMain.Width), random.Next(0, pbMain.Height), 0, 10);
			objects.Add(area);

			// добавляю реакцию на пересечение
			player.OnOverlap += (p, obj) =>
			{
				txtLog.Text = $"[{DateTime.Now:HH:mm:ss:ff}] Игрок пересекся с {obj}\n" + txtLog.Text;
			};
			// добавил реакцию на пересечение с маркером
			player.OnMarkerOverlap += (m) =>
			{
				objects.Remove(m);
				marker = null;
			};
			// добавил реакцию на пересечение с зеленым кругом
			player.OnCircleOverlap += (c) =>
			{
				objects.Remove(c);
				if (c == firstCircle)
				{
					firstCircle = new GreenCircle(random.Next(25, pbMain.Width - 25), random.Next(25, pbMain.Height - 25), 0);
					objects.Add(firstCircle);
				}
				else
				{
					secondCircle = new GreenCircle(random.Next(25, pbMain.Width - 25), random.Next(25, pbMain.Height - 25), 0);
					objects.Add(secondCircle);
				}
				score++;
				labelScore.Text = "Очки: ";
				labelScore.Text += score.ToString();
			};
			// добавил реакцию на пересечение с красной зоной
			player.OnAreaOverlap += (a) =>
			{
				objects.Remove(a);
				area = new RedArea(random.Next(0, pbMain.Width), random.Next(0, pbMain.Height), 0, 10);
				objects.Add(area);

				score--;
				labelScore.Text = "Очки: ";
				labelScore.Text += score.ToString();
			};
		}

		private void pbMain_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics; // вытащили объект графики из события
			g.Clear(Color.White); // залил фон

			updatePlayer(); // теперь сначала вызываем перерасчет игрока

			foreach (var obj in objects.ToList())
			{
				if (obj != player && player.Overlaps(obj, g))
				{
					player.Overlap(obj);
					obj.Overlap(player);
				}
			}

            // Сначала рисуем все объекты, кроме зеленых кружков и маркера
            foreach (var obj in objects.Where(o => !(o is GreenCircle || o is Marker)))
            {
                g.Transform = obj.GetTransform();
                obj.Render(g);
            }

            // Затем рисуем зеленые кружки
            foreach (var obj in objects.Where(o => o is GreenCircle))
            {
                g.Transform = obj.GetTransform();
                obj.Render(g);
            }

            // Затем рисуем маркер
            foreach (var obj in objects.Where(o => o is Marker))
            {
                g.Transform = obj.GetTransform();
                obj.Render(g);
            }
        }

		private void timer1_Tick(object sender, EventArgs e)
		{
			// Увеличиваем размер красной зоны
			if (area != null)
			{
				area.Size += areaGrowthRate;
			}

			// запрашиваем обновление pbMain это вызовет метод pbMain_Paint по новой
			pbMain.Invalidate();
		}

		private void pbMain_MouseClick(object sender, MouseEventArgs e)
		{
			// тут добавил создание маркера по клику если он еще не создан
			if (marker == null)
			{
				marker = new Marker(0, 0, 0);
				objects.Add(marker); // и главное не забыть пололжить в objects
			}
			marker.X = e.X;
			marker.Y = e.Y;
		}
		private void updatePlayer()
		{
			if (marker != null)
			{
				float dx = marker.X - player.X;
				float dy = marker.Y - player.Y;
				float length = MathF.Sqrt(dx * dx + dy * dy);
				dx /= length;
				dy /= length;

				player.vX += dx * 2f;
				player.vY += dy * 2f;

				// расчитываем угол поворота игрока 
				player.Angle = 90 - MathF.Atan2(player.vX, player.vY) * 180 / MathF.PI;
			}
			// тормозящий момент, нужен чтобы, когда игрок достигнет маркера произошло постепенное замедление
			player.vX += -player.vX * 0.3f;
			player.vY += -player.vY * 0.3f;

			// пересчет позиция игрока с помощью вектора скорости
			player.X += player.vX;
			player.Y += player.vY;
		}
	}
}