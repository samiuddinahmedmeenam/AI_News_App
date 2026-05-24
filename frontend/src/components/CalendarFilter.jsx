import "./CalendarFilter.css";
import { useMemo } from "react";

export function CalendarFilter({ articles, onDateSelect, onClearDate, selectedDate }) {
  // Group articles by date into a hashmap
  const articlesByDate = useMemo(() => {
    const grouped = {};
    
    articles.forEach((article) => {
      const dateStr = article.publishedAt 
        ? new Date(article.publishedAt).toISOString().split('T')[0]
        : "unknown";
      
      if (!grouped[dateStr]) {
        grouped[dateStr] = [];
      }
      grouped[dateStr].push(article);
    });
    
    return grouped;
  }, [articles]);

  // Sort dates in descending order (newest first)
  const sortedDates = Object.keys(articlesByDate).sort().reverse();

  return (
    <div className="calendar-filter">
      <div className="filter-header">
        <h3>Filter by Date</h3>
        {selectedDate && (
          <button className="clear-filter-btn" onClick={onClearDate}>
            Clear
          </button>
        )}
      </div>

      <div className="date-list">
        {sortedDates.map((dateKey) => {
          const count = articlesByDate[dateKey].length;
          const isActive = selectedDate === dateKey;

          return (
            <button
              key={dateKey}
              className={`date-button ${isActive ? "active" : ""}`}
              onClick={() => onDateSelect(dateKey, articlesByDate[dateKey])}
            >
              <span className="date-label">{dateKey}</span>
              <span className="article-count">{count}</span>
            </button>
          );
        })}
      </div>
    </div>
  );
}