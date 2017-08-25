using Kovai.BizTalk360.BusinessService.EntityObjects;
using Kovai.BizTalk360.BusinessService.Managers.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kovai.BizTalk360.Troubleshooter
{
   public  class BizTalkServerHelper

    { 
        public static string _mgmtDbName { get; private set; }
        public static string _mgmtDbSQLInstanceName { get; private set; }

        public static ChoiceYesNoUnknown IsSendingNode(string serverName)
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append("SELECT count(*)");
            sbQuery.Append(" FROM bts_sendport_transport AS spt WITH (NOLOCK)");
            sbQuery.Append(" JOIN bts_sendport AS sp ON spt.nSendPortID = sp.nID AND spt.bIsPrimary=1");
            sbQuery.Append(" JOIN adm_SendHandler AS sh ON sh.Id=spt.nSendHandlerID");
            sbQuery.Append(" JOIN adm_Host AS h ON h.Id=sh.HostId");
            sbQuery.Append(" JOIN adm_Server2HostMapping AS shm ON shm.HostId = h.Id");
            sbQuery.Append(" JOIN adm_Server AS s on s.Id = shm.ServerId");
            sbQuery.Append(" WHERE shm.IsMapped !=0 and s.Name = @serverName");

            SqlCommand cmd = new SqlCommand(sbQuery.ToString());
            cmd.Parameters.Add("@serverName", SqlDbType.NVarChar);
            cmd.Parameters["@serverName"].Value = serverName;

            DatabaseManager dbExecutor = new DatabaseManager();
            object count = dbExecutor.ExecuteScalar(_mgmtDbSQLInstanceName, _mgmtDbName, cmd);

            int recordCount = Convert.ToInt32(count);
            if (recordCount == 0)
                return ChoiceYesNoUnknown.NO;
            if (recordCount > 0)
                return ChoiceYesNoUnknown.YES;

            return ChoiceYesNoUnknown.UNKNOWN;
        }
        public static ChoiceYesNoUnknown IsTrackingNode(string serverName)
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append("SELECT count(*)");
            sbQuery.Append(" FROM adm_Host AS h");
            sbQuery.Append(" JOIN adm_Server2HostMapping AS shm ON shm.HostId = h.Id");
            sbQuery.Append(" JOIN adm_Server AS s on s.Id = shm.ServerId");
            sbQuery.Append(" WHERE");
            sbQuery.Append(" shm.IsMapped !=0");
            sbQuery.Append(" and h.HostTracking = -1");
            sbQuery.Append(" and s.Name = @serverName");

            SqlCommand cmd = new SqlCommand(sbQuery.ToString());
            cmd.Parameters.Add("@serverName", SqlDbType.NVarChar);
            cmd.Parameters["@serverName"].Value = serverName;

            DatabaseManager dbExecutor = new DatabaseManager();
            object count = dbExecutor.ExecuteScalar(_mgmtDbSQLInstanceName, _mgmtDbName, cmd);

            int recordCount = Convert.ToInt32(count);
            if (recordCount == 0)
                return ChoiceYesNoUnknown.NO;
            if (recordCount > 0)
                return ChoiceYesNoUnknown.YES;

            return ChoiceYesNoUnknown.UNKNOWN;
        }
        public static ChoiceYesNoUnknown IsProcessingNode(string serverName)
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append("SELECT count(*)");
            sbQuery.Append(" FROM bts_orchestration AS o WITH (NOLOCK)");
            sbQuery.Append(" JOIN adm_Host AS h ON h.Id=o.nAdminHostId ");
            sbQuery.Append(" JOIN adm_Server2HostMapping AS shm ON shm.HostId = h.Id");
            sbQuery.Append(" JOIN adm_Server AS s on s.Id = shm.ServerId");
            sbQuery.Append(" WHERE shm.IsMapped !=0 and s.Name = @serverName");

            SqlCommand cmd = new SqlCommand(sbQuery.ToString());
            cmd.Parameters.Add("@serverName", SqlDbType.NVarChar);
            cmd.Parameters["@serverName"].Value = serverName;

            DatabaseManager dbExecutor = new DatabaseManager();
            object count = dbExecutor.ExecuteScalar(_mgmtDbSQLInstanceName, _mgmtDbName, cmd);

            int recordCount = Convert.ToInt32(count);
            if (recordCount == 0)
                return ChoiceYesNoUnknown.NO;
            if (recordCount > 0)
                return ChoiceYesNoUnknown.YES;
            return ChoiceYesNoUnknown.UNKNOWN;
        }
        public static ChoiceYesNoUnknown IsReceivingNode(string serverName)
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append("SELECT count(*)");
            sbQuery.Append(" FROM adm_ReceiveLocation AS rl WITH (NOLOCK)");
            sbQuery.Append(" JOIN adm_ReceiveHandler AS rh ON rh.Id=rl.ReceiveHandlerId");
            sbQuery.Append(" JOIN adm_Host AS h ON h.Id=rh.HostId");
            sbQuery.Append(" JOIN adm_Server2HostMapping AS shm ON shm.HostId = h.Id");
            sbQuery.Append(" JOIN adm_Server AS s on s.Id = shm.ServerId");
            sbQuery.Append(" WHERE shm.IsMapped !=0 and s.Name = @serverName");

            SqlCommand cmd = new SqlCommand(sbQuery.ToString());
            cmd.Parameters.Add("@serverName", SqlDbType.NVarChar);
            cmd.Parameters["@serverName"].Value = serverName;

            DatabaseManager dbExecutor = new DatabaseManager();
            object count = dbExecutor.ExecuteScalar(_mgmtDbSQLInstanceName, _mgmtDbName, cmd);

            int recordCount = Convert.ToInt32(count);
            if (recordCount == 0)
                return ChoiceYesNoUnknown.NO;
            if (recordCount > 0)
                return ChoiceYesNoUnknown.YES;

            return ChoiceYesNoUnknown.UNKNOWN;
        }
    }
}
