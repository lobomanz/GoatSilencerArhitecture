document.addEventListener("DOMContentLoaded", function () {
    const slides = document.querySelectorAll(".hero-slide");
    const dots = document.querySelectorAll(".hero-dot");
    let current = 0;

    function showSlide(index) {
        slides.forEach((s, i) => {
            s.classList.remove("active", "prev");
            if (i === index) {
                s.classList.add("active");
            } else if (i === (index - 1 + slides.length) % slides.length) {
                s.classList.add("prev");
            }
        });

        dots.forEach((d, i) => {
            d.classList.toggle("active", i === index);
        });

        current = index;
    }

    function nextSlide() {
        let next = (current + 1) % slides.length;
        showSlide(next);
    }

    dots.forEach(d => {
        d.addEventListener("click", () => {
            showSlide(parseInt(d.dataset.index));
        });
    });

    setInterval(nextSlide, 4000); // 4s autoplay
});
