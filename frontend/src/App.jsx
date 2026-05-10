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

  async function handleRefreshNews() {
  try {
    setLoading(true);
    setError("");

    const response = await fetch("http://localhost:5190/api/refresh-news", {
      method: "POST",
    });

    if (!response.ok) {
      throw new Error("Failed to refresh news.");
    }

    const result = await response.json();
    console.log("Refresh result:", result);

    // Reload articles after refresh
    const newsResponse = await fetch("http://localhost:5190/api/news");

    if (!newsResponse.ok) {
      throw new Error("Failed to reload articles.");
    }

    const newsData = await newsResponse.json();
    setArticles(newsData.articles || []);
    setSelectedArticle(null);
  } catch (err) {
    setError(err.message);
  } finally {
    setLoading(false);
  }
}

  function shortenText(text, maxLength = 160) {
  if (!text) {
    return "No description available.";
  }

  if (text.length <= maxLength) {
    return text;
  }

  return text.substring(0, maxLength) + "...";
  }

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

  

  
  

  async function handleAsk() {
  if (question.trim() === "") {
    setAnswer("Please enter a question first.");
    setEvidence([]);
    return;
  }

  try {
    setAnswer("Thinking...");
    setEvidence([]);

    const response = await fetch("http://localhost:5190/api/ask", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        question: question,
      }),
    });

    if (!response.ok) {
      throw new Error("Failed to get RAG answer.");
    }

    const data = await response.json();

    setAnswer(data.answer || "No answer returned.");
    setEvidence(data.evidence || []);
  } catch (err) {
    setAnswer(`Error: ${err.message}`);
    setEvidence([]);
  }
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
                <div>
                  <h2>Latest News</h2>
                  <span>{articles.length} articles</span>
                </div>

                <button className="refresh-button" onClick={handleRefreshNews}>
                  Refresh News
                </button>
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
                    <p>{shortenText(article.description, 160)}</p>
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
                      <span>Chunk {item.chunkIndex}</span>
                        <span>Evidence</span>
                        <p>{item.chunkText}</p>

                        {item.articleUrl && (
                          <a
                            href={item.articleUrl}
                            target="_blank"
                            rel="noreferrer"
                            className="evidence-link"
                          >
                            Open source
                          </a>
                        )}
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