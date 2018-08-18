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

            try
            {
                Directory.CreateDirectory(this.OutDirectory);
            }
            catch(IOException ex)
            {
                Console.Error.WriteLine("Could not create output directory: " + ex.Message);
                return 1;
            }

            try
            {
                model_group.Export(this.OutDirectory);
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine("Could export model: " + ex.Message);
                return 1;
            }


            return 0;
        }
    }

    class PokemonBatchExportOptions : VerbBase
    {
        [ValueOption(0)]
        public string InDirectory { get; set; }

        [ValueOption(0)]
        public string OutDirectory { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = HelpText.AutoBuild(this);
            help.AddPreOptionsLine("\nUsage: pokemon-batch-export <in-folder> <out-folder>");
            help.AddPreOptionsLine("The model for each Pokemon is split across several files, consisting of one mesh file, several texture files, and several animation files.");
            help.AddPreOptionsLine("This command combines and exports that data for each Pokemon.");
            return help;
        }

        public override int Execute()
        {
            if (this.InDirectory == null || this.OutDirectory == null)
            {
                Console.WriteLine(this.GetUsage());
                return Parser.DefaultExitCodeFail;
            }

            try
            {
                Directory.CreateDirectory(this.OutDirectory);
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine("Could not create output directory: " + ex.Message);
                return 1;
            }

            string[] files;
            try
            {
                files = Directory.GetFiles(this.InDirectory, "*.bin").Select(filename => Path.GetFileName(filename)).OrderBy(v=>v).ToArray();
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine("Could not access input directory: " + ex.Message);
                return 1;
            }

            Console.WriteLine("Reading from " + this.InDirectory + ", exporting to " + this.OutDirectory);

            string current_name = null;
            string current_folder_name = null;
            RenderBase.OModelGroup current_group = null;
            foreach(var filename in files)
            {
                Console.WriteLine("Reading file " + filename);
                FileIO.file import_file;
                try
                {
                    import_file = FileIO.load(Path.Combine(this.InDirectory, filename));
                }
                catch (IOException ex)
                {
                    Console.Error.WriteLine("Could not open file: " + ex.Message);
                    return 1;
                }

                RenderBase.OModelGroup this_group = import_file.data as RenderBase.OModelGroup;
                if (this_group == null)
                {
                    Console.Error.WriteLine("Unrecognized file type for file " + Path.Combine(this.InDirectory, filename) + "; skipping");
                    continue;
                }

                if (this_group.model.Count != 0)
                {
                    // Starting new model, export the old one
                    if (current_name != null)
                    {
                        try
                        {
                            current_group.ExportModels(current_folder_name);
                            current_group.ExportSkeletalAnimations(0, current_folder_name);
                        }
                        catch (IOException ex)
                        {
                            Console.Error.WriteLine("Could not export " + current_name + ": " + ex.Message);
                            return 1;
                        }
                    }

                    // Move to next one
                    current_name = filename;
                    current_folder_name = Path.Combine(this.OutDirectory, filename + "_exported");
                    current_group = this_group;
                    Console.WriteLine("Model found: " + current_name);
                    try
                    {
                        Directory.CreateDirectory(current_folder_name);
                    }
                    catch (IOException ex)
                    {
                        Console.Error.WriteLine("Could not create " + current_folder_name + ": " + ex.Message);
                        return 1;
                    }
                }
                else if (this_group.skeletalAnimation.Count != 0)
                {
                    // Append animations to model
                    if (current_name == null)
                    {
                        Console.Error.WriteLine(filename + " contains animations not corresponding to any model, skipping.");
                        continue;
                    }

                    current_group.skeletalAnimation.AddRange(this_group.skeletalAnimation);
                }

                if(this_group.texture.Count != 0)
                {
                    // Export textures
                    if (current_name == null)
                    {
                        Console.Error.WriteLine(filename + " contains texture not corresponding to any model, skipping.");
                        continue;
                    }

                    string texture_folder_name = Path.Combine(current_folder_name, filename);
                    try
                    {
                        Directory.CreateDirectory(texture_folder_name);
                    }
                    catch (IOException ex)
                    {
                        Console.Error.WriteLine("Could not create " + current_folder_name + ": " + ex.Message);
                        return 1;
                    }

                    this_group.ExportTextures(texture_folder_name);
                }
            }

            // Export last model
            if(current_name != null)
            {
                try
                {
                    current_group.ExportModels(current_folder_name);
                    current_group.ExportSkeletalAnimations(0, current_folder_name);
                }
                catch (IOException ex)
                {
                    Console.Error.WriteLine("Could not export " + current_name + ": " + ex.Message);
                    return 1;
                }
            }
            else
            {
                Console.Error.WriteLine("No files found in " + this.InDirectory);
                return 1;
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

        [VerbOption("pokemon-batch-export", HelpText = "Combines and exports pokemon models")]
        public PokemonBatchExportOptions PokemonBatchExportOptions { get; set; }

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
                case "pokemon-batch-export":
                    return new PokemonBatchExportOptions().GetUsage();
            }
            throw new Exception("Unrecognized verb: " + verb);
        }
    }

    static class Program
    {
        /// <summary>
        /// Exports models, animations, and textures in a model group to a directory
        /// </summary>
        /// <param name="model_group"></param>
        /// <param name="out_directory"></param>
        public static void Export(this RenderBase.OModelGroup model_group, string out_directory)
        {
            model_group.ExportModels(out_directory);

            // TODO: support selecting the model to apply animations to
            if (model_group.model.Count != 0)
            {
                model_group.ExportSkeletalAnimations(0, out_directory);
            }
            else if (model_group.skeletalAnimation.Count != 0)
            {
                Console.Error.WriteLine("Warning: File contains animations but no models. Animations will not be exported.");
            }
            
            model_group.ExportTextures(out_directory);
        }

        public static void ExportModels(this RenderBase.OModelGroup model_group, string out_directory)
        {
            var duplicate_model_name_counters = model_group.model.GroupBy((v) => v.name).Where(g => g.Count() > 1).Select(g => g.Key).ToDictionary(name => name, name => 0);
            for (int i = 0; i < model_group.model.Count; i++)
            {
                var model = model_group.model[i];
                var name = model.name;
                if (duplicate_model_name_counters.ContainsKey(name))
                {
                    duplicate_model_name_counters[name] += 1;
                    name = String.Format("{0} ({1})", name, duplicate_model_name_counters[name]);
                }
                string path = Path.Combine(out_directory, name + ".model.smd");
                Console.WriteLine("Exporting " + path);
                var warnings = SMD.export(model_group, path, i);
                foreach (var warning in warnings)
                {
                    Console.Error.WriteLine("Warning when exporting " + name + ": " + warning);
                }
            }
        }

        public static void ExportSkeletalAnimations(this RenderBase.OModelGroup model_group, int model_index, string out_directory)
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
                string path = Path.Combine(out_directory, name + ".anim.smd");
                Console.WriteLine("Exporting " + path);
                var warnings = SMD.export(model_group, path, model_index, i);
                foreach (var warning in warnings)
                {
                    Console.Error.WriteLine("Warning when exporting " + name + ": " + warning);
                }
            }
        }

        public static void ExportTextures(this RenderBase.OModelGroup model_group, string out_directory)
        {
            var duplicate_texture_name_counters = model_group.texture.GroupBy((v) => v.name).Where(g => g.Count() > 1).Select(g => g.Key).ToDictionary(name => name, name => 0);
            foreach (var texture in model_group.texture)
            {
                var name = texture.name;
                if (duplicate_texture_name_counters.ContainsKey(name))
                {
                    duplicate_texture_name_counters[name] += 1;
                    name = String.Format("{0} ({1})", name, duplicate_texture_name_counters[name]);
                }
                string path = Path.Combine(out_directory, name + ".png");
                Console.WriteLine("Exporting " + path);
                texture.texture.Save(path);
            }
        }

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