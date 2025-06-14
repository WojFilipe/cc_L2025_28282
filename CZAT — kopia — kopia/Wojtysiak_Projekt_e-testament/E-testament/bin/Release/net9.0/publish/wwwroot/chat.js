const token = localStorage.getItem("token");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5215/chathub", {
        accessTokenFactory: () => token || ""
    })
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Odbieranie wiadomości z serwera
connection.on("ReceiveMessage", function (user, message) {
    const msgBox = document.createElement("div");
    msgBox.className = "message";
    msgBox.innerHTML = `<strong>${user}</strong>: ${message}`;
    document.getElementById("messages").appendChild(msgBox);

    // Odśwież listę po otrzymaniu nowej wiadomości
    fetchMessages();
});

// Pobieranie wiadomości
async function fetchMessages() {
    try {
        const response = await fetch("http://localhost:5215/api/message", {
            headers: {
                "Authorization": "Bearer " + token
            }
        });
        if (!response.ok) {
            console.warn("Błąd pobierania wiadomości:", response.status);
            return;
        }
        const messages = await response.json();
        const messagesDiv = document.getElementById("messages");
        messagesDiv.innerHTML = ""; // Wyczyść przed aktualizacją
        messages.forEach(m => {
            const msgBox = document.createElement("div");
            msgBox.className = "message";
            msgBox.innerHTML = `<strong>${m.SenderName}</strong>: ${m.Content} (${m.SentAt})`;
            messagesDiv.appendChild(msgBox);
        });
    } catch (err) {
        console.error("Fetch error:", err);
    }
}

// Połączenie
connection.start()
    .then(() => {
        console.log("✅ Połączono z serwerem SignalR");
        fetchMessages(); // Pobierz wiadomości po połączeniu
    })
    .catch(err => console.error("❌ Błąd połączenia:", err.toString()));

// Wysyłanie wiadomości
document.getElementById("sendButton").addEventListener("click", function () {
    const user = document.getElementById("username").value.trim();
    const message = document.getElementById("message").value.trim();

    if (!user || !message) {
        alert("Uzupełnij nazwę użytkownika i wiadomość.");
        return;
    }

    connection.invoke("SendMessage", user, message)
        .then(() => {
            fetch("http://localhost:5215/api/message", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + token
                },
                body: JSON.stringify({ content: message, senderName: user })
            }).then(response => {
                if (!response.ok) {
                    console.warn("Nie udało się zapisać wiadomości w bazie:", response.status);
                } else {
                    console.log("Wiadomość zapisana w bazie.");
                }
            }).catch(err => console.error("Fetch error:", err));
            document.getElementById("message").value = "";
        })
        .catch(err => {
            console.error("❌ Błąd wysyłania wiadomości:", err.toString());
        });
});