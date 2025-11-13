// intro.js
document.addEventListener('DOMContentLoaded', function() {
    const animatedHeading = document.querySelector('.animated-heading');
    if (animatedHeading) {
        animatedHeading.classList.add('animate');
    }
    
    const removeIntro = document.querySelector('.intro-container');
    if (removeIntro) {
        requestAnimationFrame(() => {
            setTimeout(() => {
                    removeIntro.classList.add("no-opacity");
                setTimeout(() => {
                    removeIntro.classList.add("display-none");

                }, 1500);
            }, 4000);
        });
    }
});