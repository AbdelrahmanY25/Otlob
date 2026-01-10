const MAPBOX_TOKEN = "pk.eyJ1IjoiYWJkZWxyYWhtYW4yNSIsImEiOiJjbWZpN2h0c2kwazVkMmpzYXZjZzYxcjQ1In0.JrBlnVg0YqMqrGqpgj2CnQ";

function selectPayment(element) {
    // Remove selected class from all options
    document.querySelectorAll('.payment-option').forEach(opt => {
        opt.classList.remove('selected');
    });
    // Add selected class to clicked option
    element.classList.add('selected');
    // Check the radio button
    const radio = element.querySelector('input[type="radio"]');
    radio.checked = true;
}

// Initialize map
document.addEventListener('DOMContentLoaded', function() {
    const mapContainer = document.getElementById('map');
    if (mapContainer && typeof mapboxgl !== 'undefined') {
        const lat = parseFloat(mapContainer.dataset.lat);
        const lng = parseFloat(mapContainer.dataset.lng);
        
        if (!isNaN(lat) && !isNaN(lng)) {
            mapboxgl.accessToken = MAPBOX_TOKEN;
            
            const map = new mapboxgl.Map({
                container: 'map',
                style: 'mapbox://styles/mapbox/streets-v12',
                center: [lng, lat], // Mapbox uses [lng, lat]
                zoom: 15,
                interactive: false // Static view for checkout
            });

            // Add marker
            new mapboxgl.Marker({ color: "#F04335" })
                .setLngLat([lng, lat])
                .addTo(map);
                
            // Add navigation control just in case user wants to zoom
            map.addControl(new mapboxgl.NavigationControl(), 'top-right');
        }
    }
});
