using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace PPChecker
{
    internal class Subnet
    {
        private const string PATTERN = @"^\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}(/\d{1,2})?$";

        public IPAddress NetworkAddress { get; private set; }
        public uint PrefixBits { get; private set; }
        public int PrefixLength { get; private set; }

        public Subnet(string networkDefinition)
        {
            AssertValidDescription(networkDefinition);
            var parts = networkDefinition.Split('/');
            NetworkAddress = IPAddress.Parse(parts[0]);
            var ipBits = IpToUInt(NetworkAddress);
            PrefixLength = parts.Length == 2 ? int.Parse(parts[1]) : 32;
            PrefixBits = GetNetworkAddressMaskFor(ipBits, PrefixLength);
        }

        public bool Matches(string ip)
        {
            return Matches(IPAddress.Parse(ip));
        }

        public bool Matches(IPAddress ip)
        {
            if (PrefixLength == 0)
            {
                return true;
            }

            var ipBits = IpToUInt(ip);
            var subnetBits = GetNetworkAddressMaskFor(ipBits, PrefixLength);

            return subnetBits == PrefixBits;
        }

        public static uint IpToUInt(IPAddress address)
        {
            byte[] bytes = address.GetAddressBytes();
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        private uint GetNetworkAddressMaskFor(uint ip, int cidr)
        {
            uint bitMask = uint.MaxValue << (32 - cidr);
            return bitMask & ip;
        }

        private void AssertValidDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description) || !Regex.IsMatch(description, PATTERN, RegexOptions.Compiled))
            {
                throw new ArgumentException("Value is not a valid CIDR subnet string: " + description);
            }
        }
    }
    public static class UserIpAddress
    {
        // We ignore ip address specified in the X-Forwarded-For header if they are private.
        // X-Forwarded-For header can be a list of comma separated IP address, each potentially added by an internal proxy;
        // the first one which is not a subnet address is the original client ip.
        private static readonly List<Subnet> PrivateSubnets =
            new List<Subnet>()
                {
                    new Subnet("10.0.0.0/8"),
                    new Subnet("172.16.0.0/12"),
                    new Subnet("192.168.0.0/16"),
                };


        /// <summary>
        /// Extract the client IP from HTTP headers ('X-Forwarded-For' or 'Forwarded'). Ignores any address that 
        /// belongs to a private subnet.
        /// </summary>
        /// <param name="requestHeaders">The request headers.</param>
        /// <returns>A string containing a valid IP address (e.g. "1.2.3.4").</returns>
        public static string GetFromHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders)
        {
            if (requestHeaders == null)
            {
                throw new ArgumentNullException("requestHeaders");
            }

            requestHeaders = requestHeaders.ToList();
            IEnumerable<string> headers;
            if (TryGetHeader(requestHeaders, "X-Forwarded-For", out headers))
            {
                return GetIpFromXff(headers);
            }

            // note: this part is not complete
            // it does not cover cases like the following:
            // Forwarded: for=192.0.2.43, for=198.51.100.17
            if (TryGetHeader(requestHeaders, "Forwarded", out headers))
            {
                foreach (var header in headers)
                {
                    var ipPart = header.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(x => x.Split('='))
                                       .FirstOrDefault(kvp => kvp[0] == "for");
                    if (ipPart != null)
                    {
                        return ipPart[1];
                    }
                }
            }

            return null;
        }

        private static bool TryGetHeader(IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders, string headerName, out IEnumerable<string> headers)
        {
            requestHeaders = requestHeaders.ToList();
            if (requestHeaders.Any(kvp => kvp.Key.Equals(headerName, StringComparison.InvariantCultureIgnoreCase)))
            {
                var keyValuePair = requestHeaders.First(kvp => kvp.Key.Equals(headerName, StringComparison.InvariantCultureIgnoreCase));
                if (keyValuePair.Value != null)
                {
                    headers = keyValuePair.Value;
                    return true;
                }
            }

            headers = null;
            return false;
        }

        private static string GetIpFromXff(IEnumerable<string> headers)
        {
            var listOfIps = headers.First();
            string[] addresses = listOfIps.Split(',').Select(x => x.Trim()).ToArray();
            if (addresses.Length == 0)
            {
                return null;
            }

            if (addresses.Length == 1)
            {
                return addresses[0];
            }

            return addresses.FirstOrDefault(IsNotPrivateSubnet);
        }

        private static bool IsNotPrivateSubnet(string address)
        {
            var ipAddress = IPAddress.Parse(address);
            return !PrivateSubnets.Any(x => x.Matches(ipAddress));
        }
    }
}