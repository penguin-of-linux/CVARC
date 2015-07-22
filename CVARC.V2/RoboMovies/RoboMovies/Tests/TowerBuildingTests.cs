﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.V2;
using AIRLab.Mathematics;

namespace RoboMovies
{
    partial class RMLogicPartHelper
    {
        [TestLoaderMethod]
        public void LoadTowerBuilderTests(LogicPart logic, MoveAndBuildRules rules)
        {
            AddTest(logic, "Grip_Single_Detail", TowerBuilderTest(
                1,
                rules.Move(65),
                rules.Rotate(-Angle.HalfPi),
                rules.Move(25),
                rules.Stand(0.1),
                rules.Collect()
            ));

            AddTest(logic, "Grip_Multiple_Details", TowerBuilderTest(
                2,
                rules.Move(65),
                rules.Rotate(-Angle.HalfPi),
                rules.Move(25),
                rules.Stand(0.1),
                rules.Collect(),
                rules.Move(15),
                rules.Rotate(Angle.HalfPi),
                rules.Move(10),
                rules.Stand(1),
                rules.Collect()
            ));

            AddTest(logic, "Grip_Light_Before_Stand", TowerBuilderTest(
                1,
                rules.Rotate(Angle.Pi),
                rules.Move(10),
                rules.Stand(0.1),
                rules.Collect()
            ));

            AddTest(logic, "Grip_Light_After_Stand", TowerBuilderTest(
                1,
                rules.Move(65),
                rules.Rotate(-Angle.HalfPi),
                rules.Move(25),
                rules.Stand(0.1),
                rules.Collect(),
                rules.Rotate(Angle.Pi),
                rules.Move(25),
                rules.Rotate(Angle.HalfPi),
                rules.Move(75),
                rules.Stand(0.1),
                rules.Collect()
            ));
            
            AddTest(logic, "Release_Single_Detail", TowerBuilderTest(
                0,
                rules.Move(65),
                rules.Rotate(-Angle.HalfPi),
                rules.Move(25),
                rules.Stand(0.1),
                rules.Collect(),
                rules.BuildTower()
            ));

            AddTest(logic, "Release_Multiple_Detail", TowerBuilderTest(
                0,
                rules.Move(65),
                rules.Rotate(-Angle.HalfPi),
                rules.Move(25),
                rules.Stand(0.1),
                rules.Collect(),
                rules.Move(15),
                rules.Rotate(Angle.HalfPi),
                rules.Move(10),
                rules.Stand(0.1),
                rules.Collect(),
                rules.BuildTower()
            ));

            AddTest(logic, "Release_Through_Border", TowerBuilderTest(
                1,
                rules.Move(100),
                rules.Rotate(-Angle.HalfPi),
                rules.Move(25),
                rules.Stand(0.1),
                rules.Collect(),
                rules.Move(100),
                rules.Stand(0.1),
                rules.BuildTower(),
                rules.Move(-50),
                rules.Stand(0.1)
            ));

            AddTest(logic, "Grip_Through_Border", TowerBuilderTest(
                0,
                rules.Move(100),
                rules.Rotate(-Angle.HalfPi),
                rules.Move(25),
                rules.Stand(0.1),
                rules.Collect(),
                rules.Move(100),
                rules.Stand(0.1),
                rules.BuildTower(),
                rules.Collect(),
                rules.Move(-50),
                rules.Stand(0.1)
            ));

            AddTest(logic, "Grip_Tower", TowerBuilderTest(
                2,
                rules.Move(65),
                rules.Rotate(-Angle.HalfPi),
                rules.Move(25),
                rules.Stand(0.1),
                rules.Collect(),
                rules.Move(15),
                rules.Rotate(Angle.HalfPi),
                rules.Move(10),
                rules.Stand(0.1),
                rules.Collect(),
                rules.BuildTower(),
                rules.Collect()
            ));
        }
    }
}
