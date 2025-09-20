// Rotate the magic ring with JS instead of CSS keyframes
let angle = 0;
const ring = document.querySelector('.magic-circle .ring');
setInterval(() => {
    angle = (angle + 2) % 360; // increase rotation angle
    ring.style.transform = `rotate(${angle}deg)`;
}, 30); // ~33fps (smooth enough)

// Hide loader after 3 seconds
window.addEventListener("load", function () {
    setTimeout(function () {
        document.getElementById("loader").style.display = "none";
    }, 3000);
});
