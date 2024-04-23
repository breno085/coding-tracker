using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coding_tracker.Models
{
    public class CodingTracker
    {
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan Duration { get; set; }
    }
}