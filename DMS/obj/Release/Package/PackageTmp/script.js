// Get the kebab icon and dropdown content
const kebabIcon = document.getElementById('kebabIcon');
const kebabDropdown = document.getElementById('kebabDropdown');

// Toggle the visibility of the dropdown content when the kebab icon is clicked
kebabIcon.addEventListener('click', function () {
    kebabDropdown.style.display = (kebabDropdown.style.display === 'block') ? 'none' : 'block';
});


function openNav() {
    document.getElementById("myNav").style.height = "100%";
}

function closeNav() {
    document.getElementById("myNav").style.height = "0%";
}



//const btn1 = document.querySelector(".toggle-btn");
//const btn2 = document.querySelector(".images");

//btn1.addEventListener("click", function () {
//    document.querySelector("#sidebar").classList.toggle("expand");
//});

//btn2.addEventListener("click", function () {
//    document.querySelector("#sidebar").classList.toggle("expand");
//})

//// Get all sidebar items
//const sidebarItems = document.querySelectorAll('.sidebar-item');

//// Loop through each sidebar item
//sidebarItems.forEach(item => {
//    // Get the label content for this item
//    const labelContent = item.getAttribute('data-label');

//    // Set the CSS variable for label content
//    item.style.setProperty('--label-content', `"${labelContent}"`);
//});







