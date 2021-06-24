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

        List<IMyTerminalBlock> lcds = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> rotor = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> drills = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> hinges_d = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> hinges_s1 = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> hinges_s2 = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> hinges_s3 = new List<IMyTerminalBlock>();

        public Program()
        {
            //Наполнение списков уникальными именами объектов из игры

            GridTerminalSystem.SearchBlocksOfName("[lcd]", lcds, b => b is IMyTextPanel);
            GridTerminalSystem.SearchBlocksOfName("[rotor]", rotor, b => b is IMyMotorStator);
            GridTerminalSystem.SearchBlocksOfName("[drill]", drills, b => b is IMyShipDrill);
            GridTerminalSystem.SearchBlocksOfName("[hinge_d]", hinges_d, b => b is IMyMotorStator);
            GridTerminalSystem.SearchBlocksOfName("[hinge_s1]", hinges_s1, b => b is IMyMotorStator);
            GridTerminalSystem.SearchBlocksOfName("[hinge_s2]", hinges_s2, b => b is IMyMotorStator);
            GridTerminalSystem.SearchBlocksOfName("[hinge_s3]", hinges_s3, b => b is IMyMotorStator);
        }
        int step_s = 0;
        int step_d = 0;
        int time = 0;

        string args_b = null;

        public void Main(string args)
        {
            if (   //Считывание аргументов после Runtime.UpdateFrequency + список исключений для ввода нового аргумента в программном блоке
                args_b != null
                & args != "start"
                & args != "stop"
                & args != "side_go"
                & args != "side_back"
                & args != "down_go"
                & args != "all_back"
                )   
            {
                args = args_b.ToString();
            };

            switch (args)
            {
                case "side_go":   //Лапы в стороны

                    step_s++;
                    if (step_s > 180) { step_s = 180; };   //Защита от закликивания игроком

                    for (int step_c = step_s; step_c < 181;)   //Счетчик от 0 до 180 и математика шарниров
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
                            hinge_s1.TargetVelocityRPM = 0.1F;
                            hinge_s2.SetValueFloat("UpperLimit", step_s2);
                            hinge_s2.TargetVelocityRPM = 0.2F;
                            hinge_s3.SetValueFloat("UpperLimit", step_s3);
                            hinge_s3.TargetVelocityRPM = 0.1F;
                        };

                        if (step_c != 180)   //Проверка на полное раскрытие
                        {
                            time = 0;
                            args = "wait_side_go";
                            break;
                        }
                        else
                        {
                            time = 0;
                            args = "wait_side_back";
                            break;
                        };
                    };
                    break;

                case "wait_side_go":   //Таймер раскрытия лап
                        
                    for (int i = 0; i < rotor.Count; i++)
                    {
                        var rotor_1 = rotor[i] as IMyMotorStator;

                        if (Math.Round(rotor_1.Angle, 2) == 0 & time > 300)
                        {
                            time = 0;
                            args = "side_go";
                        }
                        else if (Math.Round(rotor_1.Angle, 2) == 0.00 & time > 300)
                        {
                            time = 0;
                            args = "side_go";
                        }
                        else if (Math.Round(rotor_1.Angle, 2) == 3.14 & time > 300)
                        {
                            time = 0;
                            args = "side_go";
                        }
                        else
                        {
                            time++;
                            args = "wait_side_go";
                        };
                    };
                    break;

                case "wait_side_back":   //Таймер между полноым раскрытием лап и их возвратом

                    for (int i = 0; i < rotor.Count; i++)
                    {
                        var rotor_1 = rotor[i] as IMyMotorStator;

                        if (Math.Round(rotor_1.Angle, 2) == 0 & time > 300)
                        {
                            time = 0;
                            args = "side_back";
                        }
                        else if (Math.Round(rotor_1.Angle, 2) == 0.00 & time > 300)
                        {
                            time = 0;
                            args = "side_go";
                        }
                        else if (Math.Round(rotor_1.Angle, 2) == 3.14 & time > 300)
                        {
                            time = 0;
                            args = "side_back";
                        }
                        else
                        {
                            time++;
                            args = "wait_side_back";
                        };
                    };
                    break;

                case "side_back":   //Лапы обратно

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

                    time = 0;
                    args = "wait_down_go";
                    break;

                case "wait_down_go":   //Таймер между возвратом лап и шагом платформы вниз
                    if (time != 100)
                    {
                        time++;
                        args = "wait_down_go";
                    }
                    else
                    {
                        time = 0;
                        args = "down_go";
                    };
                    break;

                case "down_go":   //Платформа вниз

                    step_d++;
                    if (step_d > 180) { step_d = 180; };   //Защита от закликивания игроком

                    for (int step_c = step_d; step_c < 181;)   //Счетчик от 0 до 180
                    {
                        for (int i = 0; i < hinges_d.Count; i++)   //Передача значений в шарниры
                        {
                            var hinge_d = hinges_d[i] as IMyMotorStator;

                            int step_d1; step_d1 = -90;
                            step_d1 = step_d1 + step_c;

                            hinge_d.SetValueFloat("UpperLimit", step_d1);
                            hinge_d.TargetVelocityRPM = 0.1F;
                        };

                        if (step_c != 180)   //Проверка на полное раскрытие
                        {
                            time = 0;
                            args = "side_go";
                            break;
                        }
                        else
                        {
                            time = 0;
                            args = "wait_all_back";
                            break;
                        };
                    };
                    break;

                case "wait_all_back":   //Таймер между полным опусканием платформы и перед её возвратом

                    for (int i = 0; i < rotor.Count; i++)
                    {
                        var rotor_1 = rotor[i] as IMyMotorStator;

                        if (Math.Round(rotor_1.Angle, 2) == 0 & time > 300)
                        {
                            time = 0;
                            args = "all_back";
                        }
                        else if (Math.Round(rotor_1.Angle, 2) == 0.00 & time > 300)
                        {
                            time = 0;
                            args = "side_go";
                        }
                        else if (Math.Round(rotor_1.Angle, 2) == 3.14 & time > 300)
                        {
                            time = 0;
                            args = "all_back";
                        }
                        else
                        {
                            time++;
                            args = "wait_all_back";
                        };
                    };

                    break;

                case "all_back":   //Платформа обратно

                    step_d = 0;
                    step_s = 0;

                    for (int i = 0; i < hinges_d.Count; i++)   //Передача значений в шарниры
                    {
                        var hinge_d = hinges_d[i] as IMyMotorStator;

                        hinge_d.SetValueFloat("UpperLimit", 90);
                        hinge_d.TargetVelocityRPM = -1;
                    };
                    
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

                    time = 0;
                    args = "wait_stop";

                    break;

                case "wait_stop":   //Таймер между возвратом платформы и остановкой срипта

                    if (time != 100)
                    {
                        time++;
                        args = "wait_stop";
                    }
                    else
                    {
                        time = 0;
                        args = "stop";
                    };
                    break;

                case "start":   //Старт работы

                    for (int i = 0; i < rotor.Count; i++)   //Запуск ротора
                    {
                        var rotor_1 = rotor[i] as IMyMotorStator;

                        rotor_1.ApplyAction("OnOff_On");
                        rotor_1.SetValueFloat("Torque", 100000000);
                        rotor_1.SetValueFloat("BrakingTorque", 0);
                        rotor_1.TargetVelocityRPM = 0.1F;
                    };

                    for (int i = 0; i < drills.Count; i++)   //Запуск буров
                    {
                        var drills_1 = drills[i] as IMyShipDrill;

                        drills_1.ApplyAction("OnOff_On");
                    };

                    for (int i = 0; i < hinges_s1.Count; i++)   //Вычисление положения лап
                    {
                        var hinge_s1 = hinges_s1[i] as IMyMotorStator;
                        int step_s2;

                        
                        step_s2 = (int)(hinge_s1.Angle * 57.3);
                        if (step_s2 > 90)
						{
                            step_s2 = step_s2 - 360;
						}
                        step_s = 2 * (step_s2 + 90) - 1;
                    };

                    for (int i = 0; i < hinges_d.Count; i++)   //Вычисление положения нижней платформы
                    {
                        var hinge_s1 = hinges_d[i] as IMyMotorStator;
                        int step_d2;


                        step_d2 = (int)(hinge_s1.Angle * 57.3);
                        if (step_d2 > 90)
                        {
                            step_d2 = step_d2 - 360;
                        }
                        step_d = step_d2 + 90;
                    };

                    Runtime.UpdateFrequency = UpdateFrequency.Update10;
                    time = 0;
                    args = "side_go";
                    break;

                case "stop":   //Остановка ротора, буров и скрипта

                    for (int i = 0; i < rotor.Count; i++)
                    {
                        var rotor_1 = rotor[i] as IMyMotorStator;

                        rotor_1.ApplyAction("OnOff_Off");
                        rotor_1.SetValueFloat("Torque", 0);
                        rotor_1.SetValueFloat("BrakingTorque", 100000000);
                        rotor_1.TargetVelocityRPM = 0;
                    };
                    for (int i = 0; i < drills.Count; i++)
                    {
                        var drills_1 = drills[i] as IMyShipDrill;

                        drills_1.ApplyAction("OnOff_Off");
                    };
                    time = 0;
                    args = "stop";
                    Runtime.UpdateFrequency = UpdateFrequency.None;
                    break;

                default:   //Любой иной аргумен не делает ничего.
                    break;

            }    //switch end


            for (int i1 = 0; i1 < rotor.Count; i1++)   //Выведение переменных для дебага на дипсплей
            {
                var rotor_1 = rotor[i1] as IMyMotorStator;
                int time_hour_i; int time_min_i; int time_sec_i;
                string time_hour_s; string time_min_s; string time_sec_s;

                for (int i = 0; i < lcds.Count; i++)
                {
                    var lcd = lcds[i] as IMyTextPanel;
                    lcd.SetValue("FontSize", 1.7F);

                    //Математика часов

                    time_hour_i = (time / 1560) % 60;
                    time_min_i = (time / 360) % 60;
                    time_sec_i = (time / 6) % 60;

                    if
                        (time_hour_i < 10)
                        { time_hour_s = "0" + (time_hour_i).ToString(); }
                    else
                        { time_hour_s = (time_hour_i).ToString(); };

                    if
                        (time_min_i < 10)
                        { time_min_s = "0" + (time_min_i).ToString(); }
                    else
                        { time_min_s = (time_min_i).ToString(); };

                    if
                        (time_sec_i < 10)
                        { time_sec_s = "0" + (time_sec_i).ToString(); }
                    else
                        { time_sec_s = (time_sec_i).ToString(); };

                    //Выведение на дисплей

                    lcd.WriteText
                        (
                        "argument = " + args.ToString(), false
                        );
                    lcd.WriteText
                        (
                        "\n" + "\n" + "step side = " + step_s.ToString(), true
                        );
                    lcd.WriteText
                        (
                        "\n" + "step down = " + step_d.ToString(), true
                        );
                    lcd.WriteText
                        (
                        "\n" + "\n" + "time value  = " + time.ToString(), true
                        );
                    lcd.WriteText
                        (
                        "\n" + "time = " + time_hour_s + ":" + time_min_s + ":" + time_sec_s, true
                        );
                    lcd.WriteText
                        (
                        "\n" + "rotor radian = " + Math.Round(rotor_1.Angle, 2).ToString(), true
                        );
                    lcd.WriteText
                        (
                        "\n" + "rotor degree = " + Math.Round(rotor_1.Angle * 57.3) .ToString() + "°", true
                        );
                };

            };

            args_b = args.ToString();   //Сохранение аргументов от Runtime.UpdateFrequency
        }

        public void Save()
        {

        }

        //------------END--------------
    }
}