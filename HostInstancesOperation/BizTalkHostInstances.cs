using System;
using System.Linq;
using Kovai.BizTalk360.BusinessService.Aspects;
using Kovai.BizTalk360.BusinessService.EntityObjects;
using System.Management;
using System.Net;
using Kovai.BizTalk360.BusinessService.Utility;
using Microsoft.BizTalk.SnapIn.Framework;

namespace Kovai.BizTalk360.Troubleshooter
{
    
        internal class BizTalkHostInstance
        {
            //Important values
            //Service State Enums
            //Stopped	1
            //Start pending	2
            //Stop pending	3
            //Running	4
            //Continue pending	5
            //Pause pending	6
            //Paused	7
            //Unknown	8

            //Host Type
            //In-process	1
            //Isolated	2
            readonly WmiProvider wmiProvider;
            readonly ManagementObject hostInstance;

            private readonly string runningServer;
            private readonly string hostName;

            public BizTalkHostInstance(BizTalkEnvironmentSetting settings, string runningServer, string hostName)
            {
                wmiProvider = new WmiProvider(settings.mgmtDbSqlInstanceName, settings.mgmtDbName);
                this.runningServer = runningServer;
                this.hostName = hostName;

                //Host Instance name format
                //"Microsoft BizTalk Server BTS_CLUSTERED_INPROC_HOST KOV007W2008R2"

                //string hostInstanceName = string.Format("Microsoft BizTalk Server {0} {1}",hostName, runningServer);

                // string condition = string.Format("hostName='{0}'", hostName);
                //
                //0 UnClusteredInstance
                //1 ClusteredInstance
                //2 ClusteredVirtualInstance - Active

                //3 - Not documented, but that's what displayed for active instance.

                this.hostInstance = GetHostInstance();
            }


            internal ManagementObject GetHostInstance()
            {
                string condition = string.Format("RunningServer='{0}' and HostName='{1}'", runningServer, hostName);

                ManagementObjectCollection mgmtObjs = wmiProvider.SelectRemote("MSBTS_HostInstance", condition);
                return mgmtObjs.Cast<ManagementObject>().FirstOrDefault();
            }
            internal void Start()
            {
                //Start
                if (hostInstance != null && hostInstance["HostType"].ToString() == "1"
                 && (hostInstance["ServiceState"].ToString() == "1" || hostInstance["ServiceState"].ToString() == "8")) //Inprocess and Stopped/Unknown
                    hostInstance.InvokeMethod("Start", null);
                else
                {
                    if (hostInstance != null)
                    {
                        const string error =
                            "Host Instance is either not found or in correct state to perform the operation. Name:{0}. HostType:{1}. ServiceState:{2}";
                        throw new Exception(string.Format(error, hostInstance["Name"], hostInstance["HostType"], hostInstance["ServiceState"]));
                    }

                    throw new Exception("Unable to start host instance");
                }
            }
            internal void Stop()
            {
                if (hostInstance["HostType"].ToString() == "1"
                  && hostInstance["ServiceState"].ToString() == "4") //Inprocess and Running
                    hostInstance.InvokeMethod("Stop", null);
                else
                    throw new Exception("Host Instance is either not found or in correct state to perform the operation. Name:" + hostInstance["Name"]);
            }
            internal void Delete()
            {
                //TODO For V1, its only read only
                //string name = GetNameFromHostName(_hostName, _runningServer);

                //if (_hostInstance.ServiceState == HostInstance.ServiceStateValues.Stopped)
                //{
                //    // uninstall host instance
                //    _hostInstance.Uninstall();

                //    // delete host instance mapping
                //    string condition = string.Format("ServerName='{0}' AND hostName='{1}'",_runningServer,_hostName);
                //    ServerHost.StaticScope = ManagementScopeHelper.GetScope(typeof(ServerHost), _runningServer, _instance, _database); ;
                //    foreach (ServerHost server in ServerHost.GetInstances(condition))
                //    {
                //        server.Unmap();
                //    }
                //}
            }
            internal void CreateNew(NetworkCredential credential)
            {
                //TODO For V1 its only ready only
                //ManagementScope mgmtScope = ManagementScopeHelper.GetScope(typeof(HostSetting), _runningServer, _instance, _database);

                ////Make sure the host instance is not already present in the server.
                //ServerHost.StaticScope = mgmtScope;

                //// map BizTalk Host
                //ServerHost serverHost = ServerHost.CreateInstance();
                //serverHost.LateBoundObject["ServerName"] = _runningServer;
                //serverHost.LateBoundObject["HostName"] = _hostName;
                //serverHost.Map();

                //// add new BizTalk Host Instance
                //string name = GetNameFromHostName(_hostName, _runningServer);
                //HostInstance.StaticScope = mgmtScope;
                //HostInstance hostInstance = HostInstance.CreateInstance();
                //hostInstance.LateBoundObject["Name"] = name;
                //hostInstance.LateBoundObject["IsDisabled"] = true;
                //hostInstance.Install(true, credential.UserName, credential.Password);

                //_hostInstance = hostInstance;
            }
            internal void ChangeCredential(NetworkCredential credential)
            {
                //TODO For V1 is only read only
                //_hostInstance.Uninstall();
                //_hostInstance.Install(true, credential.UserName, credential.Password);
            }
            internal bool Disable
            {
                get
                {
                    return Convert.ToBoolean(this.hostInstance["IsDisabled"]);
                }
                set
                {
                    //this.hostInstance["IsDisabled"] = value; 
                    this.hostInstance.SetPropertyValue("IsDisabled", value);
                    this.hostInstance.Put();
                }
            }

            private static string GetNameFromHostName(string HostName, string runningServer)
            {
                return "Microsoft BizTalk Server " + HostName + " " + runningServer;
            }
        }
   
}
