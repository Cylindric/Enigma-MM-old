using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EnigmaMM
{
    public partial class ConsoleForm : Form
    {
        private Client mClient;

        public ConsoleForm()
        {
            InitializeComponent();
            mClient = new Client();
            mClient.StartClient();
        }

        public ConsoleForm(Client client)
        {
            InitializeComponent();
            mClient = client;
        }

        private void ConsoleForm_Load(object sender, EventArgs e)
        {

        }
    }
}
