using System.IO;
using System.Linq;
using Ohana3DS_Rebirth.Ohana.Containers;
using Ohana3DS_Rebirth.Ohana.Models;
using System.Collections.Generic;

namespace Ohana3DS_Rebirth.Ohana.Animations.PocketMonsters
{
    class BS
    {
        /// <summary>
        ///     Loads a BS animation file from Pokémon.
        /// </summary>
        /// <param name="data">The data</param>
        /// <returns>The Model group with the animations</returns>
        public static RenderBase.OModelGroup load(Stream data)
        {
            List<RenderBase.OModelGroup> models = new List<RenderBase.OModelGroup>();
            OContainer naCont = PkmnContainer.load(data); //Get NA containers from BS
            var naList = naCont.GetList();
            foreach(var entry in naCont.Skip(1))
            {
                OContainer bchCont = PkmnContainer.load(new MemoryStream(entry.data)); //Get BCH from NA containers
                models.Add(BCH.load(new MemoryStream(bchCont.First().data)));
            }

            return models[0]; //TODO: Figure out how to load all anim BCHs
        }
    }
}
