import "./ArticleList.css";
import { ArticleCard } from "./ArticleCard";
import { ArticleDetail } from "./ArticleDetail";

export function ArticleList({
  articles,
  selectedArticle,
  onSelectArticle,
  onRefresh,
  refreshing,
  refreshStatus,
  refreshProgress,
  refreshResult,
  loading,
  error,
}) {
  if (loading) {
    return <p className="helper-text">Loading articles...</p>;
  }

  if (error) {
    return <p className="helper-text">Error: {error}</p>;
  }

  if (selectedArticle) {
    return (
      <ArticleDetail
        article={selectedArticle}
        onBack={() => onSelectArticle(null)}
      />
    );
  }

  return (
    <div className="article-list-wrapper">
      <div className="section-header">
        <div>
          <h2>Latest News</h2>
          <span>{articles.length} articles</span>
        </div>

        {refreshResult && !refreshing ? (
          <div className="refresh-summary">
            <p>Refresh complete</p>
            <span>Fetched: {refreshResult.fetchedArticles ?? 0}</span>
            <span>Saved: {refreshResult.savedArticles ?? 0}</span>
            <span>Skipped: {refreshResult.skippedArticles ?? 0}</span>
            <span>Total: {refreshResult.totalArticles ?? articles.length}</span>
            <span>New embeddings: {refreshResult.newEmbeddings ?? 0}</span>
          </div>
        ) : (
          <button
            className={`refresh-button ${refreshing ? "is-refreshing" : ""}`}
            onClick={onRefresh}
            disabled={refreshing}
            style={{ "--refresh-progress": `${refreshProgress || 0}%` }}
          >
            {refreshing ? (
              <span className="refresh-status">
                {refreshStatus === "Thumbs up" ? (
                  <span className="refresh-complete" aria-hidden="true">
                    &#128077;
                  </span>
                ) : (
                  <span className="refresh-spinner" aria-hidden="true"></span>
                )}
                <span>{refreshStatus || "Refreshing"}</span>
              </span>
            ) : (
              "Refresh News"
            )}
          </button>
        )}
      </div>

      <div className="news-list scrollable-news">
        {articles.map((article, index) => (
          <ArticleCard
            key={article.url || index}
            article={article}
            index={index}
            onSelect={onSelectArticle}
          />
        ))}
      </div>
    </div>
  );
}
