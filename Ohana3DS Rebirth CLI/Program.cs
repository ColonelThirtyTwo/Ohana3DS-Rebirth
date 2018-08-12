using CommandLine;
using CommandLine.Text;
using Ohana3DS_Rebirth.Ohana;
using System;
using System.IO;

namespace Ohana3DS_Rebirth_CLI
{
    class InfoOptions
    {
        [ValueOption(0)]
        public string InFile { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = HelpText.AutoBuild(this);
            help.AddPreOptionsLine("\nUsage: info <in-file>");
            return help;
        }
    }

    class ExportOptions
    {
        [ValueOption(0)]
        public string InFile { get; set; }

        [ValueOption(1)]
        public string OutDirectory { get; set; }
    }

    class Options
    {
        [VerbOption("info", HelpText = "Shows information about a file")]
        public InfoOptions InfoOptions { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current), true);
        }
        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            switch(verb)
            {
                case null:
                case "":
                    return this.GetUsage();
                case "info":
                    return new InfoOptions().GetUsage();
            }
            throw new Exception("Unrecognized verb: " + verb);
        }
    }

    class Program
    {

        static int Main(string[] args)
        {
            var options = new Options();
            string verb = null;
            object sub_options = null;
            if(!Parser.Default.ParseArgumentsStrict(args, options, (the_verb, the_options) => { verb = the_verb; sub_options = the_options; }))
            {
                Environment.Exit(Parser.DefaultExitCodeFail);
            }
            
            if(verb == "info")
            {
                var info_options = (InfoOptions)sub_options;
                if (info_options.InFile == null) // Dear CommandLineParser v1.9, please support required positional arguments
                {
                    Console.WriteLine("Missing input file");
                    return Parser.DefaultExitCodeFail;
                }

                FileIO.file file;
                try
                {
                    file = FileIO.load(info_options.InFile);
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Could not open file: " + ex.Message);
                    return 1;
                }
                switch (file.type)
                {
                    case FileIO.formatType.model:
                        var model_group = (RenderBase.OModelGroup)file.data;
                        Console.WriteLine("File type: model");

                        if (model_group.model.Count != 0)
                        {
                            Console.WriteLine("Models:");
                            uint model_i = 1;
                            foreach (var model in model_group.model)
                            {
                                Console.WriteLine(String.Format("{0}: {1} ({2} meshes, {3} vertices)", model_i, model.name, model.mesh.Count, model.verticesCount));
                                model_i++;
                            }
                        }

                        if (model_group.texture.Count != 0)
                        {
                            Console.WriteLine("Textures:");
                            uint i = 1;
                            foreach (var texture in model_group.texture)
                            {
                                Console.WriteLine(String.Format("{0}: {1}", i, texture.name));
                                i++;
                            }
                        }

                        if (model_group.skeletalAnimation.Count != 0)
                        {
                            Console.WriteLine("Skeletal Animations:");
                            uint anim_i = 1;
                            foreach (var animation in model_group.skeletalAnimation)
                            {
                                Console.WriteLine(String.Format("{0}: {1} ({2} frames)", anim_i, animation.name, animation.frameSize));
                                anim_i++;
                            }
                        }

                        return 0;
                    default:
                        Console.WriteLine("Unrecognized file");
                        return 1;
                }
            }
            else
            {
                throw new Exception("Unrecognized verb: " + verb);
            }
            
        }
    }
}