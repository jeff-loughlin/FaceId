using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceId
{
    public partial class ConfigForm : Form
    {
        public bool configRequired = false;

        public ConfigForm()
        {
            InitializeComponent();
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            if (configRequired)
                cancelButton.Enabled = false;
        }

        public string GetApiKey()
        {
            return apiKeyBox.Text;
        }

        public string GetURL()
        {
            return urlBox.Text;
        }

        public void SetApiKey(string apiKey)
        {
            apiKeyBox.Text = apiKey;
        }

        public void SetURL(string url)
        {
            urlBox.Text = url;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
