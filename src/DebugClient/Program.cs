using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Stratum;
using Stratum.Graphics;
using Stratum.Input;

namespace DebugClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            form = new Form1();
            form.Width = 1920;
            form.Height = 1080;
            Engine.Instance.Initialize(form);
            service = Engine.GraphicsDeviceService;
            //form.ResizeBegin += (o, e) => { Engine.Pause(); };
            //form.ResizeEnd += (o, e) => { Engine.UnPause(); };
            form.SizeChanged += form_SizeChanged;

            Engine.Instance.Start();
            Application.Run(form);
        }

        static Form form;
        static IFormsGraphicsService service;

        static void form_SizeChanged(object sender, EventArgs e)
        {
            service.ResizeSwapChain(form.Width, form.Height);
        }
    }
}
