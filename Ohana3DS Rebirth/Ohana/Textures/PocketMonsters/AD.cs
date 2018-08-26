using System.IO;

using Ohana3DS_Rebirth.Ohana.Containers;

namespace Ohana3DS_Rebirth.Ohana.Textures.PocketMonsters
{
    class AD
    {
        /// <summary>
        ///     Loads all map textures (and other data) on a AD Pokémon container.
        /// </summary>
        /// <param name="data">The data</param>
        /// <returns>The Model group with textures and stuff</returns>
        public static RenderBase.OModelGroup load(Stream data)
        {
            RenderBase.OModelGroup models = new RenderBase.OModelGroup();

            OContainer container = PkmnContainer.load(data);
            foreach(var entry in container)
            {
                FileIO.file file = FileIO.load(new MemoryStream(entry.data));
                if (file.type == FileIO.formatType.model) models.merge((RenderBase.OModelGroup)file.data);
            }

            return models;
        }
    }
}
