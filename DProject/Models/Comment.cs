using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DProject.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PublicationDate { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int PropositionId { get; set; }
        public Proposition Proposition { get; set; }
    }
}
