using System;
using System.IO;
using Infrastructure.Interfaces;


namespace Infrastructure.Extensions
{

    public static class PathExtensions
    {
        public static string DefaultVideoDirectory(this IMeeting current)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "OGV2", "Videos");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "OGV2", "Videos"));
            }

            return path;
        }

        public static string DefaultAgendaDirectory()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "OGV2", "Agendas");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "OGV2", "Videos"));
            }

            return path;
        }
    }
}
