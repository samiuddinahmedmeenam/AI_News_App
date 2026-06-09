import { useEffect, useRef, useState } from "react";
import "./App.css";
import { ArticleList } from "./components/ArticleList";
import { AskPanel } from "./components/AskPanel";
import { CalendarFilter } from "./components/CalendarFilter";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;
const REFRESH_STEP_DELAY_MS = 450;
const REFRESH_SUMMARY_DURATION_MS = 10000;
const REFRESH_STEPS = [
  "Calling provider",
  "Fetching news",
  "Parsing JSON",
  "Storing to database",
  "Loading news",
  "Thumbs up",
];

function waitForRefreshStep() {
  return new Promise((resolve) => {
    setTimeout(resolve, REFRESH_STEP_DELAY_MS);
  });
}

function App() {
  const [articles, setArticles] = useState([]);
  const [allArticles, setAllArticles] = useState([]);
  const [selectedArticle, setSelectedArticle] = useState(null);
  const [selectedDate, setSelectedDate] = useState(null);
  const [selectedProvider, setSelectedProvider] = useState(null);
  const [selectedGenre, setSelectedGenre] = useState(null);
  const [question, setQuestion] = useState("");
  const [answer, setAnswer] = useState("");
  const [evidence, setEvidence] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [refreshStatus, setRefreshStatus] = useState("");
  const [refreshProgress, setRefreshProgress] = useState(0);
  const [refreshResult, setRefreshResult] = useState(null);
  const [error, setError] = useState("");
  const refreshSummaryTimerRef = useRef(null);

  async function handleRefreshNews() {
    const showRefreshStep = async (status) => {
      const stepIndex = REFRESH_STEPS.indexOf(status);

      setRefreshStatus(status);
      setRefreshProgress(((stepIndex + 1) / REFRESH_STEPS.length) * 100);
      await waitForRefreshStep();
    };

    try {
      setRefreshing(true);
      setError("");
      setRefreshResult(null);

      if (refreshSummaryTimerRef.current) {
        clearTimeout(refreshSummaryTimerRef.current);
      }

      await showRefreshStep("Calling provider");

      const refreshResponse = await fetch(`${API_BASE_URL}/api/refresh-news`, {
        method: "POST",
      });

      if (!refreshResponse.ok) {
        throw new Error("Failed to refresh news.");
      }

      await showRefreshStep("Fetching news");

      const ingestionResult = await refreshResponse.json();
      console.log("Ingestion result:", ingestionResult);

      await showRefreshStep("Parsing JSON");
      await showRefreshStep("Storing to database");
      await showRefreshStep("Loading news");

      const newsResponse = await fetch(`${API_BASE_URL}/api/news`);

      if (!newsResponse.ok) {
        throw new Error("Failed to reload articles.");
      }

      const newsData = await newsResponse.json();
      const allArticlesData = newsData.articles || [];

      setAllArticles(allArticlesData);
      setArticles(allArticlesData);
      setSelectedArticle(null);
      setSelectedDate(null);
      setSelectedProvider(null);
      setSelectedGenre(null);

      await showRefreshStep("Thumbs up");

      setRefreshResult(ingestionResult);
      refreshSummaryTimerRef.current = setTimeout(() => {
        setRefreshResult(null);
      }, REFRESH_SUMMARY_DURATION_MS);
    } catch (err) {
      setError(err.message);
    } finally {
      setRefreshing(false);
      setRefreshStatus("");
      setRefreshProgress(0);
    }
  }

  useEffect(() => {
    return () => {
      if (refreshSummaryTimerRef.current) {
        clearTimeout(refreshSummaryTimerRef.current);
      }
    };
  }, []);

  useEffect(() => {
    async function loadArticles() {
      try {
        const response = await fetch(`${API_BASE_URL}/api/news`);

        if (!response.ok) {
          throw new Error("Failed to load articles.");
        }

        const data = await response.json();

        const allArticlesData = data.articles || [];
        setArticles(allArticlesData);
        setAllArticles(allArticlesData);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    loadArticles();
  }, []);

  const handleDateSelect = (dateKey, filteredArticles) => {
    setSelectedDate(dateKey);
    setSelectedProvider(null);
    setSelectedGenre(null);
    setArticles(filteredArticles);
    setSelectedArticle(null);
  };

  const handleClearDate = () => {
    setSelectedDate(null);
    setArticles(allArticles);
    setSelectedArticle(null);
  };

  const handleProviderSelect = (provider, filteredArticles) => {
    setSelectedProvider(provider);
    setSelectedDate(null);
    setSelectedGenre(null);
    setArticles(filteredArticles);
    setSelectedArticle(null);
  };

  const handleGenreSelect = (genre, filteredArticles) => {
    setSelectedGenre(genre);
    setSelectedDate(null);
    setSelectedProvider(null);
    setArticles(filteredArticles);
    setSelectedArticle(null);
  };

  async function handleAsk() {
    if (question.trim() === "") {
      setAnswer("Please enter a question first.");
      setEvidence([]);
      return;
    }

    try {
      setAnswer("Thinking...");
      setEvidence([]);

      const response = await fetch(`${API_BASE_URL}/api/ask`, {
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
        <aside className="left-column">
          <section className="card calendar-card">
            <CalendarFilter
              articles={allArticles}
              selectedDate={selectedDate}
              selectedProvider={selectedProvider}
              selectedGenre={selectedGenre}
              onDateSelect={handleDateSelect}
              onClearDate={handleClearDate}
              onProviderSelect={handleProviderSelect}
              onGenreSelect={handleGenreSelect}
            />
          </section>

          <section className="card articles-card">
            <ArticleList
              articles={articles}
              selectedArticle={selectedArticle}
              onSelectArticle={setSelectedArticle}
              onRefresh={handleRefreshNews}
              refreshing={refreshing}
              refreshStatus={refreshStatus}
              refreshProgress={refreshProgress}
              refreshResult={refreshResult}
              loading={loading}
              error={error}
            />
          </section>
        </aside>

        <section className="card ask-card">
          <AskPanel
            question={question}
            setQuestion={setQuestion}
            answer={answer}
            evidence={evidence}
            onAsk={handleAsk}
          />
        </section>
      </main>
    </div>
  );
}

export default App;
