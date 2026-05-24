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

export function ArticleCard({ article, index, onSelect }) {
  return (
    <article
      className="news-card"
      key={article.url || index}
      onClick={() => onSelect(article)}
    >
      <div className="news-meta">
        <span>#{index + 1}</span>
        <span>{article.sourceName || "Unknown Source"}</span>
      </div>
      <h3>{article.title || "No title available"}</h3>
      <p>{shortenText(article.description, 160)}</p>
    </article>
  );
}
