﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.Basic;

namespace CVARC.V2
{
    public class TutorialRunMode : StandardRunMode
    {
        IKeyboardControllerPool pool;

        override public IController GetController(string controllerId)
        {
            return pool.CreateController(controllerId);
        }

        override public void Initialize(Configuration configuration, Competitions competitions)
        {
            base.Initialize(configuration, competitions);
            this.pool = competitions.Logic.KeyboardControllerPoolFactory();
            pool.Initialize(competitions.Logic.World, competitions.Engine.Keyboard);
        }

    }
}
