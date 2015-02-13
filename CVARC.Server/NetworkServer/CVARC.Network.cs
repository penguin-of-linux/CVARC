﻿using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using CVARC.Basic;
using CVARC.Basic.Core.Participants;
using Gems;

namespace CVARC.Network
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            InternalMain();
        }

        public static void InternalMain(CompetitionsSettings settings = null)
        {
            SimpleLogger.Run();
            RunForm(settings ?? InitCompetition());
        }

        private static void RunForm(CompetitionsSettings settings)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new TutorialForm(settings.CompetitionsBundle.competitions);
            Task.Factory.StartNew(() => settings.CompetitionsBundle.competitions.ProcessParticipants(settings.RealTime, 60 * 1000, settings.Participants));
            Application.Run(form);
        }

        private static readonly HelloPackage CustomHelloPackage = new HelloPackage
        {
            LevelName = "Level2",
            MapSeed = 276,
            Opponent = "None",
            Side = Side.Left
        };

        private static CompetitionsSettings InitCompetition()
        {
            var participantsServer = new ParticipantsServer("Fall2013.0.dll");
            var participant = participantsServer.GetParticipant(CustomHelloPackage);
            var participants = new Participant[2];
            participants[participant.ControlledRobot] = participant;
            var botNumber = participant.ControlledRobot == 0 ? 1 : 0;
            participantsServer.CompetitionsBundle.competitions.Initialize(new CVARCEngine(participantsServer.CompetitionsBundle.Rules),
                new[] { new RobotSettings(participant.ControlledRobot, false), new RobotSettings(botNumber, true) });
            var botName = participantsServer.CompetitionsBundle.competitions.HelloPackage.Opponent ?? "None";
            participants[botNumber] = participantsServer.CompetitionsBundle.competitions.CreateBot(botName, botNumber);
            return new CompetitionsSettings
            {
                CompetitionsBundle = participantsServer.CompetitionsBundle,
                Participants = participants,
                RealTime = true
            };
        }
    }
}
