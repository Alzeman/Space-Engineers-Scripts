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

        List<IMyTerminalBlock> rotor = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> drills = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> hinges_d = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> hinges_s1 = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> hinges_s2 = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> hinges_s3 = new List<IMyTerminalBlock>();

        public Program()
        {
            //Наполнение списков уникальными именами объектов из игры

            GridTerminalSystem.SearchBlocksOfName("[rotor]", rotor, b => b is IMyMotorStator);
            GridTerminalSystem.SearchBlocksOfName("[drill]", drills, b => b is IMyShipDrill);
            GridTerminalSystem.SearchBlocksOfName("[hinge_d]", hinges_d, b => b is IMyMotorStator);
            GridTerminalSystem.SearchBlocksOfName("[hinge_s1]", hinges_s1, b => b is IMyMotorStator);
            GridTerminalSystem.SearchBlocksOfName("[hinge_s2]", hinges_s2, b => b is IMyMotorStator);
            GridTerminalSystem.SearchBlocksOfName("[hinge_s3]", hinges_s3, b => b is IMyMotorStator);
        }
        int step_d = 0;
        int step_s = 0;
        public void Main(string args)
        {
            switch (args)
            {
                case "stepside_go":   //Лапы в стороны
                    step_s++;
                    for (int step_c = 0; step_c < 181 & step_c < step_s; step_c++)   //Счетчик от 0 до 180 и математика шарниров
                    {
                        int step_s1; int step_s2; int step_s3;

                        step_s1 = -90; step_s2 = -90; step_s3 = 0;
                        step_s1 = step_s1 + step_c / 2;   //-90 to 0
                        step_s2 = step_s2 + step_c;   //-90 to 90
                        step_s3 = step_s3 + step_c / 2;   //0 to 90

                        for (int i = 0; i < hinges_s1.Count; i++)   //Передача значений в шарниры
                        {
                            var hinge_s1 = hinges_s1[i] as IMyMotorStator;
                            var hinge_s2 = hinges_s2[i] as IMyMotorStator;
                            var hinge_s3 = hinges_s3[i] as IMyMotorStator;

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
                    step_s = 0;
                    for (int i = 0; i < hinges_s1.Count; i++)   //Передача значений в шарниры
                    {
                        var hinge_s1 = hinges_s1[i] as IMyMotorStator;
                        var hinge_s2 = hinges_s2[i] as IMyMotorStator;
                        var hinge_s3 = hinges_s3[i] as IMyMotorStator;

                        hinge_s1.SetValueFloat("UpperLimit", 0);
                        hinge_s1.TargetVelocityRPM = -1;
                        hinge_s2.SetValueFloat("UpperLimit", 90);
                        hinge_s2.TargetVelocityRPM = -2;
                        hinge_s3.SetValueFloat("UpperLimit", 90);
                        hinge_s3.TargetVelocityRPM = -1;
                    };
                    // Сделать таймер с циклом
                    // Сделать переход на stepdown_go
                    break;

                case "stepdown_go":   //Платформа вниз
                    step_d++;
                    for (int step_c = 0; step_d < 181 & step_c < step_d; step_c++) //Счетчик от 0 до 180
                    {
                        for (int i = 0; i < hinges_d.Count; i++)   //Передача значений в шарниры
                        {
                            var hinge_d = hinges_d[i] as IMyMotorStator;

                            int step_d1; step_d1 = -90;
                            step_d1 = step_d1 + step_c;

                            hinge_d.SetValueFloat("UpperLimit", step_d1);
                            hinge_d.TargetVelocityRPM = 0.01F;
                        };
                    };
                    // Сделать переход на stepdown_back
                    // Сделать таймер с циклом
                    break;

                case "stepdown_back":   //Платформа обратно
                    step_d = 0;
                    for (int i = 0; i < hinges_d.Count; i++)   //Передача значений в шарниры
                    {
                        var hinge_d = hinges_d[i] as IMyMotorStator;

                        hinge_d.SetValueFloat("UpperLimit", 90);
                        hinge_d.TargetVelocityRPM = -1;
                    };
                    // Сделать переход на stop
                    // Сделать таймер с циклом
                    break;

                case "start":   //Старт работы
                    for (int i = 0; i < hinges_s1.Count; i++)
                    {
                        var rotor_1 = rotor[i] as IMyMotorStator;
                        var drills_1 = drills[i] as IMyMotorStator;

                        rotor_1.ApplyAction("OnOff_On");
                        rotor_1.SetValueFloat("Torque", 100);
                        rotor_1.SetValueFloat("BrakingTorque", 0);
                        rotor_1.TargetVelocityRPM = 0.01F;
                        drills_1.ApplyAction("OnOff_On");
                    };
                    break;

                case "stop":   //Остановка работы
                    for (int i = 0; i < hinges_s1.Count; i++)
                    {
                        var rotor_1 = rotor[i] as IMyMotorStator;
                        var drills_1 = drills[i] as IMyMotorStator;

                        rotor_1.ApplyAction("OnOff_Off");
                        rotor_1.SetValueFloat("Torque", 0);
                        rotor_1.SetValueFloat("BrakingTorque", 100);
                        rotor_1.TargetVelocityRPM = 0;
                        drills_1.ApplyAction("OnOff_Off");
                    };
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