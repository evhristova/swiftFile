using System.ComponentModel.DataAnnotations;

namespace AspNetCoreDemo.Models
{
    public class Swift
    {
        public int Id { get; set; }

        [Required]
        public string BasicHeaderBlock { get; set; }

        [Required]
        public string ApplicationHeaderBlock { get; set; }

        public string? UserHeaderBlock { get; set; }

        [Required]
        public string TransactionReferenceNumber { get; set; }

        public string? RelatedReference { get; set; }

        [Required]
        public string Narrative { get; set; }

        [Required]
        public string TrailerBlockMac { get; set; }
        
        [Required]
        public string TrailerBlockChk { get; set; }
    }
}
