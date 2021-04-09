using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Exceptions;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace Discord_Crash_MP4_Generator
{
    #region To Do
    /*
        - Add shitty hardware accel so this gets more consistent
        - figure out a better way to do this 
        - maybe embed in a gif? 
    */
    #endregion

    class Program
    {
        public static string goodSample = Path.GetTempPath() + Guid.NewGuid().ToString() + ".mp4";
        public static string badSample = Path.GetTempPath() + Guid.NewGuid().ToString();
        public static string sampleCollection = Path.GetTempPath() + Guid.NewGuid().ToString() + ".txt";

        public static int brokenSamples = 5; // 3 seems to be quite alright tbh

        public static string filePathPattern = @"^(?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$";

        public static void write(params object[] objects)
        {
            foreach (var o in objects)
                if (o == null)
                    Console.ResetColor();
                else
                if (o is ConsoleColor)
                    Console.ForegroundColor = (ConsoleColor)o;
                else
                if (o.HasMethod("ToString"))
                    Console.Write(o.ToString());
                else
                    throw new InvalidInputException("The Object " + o.GetType().ToString() + " does not have a ToString function.");
        }

        private static PixelFormat last = PixelFormat.yuv420p;
        public static PixelFormat randomPixelFormat()
        {
            /*
            if(last == PixelFormat.yuv420p)
            {
                last = PixelFormat.yuv444p;
                return PixelFormat.yuv444p;
            }


            last = PixelFormat.yuv420p;
            return PixelFormat.yuv420p;*/
            var values = (PixelFormat[])Enum.GetValues(typeof(PixelFormat));
            return values[new Random().Next(0, values.Length)];
        }

        public static VideoSize randomVideoSize()
        {
            var values = (VideoSize[])Enum.GetValues(typeof(VideoSize));
            return values[new Random().Next(0, values.Length)];
        }

        public static VideoCodec randomCodec()
        {
            var values = (VideoCodec[])Enum.GetValues(typeof(VideoCodec));
            return values[new Random().Next(0, values.Length)];
        }

        public static void writeLine(params object[] oo)
        {
            if (oo == null)
                Console.ForegroundColor = ConsoleColor.Gray;
            else
                write(oo);

            Console.WriteLine();
        }

        public static async Task<string> Get10k()
        {
            string path = Path.GetTempPath() + "10k10k10k10k10k10k10k10k10k10k10k10k10k10k10k10k10k10k.mp4";

            IConversion c = FFmpeg.Conversions.New();
            c.SetOverwriteOutput(true);
            c.AddParameter("-ss 999 -framerate 1 -i C:\\crasherstest\\10k.png -codec copy -pix_fmt yuv420p " + path);

            await c.Start();

            return path;
        }

        [Obsolete("This never did anything anyways")]
        public static async Task<IConversionResult> gifBreaker(string path)
        {
            string temp = Path.GetTempPath() + Guid.NewGuid().ToString() + ".gif";

            IConversion c = await FFmpeg.Conversions.FromSnippet.ToGif(path, temp, 0);
            c.SetOverwriteOutput(true);
            c.SetPixelFormat(randomPixelFormat());
            randomizeScaleAndAspectRatio(c);
            c.AddParameter($"-ignore_loop 0");

            await c.Start();

            IConversion c2 = await FFmpeg.Conversions.FromSnippet.ToMp4(temp, path);
            c2.SetOverwriteOutput(true);
            return await c2.Start();
        }

        [Obsolete("didnt finish this")]
        public static async Task<IConversionResult> tsTest(string path)
        {
            string temp = Path.GetTempPath() + Guid.NewGuid().ToString() + ".ts";

            IConversion c = await FFmpeg.Conversions.FromSnippet.ToTs(path, temp);
            c.SetOverwriteOutput(true);
            await c.Start();

            IConversion c2 = await FFmpeg.Conversions.FromSnippet.ToMp4(temp, path);
            c2.SetOverwriteOutput(true);
            return await c2.Start();
        }

        [Obsolete("does 0")]
        public static async Task<IConversionResult> webmBreaker(string path)
        {
            string temp = Path.GetTempPath() + Guid.NewGuid().ToString() + ".webm";

            IConversion c = await FFmpeg.Conversions.FromSnippet.ToWebM(path, temp);
            c.SetOverwriteOutput(true);
            // c.SetPixelFormat(randomPixelFormat());
            //  randomizeScaleAndAspectRatio(c);
            c.AddParameter("-vf scale=10000:10000");
            await c.Start();

            IConversion c2 = await FFmpeg.Conversions.FromSnippet.ToMp4(temp, path);
            c2.SetOverwriteOutput(true);
            return await c2.Start();
        }

        public static void randomizeScaleAndAspectRatio(IConversion some)
        {
            Random r = new Random();

            int x = 0;
            while (x % 2 != 0)
                x = r.Next(100, 1000);

            int y = 0;
            while (y % 2 != 0)
                y = r.Next(100, 1000);

            string aspectRatio = r.Next(1, 20) + ":" + r.Next(1, 20);

            some.AddParameter("-vf scale=" + x + ":" + y + ",setdar=" + aspectRatio); // this is just an extra, didnt seem to work in discord ... 
        }

        public static async Task<IConversionResult> generateBrokenSample(string master, string output, TimeSpan start, TimeSpan duration)
        {
            string temp = Path.GetFullPath(output) + "temp-" + Path.GetFileName(output);

            if (File.Exists(temp))
                File.Delete(temp);

            IConversion bad = await FFmpeg.Conversions.FromSnippet.Split(master, temp, start, duration);
            bad.SetOverwriteOutput(true);
            bad.SetPixelFormat(randomPixelFormat()); // this does 90% of the crash
            randomizeScaleAndAspectRatio(bad);

            await bad.Start();

            IMediaInfo meta = await FFmpeg.GetMediaInfo(temp);

            IVideoStream v = meta.VideoStreams.ToList().FirstOrDefault();
            v.SetSize(randomVideoSize());

            {
                var sub = SubtitleGenerator.New();

                await sub.AddSubtitle(TimeSpan.FromSeconds(0), meta.Duration, " ̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺ͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩ 𓀐 ̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺ͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩ 𓀐 ̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺ͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩ 𓀐 ̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺ͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩ 𓀐 ̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺ͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩ 𓀐 ̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺̺ͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩͩ 𓀐\n".Repeat(100));

                v.AddSubtitles(sub.Filepath);
            }

            IAudioStream a = meta.AudioStreams.ToList().FirstOrDefault();
            a.Reverse(); // the people who survive will get confused
            a.ChangeSpeed(0.5);

            var result = await FFmpeg.Conversions.New()
                .SetOverwriteOutput(true)
                .AddStream(v)
                .AddStream(a)
                .SetOutput(output)
                .SetPixelFormat(PixelFormat.yuv444p)
                .SetVideoSyncMethod(VideoSyncMethod.passthrough)
                .Start();

            File.Delete(temp);

            return result;
        }

        public static void Cleanup()
        {
            if (File.Exists(goodSample))
                File.Delete(goodSample);

            if (File.Exists(sampleCollection))
                File.Delete(sampleCollection);

            int i = 0;
            while (File.Exists(badSample + i + ".mp4"))
            {
                File.Delete(badSample + i + ".mp4");
                i++;
            }
        }

        public async static Task<IConversionResult> buildBrokenMp4From(string input, string output, double crashTiming)
        {
            List<string> badParts = new List<string>();

            try
            {
                writeLine("Getting total Length...");
                TimeSpan totalLength = (await FFmpeg.GetMediaInfo(input)).VideoStreams.ToList().FirstOrDefault().Duration;

                if (totalLength.TotalSeconds <= 1)
                    throw new Exception("The Video has to be longer than 1 second!");

                if (crashTiming <= 1 || crashTiming > totalLength.TotalSeconds)
                    throw new Exception("The Crash Timing cannot be before the first second and cannot exceed the total length of the Video!");

                writeLine("Checking for Files...");

                #region Generate View-Able portion of the video
                writeLine("Generating View-Able Part...");
                IConversion good = await FFmpeg.Conversions.FromSnippet.Split(input, goodSample, TimeSpan.Zero, TimeSpan.FromSeconds(crashTiming));
                good.SetPixelFormat(PixelFormat.yuv420p);
                good.SetOverwriteOutput(true);
                IConversionResult goodResult = await good.Start();
                #endregion


                #region Generate multiple broken parts of the Video
                writeLine("Generating Broken Video Parts...");

                {
                    TimeSpan time2workwith = (totalLength - TimeSpan.FromSeconds(crashTiming)) / brokenSamples;

                    TimeSpan position = TimeSpan.FromSeconds(crashTiming);

                    for (int i = 0; i < brokenSamples; i++)
                    {
                        string samplePath = badSample + i + ".mp4";

                        writeLine("> Generating Broken Sample " + (i + 1) + " out of " + brokenSamples);

                    anotherAttempt:
                        try
                        {
                            await generateBrokenSample(input, samplePath, position, time2workwith);
                            //  await gifBreaker(samplePath);
                        }
                        catch (Exception e)
                        { // in some cases conversion will fuck up, so just try again cuz i cba to filter out which cant be converted
                            if (e is ConversionException)
                            {
                                writeLine(ConsoleColor.Red, "Invalid Conversion occurred. Trying again...\n" + e, null);
                                Thread.Sleep(1000);
                                goto anotherAttempt;
                            }
                            else
                                throw;
                        }

                        badParts.Add(samplePath);

                        position += time2workwith;
                    }
                }

                badParts.Add(await Get10k());

                #endregion


                #region Files Collection
                writeLine("Creating Files Collection...");

                if (File.Exists(sampleCollection))
                    File.Delete(sampleCollection);

                File.Create(sampleCollection).Close();

                {
                    string content = "file '" + goodSample + "'\n";

                    foreach (string sample in badParts)
                        content += "file '" + sample + "'\n";

                    await File.WriteAllTextAsync(sampleCollection, content);
                }
                #endregion


                #region Concat
                writeLine("Merging Files...");

                List<IMediaInfo> mediaInfos = new List<IMediaInfo>();

                IConversion conversion = FFmpeg.Conversions.New();

                conversion.SetOverwriteOutput(true);
                conversion.AddParameter($"-f concat");
                conversion.AddParameter($"-safe 0");
                conversion.AddParameter($"-i \"{sampleCollection}\"");
                conversion.AddParameter($"-codec copy");

                writeLine("Building output...");

                writeLine("File can be found in: ", ConsoleColor.Yellow, output, null);

                return await conversion.SetOutput(output)
                    .Start();

                #endregion
            }
            catch (Exception e)
            {
                if (e is FFmpegNotFoundException)
                {
                    writeLine(ConsoleColor.Yellow, "FFmpeg not found. You must install FFmpeg before using this Program.");
                    writeLine("Download the executables here: '", ConsoleColor.Red, "https://github.com/BtbN/FFmpeg-Builds/releases", null, "'");
                    writeLine(ConsoleColor.Yellow, "Place the Executables into: ", ConsoleColor.Red, @"C:\FFmpeg\bin", ConsoleColor.Gray);
                }
                else
                    writeLine("An exception was Thrown: ", ConsoleColor.Yellow, e, null);
            }
            finally
            {
                Cleanup();
            }

            return null;
        }

        public static async Task<Task> execute()
        {
            writeLine(ConsoleColor.Green, "#".Repeat(50));
            writeLine("Discord Crash MP4 Generator by Sixmax");
            writeLine("--Version: ", Config.Version);
            writeLine("#".Repeat(50));

            writeLine(null);

            FFmpeg.SetExecutablesPath(@"C:\FFmpeg\bin");

            string filePath = "";
            string outputDir = "";
            double crashTiming = 3;

#if DEBUG
            filePath = @"C:\crasherstest\kitten.mp4";
            outputDir = @"C:\crasherstest\hot_bitch.mp4";
            crashTiming = 2;
#else
            #region Get Filepath 
            {
            readFilePathAgain:

                writeLine("Enter the Filepath of the file, which you want to make Crash Discord.");

                filePath = Console.ReadLine().Replace(" ", "");

                if (!Regex.IsMatch(filePath, filePathPattern))
                {
                    writeLine(ConsoleColor.Yellow, filePath, null, "is not a valid Filepath.");
                    goto readFilePathAgain;
                }

                if(File.Exists(filePath) == false)
                {
                    writeLine("Could not find any File at '", ConsoleColor.Yellow, filePath, null,"'");
                    goto readFilePathAgain;
                }

                if(!filePath.EndsWith(".mp4"))
                {
                    writeLine("The File at the defined location is not a mp4 file.");
                    goto readFilePathAgain;
                }
            }
            #endregion

            writeLine("-".Repeat(100));

            #region Get Output Directory
            {
                writeLine("Enter the Output path, if you dont want to define the Output Path, your Download Folder will be used as default.");

                outputDir = Console.ReadLine().Replace(" ", ""); ;

                if (!Regex.IsMatch(outputDir, filePathPattern))
                {
                    outputDir = $@"C:\Users\{Environment.UserName}\Downloads\{Path.GetFileName(filePath)}";
                    writeLine("The entered Filepath was invalid.\n'", ConsoleColor.Blue, outputDir, null, "' will be used instead.");
                }
            }
            #endregion

            writeLine("-".Repeat(100));

            #region Get Crash Timing 
            {
            readCrashTimingAgain:

                writeLine("Enter the Amount of Time (In Seconds) until the Video should Crash.");

                if (double.TryParse(Console.ReadLine().Replace(" ", ""), out double result))
                    crashTiming = result;
                else
                {
                    writeLine(ConsoleColor.Yellow, crashTiming, null, "is an Invalid Time. It has to be Format 'double'");
                    goto readCrashTimingAgain;
                }
            }
            #endregion

            writeLine("-".Repeat(100));
#endif

            await buildBrokenMp4From(filePath, outputDir, crashTiming);

            return Task.CompletedTask;
        }

        static void Main(string[] args) => execute().GetAwaiter().GetResult();

        ~Program() => Cleanup();
    }
}
