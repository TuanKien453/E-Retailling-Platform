﻿const chatToggleIcon = document.querySelector(".chat-toggle-icon");
const chatBox = document.getElementById("chatBox");
const chatBoxContent = document.getElementById("chatBoxContent");
const userMessageInput = document.getElementById("userMessage");
const sendMessageButton = document.getElementById("sendMessage");

chatToggleIcon.addEventListener("click", function () {
    if (chatBox.style.display === "none" || chatBox.style.display === "") {
        chatBox.style.display = "block";
    } else {
        chatBox.style.display = "none";
    }
});

async function sendMessage() {
    const userMessage = userMessageInput.value.trim();

    if (userMessage) {
        const userItem = document.createElement("div");
        userItem.classList.add("item");
        userItem.innerHTML = `
            <div class="msg">
                <p>${userMessage}</p>
            </div>
        `;
        chatBoxContent.appendChild(userItem);

        //send ajax
        const formData = new FormData();
        formData.append("question", userMessage);
        const response = await fetch("/AiAssistant/Ask", {
            method: "POST",
            body: formData
        });

        const responseData = await response.text(); 

        const assistantItem = document.createElement("div");
        assistantItem.classList.add("item", "right");
        assistantItem.innerHTML = `
            <div class="msg">
                <p>${responseData}</p>
            </div>
        `;
        chatBoxContent.appendChild(assistantItem);

        chatBoxContent.scrollTop = chatBoxContent.scrollHeight;

        userMessageInput.value = "";
    }
}

userMessageInput.addEventListener("keydown", function (e) {
    if (e.key === "Enter") {
        sendMessage();
    }
});

sendMessageButton.addEventListener("click", sendMessage);

