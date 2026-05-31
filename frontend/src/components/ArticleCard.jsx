import "./ArticleCard.css";

function shortenText(text, maxLength = 160) {
  if (!text) {
    return "No description available.";
  }

  if (text.length <= maxLength) {
    return text;
  }

  return text.substring(0, maxLength) + "...";
}

function getArticleImageUrl(article) {
  const imageUrl = article.imageUrl || article.imageurl || "";

  if (!imageUrl || imageUrl.toLowerCase().includes("no image")) {
    return "";
  }

  return imageUrl;
}

export function ArticleCard({ article, index, onSelect }) {
  const imageUrl = getArticleImageUrl(article);

  return (
    <article
      className={`news-card ${imageUrl ? "has-image" : ""}`}
      key={article.url || index}
      onClick={() => onSelect(article)}
      style={imageUrl ? { backgroundImage: `url("${imageUrl}")` } : undefined}
    >
      <div className="news-meta">
        <span>#{index + 1}</span>
        <span>{article.sourceName || "Unknown Source"}</span>
      </div>
      <div className="news-card-content">
        <h3>{article.title || "No title available"}</h3>
        <p>{shortenText(article.description, 160)}</p>
      </div>
    </article>
  );
}
