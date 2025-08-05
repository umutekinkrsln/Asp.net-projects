
using MapApp.Data;
using MapApp.Dtos;
using MapApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Microsoft.AspNetCore.Http; 

namespace MapApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapPointsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly WKTReader _wktReader;
        private readonly WKTWriter _wktWriter;

        public MapPointsController(AppDbContext context)
        {
            _context = context;
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            _wktReader = new WKTReader(geometryFactory);
            _wktWriter = new WKTWriter();
        }

        // GET: api/MapPoints
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MapPointsDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MapPointsDto>>> GetAll()
        {
            var data = await _context.MapPoints.ToListAsync();
            var dtoList = data.Select(mp => new MapPointsDto
            {
                Id = mp.Id,
                Title = mp.Title,
                Description = mp.Description,
                GeometryType = mp.GeometryType,
                GeometryWKT = mp.Geometry != null ? _wktWriter.Write(mp.Geometry) : ""
            }).ToList();

            return dtoList;
        }

        // GET: api/MapPoints/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MapPointsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MapPointsDto>> GetById(int id)
        {
            var mp = await _context.MapPoints.FindAsync(id);
            if (mp == null) return NotFound();

            var dto = new MapPointsDto
            {
                Id = mp.Id,
                Title = mp.Title,
                Description = mp.Description,
                GeometryType = mp.GeometryType,
                GeometryWKT = mp.Geometry != null ? _wktWriter.Write(mp.Geometry) : ""
            };

            return dto;
        }

        // POST: api/MapPoints
        [HttpPost]
        [ProducesResponseType(typeof(MapPointsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MapPointsDto>> PostPoint(MapPointsDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.GeometryWKT))
                return BadRequest(new { error = "GeometryWKT is required." });

            var point = new MapPoints
            {
                Title = dto.Title,
                Description = dto.Description,
                GeometryType = dto.GeometryType,
                Geometry = _wktReader.Read(dto.GeometryWKT)
            };

            _context.MapPoints.Add(point);
            await _context.SaveChangesAsync();

            dto.Id = point.Id;
            return CreatedAtAction(nameof(GetById), new { id = point.Id }, dto);
        }

        // PUT: api/MapPoints/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutMapPoint(int id, MapPointsDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var existing = await _context.MapPoints.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.GeometryType = dto.GeometryType;
            existing.Geometry = _wktReader.Read(dto.GeometryWKT);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/MapPoints/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePoint(int id)
        {
            var point = await _context.MapPoints.FindAsync(id);
            if (point == null)
                return NotFound();

            _context.MapPoints.Remove(point);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

