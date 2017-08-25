using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Kovai.BizTalk360.BusinessService.Managers.Database;
using Kovai.BizTalk360.BusinessService.Utility;
using wmi = Kovai.BizTalk360.BusinessService.Managers.Wmi;
using Kovai.BizTalk360.BusinessService.EntityObjects;
using Kovai.BizTalk360.BusinessService.Managers.ExplorerOM;
using System.IO;

namespace Kovai.BizTalk360.Troubleshooter
{
    public partial class BizTalkServer //: KovaiEnt.Interfaces.IBizTalkServer
    {
        private string name;
        private BizTalkServerProperties serverProperties;
        private BizTalkHostInstances hostInstances;

        internal BizTalkServer() { }

        internal BizTalkServer(string serverName)
        {
            this.name = serverName;
        }

        //internal Hardware GetHardware()
        //{
        //    if (this.hardware != null)
        //        return this.hardware;

        //    this.hardware = Environment.GetHardwareDetails(this.name);
        //    return this.hardware;
        //}

        //internal OperatingSystem GetOperatingSystem()
        //{
        //    if (this.operatingSystem != null)
        //        return this.operatingSystem;

        //    this.operatingSystem = Environment.GetOperatingSystemDetails(this.name);
        //    return this.operatingSystem;
        //}

        //internal NTServices GetServices()
        //{
        //    if (this.services != null)
        //        return this.services;

        //    this.services = Environment.GetNTServices(this.name);

        //    return this.services;
        //}

        internal BizTalkServerProperties GetServerProperties(BizTalkEnvironmentSetting settings)
        {
            if (this.serverProperties != null)
                return this.serverProperties;

           
            BizTalkServerProperties properties = new BizTalkServerProperties
            {
                isClusterNode = ChoiceYesNoUnknown.UNKNOWN,
                isProcessingNode = BizTalkServerHelper.IsProcessingNode(this.name),
                isReceivingNode = BizTalkServerHelper.IsReceivingNode(this.name),
                isSendingNode = BizTalkServerHelper.IsSendingNode(this.name),
                isTrackingNode = BizTalkServerHelper.IsTrackingNode(this.name)
            };

            this.serverProperties = properties;
            return this.serverProperties;
        }


        internal BizTalkHostInstances GetHostInstances(BizTalkEnvironmentSetting settings)
        {
            if (this.hostInstances != null)
                return this.hostInstances;

            BizTalkMgmtDb biztalkMgmtDbFactory =
                new BizTalkMgmtDb(settings);

            //SK (4th Jan 2012) - Added logic to return host instances based on UAP application list
            // List<BizTalkHostInstance> hostInstances = wmi.BizTalkGeneral.GetBizTalkHostInstances(settings);
            BizTalkGroupSettings bizTalkGroup = new BizTalkGroupSettings(settings);
            return wmi.BizTalkGeneral.GetBizTalkHostInstances(settings);

          
        }


      

        public static BizTalkServer GetBizTalkServer(string serverName, BizTalkEnvironmentSetting settings, BizTalkServerPropertiesOptions retrieve)
        {

            BizTalkServer btsServer = new BizTalkServer(serverName);

            //if ((retrieve & BizTalkServerPropertiesOptions.Hardware) == BizTalkServerPropertiesOptions.Hardware)
            //    btsServer.hardware = btsServer.GetHardware();
            //if ((retrieve & BizTalkServerPropertiesOptions.OperatingSystem) == BizTalkServerPropertiesOptions.OperatingSystem)
            //    btsServer.operatingSystem = btsServer.GetOperatingSystem();
            //if ((retrieve & BizTalkServerPropertiesOptions.BiztalkProductInfo) == BizTalkServerPropertiesOptions.BiztalkProductInfo)
            //    btsServer.biztalkProductInfo = btsServer.GetBizTalkProductInfo(settings);
            //if ((retrieve & BizTalkServerPropertiesOptions.IISProductInfo) == BizTalkServerPropertiesOptions.IISProductInfo)
            //    btsServer.iisProductInfo = btsServer.GetIISProductInfo();
            //if ((retrieve & BizTalkServerPropertiesOptions.ServerProperties) == BizTalkServerPropertiesOptions.ServerProperties)
            //    btsServer.serverProperties = btsServer.GetServerProperties(settings);
            //if ((retrieve & BizTalkServerPropertiesOptions.HostInstances) == BizTalkServerPropertiesOptions.HostInstances)
            //    btsServer.hostInstances = btsServer.GetHostInstances(settings);
            //if ((retrieve & BizTalkServerPropertiesOptions.Services) == BizTalkServerPropertiesOptions.Services)
            //    btsServer.services = btsServer.GetServices();


            return btsServer;
        }

        #region Host Instance Methods
        internal void StartBizTalkHostInstance(string hostName, BizTalkEnvironmentSetting settings)
        {
            //Perform the operation using WMI first
           BizTalkHostInstance hostInstance = new BizTalkHostInstance(settings, this.name, hostName);
            hostInstance.Start();
        }
        internal void StopBizTalkHostInstance(string hostName, BizTalkEnvironmentSetting settings)
        {
            //Perform the operation using WMI first
           BizTalkHostInstance hostInstance = new BizTalkHostInstance(settings, this.name, hostName);
            hostInstance.Stop();
        }
        internal void DeleteBizTalkHostInstance(string hostName, BizTalkEnvironmentSetting settings)
        {
            //Perform the operation using WMI first
            BizTalkHostInstance hostInstance = new BizTalkHostInstance(settings, this.name, hostName);
            hostInstance.Delete();
        }
        internal void DisableBizTalkHostInstance(string hostName, BizTalkEnvironmentSetting settings)
        {
            //Perform the operation using WMI first
           BizTalkHostInstance hostInstance = new BizTalkHostInstance(settings, this.name, hostName) { Disable = true };
        }
        internal void EnableBizTalkHostInstance(string hostName, BizTalkEnvironmentSetting settings)
        {
            //Perform the operation using WMI first
           BizTalkHostInstance hostInstance = new BizTalkHostInstance(settings, this.name, hostName) { Disable = false };
        }
        internal void CreateBizTalkHostInstance(string hostName, NetworkCredential credential, BizTalkEnvironmentSetting settings)
        {
            //Perform the operation using WMI first
            BizTalkHostInstance hostInstance = new BizTalkHostInstance(settings, this.name, hostName);
            hostInstance.CreateNew(credential);
        }
        internal void ChangeCredentialsOfBizTalkHostInstance(string hostName, NetworkCredential credential, BizTalkEnvironmentSetting settings)
        {
            //Perform the operation using WMI first
            BizTalkHostInstance hostInstance = new BizTalkHostInstance(settings, this.name, hostName);
            hostInstance.ChangeCredential(credential);
        }
        #endregion

        #region Utility Methods


        internal void WriteToLog(Exception ex, string source)
        {
            string filePath = Environment.CurrentDirectory + "\\HostInstancesOperation.log";

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(source);
                if (ex != null)
                {
                    writer.WriteLine("Message :" + ex.Message + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                }
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }

        }

        internal void WriteToLog(string messageSource)
        {
            string filePath = Environment.CurrentDirectory + "\\HostInstancesOperation.log";

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(messageSource);

            }

        }

        #endregion
    }

    [Flags]
    public enum BizTalkServerPropertiesOptions
    {
        Hardware = 1,
        OperatingSystem = 2,
        BiztalkProductInfo = 4,
        IISProductInfo = 8,
        ServerProperties = 16,
        HostInstances = 32,
        Services = 64,
        All = 127
    }
}
