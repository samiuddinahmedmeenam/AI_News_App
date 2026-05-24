import "./EvidenceCard.css";

export function EvidenceCard({ item }) {
  return (
    <div className="evidence-card">
      <div className="evidence-meta">
        <span>Chunk {item.chunkIndex}</span>
        <span>Evidence</span>
      </div>
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
  );
}
