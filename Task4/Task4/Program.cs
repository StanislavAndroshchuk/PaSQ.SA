using System;
using System.Drawing;
using System.Windows.Forms;

namespace Task4;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Thread[] threads = new Thread[4];
        
        threads[0] = new Thread(() => { Application.Run(new FirstForm()); });
        threads[1] = new Thread(() => { Application.Run(new SecondForm()); });
        threads[2] = new Thread(() => { Application.Run(new ThirdForm()); });
        threads[3] = new Thread(() => { Application.Run(new FourthForm()); });
        
        foreach (var x in threads)
            x.Start();
        foreach (var x in threads)
            x.Join();
        
    }
}