using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prover.CommProtocol.Debugger
{
    public partial class Form1 : Form
    {
        private InstrumentDebugger _debugger = new InstrumentDebugger();

        public Form1()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            _debugger.Client.Connect();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _debugger.Client.Disconnect();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

    public class InstrumentDebugger
    {
        public CommunicationClient Client;

        public InstrumentDebugger()
        {
            Client = CommunicationClient.Create(InstrumentType.MiniMax);
        }
    }
}
