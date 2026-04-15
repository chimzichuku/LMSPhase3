using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignment
    {
        public Assignment()
        {
            Submissions = new HashSet<Submission>();
        }

        public uint AssignmentId { get; set; }
        public string Name { get; set; } = null!;
        public uint CategoryId { get; set; }
        public uint MaxPoints { get; set; }
        public string Content { get; set; } = null!;
        public DateTime Due { get; set; }

        public virtual AssignmentCategory Category { get; set; } = null!;
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
