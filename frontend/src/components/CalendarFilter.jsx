import "./CalendarFilter.css";
import { useState, useMemo } from "react";

export function CalendarFilter({ articles, onDateSelect, onClearDate, selectedDate }) {
  const [currentMonth, setCurrentMonth] = useState(new Date());

  // Group articles by date
  const articlesByDate = useMemo(() => {
    const grouped = {};
    articles.forEach((article) => {
      const dateStr = article.publishedAt
        ? new Date(article.publishedAt).toISOString().split("T")[0]
        : "unknown";
      if (!grouped[dateStr]) {
        grouped[dateStr] = [];
      }
      grouped[dateStr].push(article);
    });
    return grouped;
  }, [articles]);

  // Get all dates that have articles
  const datesWithArticles = new Set(Object.keys(articlesByDate));

  // Calendar calculations
  const year = currentMonth.getFullYear();
  const month = currentMonth.getMonth();
  const firstDay = new Date(year, month, 1);
  const lastDay = new Date(year, month + 1, 0);
  const startDate = new Date(firstDay);
  startDate.setDate(startDate.getDate() - firstDay.getDay());

  const days = [];
  const current = new Date(startDate);
  while (current <= lastDay || current.getDay() !== 0) {
    days.push(new Date(current));
    current.setDate(current.getDate() + 1);
  }

  const goToPreviousMonth = () => {
    setCurrentMonth(new Date(year, month - 1));
  };

  const goToNextMonth = () => {
    setCurrentMonth(new Date(year, month + 1));
  };

  const handleDateClick = (date) => {
    const dateStr = date.toISOString().split("T")[0];
    if (articlesByDate[dateStr]) {
      onDateSelect(dateStr, articlesByDate[dateStr]);
    }
  };

  const monthName = currentMonth.toLocaleString("default", {
    month: "long",
    year: "numeric",
  });

  const isToday = (date) => {
    const today = new Date();
    return (
      date.getDate() === today.getDate() &&
      date.getMonth() === today.getMonth() &&
      date.getFullYear() === today.getFullYear()
    );
  };

  const isCurrentMonth = (date) => date.getMonth() === month;

  return (
    <div className="calendar-filter">
      <div className="calendar-header">
        <button className="nav-btn" onClick={goToPreviousMonth}>
          ◀
        </button>
        <h3>{monthName}</h3>
        <button className="nav-btn" onClick={goToNextMonth}>
          ▶
        </button>
      </div>

      <div className="calendar-grid">
        {/* Day labels */}
        <div className="day-labels">
          {["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"].map((day) => (
            <div key={day} className="day-label">
              {day}
            </div>
          ))}
        </div>

        {/* Calendar days */}
        <div className="days-grid">
          {days.map((date, idx) => {
            const dateStr = date.toISOString().split("T")[0];
            const hasArticles = datesWithArticles.has(dateStr);
            const isSelected = selectedDate === dateStr;
            const isCurrent = isCurrentMonth(date);
            const isCurrentDay = isToday(date);

            return (
              <button
                key={idx}
                className={`calendar-day ${!isCurrent ? "other-month" : ""} ${
                  isSelected ? "selected" : ""
                } ${isCurrentDay ? "today" : ""} ${hasArticles ? "has-articles" : ""}`}
                onClick={() => hasArticles && handleDateClick(date)}
                disabled={!hasArticles}
              >
                <span className="day-number">{date.getDate()}</span>
                {hasArticles && <span className="article-dot"></span>}
              </button>
            );
          })}
        </div>
      </div>

      {selectedDate && (
        <button className="clear-filter-btn" onClick={onClearDate}>
          Clear Filter
        </button>
      )}
    </div>
  );
}