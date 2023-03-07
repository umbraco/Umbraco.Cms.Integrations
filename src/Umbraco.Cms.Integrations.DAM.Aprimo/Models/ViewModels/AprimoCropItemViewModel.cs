using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Models.ViewModels
{
    public class AprimoCropItemViewModel
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int ResizeWidth { get; set; }

        public int ResizeHeight { get; set; }

        public string PresetName { get; set; }

        public string Label { get; set; }

        public string FileName { get; set; }

        public string PublicLink { get; set; }

        public AprimoCropItemViewModel(
            int x, int y,
            int width, int height,
            int resizeWidth, int resizeHeight,
            string presetName, string label, string fileName,
            string publicLink)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            ResizeWidth = resizeWidth;
            ResizeHeight = resizeHeight;
            PresetName = presetName;
            Label = label;
            FileName = fileName;
            PublicLink = publicLink;
        }
    }
}
