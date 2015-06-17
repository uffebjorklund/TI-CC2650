namespace CC2650.Modules.ScaleOut
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using XSockets.Protocol;
    using XSockets.Core.Common.Enterprise;
    using XSockets.Core.Common.Socket;
    using XSockets.Core.Common.Socket.Event.Interface;
    using XSockets.Core.Common.Utility.Logging;
    using XSockets.Core.XSocket.Model;
    using XSockets.Enterprise.Scaling;
    using XSockets.Plugin.Framework;
    using XSockets.Core.Common.Protocol;
    using XSockets.Plugin.Framework.Attributes;

    /// <summary>
    /// To use this scaleout you should install the nuget package for Azure Service Bus
    /// 
    /// Install-Package WindowsAzure.ServiceBus
    /// 
    /// Then get you connectionstring from http://manage.windowsazure.com/ and add it to 
    /// the app.config for the key "Microsoft.ServiceBus.ConnectionString"    
    /// 
    /// Also add a GUID in the app.config that is unique, every server in the scaleout must have a unique id
    /// for the "XSockets.Scaleout.ServerId" appsetting
    /// </summary>
    [Export(typeof(IXSocketsScaleOut), null, Rewritable.No, InstancePolicy.Shared)]
    public class AzureServiceBusScaleOut : BaseScaleOut
    {
        /// <summary>
        /// Server Identifier, to filter away messages from the sending server
        /// </summary>
        private string SID;

        /// <summary>
        /// Azure Service Bus Connection String
        /// </summary>
        private string _connString;

        /// <summary>
        /// Topic for scaled data
        /// </summary>
        private const string TopicName = "X";

        /// <summary>
        /// Publisher - for sending messages to Azure Service Bus
        /// </summary>
        private TopicClient _topicClient;

        /// <summary>
        /// Subscriber - for getting messages from Azure Service Bus
        /// </summary>
        private SubscriptionClient _subscriptionClient;

        private bool _serviceBusConfigurationIsValid;
        
        /// <summary>
        /// Called at startup, setup/prepare your scaleout
        /// </summary>
        public override void Init()
        {
            try
            {
                this._connString = ConfigurationManager.AppSettings.Get("Microsoft.ServiceBus.ConnectionString");
                this.SID = ConfigurationManager.AppSettings.Get("XSockets.Scaleout.ServerId");
                SetupAzureServiceBus();
            }
            catch (Exception ex)
            {
                Composable.GetExport<IXLogger>().Error(ex, "Could not initialize Azure Service Bus ScaleOut");
            }
        }

        private void SetupAzureServiceBus()
        {
            Composable.GetExport<IXLogger>().Debug("Azure ServiceBus Scaling - INIT");
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(_connString);
            try
            {
                TopicDescription myTopic = null;

                if (!namespaceManager.TopicExists(TopicName))
                {
                    Composable.GetExport<IXLogger>().Debug("Creating Topic for Azure Service Bus");
                    myTopic = namespaceManager.CreateTopic(TopicName);
                }
                else
                {
                    Composable.GetExport<IXLogger>().Debug("Getting Topic for Azure Service Bus");
                    myTopic = namespaceManager.GetTopic(TopicName);
                }

                if (namespaceManager.SubscriptionExists(myTopic.Path, SID))
                {
                    Composable.GetExport<IXLogger>().Debug("Delete old subscription for Azure Service Bus");
                    namespaceManager.DeleteSubscription(myTopic.Path, SID);
                }
                Composable.GetExport<IXLogger>().Debug("Creating Subscription for Azure Service Bus");
                var filter = new SqlFilter(string.Format("SID != '{0}'", SID));
                namespaceManager.CreateSubscription(TopicName, SID, filter);

                this._topicClient = TopicClient.CreateFromConnectionString(_connString, myTopic.Path);
                this._subscriptionClient = SubscriptionClient.CreateFromConnectionString(_connString, myTopic.Path, SID);

                _serviceBusConfigurationIsValid = true;
            }
            catch (MessagingException e)
            {
                Composable.GetExport<IXLogger>().Error("Failed to setup scaling with Azure Service Bus: {e}", e.Message);
            }
        }

        public override async Task Publish(MessageDirection direction, IMessage message, ScaleOutOrigin scaleOutOrigin)
        {
            if (!_serviceBusConfigurationIsValid) return;
            Composable.GetExport<IXLogger>().Debug("Azure ServiceBus Scaling - PUBLISH {@m}", message);
            await _topicClient.SendAsync(GetBrokerMessage(message));
        }

        private BrokeredMessage GetBrokerMessage(IMessage message)
        {
            // Create message, passing a string message for the body
            var m = new BrokeredMessage();
            m.Properties["JSON"] = this.Serializer.SerializeToString(message);
            m.Properties["SID"] = SID;
            return m;
        }

        public override async Task Subscribe()
        {
            if (!_serviceBusConfigurationIsValid) return;
            Composable.GetExport<IXLogger>().Debug("Azure ServiceBus Scaling - SUBSCRIBE");

            var options = new OnMessageOptions { AutoComplete = false, AutoRenewTimeout = TimeSpan.FromMinutes(30) };
            await Task.Run(() => _subscriptionClient.OnMessage(OnBrokerMessage, options)).ConfigureAwait(false);
        }
        private void OnBrokerMessage(BrokeredMessage message)
        {
            try
            {
                Composable.GetExport<IXLogger>().Debug("Message Arrived {@m}", message);
                var m = this.Serializer.DeserializeFromString<Message>(message.Properties["JSON"].ToString());
                var pipe = Composable.GetExport<IXSocketPipeline>();
                var ctrl = Composable.GetExports<IXSocketController>().First(p => p.Alias == m.Controller);
                ctrl.ProtocolInstance = new XSocketInternalProtocol();
                pipe.OnIncomingMessage(ctrl, m);
                message.Complete();
            }
            catch (Exception)
            {
                // Indicates a problem, unlock message in subscription
                if (message.DeliveryCount > 3)
                    message.DeadLetter();
                else
                    message.Abandon();
            }
        }
    }
}
