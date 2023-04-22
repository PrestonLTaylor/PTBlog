let profilePictures = document.querySelectorAll(".profile-picture, .nav-profile-picture");
profilePictures.forEach(function (element) {
    element.addEventListener("error", function (event) {
        event.target.src = "https://i.imgur.com/ei3kdCj.png";

        // Prevents an onerror being called over and over again if the default profile picture breaks
        event.target.onerror = null;
    });
});