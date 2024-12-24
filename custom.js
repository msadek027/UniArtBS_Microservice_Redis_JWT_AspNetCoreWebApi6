window.onload = function () {
    // Add a link to an external Swagger UI
    const externalLink = document.createElement("a");
    externalLink.href = "http://172.16.201.17:84/swagger/index.html";
    externalLink.target = "_blank";
    externalLink.innerText = "Documents Swagger UI";

    // Add the link to the top of the Swagger UI
    const container = document.querySelector(".topbar-wrapper");
    if (container) {
        container.appendChild(externalLink);
        externalLink.style.marginLeft = "20px"; // Optional: Add spacing
        externalLink.style.color = "#007bff";  // Optional: Style link color
    }
};
