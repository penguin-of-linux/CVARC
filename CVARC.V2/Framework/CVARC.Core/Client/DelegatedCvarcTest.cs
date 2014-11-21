﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CVARC.V2
{
    public class DelegatedCvarcTest<TSensorData, TCommand, TWorld> : CvarcTest<TSensorData, TCommand, TWorld>
        where TSensorData : class
    {
        public abstract ConfigurationProposal GetConfiguration();
        
        Action<CvarcClient<TSensorData,TCommand>,TWorld, IAsserter> test;
        public override void Test(CvarcClient<TSensorData, TCommand> client, TWorld world, IAsserter asserter)
        {
            test(client, world, asserter);
        }
        public DelegatedCvarcTest(Action<CvarcClient<TSensorData,TCommand>,TWorld,IAsserter> test)
        {
            this.test = test;
        }
    }
}
