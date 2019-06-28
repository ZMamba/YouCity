using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DProject.Models
{
    public class Like
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int PropositionId { get; set; }
        public Proposition Proposition { get; set; }
    }
}
