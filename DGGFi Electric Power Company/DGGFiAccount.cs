using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class DGGFiAccount
        {
            private long _ownerId;
            public long OwnerId { get { return _ownerId; } }

            public SortedSet<long> ConstructsServiced = new SortedSet<long>();

            public int RechargingBalance;
            public int PowerBalance;
            public int PastDueBalance;
            
            public bool AgendaInsured = false;

            //public DateTime TCreated = DateTime.UtcNow;

            public DGGFiAccount(long OwnerId)
            {
                _ownerId = OwnerId;
            }
        }

        // Future implementation

        //public class DGGFiCycle
        //{
        //    private DateTime _tCreated;

        //    public DGGFiCycle()
        //    {

        //    }
        //}

        //public class DGGFiPayment
        //{
        //    public DateTime TPaid { get; }
        //    public decimal AmountPaid { get; }
        //    // public DGGFiCycle BillingCycle {get; }

        //    public DGGFiPayment(DateTime tPaid, decimal amountPaid)
        //    {
        //        TPaid = tPaid;
        //        AmountPaid = amountPaid;
        //        BillingCycle = cycle;
        //    }
        //}
    }
}
