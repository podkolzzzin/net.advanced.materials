using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetAdvancedLiveCoding._2020._08
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ObserveButton();
        }

        private async void ObserveButton()
        {
            await foreach(var args in AsStream(button1))
            {
                button1.Text += args.ToString();
            }
        }

        private async IAsyncEnumerable<EventArgs> AsStream(Button button)
        {
            TaskCompletionSource<EventArgs> tcs = new TaskCompletionSource<EventArgs>();
            button.Click += (o, e) =>
            {
                tcs.SetResult(e);
            };

            while (true)
            {
                await tcs.Task;
                yield return tcs.Task.Result;
                tcs = new TaskCompletionSource<EventArgs>();
            }
        }
    }
}
