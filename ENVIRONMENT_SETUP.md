# Environment Setup

This project uses environment variables for local development and future cloud deployment.

## Backend Environment Variables

These are used by the ASP.NET API and backend services.

### OPENAI_API_KEY

Used for OpenAI embeddings and RAG answers.

```powershell
$env:OPENAI_API_KEY="your_openai_api_key_here"

### NEWS_DATA_API_KEY

Used to fetch latest news from NewsData

'''Powershall
$env:NEWSDATA_API_KEY="your_newsdata_api_key_here"

### NEWS_DB_PATH

''' Powershall
$env:NEWS_DB_PATH="C:\Users\samiu\source\repos\News_App\news.db"

### FRONTEND_ORIGIN

'''Powershall
$env:FRONTEND_ORIGIN="http"//localhost:5173"


#### Frontend Environment Variables

these are used by the React/Vite frontend

Create a .env file inside the frontend folder,
Add: VITE_API_BASE_URL=http://localhost:5190

#### Local Run Instruction

Run backend API from the repo root:

$env:NEWS_DB_PATH="C:\Users\samiu\source\repos\News_App\news.db"
$env:FRONTEND_ORIGIN="http://localhost:5173"

dotnet run --project .\News_App.Api\News_App.Api.csproj

#### Run Frontend from repo root:

cd frontend
npm run dev

#### Health Check

http://localhost:5190/api/health

this service confirms if all the backend environment variables are configured or not

#### in order proceed with AWS deployment, these values should be configured in the AWS service environment settings.

#### the backend deployment will need:

OPENAI_API_KEY
NEWSDATA_API_KEY
NEWS_DB_PATH
FRONTEND_ORIGIN

#### the frontend deployment will need:

VITE_API_BASE_URL



#### IMPORTANT SECURITY NOTE:

Do not commit real API keys to GitHub.