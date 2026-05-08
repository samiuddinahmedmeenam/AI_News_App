import { useState } from "react";
import "./App.css";

function App() {
  const mockArticles = [
  {
    id: 1,
    title: "Netflix announces new May releases",
    source: "Entertainment Daily",
    description:
      "Netflix is adding several movies and TV shows in May, including new documentaries and original series.",
  },
  {
    id: 2,
    title: "Tech companies expand AI tools",
    source: "Tech World",
    description:
      "Major technology companies are releasing new AI tools for search, productivity, and customer support.",
  },
  {
    id: 3,
    title: "Local officials discuss transportation updates",
    source: "City News",
    description:
      "Officials announced plans to improve public transportation routes and reduce traffic congestion.",
  },
  {
    id: 4,
    title: "Global markets react to new economic data",
    source: "Finance Brief",
    description:
      "Investors are watching inflation, interest rates, and employment numbers as markets respond to fresh data.",
  },
  {
    id: 5,
    title: "New cybersecurity warning issued for users",
    source: "Security Watch",
    description:
      "Experts are warning users to update their devices and avoid suspicious links after a rise in phishing attacks.",
  },
];

  const [question, setQuestion] = useState("");
  const [answer, setAnswer] = useState("");

  function handleAsk() {
    if (question.trim() === "") {
      setAnswer("Please enter a question first.");
      return;
    }

    setAnswer(
      `Fake RAG answer for now: You asked "${question}". Later this will come from the C# backend.`
    );
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
          <div className="section-header">
            <h2>Latest News</h2>
              <span>{mockArticles.length} articles</span>
          </div>

  <div className="news-list scrollable-news">
            {mockArticles.map((article) => (
              <article className="news-card" key={article.id}>
                <div className="news-meta">
                  <span>#{article.id}</span>
                  <span>{article.source}</span>
                </div>
                <h3>{article.title}</h3>
                <p>{article.description}</p>
              </article>
            ))}
          </div>
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
        </section>
      </main>
    </div>
  );
}

export default App;