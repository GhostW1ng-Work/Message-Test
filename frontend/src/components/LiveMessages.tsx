import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";

const HUB_URL = "http://backend/messageHub";

function LiveMessages() {
    const [messages, setMessages] = useState<{ text: string; order: number; timestamp: number }[]>([]);

    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(HUB_URL, {
                skipNegotiation: true,
                transport: signalR.HttpTransportType.WebSockets,
            })
            .withAutomaticReconnect()
            .build();

        let isMounted = true;

        async function startConnection() {
            try {
                await connection.start();
                console.log("SignalR connected");

                connection.on("ReceiveMessage", (text, order, timestamp) => {
                    if (isMounted) {
                        setMessages((prev) => [...prev, { text, order, timestamp }]);
                        console.log(`Получено сообщение: #${order} - ${text} [${new Date(timestamp).toLocaleTimeString()}]`);
                    }
                });
            } catch (error) {
                console.error("Ошибка при подключении SignalR:", error);
            }
        }

        startConnection();

        return () => {
            isMounted = false;
            connection.stop().then(() => {
                console.log("SignalR connection stopped");
            }).catch(error => {
                console.error("Ошибка при остановке соединения SignalR:", error);
            });
        };
    }, []);

    return (
        <div className="flex flex-col items-center justify-center min-h-screen bg-gray-900 text-white p-8">
            <div className="bg-gray-800 p-10 rounded-2xl shadow-xl w-full max-w-2xl text-center">
                <h2 className="text-3xl font-bold mb-6 text-blue-400">📡 Live Messages</h2>
                <ul className="w-full bg-gray-700 p-6 rounded-xl shadow-md overflow-auto max-h-[500px] text-left text-lg">
                    {messages.length === 0 ? (
                        <p className="text-gray-400 text-xl">No messages yet...</p>
                    ) : (
                        messages.map((msg, index) => (
                            <li key={index} className="border-b border-gray-600 py-3">
                                <span className="text-blue-300 font-semibold">#{msg.order + " - "}
                                    [{new Date(msg.timestamp).toLocaleTimeString()}]
                                </span>{" "}
                                {msg.text}
                            </li>
                        ))
                    )}
                </ul>
            </div>
        </div>
    );
}

export default LiveMessages;
