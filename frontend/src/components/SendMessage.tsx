import { useState } from "react";
import axios from "axios";

const API_URL = "http://message-test-backend-1/api/messages/send";

function SendMessage() {
    const [text, setText] = useState("");

    const sendMessage = async () => {
        if (!text.trim()) return;

        const payload = {
            text,
            order_number: Date.now(),
            timestamp: new Date().toISOString(),
        };

        console.log("Отправка сообщения:", payload); 

        try {
            await axios.post(API_URL, payload, {
                headers: { "Content-Type": "application/json" },
            });
            console.log("Сообщение успешно отправлено:", payload);
        } catch (error) {
            console.error("Ошибка при отправке сообщения:", error);
        }

        setText("");
    };

    return (
        <div className="flex flex-col items-center justify-center min-h-screen bg-gray-900 text-white p-6">
            <div className="bg-gray-800 p-8 rounded-lg shadow-lg w-full max-w-md text-center">
                <h2 className="text-2xl font-bold mb-4">Send a Message</h2>
                <input
                    type="text"
                    value={text}
                    onChange={(e) => setText(e.target.value)}
                    maxLength={128}
                    className="w-full p-3 border border-gray-600 rounded bg-gray-700 text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
                    placeholder="Type your message..."
                />
                <button
                    onClick={sendMessage}
                    className="mt-4 w-full px-6 py-3 bg-blue-600 hover:bg-blue-700 rounded text-white font-bold transition"
                >
                    Send
                </button>
            </div>
        </div>
    );
}

export default SendMessage;
