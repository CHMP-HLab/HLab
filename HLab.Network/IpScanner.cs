using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using HLab.Notify.Annotations;

namespace HLab.Network
{
    public class IpScanner
    {
        readonly IEventHandlerService _eventHandlerService;

        public ReadOnlyObservableCollection<string> FoundServers { get; }

        readonly ObservableCollection<string> _foundServers = new ObservableCollection<string>();

        public IpScanner(IEventHandlerService eventHandlerService)
        {
            _eventHandlerService = eventHandlerService;
            FoundServers = new ReadOnlyObservableCollection<string>(_foundServers);
        }

        public void Scan(int port)
        {
            foreach (var ipMask in GetIpAddresses())
            {
                if(ipMask.Item1.GetAddressBytes()[0]==192)
                    foreach (var ip in GetAllIpFromIpMask(ipMask.Item1,ipMask.Item2))
                    {
                        ScanAsync(ip, port);
                    }
            }

        }

        public bool Scan(IPAddress ip, int port)
        {
            var ping = new Ping();
            var ret = ping.Send(ip, 5);
            if (ret.Status == IPStatus.Success)
            {
                using var scan = new TcpClient();
                try
                {

                    scan.ReceiveTimeout = 30;
                    scan.SendTimeout = 30;
                    scan.Connect(ip, port);
                    if (scan.Connected)
                    {
                        var host = Dns.GetHostEntry(ip);
                        _eventHandlerService.Invoke(() =>
                        {
                            _foundServers.Add(host.HostName);
                        });
                    }
                }
                catch
                {
                    return false;
                }

            }

            return false;

        }


        Task<bool> ConnectAsync (IPAddress ip, int port)
        {
            var task = new Task<bool>(() =>
            {
                using var scan = new TcpClient();
                try
                {

                    scan.ReceiveTimeout = 30;
                    scan.SendTimeout = 30;
                    scan.Connect(ip, port);
                    return scan.Connected;
                }
                catch(Exception e)
                {
                    return false;
                }
            });

            task.Start();

            return task;
        }

        public async Task<bool> ScanAsync(IPAddress ip, int port)
        {
            var ret = await new Ping().SendPingAsync(ip, 500);

            if (ret.Status == IPStatus.Success)
            {
                Debug.WriteLine($"ping {ret.Address} -> {ret.RoundtripTime}");
                if (await ConnectAsync(ip, port))
                {
                    string server = ip.ToString();
                    try
                    {
                        server = (await Dns.GetHostEntryAsync(ip)).HostName;
                    }
                    catch(SocketException)
                    {

                    }
                    _eventHandlerService.Invoke(() => { _foundServers.Add(server); });
                }
            }

            return false;
        }

        static IEnumerable<Tuple<IPAddress,IPAddress>> GetIpAddresses()
        {
            foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (var ipInfo in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (ipInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        yield return Tuple.Create(ipInfo.Address,ipInfo.IPv4Mask);
                    }
                }
            }
        }

        static IEnumerable<IPAddress> GetAllIpFromIpMask(IPAddress ipAddress, IPAddress maskAddress)
        {
            var ip = ToUInt32(ipAddress);
            var mask = ToUInt32(maskAddress);

            var network = ip & mask;
            var broadcast = network | ~mask;


            var current = network + 1;
            while (current<broadcast)
            {
                yield return FromUInt32(current);
                current++;
            }

        }

        static IPAddress GetNetworkAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
            }
            return new IPAddress(broadcastAddress);
        }

        static UInt32 ToUInt32(IPAddress addr)
        {
            var ipBytes = addr.GetAddressBytes();
            var ip = (uint)ipBytes [0] << 24;
            ip += (uint)ipBytes [1] << 16;
            ip += (uint)ipBytes [2] <<8;
            ip += (uint)ipBytes [3];
            return ip;
        }

        static IPAddress FromUInt32(UInt32 addr)
        {
            return new IPAddress(new byte[]{(byte)(addr>>24),(byte)(addr>>16),(byte)(addr>>8),(byte)(addr)});
        }

    }
}
