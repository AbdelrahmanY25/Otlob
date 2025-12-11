const MAPBOX_TOKEN = "pk.eyJ1IjoiYWJkZWxyYWhtYW4yNSIsImEiOiJjbWZpN2h0c2kwazVkMmpzYXZjZzYxcjQ1In0.JrBlnVg0YqMqrGqpgj2CnQ";

mapboxgl.accessToken = MAPBOX_TOKEN;

let map = new mapboxgl.Map({
    container: 'map',
    style: 'mapbox://styles/mapbox/streets-v12',
    center: [31.2357, 30.0444],
    zoom: 12,
    pitch: 45,
    bearing: -17.6
});

map.addControl(new mapboxgl.NavigationControl());

let timeout = null;
let marker = null;

function add3DBuildings() {
    if (!map.getSource('composite')) return;

    if (!map.getLayer('3d-buildings')) {
        map.addLayer({
            'id': '3d-buildings',
            'source': 'composite',
            'source-layer': 'building',
            'filter': ['==', 'extrude', 'true'],
            'type': 'fill-extrusion',
            'minzoom': 15,
            'paint': {
                'fill-extrusion-color': '#aaa',
                'fill-extrusion-height': ['get', 'height'],
                'fill-extrusion-base': ['get', 'min_height'],
                'fill-extrusion-opacity': 0.6
            }
        });
    }
}

map.on('load', () => {
    document.getElementById('mapLoading').style.display = 'none';

    const testMarker = new mapboxgl.Marker({ color: "green" })
        .setLngLat([31.2357, 30.0444])
        .addTo(map);

    setTimeout(() => {
        testMarker.remove();
    }, 5000);

    add3DBuildings();
});

map.on('style.load', add3DBuildings);

function searchAddress() {
    clearTimeout(timeout);
    timeout = setTimeout(() => {
        let query = document.getElementById("CustomerAddress").value;
        let resultsDiv = document.getElementById("searchResults");

        if (query.length < 3) {
            resultsDiv.classList.remove("show");
            return;
        }

        let url = `https://api.mapbox.com/geocoding/v5/mapbox.places/${encodeURIComponent(query)}.json?access_token=${MAPBOX_TOKEN}&autocomplete=true&country=eg&limit=5`;

        fetch(url)
            .then(res => res.json())
            .then(data => {
                resultsDiv.innerHTML = "";
                if (data.features && data.features.length > 0) {
                    data.features.forEach(item => {
                        let div = document.createElement("div");
                        div.className = "search-result-item";
                        div.innerHTML = `<i class="fas fa-map-marker-alt me-2 text-primary"></i>${item.place_name}`;
                        div.onclick = () => selectAddress(item);
                        resultsDiv.appendChild(div);
                    });
                    resultsDiv.classList.add("show");
                } else {
                    resultsDiv.classList.remove("show");
                }
            })
            .catch(error => {
                resultsDiv.classList.remove("show");
            });
    }, 500);
}

function selectAddress(item) {
    document.getElementById("searchResults").classList.remove("show");
    document.getElementById("CustomerAddress").value = item.place_name;
    document.getElementById("LonCode").value = item.center[0];
    document.getElementById("LatCode").value = item.center[1];
    updateMarker(item.center[0], item.center[1], item.place_name);
    map.flyTo({ center: item.center, zoom: 14 });
}

function getCurrentLocation() {
    if (!navigator.geolocation) {
        alert("Geolocation not supported.");
        return;
    }

    navigator.geolocation.getCurrentPosition(success, error);
}

function success(position) {
    const lat = position.coords.latitude;
    const lon = position.coords.longitude;

    let url = `https://api.mapbox.com/geocoding/v5/mapbox.places/${lon},${lat}.json?access_token=${MAPBOX_TOKEN}&country=eg`;

    fetch(url)
        .then(res => res.json())
        .then(data => {
            let placeName = "No address found";
            if (data.features && data.features.length > 0) {
                placeName = data.features[0].place_name;
            }
            document.getElementById("CustomerAddress").value = placeName;
            document.getElementById("LonCode").value = lon;
            document.getElementById("LatCode").value = lat;
            updateMarker(lon, lat, placeName);
            map.flyTo({ center: [lon, lat], zoom: 14 });
        })
        .catch(error => {
            console.error('Geolocation error:', error);
        });
}

function error() {
    alert("Unable to retrieve location.");
}

map.on('click', (e) => {
    const lon = e.lngLat.lng;
    const lat = e.lngLat.lat;

    let url = `https://api.mapbox.com/geocoding/v5/mapbox.places/${lon},${lat}.json?access_token=${MAPBOX_TOKEN}&country=eg`;

    fetch(url)
        .then(res => res.json())
        .then(data => {
            let placeName = "No address found";
            if (data.features && data.features.length > 0) {
                placeName = data.features[0].place_name;
            }
            document.getElementById("CustomerAddress").value = placeName;
            document.getElementById("LonCode").value = lon;
            document.getElementById("LatCode").value = lat;
            updateMarker(lon, lat, placeName);
        });
});

function updateMarker(lon, lat, placeName) {
    if (marker) {
        marker.setLngLat([lon, lat]);
    } else {
        marker = new mapboxgl.Marker({ color: "red", draggable: true })
            .setLngLat([lon, lat])
            .addTo(map);

        marker.on('dragend', () => {
            const pos = marker.getLngLat();
            reverseGeocode(pos.lng, pos.lat);
        });
    }

    document.getElementById("info").innerHTML = `
                <strong>📍 ${placeName}</strong><br>
                <small class="text-muted">Lat: ${lat.toFixed(6)}, Lon: ${lon.toFixed(6)}</small>
            `;
    document.getElementById("locationInfo").style.display = "block";
}

function reverseGeocode(lon, lat) {
    let url = `https://api.mapbox.com/geocoding/v5/mapbox.places/${lon},${lat}.json?access_token=${MAPBOX_TOKEN}&country=eg`;

    fetch(url)
        .then(res => res.json())
        .then(data => {
            let placeName = "No address found";
            if (data.features && data.features.length > 0) {
                placeName = data.features[0].place_name;
            }
            document.getElementById("CustomerAddress").value = placeName;
            document.getElementById("LonCode").value = lon;
            document.getElementById("LatCode").value = lat;
            updateMarker(lon, lat, placeName);
        });
}

document.getElementById("CustomerAddress").addEventListener("input", searchAddress);

document.addEventListener("click", function (event) {
    if (!event.target.closest('.address-input-group')) {
        document.getElementById("searchResults").classList.remove("show");
    }
});

document.addEventListener("DOMContentLoaded", function () {
    const floorInput = document.querySelector('input[name="FloorNumber"]');

    if (floorInput) {
        floorInput.addEventListener("input", function () {
            if (this.value < 0) {
                this.value = 0;
            }
        });
    }
});

document.addEventListener('DOMContentLoaded', function () {
    const options = document.querySelectorAll('.place-option');
    const hiddenSelect = document.getElementById('PlaceType');

    options.forEach(option => {
        option.addEventListener('click', function () {
            options.forEach(opt => opt.classList.remove('selected'));

            this.classList.add('selected');

            const value = this.getAttribute('data-value');
            hiddenSelect.value = value;

            togglePlaceFields();
        });
    });
});

function togglePlaceFields() {
    let type = document.getElementById("PlaceType").value;
    document.querySelectorAll(".place-fields").forEach(el => el.style.display = "none");

    if (type === "Apartment") {
        document.getElementById("apartmentFields").style.display = "block";
    } else if (type === "House") {
        document.getElementById("houseFields").style.display = "block";
    } else if (type === "Office") {
        document.getElementById("officeFields").style.display = "block";
    }
}