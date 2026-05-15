# AWS Deployment Checklist

This checklist explains the steps and configuration needed to deploy the AI News RAG App to AWS.

## Current Local Architecture

```text
React Frontend
  ↓
ASP.NET Core API
  ↓
News_App.Core backend logic
  ↓
SQLite database
  ↓
OpenAI API + NewsData API


#### Target AWS Architecture

React Frontend
  ↓
AWS Amplify Hosting



ASP.NET Core API
  ↓
AWS App Runner



Database
  ↓
SQLite for early testing
  ↓
Later: Amazon RDS PostgreSQL



Secrets / Config
  ↓
AWS environment variables
  ↓
Later: AWS Secrets Manager



1. Frontend Deployment - AWS Amplify
Frontend Project Folder
frontend
Local Build Test

Run this before deploying:

cd frontend
npm run build

Expected result:

built successfully
Amplify Build Settings

Build command:

npm run build

Output folder:

dist
Frontend Environment Variables

Set this in Amplify:

VITE_API_BASE_URL=https://your-backend-api-url

For local development:

VITE_API_BASE_URL=http://localhost:5190
Important Frontend Notes
Do not store secret API keys in frontend environment variables.
Only public configuration values should use VITE_.
API keys like OpenAI and NewsData must stay in the backend.
2. Backend Deployment - AWS App Runner
Backend Project
News_App.Api
Local Publish Test

Run this before deploying:

dotnet publish .\News_App.Api\News_App.Api.csproj -c Release

Expected result:

publish succeeded
Required Backend Environment Variables

Set these in App Runner:

OPENAI_API_KEY
NEWSDATA_API_KEY
NEWS_DB_PATH
FRONTEND_ORIGIN
Example Local Values
OPENAI_API_KEY=your_openai_api_key_here
NEWSDATA_API_KEY=your_newsdata_api_key_here
NEWS_DB_PATH=C:\Users\samiu\source\repos\News_App\news.db
FRONTEND_ORIGIN=http://localhost:5173
Example AWS Values
OPENAI_API_KEY=your_openai_api_key_here
NEWSDATA_API_KEY=your_newsdata_api_key_here
NEWS_DB_PATH=/app/data/news.db
FRONTEND_ORIGIN=https://your-amplify-frontend-url
Important Backend Notes
FRONTEND_ORIGIN must match the deployed Amplify frontend URL.
NEWS_DB_PATH must point to a valid database path.
For real production use, SQLite should eventually be replaced with PostgreSQL/RDS.
Sensitive values should later move to AWS Secrets Manager or SSM Parameter Store.
3. API Endpoints To Test

After backend deployment, test these endpoints.

Health Check
GET /api/health

Expected result:

{
  "status": "OK",
  "databaseConfigured": true,
  "newsDataKeyConfigured": true,
  "openAiKeyConfigured": true,
  "frontendOrigin": "..."
}
Test Endpoint
GET /api/test

Expected result:

{
  "message": "API is working"
}
News Endpoint
GET /api/news

Expected result:

{
  "message": "Articles loaded successfully.",
  "count": 5,
  "articles": []
}
RAG Ask Endpoint
POST /api/ask

Example body:

{
  "question": "Any updates about Netflix?"
}

Expected result:

{
  "answer": "...",
  "evidence": []
}
Refresh News Endpoint
POST /api/refresh-news

Expected result:

{
  "message": "News refresh complete.",
  "fetchedArticles": 5,
  "totalChunks": 20,
  "newEmbeddings": 5,
  "skippedEmbeddings": 15
}
4. Deployment Order

Recommended order:

Step 1: Prepare Backend
Confirm backend builds.
Confirm backend publish works.
Confirm environment variables are not hardcoded.
Confirm /api/health works locally.
Step 2: Deploy Backend
Deploy News_App.Api to AWS App Runner.
Set backend environment variables.
Test /api/health.
Test /api/news.
Step 3: Deploy Frontend
Deploy frontend to AWS Amplify.
Set VITE_API_BASE_URL to the App Runner backend URL.
Build and deploy frontend.
Step 4: Update Backend CORS
Copy the Amplify frontend URL.
Set backend FRONTEND_ORIGIN to that URL.
Restart/redeploy backend if needed.
Step 5: Final Full Test

Test the deployed app:

frontend loads
articles show
article detail opens
Ask/RAG works
evidence shows
Refresh News works
5. Future Database Plan

Current:

SQLite

Future:

Amazon RDS PostgreSQL

Possible later upgrades:

PostgreSQL database
pgvector for embeddings
managed backups
production connection string
database migrations
6. Security Notes

Do not commit real secrets to GitHub.

Do not put these in frontend code:

OPENAI_API_KEY
NEWSDATA_API_KEY

Keep secrets in backend environment variables.

For production, protect sensitive endpoints like:

POST /api/refresh-news

Possible protections:

admin login
API key
rate limiting
authentication middleware

Then commit it:

```powershell
git add .
git commit -m "Add AWS deployment checklist"
git push