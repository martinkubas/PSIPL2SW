using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Projekt
{
    public enum SyslogSeverity
    {
        Emergency = 0,
        Alert = 1,
        Critical = 2,
        Error = 3,
        Warning = 4,
        Notice = 5,
        Informational = 6,
        Debug = 7
    }

    public class SyslogClient
    {
        private IPAddress sourceIP;
        private IPAddress serverIP;
        private int serverPort = 514;
        private bool isEnabled = false;
        private UdpClient udpClient;

        public SyslogClient()
        {
        }

        public bool IsEnabled => isEnabled;

        public bool Configure(string sourceIPStr, string serverIPStr)
        {
            try
            {
                if (!IPAddress.TryParse(sourceIPStr, out IPAddress newSourceIP))
                {
                    LogLocalOnly("Invalid source IP format", SyslogSeverity.Error);
                    return false;
                }

                if (!IPAddress.TryParse(serverIPStr, out IPAddress newServerIP))
                {
                    LogLocalOnly("Invalid server IP format", SyslogSeverity.Error);
                    return false;
                }

                sourceIP = newSourceIP;
                serverIP = newServerIP;

                return true;
            }
            catch (Exception ex)
            {
                LogLocalOnly($"Error configuring syslog: {ex.Message}", SyslogSeverity.Error);
                return false;
            }
        }

        public void Start()
        {
            if (sourceIP == null || serverIP == null)
            {
                LogLocalOnly("Cannot start Syslog: Source IP or Server IP not configured", SyslogSeverity.Error);
                return;
            }

            try
            {
                udpClient = new UdpClient(new IPEndPoint(sourceIP, 0));
                isEnabled = true;
                Log("Syslog service started", SyslogSeverity.Informational);
            }
            catch (Exception ex)
            {
                LogLocalOnly($"Error starting syslog service: {ex.Message}", SyslogSeverity.Error);
            }
        }

        public void Stop()
        {
            if (isEnabled)
            {
                Log("Syslog service stopping", SyslogSeverity.Informational);
                isEnabled = false;
                udpClient?.Close();
                udpClient = null;
            }
        }

        public void Log(string message, SyslogSeverity severity)
        {
            if (!isEnabled || serverIP == null)
            {
                LogLocalOnly(message, severity);
                return;
            }

            try
            {
                string timestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                string formattedMessage = $"[{timestamp}] [{severity.ToString().ToUpper()}] [{sourceIP}] {message}";

                byte[] data = Encoding.ASCII.GetBytes(formattedMessage);
                udpClient.Send(data, data.Length, new IPEndPoint(serverIP, serverPort));

                LogLocalOnly(message, severity);
            }
            catch (Exception ex)
            {
                LogLocalOnly($"Error sending syslog message: {ex.Message}", SyslogSeverity.Error);
            }
        }

        private void LogLocalOnly(string message, SyslogSeverity severity)
        {
            string timestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            Console.WriteLine($"[LOCAL] [{timestamp}] [{severity.ToString().ToUpper()}] {message}");
        }
    }
}