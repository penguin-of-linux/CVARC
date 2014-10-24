﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVARC.V2;

namespace DemoCompetitions.KroR
{
    public class Level1 : Competitions
    {
        public Level1()
            : base(new DemoLogicPart(), new KroREnginePart(), new DemoManagerPart())
        { }
    }
}