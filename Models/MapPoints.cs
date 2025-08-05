using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MapApp.Models
{
    public class MapPoints
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Geometri tipi ,Point, LineString, Polygon
        [Required]
        public string GeometryType { get; set; } = "Point";

        // Ana geometri verisi
        [Column(TypeName = "geometry")]
        public Geometry? Geometry { get; set; }

        // Uzunluk ve Alan hesaplaması için 
        [NotMapped]
        public double? Length => Geometry is LineString line ? line.Length : null;

        [NotMapped]
        public double? Area => Geometry is Polygon polygon ? polygon.Area : null;
    }
}



