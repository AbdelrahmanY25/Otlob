function goToUrl(url) {
    window.location.href = url;
}

const MAPBOX_TOKEN = "pk.eyJ1IjoiYWJkZWxyYWhtYW4yNSIsImEiOiJjbWZpN2h0c2kwazVkMmpzYXZjZzYxcjQ1In0.JrBlnVg0YqMqrGqpgj2CnQ";

let timeout = null;
let selectedCoords = null; // [lon, lat]

// Uses toastr if available, otherwise fallback to alert
function showError(message) {
    if (typeof toastr !== 'undefined') {
        toastr.error(message);
    } else {
        alert(message);
    }
}

function showInfo(message) {
    if (typeof toastr !== 'undefined') {
        toastr.info(message);
    }
}

function getCurrentLocation() {
    if (!navigator.geolocation) {
        showError("Geolocation not supported by your browser.");
        return;
    }
    
    showInfo("Detecting location...");
    navigator.geolocation.getCurrentPosition(success, geoError);
}

function success(position) {
    const lat = position.coords.latitude;
    const lon = position.coords.longitude;

    // store coords
    selectedCoords = [lon, lat];

    let url = `https://api.mapbox.com/geocoding/v5/mapbox.places/${lon},${lat}.json?access_token=${MAPBOX_TOKEN}&country=eg`;

    fetch(url)
        .then(res => res.json())
        .then(data => {
            if (data.features && data.features.length > 0) {
                document.getElementById("searchInput").value = data.features[0].place_name;
            } else {
                // If address not found, we still have coords, which is fine
            }
        })
        .catch(() => {
            // silent fail on reverse geocoding, as we primarily need coords
        });
}

function geoError(error) {
    let msg = "Unable to retrieve your location.";
    if (error.code === 1) { // PERMISSION_DENIED
        msg = "Location access denied. Please enable GPS permissions.";
    }
    showError(msg);
}

function searchAddress() {
    clearTimeout(timeout);

    timeout = setTimeout(() => {
        let query = document.getElementById("searchInput").value;
        selectedCoords = null; // reset until user selects or goes
        if (query.length < 3) {
            document.getElementById("results").innerHTML = "";
            return;
        }

        let url = `https://api.mapbox.com/geocoding/v5/mapbox.places/${encodeURIComponent(query)}.json?access_token=${MAPBOX_TOKEN}&country=eg&autocomplete=true&limit=5`;

        fetch(url)
            .then(res => res.json())
            .then(data => {
                let resultsDiv = document.getElementById("results");
                resultsDiv.innerHTML = "";
                if (data.features && data.features.length > 0) {
                    data.features.forEach(item => {
                        let div = document.createElement("div");
                        div.innerText = item.place_name;
                        // store coords on the element
                        div.dataset.lon = item.center[0];
                        div.dataset.lat = item.center[1];
                        div.onclick = () => selectAddress(item.place_name, item.center);
                        resultsDiv.appendChild(div);
                    });
                    resultsDiv.classList.add('active'); // show
                } else {
                    resultsDiv.classList.remove('active');
                }
            })
            .catch(() => showError("Error fetching search results."));
    }, 400);
}

function selectAddress(place, center) {
    document.getElementById("searchInput").value = place;
    document.getElementById("results").innerHTML = "";
    document.getElementById("results").classList.remove('active');

    if (Array.isArray(center) && center.length >= 2) {
        selectedCoords = [center[0], center[1]]; // [lon, lat]
    } else {
        selectedCoords = null;
    }
}

// "Let's Go" Button Logic
// 1. If input has value -> Use input/geocoding
// 2. If input empty -> Force "Quick Navigation" (Detect & Redirect)
function handleGoButtonClick() {
    if (!navigator.geolocation) {
        showError("Geolocation not supported by your browser.");
        return;
    }
    
    // Check search input
    const query = document.getElementById('searchInput').value.trim();
    
    // If empty, assume user wants "Check my location and go"
    if (!query) {
        handleQuickNavigation();
        return;
    }

    // if we already have selectedCoords (from selection or previous GPS), use it
    if (selectedCoords) {
        redirectWithCoords(selectedCoords);
        return;
    }

    // otherwise geocode the query
    const url = `https://api.mapbox.com/geocoding/v5/mapbox.places/${encodeURIComponent(query)}.json?access_token=${MAPBOX_TOKEN}&country=eg&limit=1`;
    fetch(url)
        .then(res => res.json())
        .then(data => {
            if (data.features && data.features.length > 0) {
                const center = data.features[0].center; // [lon, lat]
                selectedCoords = [center[0], center[1]];
                redirectWithCoords(selectedCoords);
            } else {
                showError('Address not found. Please try another search.');
            }
        })
        .catch(() => showError('Error locating address.'));
}

// "Promo Cards" Logic & Fallback for Button
// 1. Detect Location
// 2. Immediately Redirect
function handleQuickNavigation() {
    if (!navigator.geolocation) {
        showError("Geolocation not supported by your browser.");
        return;
    }
    
    showInfo("Getting your location...");
    
    navigator.geolocation.getCurrentPosition((position) => {
        const lat = position.coords.latitude;
        const lon = position.coords.longitude;
        // Direct redirect
        redirectWithCoords([lon, lat]); // redirect expects [lon, lat]
    }, geoError);
}

function redirectWithCoords(coords) {
    const lon = coords[0];
    const lat = coords[1];
    // redirect to Customer Home Restaurants with query params
    window.location.href = `/Customer/Home/Restaurants?lat=${encodeURIComponent(lat)}&lon=${encodeURIComponent(lon)}`;
}

// close results when clicking outside
document.addEventListener("click", function (event) {
    const resultsDiv = document.getElementById("results");
    const searchInput = document.getElementById("searchInput");

    if (
        resultsDiv &&
        !resultsDiv.contains(event.target) && 
        event.target !== searchInput
    ) {
        resultsDiv.innerHTML = "";
        resultsDiv.classList.remove('active');
    }
});