using CommandLine;
using CommandLine.Text;
using Ohana3DS_Rebirth.Ohana;
using System;

namespace Ohana3DS_Rebirth_CLI
{

    class Options
    {
        [Option('i', "inputfile", Required = true, HelpText = "Input file")]
        public string InFile { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }

    class Program
    {

        static int Main(string[] args)
        {
            var options = new Options();
            if(!Parser.Default.ParseArguments(args, options))
            {
                //Console.WriteLine(options.GetUsage());
                Environment.Exit(Parser.DefaultExitCodeFail);
            }

            var file = FileIO.load(options.InFile);
            switch (file.type)
            {
                case FileIO.formatType.model:
                    var model_group = (RenderBase.OModelGroup)file.data;
                    Console.WriteLine("File type: model");

                    if(model_group.model.Count != 0)
                    {
                        Console.WriteLine("Models:");
                        uint model_i = 1;
                        foreach (var model in model_group.model)
                        {
                            Console.WriteLine(String.Format("{0}: {1} ({2} meshes, {3} vertices)", model_i, model.name, model.mesh.Count, model.verticesCount));
                            model_i++;
                        }
                    }

                    if(model_group.texture.Count != 0)
                    {
                        Console.WriteLine("Textures:");
                        uint i = 1;
                        foreach (var texture in model_group.texture)
                        {
                            Console.WriteLine(String.Format("{0}: {1}", i, texture.name));
                            i++;
                        }
                    }

                    if(model_group.skeletalAnimation.Count != 0)
                    {
                        Console.WriteLine("Skeletal Animations:");
                        uint anim_i = 1;
                        foreach (var animation in model_group.skeletalAnimation)
                        {
                            Console.WriteLine(String.Format("{0}: {1}", anim_i, animation.name));
                            anim_i++;
                        }
                    }

                    return 0;
                default:
                    Console.WriteLine("Unrecognized file");
                    return 1;
            }
        }
    }
}