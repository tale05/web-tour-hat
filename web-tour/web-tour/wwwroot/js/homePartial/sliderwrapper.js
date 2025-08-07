let myIndexForSliderWarrper = 0;
let carouselTimer;
const slides = document.getElementsByClassName("mySlides");
const indicators = document.getElementsByClassName("indicator");
const prevButton = document.querySelector(".prev-slide");
const nextButton = document.querySelector(".next-slide");

function initSlider() {
    // Set up initial state
    if (slides.length > 0) {
        slides[0].classList.add('active');
    }
    if (indicators.length > 0) {
        indicators[0].classList.add('active');
    }

    // Event listeners
    if (prevButton) {
        prevButton.addEventListener('click', () => changeSlide(-1));
    }
    if (nextButton) {
        nextButton.addEventListener('click', () => changeSlide(1));
    }

    for (let i = 0; i < indicators.length; i++) {
        indicators[i].addEventListener('click', function () {
            clearTimeout(carouselTimer);
            const slideIndex = parseInt(this.getAttribute('data-index')) - 1;
            goToSlide(slideIndex);
        });
    }

    startSlideshow();

    const slideWrapper = document.querySelector(".slide-wrapper");
    if (slideWrapper) {
        slideWrapper.addEventListener('mouseenter', () => clearTimeout(carouselTimer));
        slideWrapper.addEventListener('mouseleave', () => startSlideshow());

        let touchStartX = 0;
        let touchEndX = 0;

        slideWrapper.addEventListener('touchstart', (e) => {
            touchStartX = e.changedTouches[0].screenX;
        });

        slideWrapper.addEventListener('touchend', (e) => {
            touchEndX = e.changedTouches[0].screenX;
            handleSwipe();
        });

        function handleSwipe() {
            const swipeThreshold = 50;
            if (touchEndX < touchStartX - swipeThreshold) {
                changeSlide(1);
            } else if (touchEndX > touchStartX + swipeThreshold) {
                changeSlide(-1);
            }
        }
    }
}

function startSlideshow() {
    carouselTimer = setTimeout(() => {
        changeSlide(1);
    }, 3000);
}

function changeSlide(step) {
    clearTimeout(carouselTimer);
    myIndexForSliderWarrper += step;

    if (myIndexForSliderWarrper >= slides.length) {
        myIndexForSliderWarrper = 0;
    }
    if (myIndexForSliderWarrper < 0) {
        myIndexForSliderWarrper = slides.length - 1;
    }

    goToSlide(myIndexForSliderWarrper);
}

function goToSlide(index) {
    if (index < 0 || index >= slides.length) return;

    for (let i = 0; i < slides.length; i++) {
        slides[i].classList.remove('active');
    }
    for (let i = 0; i < indicators.length; i++) {
        indicators[i].classList.remove('active');
    }

    if (slides[index]) {
        slides[index].classList.add('active');
    }
    if (indicators[index]) {
        indicators[index].classList.add('active');
    }

    myIndexForSliderWarrper = index;
    startSlideshow();
}

document.addEventListener('DOMContentLoaded', initSlider);