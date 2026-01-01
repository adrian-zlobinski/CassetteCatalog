using CassetteCatalog.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CassetteCatalog.Core.Models
{
    public class Album
    {
        public int Id { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public ushort ReleaseYear { get; set; }
        public eTapeType TapeType { get; set; }
        public string CassetteName { get; set; } = string.Empty;
        public List<Track> Tracks { get; set; }
    }
}
