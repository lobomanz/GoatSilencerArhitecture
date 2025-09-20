document.addEventListener('DOMContentLoaded', () => {
    const fullpageSlider = document.querySelector('.fullpage-slider');
    const sliderContainer = document.querySelector('.slider-container');
    let slides = document.querySelectorAll('.slide'); // Use let as it will be updated
    const dotsContainer = document.querySelector('.slider-dots');
    const body = document.body;

    let currentSlide = 0;
    let isScrolling = false;
    let autoScrollInterval;
    const transitionDuration = 800; // Match CSS transition duration

    // Add class to body for homepage specific styles (e.g., transparent header)
    body.classList.add('homepage-active');

    // Clone the first slide and append it for seamless looping
    const firstSlideClone = slides[0].cloneNode(true);
    sliderContainer.appendChild(firstSlideClone);
    slides = document.querySelectorAll('.slide'); // Update NodeList to include clone

    // Create dots if they don't exist (or update active state)
    if (dotsContainer && slides.length > 0) {
        dotsContainer.innerHTML = ''; // Clear existing dots if any
        // Only create dots for original slides, not the clone
        for (let i = 0; i < slides.length - 1; i++) {
            const dot = document.createElement('div');
            dot.classList.add('dot');
            if (i === 0) {
                dot.classList.add('active');
            }
            dot.dataset.index = i;
            dotsContainer.appendChild(dot);
        }
    }

    const dots = document.querySelectorAll('.dot');

    function goToSlide(index, animate = true) {
        if (isScrolling && animate) return; // Prevent rapid navigation during transition

        isScrolling = true;

        // Handle looping from last original slide to first (via clone)
        if (index === slides.length - 1) { // If target is the clone
            currentSlide = index;
            const offset = -currentSlide * 100;
            sliderContainer.style.transform = `translateY(${offset}vh)`;

            // After transition to clone, instantly jump to actual first slide
            setTimeout(() => {
                sliderContainer.style.transition = 'none'; // Disable transition for instant jump
                currentSlide = 0;
                sliderContainer.style.transform = `translateY(0vh)`;
                // Re-enable transition after a very short delay
                setTimeout(() => {
                    sliderContainer.style.transition = `transform ${transitionDuration / 1000}s ease-in-out`;
                    isScrolling = false;
                }, 50);
            }, transitionDuration);

        } else if (index < 0) { // Handle looping from first to last (always downwards)
            // This case is for scrolling up from the first slide
            // We need to go to the last original slide
            currentSlide = slides.length - 2; // Last original slide
            const offset = -currentSlide * 100;
            sliderContainer.style.transform = `translateY(${offset}vh)`;
            setTimeout(() => {
                isScrolling = false;
            }, transitionDuration);

        } else {
            currentSlide = index;
            const offset = -currentSlide * 100;
            sliderContainer.style.transform = `translateY(${offset}vh)`;
            setTimeout(() => {
                isScrolling = false;
            }, transitionDuration);
        }

        // Update dots for original slides
        dots.forEach((dot, idx) => {
            if (idx === (currentSlide % (slides.length - 1))) { // Use modulo for dot active state
                dot.classList.add('active');
            } else {
                dot.classList.remove('active');
            }
        });

        resetAutoScroll();
    }

    function scrollToNextSlide() {
        goToSlide((currentSlide + 1)); // No modulo here, let it go to clone
    }

    function scrollToPrevSlide() {
        // When going from first to last, we want to simulate scrolling down
        // So, if current is 0, go to the last original slide
        if (currentSlide === 0) {
            // Temporarily jump to the clone of the last slide (if we had one at the top)
            // For now, just go to the last original slide directly
            goToSlide(slides.length - 2); // Last original slide
        } else {
            goToSlide(currentSlide - 1);
        }
    }

    // Disable normal scrolling
    window.addEventListener('wheel', (e) => {
        e.preventDefault();
        if (isScrolling) return;

        isScrolling = true;

        if (e.deltaY > 0) {
            scrollToNextSlide();
        } else {
            // For prev scroll, if at first slide, go to last original slide
            if (currentSlide === 0) {
                goToSlide(slides.length - 2); // Go to last original slide
            } else {
                scrollToPrevSlide();
            }
        }

        // isScrolling is reset by goToSlide's setTimeout
    }, { passive: false });

    // Dot navigation
    if (dotsContainer) {
        dotsContainer.addEventListener('click', (e) => {
            if (e.target.classList.contains('dot')) {
                const index = parseInt(e.target.dataset.index);
                goToSlide(index);
            }
        });
    }

    // Auto-scroll functionality
    function startAutoScroll() {
        autoScrollInterval = setInterval(scrollToNextSlide, 5000);
    }

    function resetAutoScroll() {
        clearInterval(autoScrollInterval);
        startAutoScroll();
    }

    // Initial setup
    goToSlide(0, false); // Initial setup without animation
    startAutoScroll();

    // Clean up class when navigating away from homepage (optional, but good practice)
    window.addEventListener('beforeunload', () => {
        body.classList.remove('homepage-active');
    });
});
