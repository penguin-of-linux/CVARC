﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.V2;

namespace RepairTheStarship
{
    public interface IRTSWorldManager : IWorldManager
    {
        void RemoveDetail(string detailId);
        void ShutTheWall(string wallId);
    }
}
