using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
namespace Task4;

public partial class FourthForm : Form
    {
        private Timer myTimer = new Timer();
        private int angle = 0;
        private Color lineColor = Color.Red;
        private Panel panel1;
        private Button startButton;
        private Button stopButton;
        public FourthForm()
        {
            InitializeComponent();

            myTimer.Enabled = true;
            myTimer.Interval = 50;
            myTimer.Tick += new EventHandler(myTimer_Tick);
            myTimer.Start();

            panel1 = new Panel();
            panel1.Size = new Size(500, 500);
            panel1.Dock = DockStyle.None;
            Controls.Add(panel1);
            startButton = new Button();
            startButton.Text = "Start";
            startButton.Location = new Point(600, 50);
            startButton.Click += new EventHandler(startButton_Click);
            Controls.Add(startButton);

            stopButton = new Button();
            stopButton.Text = "Stop";
            stopButton.Location = new Point(600, 70);
            stopButton.Click += new EventHandler(stopButton_Click);
            Controls.Add(stopButton);
        }

        private void myTimer_Tick(object sender, EventArgs e)
        {
            Graphics g = panel1.CreateGraphics();
            
            g.Clear(Color.White);

            // Розрахунок координат для відрізка
            int centerX = panel1.Width / 2;
            int centerY = panel1.Height / 2;
            int lineLength = 400;

            // Розрахунок координат початку і кінця відрізка
            int startX = centerX - lineLength / 2;
            int startY = centerY;
            int endX = centerX + lineLength / 2;
            int endY = centerY;

            // Повернення лінії на кут angle відносно центра
            double radians = angle * Math.PI / 180.0;
            double cosAngle = Math.Cos(radians);
            double sinAngle = Math.Sin(radians);

            int rotatedStartX = centerX + (int)((startX - centerX) * cosAngle - (startY - centerY) * sinAngle);
            int rotatedStartY = centerY + (int)((startX - centerX) * sinAngle + (startY - centerY) * cosAngle);
            int rotatedEndX = centerX + (int)((endX - centerX) * cosAngle - (endY - centerY) * sinAngle);
            int rotatedEndY = centerY + (int)((endX - centerX) * sinAngle + (endY - centerY) * cosAngle);

            using (Pen pen = new Pen(lineColor, 3))
            {
                g.DrawLine(pen, rotatedStartX, rotatedStartY, rotatedEndX, rotatedEndY);
            }

            // Збільшити кут обертання та змінити колір
            angle += 5;
            if (angle >= 360)
            {
                angle = 0;
                lineColor = GetRandomColor();
            }

            g.Dispose();
        }
        private void stopButton_Click(object sender, EventArgs e)
        {
            myTimer.Stop();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            myTimer.Start();
        }
        private Color GetRandomColor()
        {
            Random random = new Random();
            return Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        }
    }
    