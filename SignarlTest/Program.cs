﻿using Microsoft.AspNet.SignalR.Client;
using System;
using System.Configuration;
using System.Data;
using System.ServiceProcess;
using System.Threading;

namespace SignarlTest
{
    // Source Code Files
    //Global.asax - The global application class, used to register the route to the autogenerated hubs proxy.
    //Default.html - The client application, written using JavaScript and the JQuery library.
    //MoveShapeHub.cs - The server application, written as an implementation of a SignalR hub.
    //Startup.cs - The OWIN startup class that creates the route for the SignalR hub.
    class Program
    {
        private SynchronizationContext uiSyncContext = null;
        HubConnection hubConnection;
        IHubProxy hubProxy;
        private string hostName = Environment.MachineName;
        DataTable gridDataTable;
        bool connected = false;
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                //new IDiagnosticsService() you can call your windows servis or another servis
            };
            ServiceBase.Run(ServicesToRun);
        }

        private bool ConnectService()
        {
            //connected = false;
            //if (hubConnection.State != Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
            //{
            hubConnection = new HubConnection(ConfigurationManager.AppSettings["hostURL"]);
            hubProxy = hubConnection.CreateHubProxy("DiagnosticsLogHub");
            //}

            hubConnection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    connected = false;
                }
                else
                {
                    connected = true;
                    // Bağlantı sağlandıysa signalR üzerinden gelen logMessage isimli mesajlarda console a girdi girilmesi sağlanır.
                    hubProxy.On<LogEvent>("LogMessage", (log) =>
                        // Context is a reference to SynchronizationContext.Current
                        uiSyncContext.Post(new SendOrPostCallback((o) =>
                        {
                            WriteMessage(log);
                        }), null)

                    );
                }

            }).Wait();
            return true;
        }

        private void WriteMessage(LogEvent log)
        {
            //you can write log bussines
        }
    }
    public class LogEvent
    {
        public string Module;
        public string WorkflowName;
        public string Message;
        public string Exception;
        public string UserName;
        public DateTime Date;
        public string Thread;
        public string Logger;
        public string Method;
        public string MachineName;
        public string Location;
        public string Duration;
    }
}

