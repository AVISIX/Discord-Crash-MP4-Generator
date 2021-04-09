using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord_Crash_MP4_Generator
{
    class Subtitles
    {
        public string GUID { get; } = Guid.NewGuid().ToString();
        public string Filepath { get => Path.GetTempPath() + GUID + ".srt"; }

        public Subtitles()
        {
            if (File.Exists(Filepath))
                File.Delete(Filepath);

            File.Create(Filepath).Close();
        }

        ~Subtitles()
        {
            if (File.Exists(Filepath))
                File.Delete(Filepath);
        }

        private int counter = 1;

        public async Task<Task> AddSubtitle(TimeSpan Start, TimeSpan End, string text)
        {
            string temp = "";

            temp += counter + "\n";
            temp += Start.Hours + ":" + Start.Minutes + ":" + Start.Seconds + "," + Start.Milliseconds;
            temp += " --> ";
            temp += End.Hours + ":" + End.Minutes + ":" + End.Seconds + "," + End.Milliseconds + "\n";
            temp += text + "\n";

            await File.AppendAllTextAsync(Filepath, temp);

            counter++;

            return Task.CompletedTask;
        }
    }

    static class SubtitleGenerator
    {
        public static Subtitles New()
        {
            return new Subtitles();
        }
    }
}
