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
        public class ChargingFrame
        {
            public float OtherOutput { get; set; }
            public float OtherInput { get; set; }
            public float OtherStored { get; set; }
            public float MyStored { get; set; }

            public ChargingFrame()
            {
                OtherOutput = 0;
                OtherInput = 0;
                OtherStored = 0;
            }
        }

        public class ServicingPort
        {
            // Init
            private Program _program;
            private IMyShipConnector _connector;
            private List<IMyTextPanel> _panels;
            private Dictionary<long, DGGFiAccount> _accounts;
            
            // State
            private MyShipConnectorStatus _prevConnectionStatus;

            // Connection
            private DGGFiAccount _account;
            private List<IMyPowerProducer> _otherPowerProducers = new List<IMyPowerProducer>();
            private List<IMyBatteryBlock> _otherBatteries = new List<IMyBatteryBlock>();

            // Frames
            private ChargingFrame _prevFrame = new ChargingFrame();
            private ChargingFrame _currFrame = new ChargingFrame();

            // Accessors
            public IMyShipConnector Connector { get { return _connector; } }


            public ServicingPort(Program program, IMyShipConnector connector, List<IMyTextPanel> panels, Dictionary<long, DGGFiAccount> accounts)
            {
                _program = program;
                _connector = connector;
                _panels = panels;
                _accounts = accounts;

                if ((connector.Status & MyShipConnectorStatus.Connected) > 0)
                {
                    _prevConnectionStatus = MyShipConnectorStatus.Connected;
                    _program.GridTerminalSystem.GetBlocksOfType(_otherPowerProducers, block => block.IsSameConstructAs(connector.OtherConnector) && block.GetType().Name != "IMyBatteryBlock");
                    _program.GridTerminalSystem.GetBlocksOfType(_otherBatteries, block => block.IsSameConstructAs(connector.OtherConnector));
                    UpdateCurrentFrame();
                }
                else
                {
                    _prevConnectionStatus =  MyShipConnectorStatus.Unconnected;
                }
            }

            public void UpdatePreviousFrame()
            {
                _prevFrame.OtherOutput = _currFrame.OtherOutput;
                _prevFrame.OtherInput = _currFrame.OtherInput;
                _prevFrame.OtherStored = _currFrame.OtherStored;
            }

            public void UpdateCurrentFrame()
            {
                float output = 0;
                float input = 0;
                float stored = 0;
                for (int i = 0; i < _otherPowerProducers.Count; i++)
                {
                    output += _otherPowerProducers[i].CurrentOutput;
                }
                for (int i = 0; i < _otherBatteries.Count; i++)
                {
                    var battery = _otherBatteries[i];
                    if (battery.IsCharging)
                    {
                        input += _otherBatteries[i].CurrentInput;
                        stored += _otherBatteries[i].CurrentStoredPower;
                    }
                }
                _currFrame.OtherOutput = output;
                _currFrame.OtherInput = input;
                _currFrame.OtherStored = stored;
            }

            public void Update()
            {
                // There is a connected grid
                if ((_connector.Status & MyShipConnectorStatus.Connected) > 0)
                {
                    UpdatePreviousFrame();
                    UpdateCurrentFrame();

                    DGGFiAccount account = _accounts.GetValueOrDefault(_connector.OwnerId, null);
                    if (account == null)
                    {
                        account = new DGGFiAccount(_connector.OtherConnector.OwnerId);
                        _accounts.Add(_connector.OtherConnector.OwnerId, account);
                    }
                    _account = account;

                    if ((_prevConnectionStatus | MyShipConnectorStatus.Unconnected) == 0)
                    {
                        _program.GridTerminalSystem.GetBlocksOfType(_otherPowerProducers, block => block.IsSameConstructAs(_connector.OtherConnector) && block.GetType().Name != "IMyBatteryBlock");
                        _program.GridTerminalSystem.GetBlocksOfType(_otherBatteries, block => block.IsSameConstructAs(_connector.OtherConnector));
                        account.RegisterConstruct(_connector.OtherConnector.CubeGrid);
                    }

                    float otherOutput = _currFrame.OtherOutput - _prevFrame.OtherOutput;
                    float otherInput = _currFrame.OtherInput - _prevFrame.OtherInput;
                    float otherStored = _currFrame.OtherStored - _prevFrame.OtherStored;
                    if (otherStored > 0)
                    {
                        account.PowerDrawn += otherStored;
                        account.RechargingBalance = account.PowerDrawn * _program._price;
                    }
                    _prevConnectionStatus = MyShipConnectorStatus.Connected;
                }
                else
                {
                    _prevConnectionStatus = MyShipConnectorStatus.Unconnected;
                }
                PaintPanel();
            }

            public void PaintPanel()
            {
                IMyTextPanel panel = _program.GridTerminalSystem.GetBlockWithName($"{_connector.CustomName} LCD") as IMyTextPanel;
                if (_account != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"\tWelcome, {_account.OwnerId}!\n\n");
                    sb.Append($"\tYou have had \t\t {_account.ConstructsServiced.Count} ship(s) serviced with us.\n");
                    sb.Append($"\tYou have recharged \t\t {_account.PowerDrawn} MW \n");
                    sb.Append($"\tYour balance with us today is \t\t ${_account.RechargingBalance:0.00} \t\t SC\n");
                    panel.WriteText(sb.ToString());
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Welcome to the DGGFi electric power station.\n");
                    sb.Append("Dock your ship to begin charging!\n");
                    sb.Append("(ONLY Space Credits accepted as legal tender)");
                    panel.WriteText(sb.ToString());
                }
            }
        }
    }
}
