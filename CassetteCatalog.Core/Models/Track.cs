using CassetteCatalog.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CassetteCatalog.Core.Models
{
    public class Track
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public eCassetteSide Side { get; set; }
        public string Title { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
    }
}
