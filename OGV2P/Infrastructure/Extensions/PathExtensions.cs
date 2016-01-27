using System;
using System.IO;
using Infrastructure.Interfaces;


namespace Infrastructure.Extensions
{

    public static class PathExtensions
    {
        public static string DefaultVideoDirectory(this IMeeting current)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ClerkBase", "Videos");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ClerkBase", "Videos"));
            }

            return path;
        }

        public static string DefaultAgendaDirectory()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ClerkBase", "Agendas");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ClerkBase", "Videos"));
            }

            return path;
        }
    }
}
