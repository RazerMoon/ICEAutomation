﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using CommandLine;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;

namespace ImageComposeEditorAutomation
{
    class Program
    {

        static void Main(string[] args)
        {
            var parser = new Parser(config => config.HelpWriter = Console.Out);
            var options = parser.ParseArguments<ComposeOptions, ProcessOptions, StructurePanoramaOptions>(args)
                .WithParsed<ComposeOptions>(options => Compose(options))
                .WithParsed<ProcessOptions>(options => Process(options))
                .WithParsed<StructurePanoramaOptions>(options => Process(options))
                .WithNotParsed(errors => { }); // errors is a sequence of type IEnumerable<Error>
            Console.ReadLine();
        }

        private static void Compose(ComposeOptions options)
        {
            Console.WriteLine("composing...");
            var composeApp = new ComposeAppService();
            var saveProject = options.Save.HasValue ? options.Save.Value : false;
            composeApp.Compose(options.Images.ToArray(), options, m => Console.WriteLine(m), i => drawTextProgressBar(1, 100), saveProject);

        }

        private static void Process(ProcessBaseOptions options)
        {
            string[] dirs = Directory.GetDirectories(options.Folder);

            foreach (string dir in dirs)
            {
                Console.WriteLine("Processing: {0}", dir);

                string filename = dir;

                if (dir.Contains('\\'))
                {
                    string[] splitFilename = dir.Split('\\');

                    filename = splitFilename[splitFilename.Length - 1];
                }

                var composeApp = new ComposeAppService();

                Console.WriteLine("process...");
                if (string.IsNullOrEmpty(options.Extension))
                    options.Extension = "*.JPG";

                if (!string.IsNullOrEmpty(dir))
                    Directory.SetCurrentDirectory(dir);
                var files = GroupFiles(options.Extension, options.Num, ignoreStichInName: true);
                int total = files.Count;
                int count = 0;
                foreach (var item in files)
                {
                    count++;
                    Console.WriteLine(string.Format("composing {0} of {1}....", count, total));
                    var saveProject = options.Save.HasValue ? options.Save.Value : false;
                    composeApp.Compose(item, options, m => Console.WriteLine(m), i => drawTextProgressBar(i, 100), saveProject: saveProject, filename: filename);
                }
                Console.WriteLine("Finished.");

            }

        }

        private static List<string[]> GroupFiles(string extension, int groupNum, bool ignoreStichInName = false)
        {
            string[] filePaths = Directory.GetFiles(Directory.GetCurrentDirectory(), extension, SearchOption.TopDirectoryOnly);
            string[] stichFilePaths = null;

            if (ignoreStichInName)
            {
                stichFilePaths = filePaths.Where(f => IsStitchResult(Path.GetFileName(f))).Select(s => Path.GetFileName(s).ToLower()).ToArray();
                filePaths = filePaths.Where(f => !IsStitchResult(Path.GetFileName(f))).ToArray();
            }

            var grouped = filePaths.Select((value, index) => new { value, index })
                    .GroupBy(x => x.index / groupNum, x => Path.GetFileName(x.value)).Select(g => g.ToArray()).ToList();

            if (ignoreStichInName)
            {
                grouped = grouped.Where(g => !FileAlreadyInStich(g[0], stichFilePaths)).ToList();
            }

            return grouped;
        }

        private static bool FileAlreadyInStich(string fileName, string[] stichFilePaths)
        {

            var stichName = Path.GetFileNameWithoutExtension(fileName) + "_stitch" + Path.GetExtension(fileName);
            return stichFilePaths.Contains(stichName.ToLower());
        }

        private static bool IsStitchResult(string fileName)
        {
            return fileName.Contains("_stitch");
        }

        private static void drawTextProgressBar(int progress, int total)
        {
            ////draw empty progress bar
            //Console.CursorLeft = 0;
            //Console.Write("["); //start
            //Console.CursorLeft = 32;
            //Console.Write("]"); //end
            //Console.CursorLeft = 1;
            //float onechunk = 30.0f / total;

            ////draw filled part
            //int position = 1;
            //for (int i = 0; i < onechunk * progress; i++)
            //{
            //    Console.BackgroundColor = ConsoleColor.Gray;
            //    Console.CursorLeft = position++;
            //    Console.Write(" ");
            //}

            ////draw unfilled part
            //for (int i = position; i <= 31; i++)
            //{
            //    Console.BackgroundColor = ConsoleColor.Green;
            //    Console.CursorLeft = position++;
            //    Console.Write(" ");
            //}

            ////draw totals
            //Console.CursorLeft = 35;
            //Console.BackgroundColor = ConsoleColor.Black;
            try
            {
                Console.CursorLeft = 15;
            }
            catch (System.Exception)
            {

            }
            Console.Write(progress.ToString() + " of " + total.ToString() + "    "); //blanks at the end remove any excess

        }
    }
}
