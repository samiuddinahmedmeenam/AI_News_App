import { useEffect, useState } from "react";
import "./App.css";

function App() {

  const [articles, setArticles] = useState([]);
  const [selectedArticle, setSelectedArticle] = useState(null);
  const [question, setQuestion] = useState("");
  const [answer, setAnswer] = useState("");
  const [evidence, setEvidence] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
  async function loadArticles() {
    try {
      const response = await fetch("http://localhost:5190/api/news");

      if (!response.ok) {
        throw new Error("Failed to load articles.");
      }

      const data = await response.json();

      setArticles(data.articles || []);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  loadArticles();
}, []);

  

  
  

  function handleAsk() {
  if (question.trim() === "") {
    setAnswer("Please enter a question first.");
    setEvidence([]);
    return;
  }

  setAnswer(
    `Fake RAG answer for now: Based on the retrieved news, your question "${question}" would be answered here by the C# backend.`
  );

  setEvidence([
    {
      id: 1,
      source: "Entertainment Daily",
      score: 0.87,
      text: "Netflix is adding several movies and TV shows in May, including new documentaries and original series.",
    },
    {
      id: 2,
      source: "Tech World",
      score: 0.74,
      text: "Major technology companies are releasing new AI tools for search, productivity, and customer support.",
    },
    {
      id: 3,
      source: "City News",
      score: 0.61,
      text: "Officials announced plans to improve public transportation routes and reduce traffic congestion.",
    },
  ]);
}

  return (
    <div className="app">
      <header className="hero">
        <p className="eyebrow">Semantic News Search</p>
        <h1>AI News RAG App</h1>
        <p className="subtitle">
          Search saved news articles, retrieve relevant chunks, and get grounded AI answers.
        </p>
      </header>

      <main className="layout">
        <section className="card news-panel">
          {loading ? (
            <p className="helper-text">Loading articles...</p>
          ) : error ? (
            <p className="helper-text">Error: {error}</p>
          ) : selectedArticle ? (
            <div className="article-detail">
              <button
                className="back-button"
                onClick={() => setSelectedArticle(null)}
              >
                ←
              </button>

              <div className="detail-meta">
                <span>{selectedArticle.category || "News"}</span>
                <span>{selectedArticle.sourceName || "Unknown Source"}</span>
              </div>

              <h2>{selectedArticle.title}</h2>

              <p className="detail-description">
                {selectedArticle.description || "No description available."}
              </p>

              {selectedArticle.url && (
                <a
                  href={selectedArticle.url}
                  target="_blank"
                  rel="noreferrer"
                  className="article-link"
                >
                  Open original article
                </a>
              )}
            </div>
          ) : (
            <>
              <div className="section-header">
                <h2>Latest News</h2>
                <span>{articles.length} articles</span>
              </div>

              <div className="news-list scrollable-news">
                {articles.map((article, index) => (
                  <article
                    className="news-card"
                    key={article.url || index}
                    onClick={() => setSelectedArticle(article)}
                  >
                    <div className="news-meta">
                      <span>#{index + 1}</span>
                      <span>{article.sourceName || "Unknown Source"}</span>
                    </div>
                    <h3>{article.title || "No title available"}</h3>
                    <p>{article.description || "No description available."}</p>
                  </article>
                ))}
              </div>
            </>
          )}
        </section>

        <section className="card">
          <h2>Ask the News Database</h2>
          <p className="helper-text">
            Ask a question like: “Any updates about Netflix?”
          </p>

          <textarea
            value={question}
            onChange={(event) => setQuestion(event.target.value)}
            placeholder="Type your question here..."
          />

          <button onClick={handleAsk}>Ask</button>

          {answer && (
            <div className="answer-box">
              <h3>RAG Answer</h3>
              <p>{answer}</p>
            </div>
          )}

          {evidence.length > 0 && (
            <div className="evidence-section">
              <h3>Retrieved Evidence</h3>

              <div className="evidence-list">
                {evidence.map((item) => (
                  <div className="evidence-card" key={item.id}>
                    <div className="evidence-meta">
                      <span>{item.source}</span>
                      <span>Score: {item.score}</span>
                    </div>
                    <p>{item.text}</p>
                  </div>
                ))}
              </div>
            </div>
          )}
        </section>
      </main>
    </div>
  );
}

export default App;