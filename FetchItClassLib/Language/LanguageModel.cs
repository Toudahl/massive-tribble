using System.ComponentModel.DataAnnotations;

namespace FetchItClassLib.Language
{
    public partial class LanguageModel
    {
        public int LanguageID { get; set; }

        [Required]
        [StringLength(50)]
        public string LanguageName { get; set; }

        [Required]
        public string LanguageFileLocation { get; set; }
    }
}
