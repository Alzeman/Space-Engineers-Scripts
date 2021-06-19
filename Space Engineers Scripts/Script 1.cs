using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using VRageMath;
using VRage.Game;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Ingame;
using Sandbox.Game.EntityComponents;
using VRage.Game.Components;
using VRage.Collections;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
namespace Script1
{
    public sealed class Program : MyGridProgram
    {
        //------------BEGIN--------------
        List<IMyTerminalBlock> hinges_d = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> hinges_s1 = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> hinges_s2 = new List<IMyTerminalBlock>();
        public Program()
        {
            GridTerminalSystem.SearchBlocksOfName("[hinge_d]", hinges_d, b => b is IMyMotorStator);
            GridTerminalSystem.SearchBlocksOfName("[hinge_s1]", hinges_s1, b => b is IMyMotorStator);
            GridTerminalSystem.SearchBlocksOfName("[hinge_s2]", hinges_s2, b => b is IMyMotorStator);
        }
        int step_d;
        int step_s1;
        int step_s2;
        int steptest;
        public void Main(string args)
        {
            switch (args)
			{
                case "stepside":

                    break;

                case "stepdown_go":
                    if (step_d == 91)
                    {
                        return;
                    }
                    else
                    { 
                        step_d++;
                        foreach (var hinge_d in hinges_d)
                        {
                            IMyMotorStator hinge_d1 = hinge_d as IMyMotorStator;

                            hinge_d1.SetValueFloat("UpperLimit", step_d);
                            hinge_d1.TargetVelocityRPM = 0.01F;
                        };
                    }
                    break;

                case "stepdown_back":
                        step_d--;
                        foreach (var hinge_d in hinges_d)
                        {
                            IMyMotorStator hinge_d1 = hinge_d as IMyMotorStator;

                            hinge_d1.SetValueFloat("UpperLimit", step_d);
                            hinge_d1.TargetVelocityRPM = -10F;
                        };
                    // Сделать переход на stop
                    break;

                case "start":
                    if (step_d == steptest)
                    {
                        step_d = -90; steptest = 999;
                        step_d++;
                        // Сделать старт ротора и буров
                    }
                    break;

                case "stop":
                    // Сделать остановку ротора и буров.
                    return;
                default:
                    return;
            }         
        }

        public void Save()
        {
        
        }

        //------------END--------------
    }
}