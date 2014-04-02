﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AIRLab.Mathematics;
using CVARC.Basic;
using CVARC.Basic.Controllers;
using Gems.Sensors;

namespace StarshipRepair.Bots
{
    public class NamiraBot : Bot
    {
        private GemsWorld world;
        private Map map;
        private DetailData? detail;
        private bool first = true;

        public override void Initialize(Competitions competitions)
        {
            base.Initialize(competitions);
            world = competitions.World as GemsWorld;
            map = world.Settings.Map;
        }
        private IEnumerable<Socket> GetSockets()
        {
            var setts = (Competitions.World as GemsWorld).Settings;
            for (var i = 0; i < setts.HorizontalWalls.GetLength(0); ++i)
                for (var j = 0; j < setts.HorizontalWalls.GetLength(1); ++j)
                {
                    if (setts.HorizontalWalls[i, j] == WallSettings.NoWall ||
                        setts.HorizontalWalls[i, j] == WallSettings.Wall)
                        continue;
                    yield return new Socket
                                     {
                                         Color = (DetailColor) (setts.HorizontalWalls[i, j] - 2),
                                         Location = new Tuple<double, double>(i, j + 0.5),
                                         LocationOne = new Point(i, j),
                                         LocationTwo = new Point(i, j + 1)
                                     };
                }
            for (var i = 0; i < setts.VerticalWalls.GetLength(0); ++i)
                for (var j = 0; j < setts.VerticalWalls.GetLength(1); ++j)
                {
                    if (setts.VerticalWalls[i, j] == WallSettings.NoWall ||
                        setts.VerticalWalls[i, j] == WallSettings.Wall)
                        continue;
                    yield return new Socket
                    {
                        Color = (DetailColor)(setts.VerticalWalls[i, j] - 2),
                        Location = new Tuple<double, double>(i+0.5, j),
                        LocationOne = new Point(i, j),
                        LocationTwo = new Point(i+1, j)
                    };
                }
        }

        public override Command MakeTurn()
        {
            var dst = Angem.Distance(
                Competitions.World.Robots[0].Body.Location.ToPoint3D(),
                Competitions.World.Robots[1].Body.Location.ToPoint3D())
                ;
            if (dst < 30) return new Command { Move = 0, Angle = Angle.FromGrad(0), Time = 1 };
            if (NeedCenter(Competitions.World.Robots[ControlledRobot]))
                return Center(Competitions.World.Robots[ControlledRobot]);
            if (detail == null)
            {
                //var details = 
                    //Competitions.Root.GetSubtreeChildrenFirst().Where(a => a.Name.StartsWith("D")).ToList();
            }
            if (first)
            {
                return new Command { Move = 30, Time = 1 };
                first = false;
            }
            return null;
        }

        private Command Center(Robot r)
        {
            var cmd = new Command();
            var pos = GetPositionOnWorld(GetPositionOnMap(r.Body.GetAbsoluteLocation()));
            var dx = pos.X - r.Body.Location.X;
            var dy = pos.Y - r.Body.Location.Y;
            var angle = Angle.FromRad(Math.Atan2(dy, dx));
            if (Math.Abs(r.Body.Location.Yaw.Grad%360 - angle.Grad%360) > 10)
            {
                var angleGrad = angle.Grad%360 - r.Body.Location.Yaw.Grad%360;
                return new Command
                           {
                               Angle = Math.Sign(angleGrad)*Competitions.AngularVelocityLimit,
                               Time = Math.Abs(angleGrad)/Competitions.AngularVelocityLimit.Grad,

                           };
            }
            var distance = Math.Sqrt(dx * dx + dy * dy);
            return new Command
                       {
                           Move = distance,
                           Time = 1
                       };
        }

        private Point GetPositionOnMap(Frame3D frame)
        {
            var pos = new Point((int) ((frame.X + 150)/50), (int) ((frame.Y + 100)/50));
            if (pos.X > 5) pos.X = 5;
            if (pos.Y > 3) pos.X = 3;
            return pos;
        }
        private Point GetPositionOnWorld(Point pnt)
        {
            return new Point(pnt.X*50-125, pnt.Y*50-75);
        }

        private bool NeedCenter(Robot robot)
        {
            var pos = GetPositionOnWorld(GetPositionOnMap(robot.Body.GetAbsoluteLocation()));
            return Math.Abs(pos.X - robot.Body.Location.X) > 20 || Math.Abs(pos.Y - robot.Body.Location.Y) > 20;
        }
    }

    internal class Socket
    {
        public DetailColor Color;
        public Point LocationOne;
        public Point LocationTwo;
        public Tuple<double, double> Location;
    }
}