﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CVARC.Basic;
using CVARC.Basic.Controllers;
using CVARC.Basic.Core.Participants;
using CVARC.Network;
using Gems;

namespace CVARC.Tutorial
{
    internal static class Program
    {
        private const string CompetitionsName = "Fall2013.0.dll";
        private static TutorialForm form;
        private static CompetitionsBundle competitionsBundle;
        private static readonly KeyboardController Controller = new KeyboardController();

        [STAThread]
        static void Main(string[] args)
        {
            SimpleLogger.Run();
            competitionsBundle = CompetitionsBundle.Load(CompetitionsName, "Level1");
            competitionsBundle.Competitions.HelloPackage = new HelloPackage { MapSeed = 1 };
            competitionsBundle.Competitions.Initialize(new CVARCEngine(competitionsBundle.Rules),
                new[] { new RobotSettings(0, false), new RobotSettings(1, true) });

            var botName = args.FirstOrDefault() ?? "None";
            RunForm(competitionsBundle.Competitions.CreateBot(botName, 1));
        }

        private static void RunForm(Participant participant)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form = new TutorialForm(competitionsBundle.Competitions) {KeyPreview = true};
            form.KeyDown += (sender, e) => competitionsBundle.Competitions.ApplyCommand(Controller.GetCommand(e.KeyCode));
            form.KeyUp += (sender, e) => competitionsBundle.Competitions.ApplyCommand(Command.Sleep(0));
            Task.Factory.StartNew(() => competitionsBundle.Competitions.ProcessParticipants(true, int.MaxValue, false, true, participant));
            Application.Run(form);
        }
    }
}
