using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace Kovai.BizTalk360.Troubleshooter
{
    public partial class frmHostOperations : Form
    {
        BizTalk360.BusinessService.EntityObjects.BizTalkEnvironmentSetting setting;
        public frmHostOperations()
        {
            InitializeComponent();

            setting = new BusinessService.EntityObjects.BizTalkEnvironmentSetting();

            setting.mgmtDbName = Convert.ToString(ConfigurationManager.AppSettings["MGMTDBNAME"]);
            setting.mgmtDbSqlInstanceName = Convert.ToString(ConfigurationManager.AppSettings["MGMTDBSQLINSTANCE"]);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            BizTalkServer bizTalkServer = new BizTalkServer(txtServerName.Text);
            bizTalkServer.StartBizTalkHostInstance(txtHostInstance.Text, setting);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            BizTalkServer bizTalkServer = new BizTalkServer(txtServerName.Text);
            bizTalkServer.StopBizTalkHostInstance(txtHostInstance.Text, setting);
        }
    }
}
