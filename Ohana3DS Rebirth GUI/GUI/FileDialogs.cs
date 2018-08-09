using Ohana3DS_Rebirth.GUI.Forms;
using Ohana3DS_Rebirth.Ohana;
using Ohana3DS_Rebirth.Ohana.Models.GenericFormats;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ohana3DS_Rebirth.GUI
{
	class FileExport
	{
		/// <summary>
		///     Exports a file of a given type.
		///     Formats available to export will depend on the type of the data.
		/// </summary>
		/// <param name="type">Type of the data to be exported</param>
		/// <param name="data">The data</param>
		/// <param name="arguments">Optional arguments to be used by the exporter</param>
		public static void export(FileIO.fileType type, object data, params int[] arguments)
		{
			using (SaveFileDialog saveDlg = new SaveFileDialog())
			{
				switch (type)
				{
					case FileIO.fileType.model:
						OModelExportForm exportMdl = new OModelExportForm((RenderBase.OModelGroup)data, arguments[0]);
						exportMdl.Show();
						break;
					case FileIO.fileType.texture:
						OTextureExportForm exportTex = new OTextureExportForm((RenderBase.OModelGroup)data, arguments[0]);
						exportTex.Show();
						break;
					case FileIO.fileType.skeletalAnimation:
						saveDlg.Title = "Export Skeletal Animation";
						saveDlg.Filter = "Source Model|*.smd";
						if (saveDlg.ShowDialog() == DialogResult.OK)
						{
							switch (saveDlg.FilterIndex)
							{
								case 1:
									var warnings = SMD.export((RenderBase.OModelGroup)data, saveDlg.FileName, arguments[0], arguments[1]);
									foreach(var warning in warnings) {
										MessageBox.Show(warning, "SMD Exporter", MessageBoxButtons.OK, MessageBoxIcon.Error);
									}
									break;
							}
						}
						break;
				}
			}
		}


		/// <summary>
		///     Imports a file of the given type.
		///     Returns data relative to the chosen type.
		/// </summary>
		/// <param name="type">The type of the data</param>
		/// <returns></returns>
		public static object import(FileIO.fileType type)
		{
			using (OpenFileDialog openDlg = new OpenFileDialog())
			{
				openDlg.Multiselect = true;

				switch (type)
				{
					case FileIO.fileType.model:
						openDlg.Title = "Import models";
						openDlg.Filter = "All files|*.*";

						if (openDlg.ShowDialog() == DialogResult.OK)
						{
							List<RenderBase.OModel> output = new List<RenderBase.OModel>();
							foreach (string fileName in openDlg.FileNames)
							{
								output.AddRange(((RenderBase.OModelGroup)FileIO.load(fileName).data).model);
							}
							return output;
						}
						break;
					case FileIO.fileType.texture:
						openDlg.Title = "Import textures";
						openDlg.Filter = "All files|*.*";

						if (openDlg.ShowDialog() == DialogResult.OK)
						{
							List<RenderBase.OTexture> output = new List<RenderBase.OTexture>();
							foreach (string fileName in openDlg.FileNames)
							{
								FileIO.file file = FileIO.load(fileName);
								switch (file.type)
								{
									case FileIO.formatType.model: output.AddRange(((RenderBase.OModelGroup)file.data).texture); break;
									case FileIO.formatType.texture: output.AddRange((List<RenderBase.OTexture>)file.data); break;
									case FileIO.formatType.image: output.Add((RenderBase.OTexture)file.data); break;
								}
							}
							return output;
						}
						break;
					case FileIO.fileType.skeletalAnimation:
						openDlg.Title = "Import skeletal animations";
						openDlg.Filter = "All files|*.*";

						if (openDlg.ShowDialog() == DialogResult.OK)
						{
							RenderBase.OAnimationListBase output = new RenderBase.OAnimationListBase();
							foreach (string fileName in openDlg.FileNames)
							{
								output.list.AddRange(((RenderBase.OModelGroup)FileIO.load(fileName).data).skeletalAnimation.list);
							}
							return output;
						}
						break;
					case FileIO.fileType.materialAnimation:
						openDlg.Title = "Import material animations";
						openDlg.Filter = "All files|*.*";

						if (openDlg.ShowDialog() == DialogResult.OK)
						{
							RenderBase.OAnimationListBase output = new RenderBase.OAnimationListBase();
							foreach (string fileName in openDlg.FileNames)
							{
								output.list.AddRange(((RenderBase.OModelGroup)FileIO.load(fileName).data).materialAnimation.list);
							}
							return output;
						}
						break;
					case FileIO.fileType.visibilityAnimation:
						openDlg.Title = "Import visibility animations";
						openDlg.Filter = "All files|*.*";

						if (openDlg.ShowDialog() == DialogResult.OK)
						{
							RenderBase.OAnimationListBase output = new RenderBase.OAnimationListBase();
							foreach (string fileName in openDlg.FileNames)
							{
								output.list.AddRange(((RenderBase.OModelGroup)FileIO.load(fileName).data).visibilityAnimation.list);
							}
							return output;
						}
						break;
				}
			}

			return null;
		}
	}
}
