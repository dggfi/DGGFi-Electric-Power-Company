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
    partial class Program : MyGridProgram
    {
        Dictionary<long, DGGFiAccount> _accounts = new Dictionary<long, DGGFiAccount>();

        // Power sources
        List<IMySolarPanel> _solarPanels = new List<IMySolarPanel>();
        List<IMyReactor> _reactors = new List<IMyReactor>();
        List<IMyBatteryBlock> _batteries = new List<IMyBatteryBlock>();
        List<IMyPowerProducer> _turbines = new List<IMyPowerProducer>();
        List<IMyPowerProducer> _engines = new List<IMyPowerProducer>();
        List<IMyPowerProducer> _myPowerBlocks = new List<IMyPowerProducer>();
        List<IMyPowerProducer> _allPowerBlocks = new List<IMyPowerProducer>();
        float _solarOutput = 0;
        float _reactorOutput = 0;
        float _batteryOutput = 0;
        float _turbineOutput = 0;
        float _engineOutput = 0;
        float _totalOutput = 0;

        // Feedback
        IMyTextPanel _lcdPanel;


        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Once | UpdateFrequency.Update100;
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if ((updateSource & (UpdateType.Terminal | UpdateType.Once | UpdateType.Update100)) > 0)
            {
                PopulateBlocks();
                CalculateOutputs();
                GridTerminalSystem.GetBlocksOfType(_myPowerBlocks);
                float output = 0;
                for (int i = 0; i < _myPowerBlocks.Count; i++)
                {
                    output += _myPowerBlocks[i].CurrentOutput;
                }
                _totalOutput = output;
                _lcdPanel = GridTerminalSystem.GetBlockWithName("LCD Panel") as IMyTextPanel;
                if (_lcdPanel != null)
                {
                    StringBuilder sb = new StringBuilder("");
                    sb.Append($"{_myPowerBlocks.Count} power blocks detected for this construct.\n");
                    sb.Append($"{_allPowerBlocks.Count - _myPowerBlocks.Count} external power blocks detected.\n");
                    sb.Append($"Currently outputting {_totalOutput:0.00} MW\n");
                    sb.Append($"Solar\t {_solarOutput:0.00} MW\n");
                    sb.Append($"Wind\t {_turbineOutput:0.00} MW\n");
                    sb.Append($"Hydrogen\t {_engineOutput:0.00} MW\n");
                    sb.Append($"Reactor\t {_reactorOutput:0.00} MW\n");
                    sb.Append($"Battery\t {_batteryOutput:0.00} MW\n");
                    sb.Append($"Servicing {_accounts.Count} accounts.");
                    _lcdPanel.WriteText(sb.ToString());
                }
                else
                {
                    Echo($"No LCD Pannel found. Minimal output.");
                }
            }
        }

        public void PopulateBlocks()
        {
            GridTerminalSystem.GetBlocksOfType(_solarPanels, block => block.IsSameConstructAs(Me));
            GridTerminalSystem.GetBlocksOfType(_reactors, block => block.IsSameConstructAs(Me));
            GridTerminalSystem.GetBlocksOfType(_batteries, block => block.IsSameConstructAs(Me));
            GridTerminalSystem.GetBlocksOfType(_turbines, block => (block.GetType().Name == "MyWindTurbine" && block.IsSameConstructAs(Me)));
            GridTerminalSystem.GetBlocksOfType(_engines, block => (block.GetType().Name == "MyHydrogenEngine" && block.IsSameConstructAs(Me)));
            GridTerminalSystem.GetBlocksOfType(_myPowerBlocks, block => block.IsSameConstructAs(Me));
            GridTerminalSystem.GetBlocksOfType(_allPowerBlocks);
        }
        public void CalculateOutputs()
        {
            _solarOutput = 0;
            for (int i = 0; i < _solarPanels.Count; i++)
            {
                _solarOutput += _solarPanels[i].CurrentOutput;
            }

            _reactorOutput = 0;
            for (int i = 0; i < _reactors.Count; i++)
            {
                _reactorOutput += _reactors[i].CurrentOutput;
            }

            _batteryOutput = 0;
            for (int i = 0; i < _batteries.Count; i++)
            {
                _batteryOutput += _batteries[i].CurrentOutput;
            }

            _turbineOutput = 0;
            for (int i = 0; i < _turbines.Count; i++)
            {
                _turbineOutput += _turbines[i].CurrentOutput;
            }

            _engineOutput = 0;
            for (int i = 0; i < _engines.Count; i++)
            {
                _engineOutput += _engines[i].CurrentOutput;
            }

            _totalOutput = 0;
            for (int i = 0; i < _myPowerBlocks.Count; i++)
            {
                _totalOutput += _myPowerBlocks[i].CurrentOutput;
            }
        }
    }
}
