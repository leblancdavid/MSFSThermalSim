function onToggleWindBtn() {
    var toggleBtn = document.getElementById('windToggleBtn');
    if(toggleBtn.classList.contains('turned-on')) {
        toggleBtn.classList.remove('turned-on');
        document.getElementById('windIndicator').style.display = 'none';
    } else {
        toggleBtn.classList.add('turned-on');
        document.getElementById('windIndicator').style.display = 'flex';
    }
}

function onToggleThermalBtn() {
    var toggleBtn = document.getElementById('thermalToggleBtn');
    if(toggleBtn.classList.contains('turned-on')) {
        toggleBtn.classList.remove('turned-on');
        document.getElementById('nearestThermalIndicator').style.display = 'none';
    } else {
        toggleBtn.classList.add('turned-on');
        document.getElementById('nearestThermalIndicator').style.display = 'flex';
    }
}