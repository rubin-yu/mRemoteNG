﻿using System;
using System.Diagnostics;
using System.Globalization;
using mRemoteNG.App.Info;
using mRemoteNG.App.Initialization;
using mRemoteNG.Config.Connections.Multiuser;
using mRemoteNG.Connection;
using mRemoteNG.Messages;
using mRemoteNG.Properties;
using mRemoteNG.Tools;
using mRemoteNG.Tools.Cmdline;
using mRemoteNG.UI.Forms;


namespace mRemoteNG.App
{
    public class Startup
    {
        private readonly ConnectionIconLoader _connectionIconLoader;
        private readonly FrmMain _frmMain = FrmMain.Default;

        public static Startup Instance { get; } = new Startup();

        private Startup()
        {
            _connectionIconLoader = new ConnectionIconLoader(GeneralAppInfo.HomePath + "\\Icons\\");
        }

        static Startup()
        {
        }

        public void InitializeProgram(MessageCollector messageCollector)
        {
            Debug.Print("---------------------------" + Environment.NewLine + "[START] - " +
                        Convert.ToString(DateTime.Now, CultureInfo.InvariantCulture));
            var startupLogger = new StartupDataLogger(messageCollector);
            startupLogger.LogStartupData();
            CompatibilityChecker.CheckCompatibility(messageCollector);
            ParseCommandLineArgs(messageCollector);
            IeBrowserEmulation.Register();
            _connectionIconLoader.GetConnectionIcons();
            DefaultConnectionInfo.Instance.LoadFrom(Settings.Default, a => "ConDefault" + a);
            DefaultConnectionInheritance.Instance.LoadFrom(Settings.Default, a => "InhDefault" + a);
        }

        private static void ParseCommandLineArgs(MessageCollector messageCollector)
        {
            var interpreter = new StartupArgumentsInterpreter(messageCollector);
            interpreter.ParseArguments(Environment.GetCommandLineArgs());
        }

        public void CreateConnectionsProvider(MessageCollector messageCollector)
        {
            messageCollector.AddMessage(MessageClass.DebugMsg, "Determining if we need a database syncronizer");
            if (!Settings.Default.UseSQLServer) return;
            messageCollector.AddMessage(MessageClass.DebugMsg, "Creating database syncronizer");
            Runtime.ConnectionsService.RemoteConnectionsSyncronizer =
                new RemoteConnectionsSyncronizer(new SqlConnectionsUpdateChecker());
            Runtime.ConnectionsService.RemoteConnectionsSyncronizer.Enable();
        }

    }
}