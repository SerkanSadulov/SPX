function setCookie(cname, cvalue, exdays) {
    const d = new Date();
    d.setTime(d.getTime() + exdays * 24 * 60 * 60 * 1000);
    let expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    let name = cname + "=";
    let ca = document.cookie.split(";");
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == " ") {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function setTheme(theme) {

    switch (theme) {

        case "light": {

            $("*").attr("data-bs-theme", "light");

            // Set local storage variable
            setCookie("theme", "light", 365);

            break;
        }

        case "dark": {

            $("*").attr("data-bs-theme", "dark");

            setCookie("theme", "dark", 365);

            break;
        }

        default: break;
    }

}

function deleteCookie(name) {
    document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
}

function redirectProfile() {
    window.location.href = "/Home/Profile";
}

function logoutUser() {
    deleteCookie("LoggedInData");
    deleteCookie("JWTToken");
    window.location.href = "/Home/Home";
}

function isTokenExpired(token) {
    if (!token) return true;

    try {
        const payloadBase64 = token.split('.')[1];
        const decodedPayload = JSON.parse(atob(payloadBase64));
        const expiryTime = decodedPayload.exp * 1000; 
        return Date.now() > expiryTime;
    } catch (error) {
        console.error("Error decoding token:", error);
        return true;
    }
}

function checkUserSession() {
    const loggedInData = getCookie("LoggedInData");
    const JWTToken = getCookie("JWTToken");

    if (!loggedInData || !JWTToken || isTokenExpired(JWTToken)) {
        logoutUser();
    }
}

function updateNavbar() {
    const loggedInData = getCookie("LoggedInData");
    const navElement = document.getElementById("nav-user");
    const addServiceButton = document.getElementById("add-service-button");

    if (loggedInData) {
        try {
            const userData = JSON.parse(loggedInData);

            let avatarUrl = userData.profilePicture ? `/profilePictures/${userData.profilePicture}` : "/profilePictures/default.png";

            let notificationBell = document.getElementById("notificationBell");
            let messagesBtnNavbar = document.getElementById("messagesBtnNavbar");

            notificationBell.classList.remove("d-none");
            notificationBell.classList.remove("d-flex");
            messagesBtnNavbar.classList.remove("d-none");

            let userProfileHtml = `
            <div class="d-md-flex justify-content-center">
                <div class="card bg-light text-dark mx-3" style="cursor: pointer; height: 2.5rem;" onclick="redirectProfile()">
                    <div class="card-body d-flex align-items-center p-2" style="overflow: hidden;">
                        <img src="${avatarUrl}" id='smallAvatar' alt="Avatar" class="rounded-circle" style="width: 20px; height: 20px; transform: scale(1.5); margin-right: 10px;" />
                        <span class="fw-bold">${userData.username}</span>
                    </div>
                </div>
            </div>`;

            navElement.innerHTML = userProfileHtml;
            document.getElementById("smallAvatar").onerror = function () {
                this.src = '/profilePictures/default.png';
            };

            if (userData.userType === "Provider") {
                addServiceButton.classList.remove("d-none");
                addServiceButton.classList.add("d-block");
            }

            startLongPolling();

        } catch (error) {
            console.error("Error parsing user data from cookie:", error);
            navElement.innerHTML = `
                <li class="nav-item">
                    <button class="btn btn-outline-light ms-lg-3" onclick="window.location.href='/Home/LogIn'" style="height: 2.5rem; padding: 0 1rem; border-radius: 5px; min-width:120px;">Вписване</button>
                </li>`;
        }
    } else {
        navElement.innerHTML = `
            <li class="nav-item">
                <button class="btn btn-outline-light ms-lg-3" onclick="window.location.href='/Home/LogIn'" style="height: 2.5rem; padding: 0 1rem; border-radius: 5px; min-width:120px;">Вписване</button>
            </li>`;
    }
}

function startLongPolling() {
    const servicesEndpoint = window.servicesEndpoint;
    const loggedInData = getCookie("LoggedInData");
    if (!loggedInData) return;

    const userData = JSON.parse(loggedInData);
    const userID = userData.userId;

    function pollForMessages() {
        const JWTToken = getCookie("JWTToken"); 
        fetch(`${servicesEndpoint}Messages/received/${userID}`, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${JWTToken}`,
                "Content-Type": "application/json",
            },
        })
            .then(response => response.json())
            .then(messages => {
                const oldMessages = JSON.parse(localStorage.getItem("storedMessages")) || [];
                const newMessages = messages.filter(message => !oldMessages.includes(message.messageID));

                updateNotifications(newMessages, messages);
                const messageIDs = messages.map(message => message.messageID);
                localStorage.setItem("storedMessages", JSON.stringify(messageIDs));

                setTimeout(pollForMessages, 10000);
            })
            .catch(error => {
                console.error("Error polling for messages:", error);
                setTimeout(pollForMessages, 10000);
            });
    }

    pollForMessages();
}

function updateNotifications(newMessages, allMessages) {
    const notificationCount = document.getElementById("notificationCount");
    const notificationsDropdown = document.getElementById("notificationsDropdown");
    const noNewMessages = document.getElementById("noNewMessages");
    const JWTToken = getCookie("JWTToken");

    if (!notificationCount || !notificationsDropdown || !noNewMessages) {
        console.error("Notification elements are missing in the DOM.");
        return;
    }

    if (allMessages.length > 0) {
        if (newMessages.length > 0) {
            notificationCount.innerHTML = `<i class="fas fa-exclamation"></i>`;
            notificationCount.classList.remove("d-none");

            localStorage.setItem("hasUnreadMessages", "true");
        } else {
            const hasUnreadMessages = localStorage.getItem("hasUnreadMessages");
            if (hasUnreadMessages === "true") {
                notificationCount.innerHTML = `<i class="fas fa-exclamation"></i>`;
                notificationCount.classList.remove("d-none");
            }
        }

        const messageItems = notificationsDropdown.querySelectorAll(".dropdown-item:not(.dropdown-header):not(#noNewMessages)");
        messageItems.forEach(item => item.remove());

        allMessages.slice(0, 3).forEach(message => {
            const li = document.createElement("li");
            li.className = "dropdown-item";
            const formattedDate = new Intl.DateTimeFormat("uk-UA", {
                day: "numeric",
                month: "numeric",
                year: "numeric",
            }).format(new Date(message.sentOn));

            fetch(`${servicesEndpoint}Users/${message.senderID}`, {
                method: "GET",
                headers: {
                    "Authorization": `Bearer ${JWTToken}`, 
                    "Content-Type": "application/json",
                },
            })
                .then(response => response.json())
                .then(users => {
                    li.innerHTML = `<a href="/Home/Profile#messagesTab" class="text-decoration-none "><h4 class="mb-0">${users.username}</h4>
                <h6 class="mb-0 ">${message.messageContent.substring(0, 30)}${message.messageContent.length > 30 ? '...' : ''}</h6>
                <p >${formattedDate}</p></a>`;
                })
                .catch(error => {
                    console.error("Error polling for messages or users:", error);
                });

            notificationsDropdown.insertBefore(li, noNewMessages);
        });
    } else {
        notificationCount.classList.add("d-none");
        noNewMessages.classList.remove("d-none");
    }
}
function escapeHtml(unsafe) {
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

document.addEventListener("DOMContentLoaded", function () {
    updateNavbar();

    let notificationBell = document.getElementById("notificationBell");

    notificationBell.addEventListener("click", function () {
        let notificationCount = document.getElementById("notificationCount");
        notificationCount.classList.add("d-none");

        localStorage.setItem("hasUnreadMessages", "false");
    });

    const hasUnreadMessages = localStorage.getItem("hasUnreadMessages");
    if (hasUnreadMessages === "true") {
        let notificationCount = document.getElementById("notificationCount");
        notificationCount.innerHTML = `<i class="fas fa-exclamation"></i>`;
        notificationCount.classList.remove("d-none");
    }

    setInterval(checkUserSession, 5 * 60 * 1000); 
});
