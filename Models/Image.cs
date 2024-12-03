using System.ComponentModel.DataAnnotations;

namespace projectOCR.Models
{
    public class Image
    {
        [Key]
        public int resimId { get; set; }
        public string resimAdi { get; set; }
        public string resimUrl { get; set; }
    }
}