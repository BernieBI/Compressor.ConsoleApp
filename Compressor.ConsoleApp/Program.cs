using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using static System.Console;

namespace Compressor.ConsoleApp
{
    public static class Program
    {
        private static string SourcePath { get; set; }
        private static string DestinationPath { get; set; }
        private static string CommandType { get; set; }
        private static string ArchiveName { get; set; }

        public static void Main()
        {
            Write("Hey! With this you can compress or extract your folders!\n\n" +
                          "Command for zipping: zip \n" +
                          "Command for unzipping: unzip\n\n" +
                          "Usage: [command] [Source folder path] [Destination folder path] [New object name (optional)]" +
                          "\n\n");
            TakeParams();
        }

        /// <summary>
        /// Takes the parameters.
        /// </summary>
        public static void TakeParams()
        {

            var commands = GetInput().Split();

            if (commands == null || (commands.Length < 3 || commands.Length > 4))
            {
                WriteLine("Thats not the right amount of parameters");
                TakeParams();
            }
            else
            {
                CommandType = commands[0];
            }

            SourcePath = CreatePath(commands[1]);
            DestinationPath = CreatePath(commands[2]);
            ArchiveName = CreateArchiveName(commands.Length != 4 ? SourcePath : commands[3]);

            WriteFile();
        }

        /// <summary>
        /// Gets the input from user.
        /// </summary>
        /// <returns></returns>
        private static string GetInput()
        {
            var input = ReadLine();
            if (input == null || input.ToLower(CultureInfo.CurrentCulture) != "exit") return input;

            WriteLine("So long! :)");
            Thread.Sleep(1000);
            Environment.Exit(1);

            return input;
        }


        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="version">The version.</param>
        private static void WriteFile(int? version = null)
        {


            var archiveNameWithVersion = ArchiveName + (version != null ? "" + version : "");

            if (version > 5)
            {
                WriteLine("You have a lot of files with the same name there, rename? \nFile name: ");
                archiveNameWithVersion = GetInput();
            };
            var newItemPath = $@"{DestinationPath}\{archiveNameWithVersion}";

            try
            {
                var stopWatch = new Stopwatch();
                var time = new TimeSpan();

                if (CommandType == "zip")
                {
                    if (newItemPath.EndsWith(".zip", StringComparison.InvariantCulture))
                    {
                        WriteLine("This is already a zipped archive");
                        return;
                    }
                    stopWatch.Start();
                    ZipFile.CreateFromDirectory(SourcePath, newItemPath + ".zip");
                    stopWatch.Stop();
                    time = stopWatch.Elapsed;

                    Write("Compression complete! \n\n" +
                          $"{SourcePath} archived and sent to {newItemPath}.zip");
                }
                else if (CommandType == "unzip")
                {
                    newItemPath = newItemPath.Replace(".zip", "", StringComparison.InvariantCulture);
                    stopWatch.Start();
                    ZipFile.ExtractToDirectory(SourcePath, newItemPath);
                    stopWatch.Stop();
                    time = stopWatch.Elapsed;

                    Write("Extraction complete! \n\n" +
                          $"{SourcePath} extracted and sent to {newItemPath}");

                }
                else
                {
                    WriteLine("Thats not the right command");
                }

                if (time.Milliseconds == 0) return;

                var elapsedTime = $"{time.Minutes:00} m, {time.Seconds:00} s, {time.Milliseconds:00} ms";
                WriteLine($"\nThe operation took this long: {elapsedTime}");

            }
            catch (DirectoryNotFoundException e)
            {
                Debug.WriteLine(e);
                WriteLine("That is not a real path..");
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.WriteLine(e);
                WriteLine("I cant access that for some reason.. Strange");
            }
            catch (FileNotFoundException e)
            {
                Debug.WriteLine(e);
                WriteLine("I cant find the source you gave me");
            }
            catch (IOException e)
            {
                Debug.WriteLine(e);
                WriteFile(version == null ? 1 : version + 1);
            }
            finally
            {
                //Write("\nReady for duty, sir!");
                TakeParams();
            }

        }

        /// <summary>
        /// Creates the name of the archive.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <returns></returns>
        private static string CreateArchiveName(string sourcePath)
        {
            return $@"{sourcePath.Split(@"\").Last()}".Trim(new char['\\']);
        }

        /// <summary>
        /// Creates the path.
        /// </summary>
        /// <param name="rawPath">The raw path.</param>
        /// <returns></returns>
        private static string CreatePath(string rawPath)
        {
            var absolutePath = "";
            try
            {
                absolutePath = Path.GetFullPath(rawPath);
            }
            catch (Exception e)
            {
                WriteLine(e);
                return null;

            }
            return absolutePath;

        }
    }
}
