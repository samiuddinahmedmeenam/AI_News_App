import "./ArticleDetail.css";

export function ArticleDetail({ article, onBack }) {
  return (
    <div className="article-detail">
      <button className="back-button" onClick={onBack}>
        ←
      </button>

      <div className="detail-meta">
        <span>{article.category || "News"}</span>
        <span>{article.sourceName || "Unknown Source"}</span>
      </div>

      <h2>{article.title}</h2>

      <p className="detail-description">
        {article.description || "No description available."}
      </p>

      {article.url && (
        <a
          href={article.url}
          target="_blank"
          rel="noreferrer"
          className="article-link"
        >
          Open original article
        </a>
      )}
    </div>
  );
}
