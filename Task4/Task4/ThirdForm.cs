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

public partial class ThirdForm : Form
{
    private Bitmap _bitmap;
    private PictureBox pictureBox1;
    private Timer timer;
    private Button startButton;
    private Button stopButton;
    public ThirdForm()
    {
        InitializeComponent();

        // Створення екземпляру PictureBox
        pictureBox1 = new PictureBox();
        pictureBox1.Width = 600;
        pictureBox1.Height = 600;
        this.Controls.Add(pictureBox1);

        _bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        pictureBox1.Image = _bitmap;

        timer = new Timer();
        timer.Tick += new EventHandler(timer_Tick);
        timer.Interval = 40;

        timer.Start();
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

    private int x = 0;
    private void stopButton_Click(object sender, EventArgs e)
    {
        timer.Stop();
    }

    private void startButton_Click(object sender, EventArgs e)
    {
        timer.Start();
    }
    void timer_Tick(object sender, EventArgs e)
    {
        if (++x < _bitmap.Width)
        {
            int y = _bitmap.Height / 2 - ((int)(Math.Sin(x * (Math.PI * 2) / _bitmap.Width) * (_bitmap.Height / 2)));
            
            if (y > _bitmap.Height - 1)
                y = _bitmap.Height - 1;

            _bitmap.SetPixel(x, y, Color.Black);
        }
        pictureBox1.Image = _bitmap;
    }
}