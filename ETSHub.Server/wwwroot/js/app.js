// DOM elements
const statusBadge = document.getElementById('status');

// Build WebSocket URL from current host
const wsProtocol = location.protocol === 'https:' ? 'wss:' : 'ws:';
const wsUrl = `${wsProtocol}//${location.host}/ws`;

let socket = null;

// Connect to server WebSocket endpoint
function connectWebSocket() {
    socket = new WebSocket(wsUrl);

    socket.onopen = () => {
        console.log('✅ WebSocket connected');
        statusBadge.textContent = '● Connected';
        statusBadge.className = 'status-badge connected';
    };

    socket.onclose = () => {
        console.warn('⚠️ Connection lost. Reconnecting...');
        statusBadge.textContent = '● Disconnected';
        statusBadge.className = 'status-badge disconnected';

        // Auto-reconnect after 1.5 seconds
        setTimeout(connectWebSocket, 1500);
    };

    socket.onerror = (error) => {
        console.error('WebSocket error:', error);
        socket.close();
    };
}

// Send button event packet to C# server
function sendButtonEvent(buttonId, action) {
    if (socket && socket.readyState === WebSocket.OPEN) {
        const payload = JSON.stringify({
            button: parseInt(buttonId, 10),
            action: action // "down" or "up"
        });
        socket.send(payload);
    }
}

// Bind touch and pointer listeners for all buttons
document.querySelectorAll('.btn[data-button]').forEach(button => {
    const buttonId = button.getAttribute('data-button');

    // Button pressed (touch down / mouse down)
    const handleDown = (event) => {
        event.preventDefault(); // Prevent iOS zoom and gesture conflicts
        button.classList.add('pressed');
        sendButtonEvent(buttonId, 'down');
    };

    // Button released (touch up / touch cancel)
    const handleUp = (event) => {
        event.preventDefault();
        button.classList.remove('pressed');
        sendButtonEvent(buttonId, 'up');
    };

    // Attach Pointer Events for lowest touch latency
    button.addEventListener('pointerdown', handleDown);
    button.addEventListener('pointerup', handleUp);
    button.addEventListener('pointercancel', handleUp);
});

// Initialize connection on page load
connectWebSocket();
