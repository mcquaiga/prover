using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Prover.CommProtocol.Common.Instruments;
using Prover.CommProtocol.MiHoneywell.Instruments;
using Prover.InstrumentProtocol.Core.IO;

namespace Prover.CommProtocol.Debugger
{
    public partial class Form1 : Form
    {
        private IInstrument _instrument;

        public Form1()
        {
            InitializeComponent();
        }

        private void button_MiniMax_Click(object sender, EventArgs e)
        {
            var commPort = CreateCommPort();
            HoneywellInstruments.CreateInstrument(commPort, HoneywellInstrumentType.MiniMax).ContinueWith(_ =>
            {
                var instr = _.Result;
            });
        }

        private CommPort CreateCommPort()
        {
            int baudRate;
            if (int.TryParse(textBox_BaudRate.Text, out baudRate))
            {
                CommPort commPort = new SerialPort(textBox_CommPort.Text.ToUpper(), baudRate);
                return commPort;
            }

            return null;
        }
    }
}
