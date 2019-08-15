using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nethereum.LogProcessing.Dynamic.Matching
{
    public class EventAddressMatcher : IEventAddressMatcher
    {
        public EventAddressMatcher(IEnumerable<string> addressesToMatch = null)
        {
            AddressesToMatch = addressesToMatch ?? Array.Empty<string>();
        }

        public IEnumerable<string> AddressesToMatch { get; }

        public bool IsMatch(FilterLog log)
        {
            //no addresses then assume it is a match
            if(!AddressesToMatch.Any()) return true; 

            return AddressesToMatch.Contains(log.Address, StringComparer.OrdinalIgnoreCase);
        }
    }
}
