# WorkTimeTracker App
This app includes:
- Backend ASP.NET Server for administrating the working sessions
- Console app which communicates with the Server via API's

## 🔗 Endpoints API

| Metodă | Endpoint                  | Descriere                       |
|--------|---------------------------|---------------------------------|
| POST   | `/api/WorkSession/start`  | Starts a new working session    |
| POST   | `/api/WorkSession/stop`   | Stops the started session       |
| POST   | `/api/WorkSession/pause`  | Pauses the session              |
| POST   | `/api/WorkSession/resume` | Resumes the session             |

The request body sends only the username(ex. `"john"`)

## ▶️ How to run the app

1. Clone repository:
    - git clone https://github.com/mihai1702/WorkTimeTracker.git
    - cd repo
2. Run the backend 
    - make sure that the 5108 port is free
    - Run the file start-backend.bat
3. Run the console app
    - Run start-frontend.bat
