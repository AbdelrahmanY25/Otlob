function goToUrl(url) {
    window.location.href = url;
}

const MAPBOX_TOKEN = "pk.eyJ1IjoiYWJkZWxyYWhtYW4yNSIsImEiOiJjbWZpN2h0c2kwazVkMmpzYXZjZzYxcjQ1In0.JrBlnVg0YqMqrGqpgj2CnQ";

let timeout = null;

function showError(message) {
    document.getElementById("error").innerText = message;
}

function clearError() {
    document.getElementById("error").innerText = "";
}

function getCurrentLocation() {
    clearError();
    if (!navigator.geolocation) {
        showError("Geolocation not supported by your browser.");
        return;
    }
    navigator.geolocation.getCurrentPosition(success, geoError);
}

function success(position) {
    const lat = position.coords.latitude;
    const lon = position.coords.longitude;

    let url = `https://api.mapbox.com/geocoding/v5/mapbox.places/${lon},${lat}.json?access_token=${MAPBOX_TOKEN}&country=eg`;

    fetch(url)
        .then(res => res.json())
        .then(data => {
            if (data.features && data.features.length > 0) {
                document.getElementById("searchInput").value = data.features[0].place_name;
            } else {
                showError("Could not find an address for your location.");
            }
        })
        .catch(() => showError("Error retrieving location data."));
}

function geoError() {
    showError("Unable to retrieve your location.");
}

function searchAddress() {
    clearTimeout(timeout);
    clearError();

    timeout = setTimeout(() => {
        let query = document.getElementById("searchInput").value;
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
                        div.onclick = () => selectAddress(item.place_name);
                        resultsDiv.appendChild(div);
                    });
                } else {
                    showError("No results found.");
                }
            })
            .catch(() => showError("Error fetching search results."));
    }, 400);
}
function selectAddress(place) {
    document.getElementById("searchInput").value = place;
    document.getElementById("results").innerHTML = "";
    clearError();
}

document.addEventListener("click", function (event) {
    const resultsDiv = document.getElementById("results");
    const searchInput = document.getElementById("searchInput");

    if (
        resultsDiv &&
        !resultsDiv.contains(event.target) && 
        event.target !== searchInput
    ) {
        resultsDiv.innerHTML = ""
    }
});