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
            // Profile
            private long _ownerId;
            public long OwnerId { get { return _ownerId; } }
            public SortedSet<long> ConstructsServiced = new SortedSet<long>();

            // Statistics
            public float PowerDrawn { get; set; }

            // Balances
            public float RechargingBalance { get; set; }
            public float PowerBalance { get; set; }
            
            private float _totalBalance;
            public float TotalBalance { get { return RechargingBalance + PowerBalance; } set { _totalBalance = value; } }
            
            public float PastDueBalance { get; set; }

            // Policy
            public bool AgendaInsured = false;

            //public DateTime TCreated = DateTime.UtcNow;

            public DGGFiAccount(long OwnerId)
            {
                _ownerId = OwnerId;
                PowerDrawn = 0;
                RechargingBalance = 0;
                PowerBalance = 0;
                PastDueBalance = 0;
            }

            public void RegisterConstruct(IMyCubeGrid grid)
            {
                ConstructsServiced.Add(grid.EntityId);
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
