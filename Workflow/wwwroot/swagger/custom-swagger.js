var originalOnLoad = window.onload;
console.log("Custom onload logic1");
debugger;
document.addEventListener('DOMContentLoaded', function () {
    console.log("Custom DOMContentLoaded logic");

    // Get the <select> element inside the download-url-wrapper class
    var selectElement = document.getElementById('select');
    debugger;
    console.log(selectElement);
    // Check if the element exists before adding event listener
    if (selectElement) {
        selectElement.addEventListener('change', function (event) {
            var selectedValue = event.target.value;
            console.log('Selected value:', selectedValue);
        });
    } else {
        console.log('The select element was not found!');
    }
});

