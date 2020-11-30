﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using Newtonsoft.Json;

namespace WizLib
{
    public interface IBulb
    {
        PhysicalAddress MACAddress { get; }

        IPAddress IPAddress { get; }

        int Port { get; }

        string Name { get; set; }

        Task<Bulb> GetBulb();

    }

    public class BulbItem : IBulb
    {
        [JsonProperty("mac")]
        public virtual PhysicalAddress MACAddress { get; protected set; }

        [JsonProperty("addr")]
        public virtual IPAddress IPAddress { get; protected set; }

        [JsonProperty("port")]
        public virtual int Port { get; protected set; }

        [JsonProperty("name")]
        public virtual string Name { get; set; }

        public static BulbItem CreateItemFromBulb(IBulb source)
        {
            return new BulbItem()
            {
                MACAddress = source.MACAddress,
                IPAddress = source.IPAddress,
                Port = source.Port,
                Name = source.Name
            };
        }

        public static async Task<IList<Bulb>> CreateBulbFromInterfaceList(IEnumerable<IBulb> source)
        {
            var l = new List<Bulb>();

            foreach (var b in source)
            {
                l.Add(await b.GetBulb());
            }

            return l;
        }

        public async Task<Bulb> GetBulb()
        {
            return await GetBulb(ScanCondition.NotFound);
        }

        public async Task<Bulb> GetBulb(ScanCondition sc)
        {
            Bulb b;

            b = await Bulb.GetBulbByMacAddr(MACAddress, sc);

            if (b == null)
            {
                b = new Bulb(IPAddress, Port);
            }

            await b?.GetPilot();
            return b;
        }

    }



}