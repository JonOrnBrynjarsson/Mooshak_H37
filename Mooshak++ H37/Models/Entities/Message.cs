namespace Mooshak___H37.Models.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    public class Message
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string from { get; set; }

        [Required]
        [StringLength(50)]
        public string To { get; set; }

        public DateTime DateSent { get; set; }

        [Column("Message")]
        [Required]
        public string Message1 { get; set; }

        public bool IsRemoved { get; set; }
    }
}
