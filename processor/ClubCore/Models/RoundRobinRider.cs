using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace ClubCore.Models
{
    [DebuggerDisplay("#{Name} {RoundRobinClub} ({Gender})")]
    public class RoundRobinRider
    {
        [Key]
        public int Id { get; set; }  // Auto-incremented by EF/SQLite

        public required string Name { get; set; }             // e.g. "John Doe"
        public required string RoundRobinClub { get; set; }   // e.g. "Ratae"
        public required bool IsFemale { get; set; }           // e.g. false

        [NotMapped]
        public string DecoratedName => $"{Name} ({RoundRobinClub})"; // e.g. "John Doe (Ratae)"

        [NotMapped]
        public string Gender => IsFemale ? "Female" : "Male";
    }
}
