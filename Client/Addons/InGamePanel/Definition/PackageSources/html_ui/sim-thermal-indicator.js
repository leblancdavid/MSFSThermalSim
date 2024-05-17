function onToggleWindBtn() {
    var toggleBtn = document.getElementById('windToggleBtn');
    var ledDisplay = document.getElementById('windSpeedDisplay');

    if(toggleBtn.classList.contains('turned-on')) {
        toggleBtn.classList.remove('turned-on');
        document.getElementById('windIndicator').style.display = 'none';
        ledDisplay.classList.remove('turned-on');
        ledDisplay.innerHTML = '-';

    } else {
        toggleBtn.classList.add('turned-on');
        document.getElementById('windIndicator').style.display = 'flex';
        ledDisplay.classList.add('turned-on');
    }
}

function onToggleThermalBtn() {
    var toggleBtn = document.getElementById('thermalToggleBtn');
    var ledDisplay = document.getElementById('distanceToThermalDisplay');

    if(toggleBtn.classList.contains('turned-on')) {
        toggleBtn.classList.remove('turned-on');
        document.getElementById('nearestThermalIndicator').style.display = 'none';
        ledDisplay.classList.remove('turned-on');
        ledDisplay.innerHTML = '-';
    } else {
        toggleBtn.classList.add('turned-on');
        document.getElementById('nearestThermalIndicator').style.display = 'flex';
        ledDisplay.classList.add('turned-on');
    }
}