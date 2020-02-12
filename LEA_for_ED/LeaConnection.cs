using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace LEA_for_ED
{
    public static class LeaConnection
    {
        static bool connected = false;
        static AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        static Settings settings;
        static List<commands.EMData> EMDList = new List<commands.EMData>();
        static List<SettingsStatusEvent> settingsStatusEvents = new List<SettingsStatusEvent>();

        static LeaConnection() { }

        public static void Start(Settings settings)
        {
            LeaConnection.settings = settings;
            TCPLayerLite.localDeviceType = TCPLayerLite.deviceType.GAME;
            TCPLayerLite.DataReceived += commands.newDataToDecode;
            TCPLayerLite.FailToConnect += TCPLayerLite_FailToConnect;
            TCPLayerLite.DirectConnectionEstablished += TCPLayerLite_ConnectionEstablished;
            TCPLayerLite.LastConnectionLost += TCPLayerLite_LastConnectionLost;
            TCPLayerLite.NoConnectedDevice += TCPLayerLite_NoConnectedDevice;
            TCPLayerLite.setDefaultSecurityOptions(TCPLayerLite.securityMode.PASS_SHA1, Encoding.UTF8.GetBytes("Anonymous"), Encoding.UTF8.GetBytes(settings.LeaConnection.Password), false);
            TCPLayerLite.launchConnection(new IPEndPoint(IPAddress.Loopback, settings.LeaConnection.Port));

            if (!autoResetEvent.WaitOne(10000))
            {
                Console.WriteLine("Fail to get answer from server.");
                return;
            }
            if (!connected)
            {
                Console.WriteLine("Connection closed. Bad port or bad password.");
                return;
            }

            Console.WriteLine("Checking if game is installed.");

            commands.GameListReceived += commands_GameListReceived;
            commands.sendGetGames();
            if (!autoResetEvent.WaitOne(1000))
            {
                Console.WriteLine("Fail to get answer from server.");
                return;
            }
            Console.WriteLine();

            Console.WriteLine("Checking if project is installed.");

            commands.ProjectListReceived += commands_ProjectListReceived;
            commands.sendGetProjectNames(settings.GameProject.GameName);
            if (!autoResetEvent.WaitOne(1000))
            {
                Console.WriteLine("Fail to get answer from server.");
                return;
            }
            Console.WriteLine();

            commands.ConnectedClientAnswerReceived += commands_ConnectedClientAnswerReceived;
            commands.ConnectedClientLoadedProjectAnswerReceived += commands_ConnectedClientLoadedProjectAnswerReceived;

            commands.sendConfigureServer(commands.serverMode.PUSH); // <- send configuration before setting the EMTag list

            commands.NewCommands += commands_NewCommands;

            foreach (SettingsStatusEvent statusEvent in settings.StatusEvents)
            {
                EMDList.Add(new commands.EMData(statusEvent.EMTag, statusEvent.Default, commands.EMType.BUTTON));
                settingsStatusEvents.Add(statusEvent);
            }
            commands.registerCommands(EMDList);
            commands.sendResynchData();

            commands.sendConnectedClientsProjects();

            Console.WriteLine();

            Console.WriteLine("Connection fully established. Processing ...");
            Console.WriteLine("Press ESC to stop");

            while (Console.ReadKey().Key != ConsoleKey.Escape) { }

        }

        public static void Stop()
        {
            try
            {
                commands.stopPullTimer();
            }
            catch (Exception) { }
            try
            {
                TCPLayerLite.shutdownAll();
            }
            catch (Exception) { }
        }

        public static void SendToClient(string triggeredEvent, string value)
        {
            if (settingsStatusEvents.Exists(e => e.Event == triggeredEvent))
            {
                List<commands.EMData> singleEMDataList = new List<commands.EMData>();
                commands.EMData emData = EMDList.Find(f => f.EMTag == settingsStatusEvents.Find(f => f.Event == triggeredEvent).EMTag);
                emData.EMValue = value;
                singleEMDataList.Add(emData);
                commands.sendPushData(singleEMDataList);
            }
        }

        #region TCP layer events
        static void TCPLayerLite_FailToConnect(List<TCPLayerLite.device> devList)
        {
            Console.WriteLine("Fail to connect to server.");
            connected = false;
            autoResetEvent.Set();
        }

        static void TCPLayerLite_ConnectionEstablished(List<TCPLayerLite.device> devList)
        {
            Console.WriteLine("Connection established.");
            connected = true;
            autoResetEvent.Set();
        }

        static void TCPLayerLite_LastConnectionLost(List<TCPLayerLite.device> devList)
        {
            Console.WriteLine("Connection lost.");
            connected = false;
            autoResetEvent.Set();
        }

        static void TCPLayerLite_NoConnectedDevice()
        {
            Console.WriteLine("Cannot send data: no connection.");
        }
        #endregion

        #region Commands events
        static void commands_GameListReceived(List<string> list)
        {
            bool found = false;
            foreach (string game in list)
            {
                if (game == settings.GameProject.GameName)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Console.WriteLine("Game is not set. Setting new game.");
                commands.sendNewGame(settings.GameProject.GameName);
            }
            else
            {
                Console.WriteLine("Game is already set.");
            }
            autoResetEvent.Set();
        }

        static void commands_ProjectListReceived(List<string> list)
        {
            bool found = false;
            foreach (string project in list)
            {
                if (project == settings.GameProject.ProjectName)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Console.WriteLine("Project is not set.");
                return;
            }
            else
            {
                Console.WriteLine("Project is already set.");
            }
            autoResetEvent.Set();
        }

        static void commands_NewCommands(List<commands.EMData> EMDList)
        {
            foreach (commands.EMData EMD in EMDList)
            {
                Console.WriteLine("Command: " + EMD.EMTag + "; Value: " + EMD.EMValue);
            }
        }

        static void commands_ConnectedClientAnswerReceived(commands.connectedClient data) { }

        static void commands_ConnectedClientLoadedProjectAnswerReceived(commands.connectedClientWithProjectLoaded data)
        {
            if (data.gameName == settings.GameProject.GameName && data.projectName == settings.GameProject.ProjectName)
            {
                Console.WriteLine("Project " + settings.GameProject.ProjectName + " is loaded on device: " + data.deviceName + " with ID hash: " + data.IDHash + ".");
            }
        }
        #endregion
    }
}
