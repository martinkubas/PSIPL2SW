using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;
using System.Net.NetworkInformation;

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
        private ushort serverPort = 514;
        private bool isEnabled = false;
        private List<LibPcapLiveDevice> devices;


        public SyslogClient()
        {
            devices = new List<LibPcapLiveDevice>();
        }

        public bool IsEnabled()
        {
            return isEnabled;
        }

        public bool Configure(string sourceIPStr, string serverIPStr, List<LibPcapLiveDevice> deviceList = null)
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

                                
                devices = new List<LibPcapLiveDevice>(deviceList);
                LogLocalOnly($"Using {devices.Count} provided network devices for Syslog", SyslogSeverity.Informational);
                

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
            if (serverIP == null || devices.Count == 0)
            {
                LogLocalOnly("Cannot start Syslog: Server IP not configured or no devices available", SyslogSeverity.Error);
                return;
            }

            try
            {
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
            }
        }

        public void Log(string message, SyslogSeverity severity)
        {
            if (!isEnabled || serverIP == null || devices.Count == 0)
            {
                LogLocalOnly(message, severity);
                return;
            }

            try
            {
                string timestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                string formattedMessage = $"[{timestamp}] [{severity.ToString().ToUpper()}] [{sourceIP}] {message}";
                byte[] messageBytes = Encoding.ASCII.GetBytes(formattedMessage);

                foreach (var device in devices)
                {
                    SendCustomPacket(device, messageBytes);
                }

                LogLocalOnly(message, severity);
            }
            catch (Exception ex)
            {
                LogLocalOnly($"Error sending syslog message: {ex.Message}", SyslogSeverity.Error);
            }
        }

        private void SendCustomPacket(LibPcapLiveDevice device, byte[] payload)
        {
            try
            {
                if (!device.Started)
                {
                    return;
                }

                EthernetPacket ethernetPacket = new EthernetPacket(
                    device.MacAddress,
                    PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"),
                    EthernetType.IPv4);

                IPv4Packet ipPacket = new IPv4Packet(sourceIP, serverIP);

                UdpPacket udpPacket = new UdpPacket(12345, serverPort);
                udpPacket.PayloadData = payload;

                ipPacket.PayloadPacket = udpPacket;
                ethernetPacket.PayloadPacket = ipPacket;

                device.SendPacket(ethernetPacket);
            }
            catch (Exception ex)
            {
                LogLocalOnly($"Error sending custom packet through device {device.Description}: {ex.Message}", SyslogSeverity.Error);
            }
        }

        private void LogLocalOnly(string message, SyslogSeverity severity)
        {
            string timestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            Console.WriteLine($"[LOCAL] [{timestamp}] [{severity.ToString().ToUpper()}] {message}");
        }
    }
}