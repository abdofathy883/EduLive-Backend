// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById("sidebar");
    const toggleBtn = document.getElementById("toggleSidebar");

    // Collapse sidebar by default on medium and small screens
    function setInitialSidebarState() {
        if (window.innerWidth <= 992) {
            sidebar.classList.add("collapsed");
        } else {
            sidebar.classList.remove("collapsed");
        }
    }

    setInitialSidebarState();

    // Update on window resize
    window.addEventListener("resize", setInitialSidebarState);

    // Toggle sidebar on button click
    toggleBtn.addEventListener("click", function () {
        sidebar.classList.toggle("collapsed");
    });
});

