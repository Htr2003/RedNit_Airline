document.addEventListener("DOMContentLoaded", function () {
    const bannerImages = document.querySelectorAll(".banner-image");
    let currentIndex = 0;

    function showImage(index) {
        for (let i = 0; i < bannerImages.length; i++) {
            bannerImages[i].classList.remove("active");
        }
        bannerImages[index].classList.add("active");
    }

    function changeImage() {
        currentIndex = (currentIndex + 1) % bannerImages.length;
        showImage(currentIndex);
    }

    setInterval(changeImage, 2000); // Chuyển đổi ảnh mỗi 5 giây

    // Bắt đầu với ảnh đầu tiên
    showImage(currentIndex);
});