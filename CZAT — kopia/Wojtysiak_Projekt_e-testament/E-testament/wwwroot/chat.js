const token = localStorage.getItem("token");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub", {
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
});

// Połączenie
connection.start()
    .then(() => {
        console.log("✅ Połączono z serwerem SignalR");
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
            // Zapis do bazy przez API
            fetch("/api/message", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + token
                },
                body: JSON.stringify({ content: message })
            }).then(response => {
                if (!response.ok) {
                    console.warn("Nie udało się zapisać wiadomości w bazie.");
                }
            }).catch(err => console.error("Fetch error:", err));

            document.getElementById("message").value = "";
        })
        .catch(err => {
            console.error("❌ Błąd wysyłania wiadomości:", err.toString());
        });
});
