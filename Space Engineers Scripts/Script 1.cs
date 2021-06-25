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
        int direction = 1;

        string args_b = null;

        public void Main(string args)
        {
            //Считывание аргументов после Runtime.UpdateFrequency + список исключений для ввода нового аргумента в программном блоке

            if (   
                args_b != null
                & args != "start"
                & args != "stop"
                & args != "side_go"
                & args != "side_back"
                & args != "down_go"
                & args != "down_back"
                & args != "all_back"
                )
            {
                args = args_b.ToString();
            };


            //Реализация аргумента (выбор действия, основываясь на аргументе.

            switch (args)
            {


                //Лапы в стороны

                case "side_go":   

                    step_s++;

                    if (step_s > 180) { step_s = 180; };   //Защита от закликивания игроком

                    for (int step_c = step_s; -1 < step_c & step_c < 181;)   //Счетчик от 0 до 180 и математика шарниров
                    {
                        int step_s1; int step_s2; int step_s3;

                        step_s1 = -90; step_s2 = -90; step_s3 = 0;
                        step_s1 = step_s1 + step_c / 2;   //-90 to 0
                        step_s2 = step_s2 + step_c;   //-90 to 90
                        step_s3 = step_s3 + step_c / 2;   //0 to 90

                        foreach (IMyMotorStator hinge_s1 in hinges_s1)   //Передача значений в шарниры
                        {
                            hinge_s1.SetValueFloat("UpperLimit", step_s1);
                            hinge_s1.SetValueFloat("LowerLimit", -90);
                            hinge_s1.TargetVelocityRPM = 0.1F;
                        };
                        foreach (IMyMotorStator hinge_s2 in hinges_s2)   //Передача значений в шарниры
                        {
                            hinge_s2.SetValueFloat("UpperLimit", step_s2);
                            hinge_s2.SetValueFloat("LowerLimit", -90);
                            hinge_s2.TargetVelocityRPM = 0.2F;
                        };
                        foreach (IMyMotorStator hinge_s3 in hinges_s3)   //Передача значений в шарниры
                        {
                            hinge_s3.SetValueFloat("UpperLimit", step_s3);
                            hinge_s3.SetValueFloat("LowerLimit", 0);
                            hinge_s3.TargetVelocityRPM = 0.1F;
                        };

                        if (step_c != 180)   //Проверка на полное раскрытие
                        {
                            time = 0;
                            args = "wait_side_go";
                        }
                        else
                        {
                            time = 0;
                            args = "wait_down_go";
                            direction = 0;
                        };
                        break;
                    };
                    break;


                //Таймер раскрытия лап

                case "wait_side_go":

                    foreach (IMyMotorStator rotor_1 in rotor)
                    {
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


                //Таймер между полноым раскрытием лап и их возвратом

                case "wait_side_back":

                    foreach (IMyMotorStator rotor_1 in rotor)
                    {
                        if (Math.Round(rotor_1.Angle, 2) == 0 & time > 300)
                        {
                            time = 0;
                            args = "side_back";
                        }
                        else if (Math.Round(rotor_1.Angle, 2) == 0.00 & time > 300)
                        {
                            time = 0;
                            args = "side_back";
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


                //Лапы обратно

                case "side_back":   

                    step_s--;
                    if (step_s < 0) { step_s = 0; };   //Защита от закликивания игроком

                    for (int step_c = step_s; -1 < step_c & step_c < 181;)   //Счетчик от 0 до 180 и математика шарниров
                    {
                        int step_s1; int step_s2; int step_s3;

                        step_s1 = -90; step_s2 = -90; step_s3 = 0;
                        step_s1 = step_s1 + step_c / 2;   //-90 to 0
                        step_s2 = step_s2 + step_c;   //-90 to 90
                        step_s3 = step_s3 + step_c / 2;   //0 to 90

                        foreach (IMyMotorStator hinge_s1 in hinges_s1)   //Передача значений в шарниры
                        {
                            hinge_s1.SetValueFloat("UpperLimit", 0);
                            hinge_s1.SetValueFloat("LowerLimit", step_s1);
                            hinge_s1.TargetVelocityRPM = -0.1F;
                        };
                        foreach (IMyMotorStator hinge_s2 in hinges_s2)   //Передача значений в шарниры
                        {
                            hinge_s2.SetValueFloat("UpperLimit", 90);
                            hinge_s2.SetValueFloat("LowerLimit", step_s2);
                            hinge_s2.TargetVelocityRPM = -0.2F;
                        };
                        foreach (IMyMotorStator hinge_s3 in hinges_s3)   //Передача значений в шарниры
                        {
                            hinge_s3.SetValueFloat("UpperLimit", 90);
                            hinge_s3.SetValueFloat("LowerLimit", step_s3);
                            hinge_s3.TargetVelocityRPM = -0.1F;
                        };

                        if (step_c != 0)   //Проверка на полное раскрытие
                        {
                            time = 0;
                            args = "wait_side_back";
                        }
                        else
                        {
                            time = 0;
                            args = "wait_down_go";
                            direction = 1;
                        };
                        break;
                    };

                    time = 0;
                    break;


                //Таймер между возвратом лап и шагом платформы вниз

                case "wait_down_go":   
                    
                    foreach (IMyMotorStator hinge_d in hinges_d)
                    {

                        if (Math.Round(hinge_d.Angle, 2) == 0 & time > 300)
                        {
                            time = 0;
                            args = "down_go";
                        }
                        else if (Math.Round(hinge_d.Angle, 2) == 0.00 & time > 300)
                        {
                            time = 0;
                            args = "down_go";
                        }
                        else if (Math.Round(hinge_d.Angle, 2) == 3.14 & time > 300)
                        {
                            time = 0;
                            args = "down_go";
                        }
                        else
                        {
                            time++;
                            args = "wait_down_go";
                        };
                    };
                    break;


                //Платформа вниз

                case "down_go":   

                    step_d++;
                    if (step_d > 180) { step_d = 180; };   //Защита от закликивания игроком

                    for (int step_c = step_d; step_c < 181;)   //Счетчик от 0 до 180
                    {
                        int step_d1; step_d1 = -90;
                        step_d1 = step_d1 + step_c;

                        foreach (IMyMotorStator hinge_d in hinges_d)   //Передача значений в шарниры
                        {
                            hinge_d.SetValueFloat("UpperLimit", step_d1);
                            hinge_d.SetValueFloat("LowerLimit", -90);
                            hinge_d.TargetVelocityRPM = 0.1F;
                        };

                        if (step_c != 180)   //Проверка на полное раскрытие
                        {
                            time = 0;

                            if (direction == 1)
                            {
                                args = "side_go";
                                step_s--;
                            }
                            else
                            {
                                args = "side_back";
                                step_s++;
                            }
                        }
                        else
                        {
                            time = 0;
                            args = "wait_all_back";
                        };
                        break;
                    };
                    break;

                //Платформа обратно

                case "down_back":
                    step_d--;
                    if (step_d < 0) { step_d = 0; };   //Защита от закликивания игроком

                    for (int step_c = step_d; step_c < 181;)   //Счетчик от 0 до 180
                    {
                        int step_d1; step_d1 = -90;
                        step_d1 = step_d1 + step_c;

                        foreach (IMyMotorStator hinge_d in hinges_d)   //Передача значений в шарниры
                        {
                            hinge_d.SetValueFloat("UpperLimit", 90);
                            hinge_d.SetValueFloat("LowerLimit", step_d1);
                            hinge_d.TargetVelocityRPM = -0.1F;
                        };
                        args = args_b.ToString();
                        break;
                    };
                    break;


                //Таймер между полным опусканием платформы и перед её возвратом

                case "wait_all_back":

                    foreach (IMyMotorStator rotor_1 in rotor)
                    {
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


                //Всё обратно

                case "all_back":   

                    step_d = 0;
                    step_s = 0;

                    foreach (IMyMotorStator hinge_d in hinges_d)   //Передача значений в шарниры
                    {
                        hinge_d.SetValueFloat("UpperLimit", 90);
                        hinge_d.SetValueFloat("LowerLimit", -90);
                        hinge_d.TargetVelocityRPM = -1;
                    };

                    for (int step_c = step_s; -1 < step_c & step_c < 181;)   //Передача значений в шарниры
                    {
                        int step_s1; int step_s2; int step_s3;

                        step_s1 = -90; step_s2 = -90; step_s3 = 0;
                        step_s1 = step_s1 + step_c / 2;   //-90 to 0
                        step_s2 = step_s2 + step_c;   //-90 to 90
                        step_s3 = step_s3 + step_c / 2;   //0 to 90

                        foreach (IMyMotorStator hinge_s1 in hinges_s1)   //Передача значений в шарниры
                        {
                            hinge_s1.SetValueFloat("UpperLimit", 0);
                            hinge_s1.SetValueFloat("LowerLimit", step_s1);
                            hinge_s1.TargetVelocityRPM = -1;
                        };
                        foreach (IMyMotorStator hinge_s2 in hinges_s2)   //Передача значений в шарниры
                        {
                            hinge_s2.SetValueFloat("UpperLimit", 90);
                            hinge_s2.SetValueFloat("LowerLimit", step_s2);
                            hinge_s2.TargetVelocityRPM = -2;
                        };
                        foreach (IMyMotorStator hinge_s3 in hinges_s3)   //Передача значений в шарниры
                        {
                            hinge_s3.SetValueFloat("UpperLimit", 90);
                            hinge_s3.SetValueFloat("LowerLimit", step_s3);
                            hinge_s3.TargetVelocityRPM = -1;
                        };
                        break;
                    };

                    time = 0;
                    args = "wait_stop";

                    break;


                //Старт скрипта

                case "start":   

                    foreach (IMyMotorStator rotor_1 in rotor)   //Запуск ротора
                    {
                        rotor_1.ApplyAction("OnOff_On");
                        rotor_1.SetValueFloat("Torque", 100000000);
                        rotor_1.SetValueFloat("BrakingTorque", 0);
                        rotor_1.TargetVelocityRPM = 1;
                    };

                    foreach (IMyShipDrill drills_1 in drills)   //Запуск буров
                    {
                        drills_1.ApplyAction("OnOff_On");
                    };

                    foreach (IMyMotorStator hinge_s1 in hinges_s1)   //Вычисление позиции лап
                    {
                        int step_s2;

                        step_s2 = (int)(hinge_s1.Angle * 57.3);
                        if (step_s2 > 90)
                        {
                            step_s2 = step_s2 - 360;
                        }
                        step_s = 2 * (step_s2 + 90) - 1;
                    };

                    foreach (IMyMotorStator hinge_d1 in hinges_d)   //Вычисление позиции платформы
                    {
                        int step_d2;

                        step_d2 = (int)(hinge_d1.Angle * 57.3);
                        if (step_d2 > 90)
                        {
                            step_d2 = step_d2 - 360;
                        }
                        step_d = step_d2 + 90;
                    };

                    Runtime.UpdateFrequency = UpdateFrequency.Update10;
                    time = 0;

                    if (Me.CustomData != null)
					{
                        args = Me.CustomData;
                    }
					else
					{
                        args = "side_go";
                    }
                    break;


                //Таймер между возвратом платформы и остановкой срипта

                case "wait_stop":   

                    if (time != 1000)
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


                //Остановка ротора, буров и скрипта

                case "stop":   

                    foreach (IMyMotorStator rotor_1 in rotor)   //Остановка ротора
                    {

                        rotor_1.ApplyAction("OnOff_Off");
                        rotor_1.SetValueFloat("Torque", 0);
                        rotor_1.SetValueFloat("BrakingTorque", 100000000);
                        rotor_1.TargetVelocityRPM = 0;
                    };

                    foreach (IMyShipDrill drills_1 in drills)   //Остановка буров
                    {
                        drills_1.ApplyAction("OnOff_Off");
                    };

                    time = 0;
                    args = "stop";
                    Runtime.UpdateFrequency = UpdateFrequency.None;
                    break;

                default:   //Любой иной аргумен не делает ничего.
                    break;

            }    //switch end


            //Выведение переменных для дебага на дипсплей

            foreach (IMyMotorStator rotor_1 in rotor)
            {
                int time_hour_i; int time_min_i; int time_sec_i;
                string time_hour_s; string time_min_s; string time_sec_s;
                string step_s_p; string step_d_p;

                foreach (IMyTextPanel lcd in lcds)
                {
                    lcd.SetValue("FontSize", 1.7F);

                    //Математика процентов

                    if (step_s != 0)
					{
                        step_s_p = Math.Round(0.55F * step_s).ToString();
                        if (step_s_p == "99") { step_s_p = "100"; }
                    }
                    else
					{
                        step_s_p = 0.ToString();
                    }
                    if (step_d != 0)
                    {
                        step_d_p = Math.Round(0.55F * step_d).ToString();
                        if (step_d_p == "99") { step_d_p = "100"; }
                    }
                    else
                    {
                        step_d_p = 0.ToString();
                    }

                    //Математика часов

                    time_hour_i = (time / 21160) % 60;   //Вычисление часов
                    time_min_i = (time / 360) % 60;   //Вычисление минут
                    time_sec_i = (time / 6) % 60;   //Вычисление секунд

                    if   //Приведение часов к нужному формату
                        (time_hour_i < 10)
                    { time_hour_s = "0" + (time_hour_i).ToString(); }
                    else
                    { time_hour_s = (time_hour_i).ToString(); };

                    if   //Приведение минут к нужному формату
                        (time_min_i < 10)
                    { time_min_s = "0" + (time_min_i).ToString(); }
                    else
                    { time_min_s = (time_min_i).ToString(); };

                    if   //Приведение секунд к нужному формату
                        (time_sec_i < 10)
                    { time_sec_s = "0" + (time_sec_i).ToString(); }
                    else
                    { time_sec_s = (time_sec_i).ToString(); };

                    //Выведение на дисплей

                    lcd.WriteText
                        (
                        "<<  " + "argument" + "  >>" + "\n" + "<<  " + args.ToString() + "  >>", false   //Аргумент и его значение
                        );
                    lcd.WriteText
                        (
                        "\n" + "\n" + "step side = " + step_s.ToString() + "  ( " + step_s_p + "% )", true   //step side (переменная step_s) с процентами
                        );
                    lcd.WriteText
                        (
                        "\n" + "step down = " + step_d.ToString() + "  ( " + step_d_p + "% )", true   //step down (переменная step_d) с процентами
                        );
                    lcd.WriteText
                        (
                        "\n" + "\n" + "time value  = " + time.ToString(), true   //Число тиков скрипта
                        );
                    lcd.WriteText
                        (
                        "\n" + "time = " + time_hour_s + ":" + time_min_s + ":" + time_sec_s, true   //Время ожидания действия
                        );
                    lcd.WriteText
                        (
                        "\n" + "rotor radian = " + Math.Round(rotor_1.Angle, 2).ToString(), true   //Положение ротора в радианах
                        );
                    lcd.WriteText
                        (
                        "\n" + "rotor degree = " + Math.Round(rotor_1.Angle * 57.3).ToString() + "°", true   //Положение ротора в градусах
                        );
                };

            };

            args_b = args.ToString();   //Сохранение аргументов от Runtime.UpdateFrequency
        }

        public void Save()
        {
            Me.CustomData = args_b;
        }

        //------------END--------------
    }
}