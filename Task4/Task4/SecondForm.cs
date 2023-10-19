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

public partial class SecondForm : Form
{
    private bool toChange = true;

    static Timer myTimer = new Timer();

    private int k = 0;
    private int j = 0;
    private Panel panel1;
    private Button startButton;
    private Button stopButton;

    public SecondForm()
    {
        myTimer.Enabled = true;
        myTimer.Interval = 5;
        myTimer.Tick += new EventHandler(myTimer_Tick);
        myTimer.Start();
        panel1 = new Panel();
        panel1.Size = new Size(500, 400); 
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
        InitializeComponent();
        
    }

    private void stopButton_Click(object sender, EventArgs e)
    {
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
        g.DrawRectangle(pen,200,100,k,j);

        if (j + 100  == panel1.Height)
            toChange = false;
        else if (j == 0)
            toChange = true;
        if (toChange)
        {
            k++;
            j += 2;
        }
        else
        {
            k--;
            j -= 2;
        }

        g.Dispose();
        pen.Dispose();
    }
}