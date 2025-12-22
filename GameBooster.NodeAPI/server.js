const express = require('express');
const cors = require('cors');
const app = express();
const port = 3001;

app.use(cors());

// Tıklanma hissi yaratmayan, sadece "Gündem" maddeleri:
const trends = [
    { 
        id: 1, 
        name: "NVIDIA RTX 5090", 
        status: "🔥 Rekor Kıran", 
        searchCount: "1.2M" 
    },
    { 
        id: 2, 
        name: "Black Myth: Wukong", 
        status: "📈 En Popüler Oyun", 
        searchCount: "850K" 
    },
    { 
        id: 3, 
        name: "Intel Core Ultra 9", 
        status: "🆕 Yeni Çıktı", 
        searchCount: "420K" 
    },
    { 
        id: 4, 
        name: "GTA VI", 
        status: "👀 Herkes Bekliyor", 
        searchCount: "50M+" 
    },
    { 
        id: 5, 
        name: "Steam Deck 2",   // "FPS Yöntemleri" gitti, donanım geldi.
        status: "🗣️ Söylentiler", 
        searchCount: "300K" 
    }
];

app.get('/api/trends', (req, res) => {
    res.json(trends);
});

app.listen(port, () => {
    console.log(`🚀 GameBooster Trend Servisi çalışıyor: http://localhost:${port}`);
});