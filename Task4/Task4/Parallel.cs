using System;
using System.Drawing;
using System.Windows.Forms;

namespace Task4;

public partial class Parallel : Form
{
    private Panel panel1; // Оголошення Panel
    private int rectCount = 3; // Кількість прямокутників
    private int rectWidth = 30; // Ширина прямокутників
    private int rectHeight = 100; // Висота прямокутників
    private int spacing = 20; // Відстань між прямокутниками

    public Parallel()
    {
        InitializeComponent();
        InitializePanel(); // Ініціалізація Panel
        this.SizeChanged += Form_SizeChanged; // Додавання обробника подій при зміні розмірів форми
    }

    private void InitializePanel()
    {
        panel1 = new Panel();
        panel1.Size = new Size(300, 300); // Встановіть розмір панелі за вашими потребами
        Controls.Add(panel1);
        panel1.Paint += panel1_Paint;
    }

    private void Form_SizeChanged(object sender, EventArgs e)
    {
        panel1.Invalidate(); // Повідомляємо панелі, що потрібно оновити малюнок
    }

    private void panel1_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;

        // Знаходження координати X для першого прямокутника так, щоб вони були в центрі форми
        int startX = (panel1.Width - (rectCount * rectWidth + (rectCount - 1) * spacing)) / 2;

        for (int i = 0; i < rectCount; i++)
        {
            int x = startX + i * (rectWidth + spacing);
            int y = (panel1.Height - rectHeight) / 2;
            Rectangle rect = new Rectangle(x, y, rectWidth, rectHeight);
            g.FillRectangle(Brushes.Blue, rect); // Заповнення прямокутника кольором
            g.DrawRectangle(Pens.Black, rect); // Малювання контуру прямокутника
        }
    }

}
