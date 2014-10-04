﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.Basic.Core.Participants;

namespace CVARC.V2
{


    public abstract class World<TSceneState,TWorldManager> : IWorld
        where TWorldManager : IWorldManager
    {
        List<IActor> actors;

        public TSceneState SceneState { get; private set; }
        public IEngine Engine { get; private set; }
        public TWorldManager Manager { get; private set; }
        IWorldManager IWorld.Manager { get { return Manager; } }
        public IdGenerator IdGenerator { get; private set; }
        public WorldClocks Clocks { get; private set; }

        protected abstract IEnumerable<IActor> CreateActors();

        public void Initialize(Competitions competitions, CompetitionsEnvironment environment)
        {
            Clocks = new WorldClocks();
            IdGenerator = new IdGenerator();

            //Initializing world
            this.SceneState = (TSceneState)environment.SceneSettings;
            this.Engine = competitions.Engine;
            this.Manager = (TWorldManager)competitions.WorldManager;
            Engine.Initialize(this);
            Manager.Initialize(this);
            Manager.CreateWorld(IdGenerator);


            //Initializing actors
            actors = CreateActors().ToList();
            foreach(var e in actors)
            {
                var actorObjectId = IdGenerator.CreateNewId(e);
                var rules=competitions.ActorManagerFactories.Where(z=>z.ActorManagerType==e.GetManagerType).FirstOrDefault();
                IActorManager manager = null;
                if (rules != null)
                    manager = rules.Generate();
                e.Initialize(manager,this, actorObjectId);
                if (manager != null)
                {
                    manager.Initialize(e);
                    manager.CreateActorBody();
                }
            }



            //Initializing controllable actors
            var controllable = actors.OfType<IControllable>().ToArray();
            var used=new HashSet<int>();
            foreach (var e in controllable)
            {
                var number = e.ControllerNumber;
                if (used.Contains(number))
                    throw new Exception("The controller number " + number + " is used more than once");
                var controller=environment.Controllers.Where(z=>z.ControllerNumber==number).FirstOrDefault();
                if (controller==null)
                    throw new Exception("The controller number " + number + " is not found in controllers pool");
                controller.Initialize(this);
                e.AcceptParticipant(controller);
                used.Add(number);
            }
            var unusedControllers=environment.Controllers
                .Select(z=>z.ControllerNumber)
                .Where(z=>!used.Contains(z))
                .ToArray();
            if (unusedControllers.Length != 0)
            {
                var unused = unusedControllers.Select(z => z.ToString()).Aggregate((a, b) => a + " " + b);
                throw new Exception("The controller numbers " + unused + " were unused");
            }
        }


    }
}
