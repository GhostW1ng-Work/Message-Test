import { useEffect, useState } from "react";
import axios from "axios";

const API_URL = "http://message-test-backend-1/api/messages/history";

function MessageHistory() {
    const [messages, setMessages] = useState<{ text: string; order: number; timestamp: number }[]>([]);

    useEffect(() => {
        const fetchMessages = async () => {
            const tenMinutesAgo = new Date(Date.now() - 10 * 60 * 1000).toISOString();
            const now = new Date().toISOString();
            console.log(`Запрос на получение сообщений с ${tenMinutesAgo} до ${now}`);
            try {
                const response = await axios.get(`${API_URL}?from=${tenMinutesAgo}&to=${now}`);
                console.log(`Получены ${response.data.length} сообщений`);
                setMessages(response.data);
            } catch (error) {
                console.error("Ошибка при получении сообщений:", error);
            }
        };

        fetchMessages();
        const interval = setInterval(() => {
            console.log("Обновление данных...");
            fetchMessages();
        }, 5000); 

        return () => {
            clearInterval(interval);
            console.log("Интервал обновления данных был очищен");
        };
    }, []);

    return (
        <div className="flex flex-col items-center justify-center min-h-screen bg-gray-900 text-white p-6">
            <div className="bg-gray-800 p-8 rounded-lg shadow-lg w-full max-w-md text-center">
                <h2 className="text-2xl font-bold mb-4">Message History</h2>
                <ul className="w-full bg-gray-700 p-4 rounded shadow-lg overflow-auto max-h-96 text-left">
                    {messages.length === 0 ? (
                        <p className="text-gray-400">No messages in the last 10 minutes...</p>
                    ) : (
                        messages.map((msg, index) => (
                            <li key={index} className="border-b border-gray-600 py-2">
                                <span className="font-bold text-yellow-400">#{msg.order}</span> -
                                [{new Date(msg.timestamp).toLocaleTimeString()}] {msg.text}
                            </li>
                        ))
                    )}
                </ul>
            </div>
        </div>
    );
}

export default MessageHistory;
