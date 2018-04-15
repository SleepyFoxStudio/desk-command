const connection = new signalR.HubConnection("/chathub");

connection.on("ReceiveMessage", (timestamp, user, message) => {
    const encodedUser = user;
    const encodedMsg = message;
    const listItem = document.createElement("li");
    listItem.innerText = timestamp + " " + encodedUser + ": " + encodedMsg;
    document.getElementById("messages").appendChild(listItem);
});


connection.on("ChangeImage", (id, actionId, imgUrl) => {
    actionId = actionId+1;
    if (isCurrentLayout(id)) {
        console.log(`updating icon ${actionId} layout for ${id}`);
        $(`#layoutDiv div:nth-child(${actionId}) a img`)[0].src = `/icons/${imgUrl}`;
    } else {
        console.log(`not updating icon ${actionId} you're on the wrong layout for ${id}`);
    }
});

document.getElementById("send").addEventListener("click", event => {
    const msg = document.getElementById("message").value;
    const usr = document.getElementById("user").value;

    connection.invoke("SendMessage", usr, msg).catch(err => showErr(err));
    event.preventDefault();
});

function showErr(msg) {
    const listItem = document.createElement("li");
    listItem.setAttribute("style", "color: red");
    listItem.innerText = msg.toString();
    document.getElementById("messages").appendChild(listItem);
}

connection.start().catch(err => showErr(err));