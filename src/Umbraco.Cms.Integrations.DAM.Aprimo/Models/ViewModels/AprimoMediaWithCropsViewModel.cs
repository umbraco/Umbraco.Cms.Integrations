
namespace Umbraco.Cms.Integrations.DAM.Aprimo.Models.ViewModels
{
    public class AprimoMediaWithCropsViewModel
    {
        public AprimoMediaItemViewModel Original { get; set; }

        public IEnumerable<AprimoMediaItemViewModel> Crops { get; set; }

        /// <summary>
        /// Get image URL by crop name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Crop URL</returns>
        public string GetImageUrl(string name)
        {
            var crop = Crops.FirstOrDefault(p => p.Name == name);
            if (crop != null)
                return crop.Url;

            return string.Empty;
        }
    }

    public class AprimoMediaItemViewModel
    {
        public int ResizeWidth { get; set; }

        public int ResizeHeight { get; set; }

        public string Name { get; set; }

        public string PresetName { get; set; }

        public string Label { get; set; }

        public string FileName { get; set; }

        public string Extension { get; set; }

        public string Url { get; set; }

        public AprimoMediaItemViewModel(
            int resizeWidth, int resizeHeight,
            string name, string presetName,
            string label, string fileName,
            string extension, string url)
        {
            ResizeWidth = resizeWidth;
            ResizeHeight = resizeHeight;
            Name = name;
            PresetName = presetName;
            Label = label;
            FileName = fileName;
            Extension = extension;
            Url = url;
        }
    }
}
