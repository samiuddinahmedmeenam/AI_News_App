import "./AskPanel.css";
import { EvidenceCard } from "./EvidenceCard";

export function AskPanel({ question, setQuestion, answer, evidence, onAsk }) {
  return (
    <>
      <h2>Ask the News Database</h2>
      <p className="helper-text">
        Ask a question like: "Any updates about Netflix?"
      </p>

      <textarea
        value={question}
        onChange={(event) => setQuestion(event.target.value)}
        placeholder="Type your question here..."
      />

      <button onClick={onAsk}>Ask</button>

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
              <EvidenceCard key={item.id} item={item} />
            ))}
          </div>
        </div>
      )}
    </>
  );
}
