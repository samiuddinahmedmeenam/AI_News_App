import "./ArticleDetail.css";

function getArticleImageUrl(article) {
  const imageUrl = article.imageUrl || article.imageurl || "";

  if (!imageUrl || imageUrl.toLowerCase().includes("no image")) {
    return "";
  }

  return imageUrl;
}

export function ArticleDetail({ article, onBack }) {
  const imageUrl = getArticleImageUrl(article);

  return (
    <div className="article-detail">
      <button className="back-button" onClick={onBack}>
        &larr;
      </button>

      <div className="detail-meta">
        <span>{article.category || "News"}</span>
        <span>{article.sourceName || "Unknown Source"}</span>
      </div>

      {imageUrl && (
        <div className="detail-image-frame">
          <img src={imageUrl} alt={article.title || "Article image"} className="detail-image" />
        </div>
      )}

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
