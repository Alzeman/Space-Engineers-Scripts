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
        List<IMyTerminalBlock> hinges_s3 = new List<IMyTerminalBlock>();
        public Program()
        {
            //Наполнение списков уникальными именами объектов из игры

            GridTerminalSystem.SearchBlocksOfName("[hinge_d]", hinges_d, b => b is IMyMotorStator);
            GridTerminalSystem.SearchBlocksOfName("[hinge_s1]", hinges_s1, b => b is IMyMotorStator);
            GridTerminalSystem.SearchBlocksOfName("[hinge_s2]", hinges_s2, b => b is IMyMotorStator);
            GridTerminalSystem.SearchBlocksOfName("[hinge_s3]", hinges_s3, b => b is IMyMotorStator);
        }
        int step_d;
        int steptest;
        //int step_s1;   //-90 to 0
        //int step_s2;   //0 to 180
        //int step_s3;   //0 to 90
        public void Main(string args)
        {
            switch (args)
			{
                case "stepside_go":   //Лапы в стороны
                    for (int step_sc = 0; step_sc <180; step_sc++)
                    {
                        int step_s1; int step_s2; int step_s3;

                        step_s1 = -90; step_s2 = -90; step_s3 = 0;
                        step_s1 = step_s1 + step_sc / 2;   //-90 to 0
                        step_s2 = step_s2 + step_sc;   //-90 to 90
                        step_s3 = step_s3 + step_sc / 2;   //0 to 90

                        for (int i = 0; i< hinges_s1.Count; i++)   //Передача значений в шарниры
                        {
                            IMyMotorStator hinge_s1 = hinges_s1[i] as IMyMotorStator;
                            IMyMotorStator hinge_s2 = hinges_s2[i] as IMyMotorStator;
                            IMyMotorStator hinge_s3 = hinges_s3[i] as IMyMotorStator;

                            hinge_s1.SetValueFloat("UpperLimit", step_s1);
                            hinge_s1.TargetVelocityRPM = 1;
                            hinge_s2.SetValueFloat("UpperLimit", step_s2);
                            hinge_s2.TargetVelocityRPM = 2;
                            hinge_s3.SetValueFloat("UpperLimit", step_s3);
                            hinge_s3.TargetVelocityRPM = 1;
                        };
                    };
                    // Сделать таймер с циклом
                    // Сделать переход на stepside_back
                    break;

                case "stepside_back":   //Лапы обратно
                    for (int step_sc = 0; step_sc < 180; step_sc++)
                    {
                        int step_s1; int step_s2; int step_s3;

                        step_s1 = 0; step_s2 = 90; step_s3 = 90;
                        step_s1 = step_s1 - step_sc / 2;   //0 to -90
                        step_s2 = step_s2 - step_sc;   //90 to -90
                        step_s3 = step_s3 - step_sc / 2;   //0 to 90

                        for (int i = 0; i < hinges_s1.Count; i++)   //Передача значений в шарниры
                        {
                            IMyMotorStator hinge_s1 = hinges_s1[i] as IMyMotorStator;
                            IMyMotorStator hinge_s2 = hinges_s2[i] as IMyMotorStator;
                            IMyMotorStator hinge_s3 = hinges_s3[i] as IMyMotorStator;

                            hinge_s1.SetValueFloat("UpperLimit", step_s1);
                            hinge_s1.TargetVelocityRPM = 1;
                            hinge_s2.SetValueFloat("UpperLimit", step_s2);
                            hinge_s2.TargetVelocityRPM = 2;
                            hinge_s3.SetValueFloat("UpperLimit", step_s3);
                            hinge_s3.TargetVelocityRPM = 1;
                        };
                    };
                    // Сделать таймер с циклом
                    // Сделать переход на stepdown_go
                    break;

                case "stepdown_go":   //Платформа вниз
                    if (step_d == 91)
                    {
                        break;
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
                    // Сделать переход на stepdown_back
                    // Сделать таймер с циклом
                    break;

                case "stepdown_back":   //Платформа обратно
                        step_d--;
                        foreach (var hinge_d in hinges_d)
                        {
                            IMyMotorStator hinge_d1 = hinge_d as IMyMotorStator;

                            hinge_d1.SetValueFloat("UpperLimit", step_d);
                            hinge_d1.TargetVelocityRPM = -10F;
                        };
                    // Сделать переход на stop
                    // Сделать таймер с циклом
                    break;

                case "start":   //Старт скрипта
                    if (step_d == steptest)
                    {
                        step_d = -90; ; steptest = 999;
                        // Сделать старт ротора и буров
                        break;
                    }
                    break;

                case "stop":   //Остановка скрипта
                    // Сделать остановку ротора и буров.
                    break;
                default:   //Любой иной аргумен вызывает остановку скрипта
                    break;
            }         
        }

        public void Save()
        {
        
        }

        //------------END--------------
    }
}