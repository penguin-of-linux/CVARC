﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.V2;
using RoboMovies.Bots;

namespace RoboMovies
{
    public partial class RMLogicPartHelper : LogicPartHelper
    {
        public override LogicPart Create()
        {
            var rules = RMRules.Current;

            var logicPart = new LogicPart();
            logicPart.CreateWorld = () => new RMWorld();
            logicPart.CreateDefaultSettings = () => new Settings { OperationalTimeLimit = 5, TimeLimit = 90 };
            logicPart.CreateWorldState = stateName => new RMWorldState() { Seed = int.Parse(stateName) };
            logicPart.PredefinedWorldStates.AddRange(Enumerable.Range(0, 10).Select(z => z.ToString()));
            logicPart.WorldStateType = typeof(RMWorldState);

            var actorFactory = ActorFactory.FromRobot(new RMRobot<FullMapSensorData>(), rules);
            logicPart.Actors[TwoPlayersId.Left] = actorFactory;
            logicPart.Actors[TwoPlayersId.Right] = actorFactory;

            logicPart.Bots["Stand"] = () => rules.CreateStandingBot();

            LoadTowerBuilderTests(logicPart, rules);

            return logicPart;
        }
    }
}
