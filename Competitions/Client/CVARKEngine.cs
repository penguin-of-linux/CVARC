﻿using System;
using System.Diagnostics;

namespace Client
{
    class CVARKEngine
    {
        private const string LocalServerFile = "Network";
        private const string TutorailFile = "Tutorial";
        private readonly ClientSettings settings = new ClientSettings();

        public CVARKEngine(string[] args)
        {
            string relativePath = args.Length == 0 ? "Server\\" : args[0];
            if (settings.Mode == Mode.Tutorial)
            {
                StartProcess(relativePath, TutorailFile);
                Environment.Exit(0);
            }
            if (settings.Mode == Mode.LocalServer)
                StartProcess(relativePath, LocalServerFile);
        }

        public Server GetServer()
        {
            return new Server(settings.Ip, settings.Port);
        }

        private void StartProcess(string path, string fileName)
        {
            Process.Start(new ProcessStartInfo(string.Format("CVARC.{0}.exe", fileName))
                {
                    WorkingDirectory = string.Format(path, fileName)
                });
        }
    }
}
