// CONFIG
const IMAGES_PER_ROW = 4;
const ROWS_PER_LOAD = 4;
const PAGE_SIZE = IMAGES_PER_ROW * ROWS_PER_LOAD;

let page = 1;
let allImages = [];
let totalLoadedRows = 0;
let loading = false;
let finished = false;
let currentIndex = 0;

function createImageItem(img, idx) {
    const div = document.createElement('div');
    div.className = "image-item";
    const imgEl = document.createElement('img');
    imgEl.src = `/Files/Feedback/${img}`;
    imgEl.alt = `Feedback ${idx + 1}`;
    imgEl.setAttribute('data-index', idx);
    imgEl.addEventListener('click', () => openLightbox(idx));
    div.appendChild(imgEl);
    return div;
}

function renderImages(images) {
    const container = document.getElementById("imageContainer");
    let startIdx = allImages.length;
    allImages.push(...images);
    images.forEach((img, i) => {
        const idx = startIdx + i;
        container.appendChild(createImageItem(img, idx));
    });
}

function loadImages() {
    if (loading || finished) return;
    loading = true;
    fetch(`/Feedback/LoadImages?page=${page}&pageSize=${PAGE_SIZE}`)
        .then(res => res.json())
        .then(images => {
            if (images.length === 0) {
                finished = true;
                return;
            }
            renderImages(images);
            totalLoadedRows += ROWS_PER_LOAD;
            page++;
        })
        .finally(() => loading = false);
}

// Show only the first N rows, load more on scroll
function shouldLoadMore() {
    const lastRowLoaded = totalLoadedRows * IMAGES_PER_ROW;
    const imgs = document.querySelectorAll('.image-grid .image-item');
    if (imgs.length === 0) return false;
    const lastImg = imgs[lastRowLoaded - 1] || imgs[imgs.length - 1];
    if (!lastImg) return false;
    const rect = lastImg.getBoundingClientRect();
    // If the last image of the last loaded row is visible in viewport, trigger load
    return rect.bottom < window.innerHeight + 80;
}

// Lightbox logic
function openLightbox(index) {
    currentIndex = index;
    document.getElementById("lightboxImage").src = `/Files/Feedback/${allImages[currentIndex]}`;
    document.getElementById("lightbox").style.display = "flex";
    updateControls();
}

function updateControls() {
    document.getElementById("prevBtn").style.display = currentIndex <= 0 ? "none" : "block";
    document.getElementById("nextBtn").style.display = currentIndex >= allImages.length - 1 ? "none" : "block";
}

document.getElementById("lightbox").addEventListener("click", function (e) {
    if (e.target.id === "lightboxImage" || e.target.classList.contains("fas")) return;
    this.style.display = "none";
});

document.getElementById("prevBtn").addEventListener("click", function (e) {
    e.stopPropagation();
    if (currentIndex > 0) {
        currentIndex--;
        document.getElementById("lightboxImage").src = `/Files/Feedback/${allImages[currentIndex]}`;
        updateControls();
    }
});

document.getElementById("nextBtn").addEventListener("click", function (e) {
    e.stopPropagation();
    if (currentIndex < allImages.length - 1) {
        currentIndex++;
        document.getElementById("lightboxImage").src = `/Files/Feedback/${allImages[currentIndex]}`;
        updateControls();
    }
});

// Initial load
loadImages();

// Intersection observer for infinite scroll (better than scroll event)
window.addEventListener('scroll', () => {
    if (shouldLoadMore()) {
        loadImages();
    }
});