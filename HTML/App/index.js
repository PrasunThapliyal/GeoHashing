var map;

var lngSouthWest;
var latSouthWest;
var lngNorthEast;
var latNorthEast;

var zoomLevel;
let markers = [];

function initMap() {
	const myHome = { lat: 28.40145127845222, lng: 77.05493457528205 };
	map = new google.maps.Map(document.getElementById("map"), {
		zoom: 7,
		center: myHome,
	});
	addMarker(myHome, "H");
  
	google.maps.event.addListener(map, "bounds_changed", function() {
		// send the new bounds back to your server
		var bounds = map.getBounds();
		const sw = map.getBounds().getSouthWest();
		const ne = map.getBounds().getNorthEast();
		lngSouthWest = sw.lng();
		latSouthWest = sw.lat();
		lngNorthEast = ne.lng();
		latNorthEast = ne.lat();
	});
	
	google.maps.event.addListener(map, 'zoom_changed', function() {
		zoomLevel = map.getZoom();
		console.log("Zoom level: " + zoomLevel);
	});
}

async function fetchDataAsync() {
	var precision = document.getElementById("Precision").value;
	const response = await fetch('https://localhost:5001/weatherforecast/foo?'+ 
		new URLSearchParams(
		{
			precision: precision,
			lngSouthWest: lngSouthWest,
			latSouthWest: latSouthWest,
			lngNorthEast: lngNorthEast,
			latNorthEast: latNorthEast
		}));
    var pointsData = await response.json();
	console.log("Zoom Level: " + zoomLevel + ", Precision: " + precision + ", Number of clusters: " + pointsData.length);
	
	for (var i in pointsData) {
		var latLong = { lat: pointsData[i].latitude, lng: pointsData[i].longitude };
		addMarker(latLong, pointsData[i].count);
	}
}

// Adds a marker to the map and push to the array.
function addMarker(location, label) {
  const marker = new google.maps.Marker({
    position: location,
    map: map,
	label: label + ""
  });
  markers.push(marker);
}

// Sets the map on all markers in the array.
function setMapOnAll(map) {
  for (let i = 0; i < markers.length; i++) {
    markers[i].setMap(map);
  }
}

// Removes the markers from the map, but keeps them in the array.
function clearMarkers() {
  setMapOnAll(null);
}

// Shows any markers currently in the array.
function showMarkers() {
  setMapOnAll(map);
}

// Deletes all markers in the array by removing references to them.
function deleteMarkers() {
  clearMarkers();
  markers = [];
}
