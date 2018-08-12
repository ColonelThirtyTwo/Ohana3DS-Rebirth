using CommandLine;
using CommandLine.Text;
using Ohana3DS_Rebirth.Ohana;
using Ohana3DS_Rebirth.Ohana.Models.GenericFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ohana3DS_Rebirth_CLI
{
    /// <summary>
    /// Base class for this program's subcommands.
    /// </summary>
    abstract class VerbBase
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>Result code</returns>
        public abstract int Execute();
    }

    /// <summary>
    /// `info` command, displays file info
    /// </summary>
    class InfoOptions : VerbBase
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

        public override int Execute()
        {
            if (this.InFile == null) // Dear CommandLineParser v1.9, please support required positional arguments
            {
                Console.WriteLine("Missing input file");
                return Parser.DefaultExitCodeFail;
            }

            FileIO.file file;
            try
            {
                file = FileIO.load(this.InFile);
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine("Could not open file: " + ex.Message);
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
                    Console.Error.WriteLine("Unrecognized file type");
                    return 1;
            }
        }
    }

    class ExportOptions: VerbBase
    {
        [ValueOption(0)]
        public string InFile { get; set; }

        [ValueOption(0)]
        public string OutDirectory { get; set; }

        [OptionArray('a', "import-anims", Required = false, HelpText = "Files to import skeletal animations from before exporting")]
        public string[] AnimImports { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = HelpText.AutoBuild(this);
            help.AddPreOptionsLine("\nUsage: export <in-file> <out-directory>");
            return help;
        }

        public override int Execute()
        {
            if (this.InFile == null || this.OutDirectory == null)
            {
                Console.WriteLine(this.GetUsage());
                return Parser.DefaultExitCodeFail;
            }

            FileIO.file file;
            try
            {
                file = FileIO.load(this.InFile);
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine("Could not open file: " + ex.Message);
                return 1;
            }

            var model_group = file.data as RenderBase.OModelGroup;
            if(model_group == null)
            {
                Console.Error.WriteLine("Unrecognized file type");
                return 1;
            }

            // Import animations
            if (this.AnimImports != null)
            {
                foreach (var import_path in this.AnimImports)
                {
                    FileIO.file import_file;
                    try
                    {
                        import_file = FileIO.load(import_path);
                    }
                    catch (IOException ex)
                    {
                        Console.Error.WriteLine("Could not open file: " + ex.Message);
                        return 1;
                    }

                    var import_model_group = import_file.data as RenderBase.OModelGroup;
                    if (import_model_group == null)
                    {
                        Console.Error.WriteLine("Unrecognized file type for animation import " + import_path);
                        return 1;
                    }

                    model_group.skeletalAnimation.AddRange(import_model_group.skeletalAnimation);
                }
            }

            // Export models
            var duplicate_model_name_counters = model_group.model.GroupBy((v) => v.name).Where(g => g.Count() > 1).Select(g => g.Key).ToDictionary(name => name, name => 0);
            for(int i = 0; i < model_group.model.Count; i++)
            {
                var model = model_group.model[i];
                var name = model.name;
                if(duplicate_model_name_counters.ContainsKey(name))
                {
                    duplicate_model_name_counters[name] += 1;
                    name = String.Format("{0} ({1})", name, duplicate_model_name_counters[name]);
                }
                string path = Path.Combine(this.OutDirectory, name + ".model.smd");
                Console.WriteLine("Exporting " + path);
                var warnings = SMD.export(model_group, path, i);
                foreach(var warning in warnings)
                {
                    Console.Error.WriteLine("Warning when exporting " + name + ": " + warning);
                }
            }

            // Export animations
            // TODO: support selecting the model to apply animations to
            if (model_group.model.Count != 0)
            {
                var duplicate_anim_name_counters = model_group.skeletalAnimation.GroupBy((v) => v.name).Where(g => g.Count() > 1).Select(g => g.Key).ToDictionary(name => name, name => 0);
                for (int i = 0; i < model_group.skeletalAnimation.Count; i++)
                {
                    var anim = model_group.skeletalAnimation[i];
                    var name = anim.name;
                    if (duplicate_anim_name_counters.ContainsKey(name))
                    {
                        duplicate_anim_name_counters[name] += 1;
                        name = String.Format("{0} ({1})", name, duplicate_anim_name_counters[name]);
                    }
                    string path = Path.Combine(this.OutDirectory, name + ".anim.smd");
                    Console.WriteLine("Exporting " + path);
                    var warnings = SMD.export(model_group, path, 0, i);
                    foreach (var warning in warnings)
                    {
                        Console.Error.WriteLine("Warning when exporting " + name + ": " + warning);
                    }
                }
            }
            else if (model_group.skeletalAnimation.Count != 0)
            {
                Console.Error.WriteLine("Warning: File contains animations but no models. Animations will not be exported.");
            }

            // Export textures
            var duplicate_texture_name_counters = model_group.texture.GroupBy((v) => v.name).Where(g => g.Count() > 1).Select(g => g.Key).ToDictionary(name => name, name => 0);
            foreach(var texture in model_group.texture)
            {
                var name = texture.name;
                if (duplicate_texture_name_counters.ContainsKey(name))
                {
                    duplicate_texture_name_counters[name] += 1;
                    name = String.Format("{0} ({1})", name, duplicate_texture_name_counters[name]);
                }
                string path = Path.Combine(this.OutDirectory, name + ".png");
                Console.WriteLine("Exporting " + path);
                texture.texture.Save(path);
            }

            return 0;
        }
    }

    class Options
    {
        [VerbOption("info", HelpText = "Shows information about a file")]
        public InfoOptions InfoOptions { get; set; }

        [VerbOption("export", HelpText = "Exports data from a file")]
        public ExportOptions ExportOptions { get; set; }

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
                case "help":
                    return this.GetUsage();
                case "info":
                    return new InfoOptions().GetUsage();
                case "export":
                    return new ExportOptions().GetUsage();
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
                return Parser.DefaultExitCodeFail;
            }
            return ((VerbBase)sub_options).Execute();
        }
    }
}