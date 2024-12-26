var originalOnLoad = window.onload;
console.log("Custom onload logic1");
debugger;
document.addEventListener('DOMContentLoaded', function () {
    console.log("Custom DOMContentLoaded logic");
 
    var intervalId = setInterval(function () {
        var selectElement = document.getElementById('select');
        if (selectElement) {
            console.log('Select element found:', selectElement);
            clearInterval(intervalId); // Stop polling

            selectElement.addEventListener('change', function (event) {
                var selectedValue = event.target.value;
                var selectedText = event.target.options[event.target.selectedIndex].text;
                console.log('Selected value:', selectedValue);
                console.log('Selected Text:', selectedText);
                if (selectedText =="Documents v1") {
                   // window.location.href = "http://172.16.201.17:84/swagger/index.html";
                    window.open("http://172.16.201.17:84/swagger/index.html", "_blank");
                }
            });
        } else {
            console.log('Waiting for select element...');
        }
    }, 500);



});

