import "./CalendarFilter.css";
import { useMemo, useState } from "react";

export function CalendarFilter({
  articles,
  onDateSelect,
  onClearDate,
  onProviderSelect,
  onGenreSelect,
  selectedDate,
  selectedProvider,
  selectedGenre,
}) {
  const [currentMonth, setCurrentMonth] = useState(new Date());
  const [isOpen, setIsOpen] = useState(false);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [isProviderListOpen, setIsProviderListOpen] = useState(false);
  const [isGenreListOpen, setIsGenreListOpen] = useState(false);

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

  const articlesByProvider = useMemo(() => {
    const grouped = {};

    articles.forEach((article) => {
      const provider = article.sourceName || "Unknown Source";

      if (!grouped[provider]) {
        grouped[provider] = [];
      }

      grouped[provider].push(article);
    });

    return grouped;
  }, [articles]);

  const articlesByGenre = useMemo(() => {
    const grouped = {};

    articles.forEach((article) => {
      const genre = article.category || "Uncategorized";

      if (!grouped[genre]) {
        grouped[genre] = [];
      }

      grouped[genre].push(article);
    });

    return grouped;
  }, [articles]);

  const datesWithArticles = new Set(Object.keys(articlesByDate));
  const providers = Object.keys(articlesByProvider).sort((first, second) =>
    first.localeCompare(second),
  );
  const genres = Object.keys(articlesByGenre).sort((first, second) =>
    first.localeCompare(second),
  );
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

  const toggleCalendar = () => {
    setIsOpen((open) => !open);
  };

  const toggleFilter = () => {
    setIsFilterOpen((open) => {
      if (open) {
        setIsProviderListOpen(false);
        setIsGenreListOpen(false);
      }

      return !open;
    });
  };

  const toggleProviderList = () => {
    setIsProviderListOpen((open) => !open);
    setIsGenreListOpen(false);
  };

  const toggleGenreList = () => {
    setIsGenreListOpen((open) => !open);
    setIsProviderListOpen(false);
  };

  const handleDateClick = (date) => {
    const dateStr = date.toISOString().split("T")[0];

    if (articlesByDate[dateStr]) {
      onDateSelect(dateStr, articlesByDate[dateStr]);
      setIsOpen(false);
    }
  };

  const handleClearDate = () => {
    onClearDate();
    setIsOpen(false);
  };

  const handleProviderClick = (provider) => {
    onProviderSelect(provider, articlesByProvider[provider]);
    setIsFilterOpen(false);
    setIsProviderListOpen(false);
    setIsGenreListOpen(false);
  };

  const handleGenreClick = (genre) => {
    onGenreSelect(genre, articlesByGenre[genre]);
    setIsFilterOpen(false);
    setIsProviderListOpen(false);
    setIsGenreListOpen(false);
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
      <div className="calendar-panel-header">
        <button
          className="calendar-toggle-btn"
          onClick={toggleCalendar}
          title={isOpen ? "Close calendar" : "Open calendar"}
          aria-label={isOpen ? "Close calendar" : "Open calendar"}
        >
          {isOpen ? "X" : "Cal"}
        </button>

        <div className="calendar-actions">
          <button
            className={`calendar-action-btn ${
              selectedDate || selectedProvider || selectedGenre || isFilterOpen ? "active" : ""
            }`}
            onClick={toggleFilter}
            title={isFilterOpen ? "Close filters" : "Open filters"}
            aria-label={isFilterOpen ? "Close filters" : "Open filters"}
          >
            {isFilterOpen
              ? "X"
              : selectedProvider
                ? `Provider: ${selectedProvider}`
                : selectedGenre
                  ? `Genre: ${selectedGenre}`
                  : selectedDate
                    ? `Filtered: ${selectedDate}`
                    : "Filter"}
          </button>
        </div>
      </div>

      {isFilterOpen && (
        <div className="filter-dropdown">
          <button className="filter-option-btn" type="button" onClick={toggleProviderList}>
            Filter by provider
          </button>
          {isProviderListOpen && (
            <div className="provider-dropdown">
              {providers.map((provider) => (
                <button
                  className={`provider-option-btn ${
                    selectedProvider === provider ? "selected" : ""
                  }`}
                  key={provider}
                  type="button"
                  onClick={() => handleProviderClick(provider)}
                >
                  <span>{provider}</span>
                  <span>{articlesByProvider[provider].length}</span>
                </button>
              ))}
            </div>
          )}
          <button className="filter-option-btn" type="button" onClick={toggleGenreList}>
            Filter by genre
          </button>
          {isGenreListOpen && (
            <div className="provider-dropdown">
              {genres.map((genre) => (
                <button
                  className={`provider-option-btn ${selectedGenre === genre ? "selected" : ""}`}
                  key={genre}
                  type="button"
                  onClick={() => handleGenreClick(genre)}
                >
                  <span>{genre}</span>
                  <span>{articlesByGenre[genre].length}</span>
                </button>
              ))}
            </div>
          )}
        </div>
      )}

      {isOpen && (
        <>
          <div className="calendar-header">
            <button className="nav-btn" onClick={goToPreviousMonth}>
              &lt;
            </button>
            <h3>{monthName}</h3>
            <button className="nav-btn" onClick={goToNextMonth}>
              &gt;
            </button>
          </div>

          <div className="calendar-grid">
            <div className="day-labels">
              {["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"].map((day) => (
                <div key={day} className="day-label">
                  {day}
                </div>
              ))}
            </div>

            <div className="days-grid">
              {days.map((date) => {
                const dateStr = date.toISOString().split("T")[0];
                const hasArticles = datesWithArticles.has(dateStr);
                const isSelected = selectedDate === dateStr;
                const isCurrent = isCurrentMonth(date);
                const isCurrentDay = isToday(date);

                return (
                  <button
                    key={dateStr}
                    className={`calendar-day ${!isCurrent ? "other-month" : ""} ${
                      isSelected ? "selected" : ""
                    } ${isCurrentDay ? "today" : ""} ${hasArticles ? "has-articles" : ""}`}
                    onClick={() => handleDateClick(date)}
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
            <button className="clear-filter-btn" onClick={handleClearDate}>
              Clear Filter
            </button>
          )}
        </>
      )}
    </div>
  );
}
