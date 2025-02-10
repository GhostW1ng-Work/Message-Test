import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";
import SendMessage from "./components/SendMessage";
import LiveMessages from "./components/LiveMessages";
import MessageHistory from "./components/MessageHistory";

function App() {
    return (
        <Router>
            <div className="min-h-screen bg-gray-900 text-white flex flex-col items-center p-6">
                <nav className="bg-gray-800 p-6 rounded-2xl shadow-lg mt-6 w-full max-w-2xl flex justify-around text-xl">
                    <Link to="/send" className="px-6 py-3 bg-blue-600 hover:bg-blue-700 rounded-xl text-white font-bold transition">
                        ✉️ Send
                    </Link>
                    <Link to="/live" className="px-6 py-3 bg-green-600 hover:bg-green-700 rounded-xl text-white font-bold transition">
                        📡 Live
                    </Link>
                    <Link to="/history" className="px-6 py-3 bg-purple-600 hover:bg-purple-700 rounded-xl text-white font-bold transition">
                        📜 History
                    </Link>
                </nav>
                <div className="p-6 w-full max-w-2xl">
                    <Routes>
                        <Route path="/" element={<SendMessage />} />
                        <Route path="/send" element={<SendMessage />} />
                        <Route path="/live" element={<LiveMessages />} />
                        <Route path="/history" element={<MessageHistory />} />
                    </Routes>
                </div>
            </div>
        </Router>
    );
}

export default App;
