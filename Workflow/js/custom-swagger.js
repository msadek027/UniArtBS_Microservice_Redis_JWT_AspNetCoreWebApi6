window.onload = function () {
    // Create a new button or link
    var customLink = document.createElement('a');
    customLink.href = '#';
    customLink.innerText = 'Go to Documents Swagger';
    customLink.style = 'margin: 10px; display: inline-block; font-weight: bold; color: blue; cursor: pointer;';

    // Attach click event
    customLink.onclick = function (event) {
        event.preventDefault();
        window.location.href = 'http://172.16.201.17:84/swagger/index.html';
    };

    // Append the link to the top bar of Swagger UI
    var topBar = document.querySelector('.topbar');
    if (topBar) {
        topBar.appendChild(customLink);
    }
};
