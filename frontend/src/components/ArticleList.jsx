import "./ArticleList.css";
import { ArticleCard } from "./ArticleCard";
import { ArticleDetail } from "./ArticleDetail";

export function ArticleList({
  articles,
  selectedArticle,
  onSelectArticle,
  onRefresh,
  refreshing,
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

        <button
          className="refresh-button"
          onClick={onRefresh}
          disabled={refreshing}
        >
          {refreshing ? "Loading..." : "Refresh News"}
        </button>
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
