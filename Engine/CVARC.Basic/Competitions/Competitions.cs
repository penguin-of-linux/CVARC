﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AIRLab.Mathematics;
using CVARC.Basic.Controllers;
using CVARC.Core;
using CVARC.Core.Replay;
using CVARC.Graphics;
using CVARC.Physics;

namespace CVARC.Basic
{
    public class Competitions
    {
        public readonly World World;
        public readonly RobotBehaviour Behaviour;
        public readonly KeyboardController KeyboardController;
        public readonly NetworkController NetworkController;
        public double GameTimeLimit { get; protected set; }
        public double NetworkTimeLimit { get; protected set; }
        public double LinearVelocityLimit { get; protected set; }
        public Angle AngularVelocityLimit { get; protected set; }
        public Body Root { get; private set; }
        public DrawerFactory DrawerFactory { get; private set; }
        public Dictionary<string, Type> AvailableBots { get; private set; }
        public ReplayLogger Logger { get; private set; }

        public Competitions(World world, RobotBehaviour behaviour, KeyboardController keyboard, NetworkController network)
        {
            World = world;
            Behaviour = behaviour;
            KeyboardController = keyboard;
            NetworkController = network;
            GameTimeLimit = 90;
            NetworkTimeLimit = 1;
            AngularVelocityLimit = Angle.FromGrad(20);
            LinearVelocityLimit = 10;
            AvailableBots = new Dictionary<string, Type>();
        }

        public void ApplyCommand(Command command)
        {
            Behaviour.ProcessCommand(World.Robots[command.RobotId], command);
        }

        public static Competitions Load(string filename)
        {
            if (!File.Exists(filename))
            {
                MessageBox.Show("The assembly file you specified does not exist", "CVARC Tutorial", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            var file = new FileInfo(filename);
            var ass = Assembly.LoadFile(file.FullName);
            var competitions = ass.GetExportedTypes().FirstOrDefault(a => a.IsSubclassOf(typeof(Competitions)));
            var ctor = competitions.GetConstructor(new Type[] { });
            return ctor.Invoke(new object[] { }) as Competitions;
        }

        public void Initialize()
        {
            Root = World.Init();
            PhysicalManager.InitializeEngine(PhysicalEngines.Farseer, Root);
            DrawerFactory = new DrawerFactory(Root);
            Behaviour.InitSensors();
            Behaviour.Sensors.ForEach(a =>
            {
                foreach (var robot in World.Robots)
                {
                    var sens = a.GetOne(robot, World, DrawerFactory);
                    robot.Sensors.Add(sens);
                }
            });
            Logger = new ReplayLogger(Root, 0.1);
        }

        public void MakeCycle(double time, bool realtime)
        {
            double dt = 1.0 / 100;
            int span=(int)(dt*1000);
            for (double t = 0; t < time; t += dt)
            {
                foreach (var robot in World.Robots)
                    robot.SetVelocity(); 
                PhysicalManager.MakeIteration(dt, Root);
                foreach (Body body in Root)
                    body.Update(1 / 60);
                if (realtime)
                    Thread.Sleep(span);
            }
            if (CycleFinished != null)
                CycleFinished(this, EventArgs.Empty);
        }

        public event EventHandler CycleFinished;

        public void ProcessOneParticipant(bool realtime, IParticipant participant)
        {
            double time = GameTimeLimit;
            while (true)
            {
                var command = participant.MakeTurn();
                Behaviour.ProcessCommand(World.Robots[command.RobotId], command);
                MakeCycle(Math.Min(time, command.Time), realtime);
                time -= command.Time;
                if (time < 0) break;
            }
        }

        public void ProcessParticipants(bool realTime, params IParticipant[] participants)
        {
            double time = GameTimeLimit;
            Command[] states = participants.Select(z => new Command()).ToArray();
            
            while (true)
            {
                for(int i=0;i<participants.Length;i++)
                    if (states[i].Time == 0)
                    {
                        states[i] = participants[i].MakeTurn();
                        Behaviour.ProcessCommand(World.Robots[states[i].RobotId], states[i]);
                    }
                var minTime = Math.Min(time,states.Min(z => z.Time));
                MakeCycle(minTime, realTime);
                foreach (var e in states) e.Time -= minTime;
                time -= minTime;
                if (time < 0) break;
            }
        }

        public Bot CreateBot(string name, int controlledBot)
        {
            if (!AvailableBots.ContainsKey(name)) throw new Exception("Bot was not found");
            var tp = AvailableBots[name];
            var ctor = tp.GetConstructor(new Type[] { });
            var bot = ctor.Invoke(new object[]{}) as Bot;
            bot.Initialize(this, controlledBot);
            return bot;

        }

        public string SendPostReplay(string key, int competitionsId, int robotNumber)
        {
            try
            {
                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection();
                    data["key"] = key;
                    data["competitions"] = competitionsId.ToString();
                    data["robotNumber"] = robotNumber.ToString();
                    data["results"] = "[" +
                                      string.Join(",",
                                                  Enumerable.Range(0, World.RobotCount)
                                                            .Select(
                                                                a =>
                                                                "{num:" + a + ", score: " +
                                                                World.Score.GetFullSumForRobot(a) + "}")) + "]";
                    var replay = ConverterToJavaScript.Convert(Logger.SerializationRoot);
                    data["log"] = replay;
                    wb.UploadValues("http://air-labs.ru/match/save", "POST", data);
                    return "???"; //TODO: where do I get id of the replay?
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "";
            }
        }
    }
}
