import { MapContainer, TileLayer, Marker, Polyline, Polygon, Popup, useMapEvents } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';
import axios from 'axios';
import { useEffect, useState } from 'react';
import { Box, Drawer, List, ListItem, ListItemText, Button, TextField, Typography, Select, MenuItem } from '@mui/material';

// Marker ikonunu düzelt
delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png',
  iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
  shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png'
});

// WKT -> Koordinat arrayi
const parseWKT = (wkt) => {
  if (wkt.startsWith("POINT")) {
    const match = wkt.match(/POINT\s*\(([-\d.]+)\s+([-\d.]+)\)/);
    return match ? [[parseFloat(match[2]), parseFloat(match[1])]] : [];
  }
  if (wkt.startsWith("LINESTRING")) {
    return wkt
      .match(/\(([^)]+)\)/)[1]
      .split(",")
      .map(p => {
        const [lng, lat] = p.trim().split(" ");
        return [parseFloat(lat), parseFloat(lng)];
      });
  }
  if (wkt.startsWith("POLYGON")) {
    return [
      wkt
        .match(/\(\(([^)]+)\)\)/)[1]
        .split(",")
        .map(p => {
          const [lng, lat] = p.trim().split(" ");
          return [parseFloat(lat), parseFloat(lng)];
        })
    ];
  }
  return [];
};

// Koordinat arrayi -> WKT
const toWKT = (geometryType, coords) => {
  if (geometryType === "Point") {
    const [lat, lng] = coords[0];
    return `POINT(${lng} ${lat})`;
  }
  if (geometryType === "LineString") {
    return `LINESTRING(${coords.map(([lat, lng]) => `${lng} ${lat}`).join(", ")})`;
  }
  if (geometryType === "Polygon") {
    return `POLYGON((${coords[0].map(([lat, lng]) => `${lng} ${lat}`).join(", ")}))`;
  }
  return "";
};

// Haritada tıklama ile ekleme
function AddGeometry({ geometryType, onAdd }) {
  const [tempCoords, setTempCoords] = useState([]);

  useMapEvents({
    click(e) {
      if (geometryType === "Point") {
        const name = prompt("İsim girin:");
        const description = prompt("Açıklama girin:");
        if (!name) return;

        const newFeature = {
          title: name,
          description,
          geometryType: "Point",
          geometryWKT: toWKT("Point", [[e.latlng.lat, e.latlng.lng]])
        };
        axios.post("https://localhost:5001/api/MapPoints", newFeature).then(res => onAdd(res.data));
      } else {
        setTempCoords(prev => [...prev, [e.latlng.lat, e.latlng.lng]]);
      }
    }
  });

  const finishShape = () => {
    if (tempCoords.length < 2 && geometryType !== "Polygon") return;
    if (geometryType === "Polygon" && tempCoords.length < 3) return;

    const name = prompt("İsim girin:");
    const description = prompt("Açıklama girin:");
    if (!name) return;

    const finalCoords = geometryType === "Polygon" ? [tempCoords] : tempCoords;

    const newFeature = {
      title: name,
      description,
      geometryType,
      geometryWKT: toWKT(geometryType, finalCoords)
    };

    axios.post("https://localhost:5001/api/MapPoints", newFeature).then(res => onAdd(res.data));
    setTempCoords([]);
  };

  return (
    <>
      {geometryType === "LineString" && tempCoords.length > 1 && <Polyline positions={tempCoords} color="blue" />}
      {geometryType === "Polygon" && tempCoords.length > 2 && <Polygon positions={[tempCoords]} color="green" />}
      {geometryType !== "Point" && tempCoords.length > 0 && (
        <Button
          variant="contained"
          color="secondary"
          onClick={finishShape}
          style={{ position: 'absolute', top: 10, left: 10, zIndex: 1000 }}
        >
          Bitir
        </Button>
      )}
    </>
  );
}

export default function App() {
  const [features, setFeatures] = useState([]);
  const [geometryType, setGeometryType] = useState("Point");

  useEffect(() => {
    axios.get("https://localhost:5001/api/MapPoints")
      .then(res => {
        const parsed = res.data.map(f => ({
          ...f,
          coords: parseWKT(f.geometryWKT)
        }));
        setFeatures(parsed);
      });
  }, []);

  const handleDelete = (id) => {
    axios.delete(`https://localhost:5001/api/MapPoints/${id}`).then(() =>
      setFeatures(prev => prev.filter(f => f.id !== id))
    );
  };

  const handleAdd = (feature) => {
    setFeatures(prev => [...prev, { ...feature, coords: parseWKT(feature.geometryWKT) }]);
  };

  return (
    <Box sx={{ display: 'flex' }}>
      {/* HARİTA */}
      <Box sx={{ flex: 1 }}>
        <MapContainer center={[41.0082, 28.9784]} zoom={13} style={{ height: "100vh", width: "100%" }}>
          <TileLayer
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
          />
          <AddGeometry geometryType={geometryType} onAdd={handleAdd} />

          {features.map(f =>
            f.geometryType === "Point" ? (
              <Marker key={f.id} position={f.coords[0]}>
                <Popup>
                  <strong>{f.title}</strong><br />
                  {f.description}<br />
                  <Button onClick={() => handleDelete(f.id)} color="error" variant="outlined">Sil</Button>
                </Popup>
              </Marker>
            ) : f.geometryType === "LineString" ? (
              <Polyline key={f.id} positions={f.coords} color="blue">
                <Popup>
                  <strong>{f.title}</strong><br />
                  {f.description}<br />
                  <Button onClick={() => handleDelete(f.id)} color="error" variant="outlined">Sil</Button>
                </Popup>
              </Polyline>
            ) : (
              <Polygon key={f.id} positions={f.coords} color="green">
                <Popup>
                  <strong>{f.title}</strong><br />
                  {f.description}<br />
                  <Button onClick={() => handleDelete(f.id)} color="error" variant="outlined">Sil</Button>
                </Popup>
              </Polygon>
            )
          )}
        </MapContainer>
      </Box>

      {/* MUI PANEL */}
      <Drawer variant="permanent" anchor="right">
        <Box sx={{ width: 300, p: 2 }}>
          <Typography variant="h6">Geometriler</Typography>
          <Select
            fullWidth
            value={geometryType}
            onChange={(e) => setGeometryType(e.target.value)}
            sx={{ my: 2 }}
          >
            <MenuItem value="Point">Point</MenuItem>
            <MenuItem value="LineString">LineString</MenuItem>
            <MenuItem value="Polygon">Polygon</MenuItem>
          </Select>

          <List>
            {features.map(f => (
              <ListItem key={f.id} secondaryAction={
                <Button onClick={() => handleDelete(f.id)} color="error">Sil</Button>
              }>
                <ListItemText
                  primary={f.title}
                  secondary={`${f.geometryType} - ${f.description}`}
                />
              </ListItem>
            ))}
          </List>
        </Box>
      </Drawer>
    </Box>
  );
}
