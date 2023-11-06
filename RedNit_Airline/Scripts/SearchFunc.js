document.addEventListener("DOMContentLoaded", function () {
    const khuhoiBtn = document.getElementById("khuhoi-btn");
    const motchieuBtn = document.getElementById("motchieu-btn");
    const nhieuchangBtn = document.getElementById("nhieuchang-btn");
    const khuhoiFields = document.getElementById("khuhoi-fields");
    const nhieuchangFields = document.getElementById("nhieuchang-fields");

    khuhoiBtn.addEventListener("click", function () {
        khuhoiFields.style.display = "block";
        nhieuchangFields.style.display = "none";
    });

    motchieuBtn.addEventListener("click", function () {
        khuhoiFields.style.display = "none";
        nhieuchangFields.style.display = "none";
    });

    nhieuchangBtn.addEventListener("click", function () {
        khuhoiFields.style.display = "none";
        nhieuchangFields.style.display = "block";
    });
});
