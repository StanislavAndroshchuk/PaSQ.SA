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

public partial class FirstForm : Form
{
    private bool ToChange = true;

    static Timer myTimer = new Timer();

    private int k = 0;
    private int step = 10;
    private Panel panel1;
    private Button startButton;
    private Button stopButton;

    public FirstForm()
    {
        myTimer.Enabled = true;
        myTimer.Interval = 10;
        myTimer.Tick += new EventHandler(myTimer_Tick);
        myTimer.Start();
        panel1 = new Panel();
        panel1.Size = new Size(600, 600);
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
        InitializeComponent();
        
    }

    private void stopButton_Click(object sender, EventArgs e)
    {
        //myTimer.Change(Timeout.Infinite, Timeout.Infinite); // Призупинити таймер
        myTimer.Stop();
    }

    private void startButton_Click(object sender, EventArgs e)
    {
        myTimer.Start();
    }

    private void myTimer_Tick(object sender, EventArgs e)
    {
        Graphics g = panel1.CreateGraphics();
        Pen pen = new Pen(Color.Red, 1);

        g.Clear(Color.White);
        g.DrawEllipse(pen, k, 0, 200, 200);

        if (k + 200 == panel1.Width)
            ToChange = false;
        else if (k == 0)
            ToChange = true;
        if (ToChange)
            k += step;
        else
            k -= step;
        g.Dispose();
        pen.Dispose();
    }
}
