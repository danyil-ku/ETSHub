// Находим статусную плашку
const statusBadge = document.getElementById('status');

// Автоматически формируем WebSocket URL на основе текущего хоста страницы
const wsProtocol = location.protocol === 'https:' ? 'wss:' : 'ws:';
const wsUrl = `${wsProtocol}//${location.host}/ws`;

let socket = null;

// Функция подключения к WebSocket
function connectWebSocket() {
    socket = new WebSocket(wsUrl);

    socket.onopen = () => {
        console.log('✅ WebSocket подключен');
        statusBadge.textContent = '● Подключено';
        statusBadge.className = 'status-badge connected';
    };

    socket.onclose = () => {
        console.warn('⚠️ Связь с сервером потеряна. Переподключение...');
        statusBadge.textContent = '● Отключено';
        statusBadge.className = 'status-badge disconnected';

        // Автоматическое переподключение через 1.5 секунды
        setTimeout(connectWebSocket, 1500);
    };

    socket.onerror = (error) => {
        console.error('Ошибка WebSocket:', error);
        socket.close();
    };
}

// Отправка события нажатия кнопки на C#-сервер
function sendButtonEvent(buttonId, action) {
    if (socket && socket.readyState === WebSocket.OPEN) {
        const payload = JSON.stringify({
            button: parseInt(buttonId, 10),
            action: action // "down" или "up"
        });
        socket.send(payload);
    }
}

// Навешиваем слушатели касаний на все кнопки с атрибутом data-button
document.querySelectorAll('.btn[data-button]').forEach(button => {
    const buttonId = button.getAttribute('data-button');

    // Нажатие на кнопку (пальцем или мышкой)
    const handleDown = (event) => {
        event.preventDefault(); // Отключаем системный зум и жесты iOS
        button.classList.add('pressed');
        sendButtonEvent(buttonId, 'down');
    };

    // Отпускание кнопки
    const handleUp = (event) => {
        event.preventDefault();
        button.classList.remove('pressed');
        sendButtonEvent(buttonId, 'up');
    };

    // Используем Pointer Events (минимальная задержка на сенсорных экранах)
    button.addEventListener('pointerdown', handleDown);
    button.addEventListener('pointerup', handleUp);
    button.addEventListener('pointercancel', handleUp);
});

// Запускаем подключение при загрузке страницы
connectWebSocket();
