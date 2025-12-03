import { useMemo, useState } from 'react'
import { useTranslation } from 'react-i18next'

export type CalendarEvent = {
  id?: number
  dateTime: string
  title?: string
  label?: string
  badgeClass?: string
}

type CalendarProps = {
  year: number
  month: number // 0-indexed
  onPrevMonth: () => void
  onNextMonth: () => void
  events?: CalendarEvent[]
  // Render custom content for a day cell
  renderDayContent?: (date: Date, dateStr: string, events: CalendarEvent[]) => React.ReactNode
  // Render custom actions/controls in the top-right of a day cell
  renderDayActions?: (date: Date, dateStr: string, isPast: boolean) => React.ReactNode
  // Highlighted dates (e.g., available days for Caregiver)
  highlightedDates?: Set<string>
  // Custom class for highlighted dates
  highlightedClass?: string
}

import { toDateString } from '../utils/dateUtils'

export default function Calendar({
  year,
  month,
  onPrevMonth,
  onNextMonth,
  events = [],
  renderDayContent,
  renderDayActions,
  highlightedDates,
  highlightedClass = 'available-day',
}: CalendarProps) {
  const { t, i18n } = useTranslation()
  const locale = i18n.language === 'nb' ? 'nb-NO' : 'en-US'

  // Track which days are expanded to show all events
  const [expandedDays, setExpandedDays] = useState<Set<string>>(new Set())

  const toggleDayExpanded = (dateStr: string) => {
    setExpandedDays(prev => {
      const next = new Set(prev)
      if (next.has(dateStr)) {
        next.delete(dateStr)
      } else {
        next.add(dateStr)
      }
      return next
    })
  }

  const today = new Date()
  const todayString = toDateString(today)

  // Group events by date
  const eventsByDate = useMemo(() => {
    const map = new Map<string, CalendarEvent[]>()
    for (const event of events) {
      const dateStr = toDateString(new Date(event.dateTime))
      if (!map.has(dateStr)) map.set(dateStr, [])
      map.get(dateStr)!.push(event)
    }
    return map
  }, [events])

  // Calendar grid calculation
  const calendarData = useMemo(() => {
    const firstDay = new Date(year, month, 1)
    const daysInMonth = new Date(year, month + 1, 0).getDate()
    // Monday = 0, Sunday = 6
    const offset = (firstDay.getDay() + 6) % 7
    const totalCells = offset + daysInMonth
    const rows = Math.ceil(totalCells / 7)
    return { firstDay, daysInMonth, offset, rows }
  }, [year, month])

  const currentMonthDate = new Date(year, month, 1)
  const prevMonthDate = new Date(year, month - 1, 1)
  const nextMonthDate = new Date(year, month + 1, 1)

  const formatMonth = (date: Date) =>
    date.toLocaleDateString(locale, { month: 'long', year: 'numeric' })

  return (
    <div className="calendar-widget">
      {/* Calendar navigation */}
      <div className="calendar-nav mb-3 d-flex justify-content-between align-items-center flex-wrap gap-2">
        <button className="btn btn-outline-secondary" onClick={onPrevMonth}>
          <i className="bi bi-chevron-left me-1"></i> {formatMonth(prevMonthDate)}
        </button>
        <h4 className="mb-0 text-capitalize fw-bold">{formatMonth(currentMonthDate)}</h4>
        <button className="btn btn-outline-secondary" onClick={onNextMonth}>
          {formatMonth(nextMonthDate)} <i className="bi bi-chevron-right ms-1"></i>
        </button>
      </div>

      {/* Calendar grid */}
      <div className="calendar-container table-responsive">
        <table className="table table-bordered table-calendar">
          <thead className="table-light">
            <tr>
              <th className="text-center">{t('calendar.monday')}</th>
              <th className="text-center">{t('calendar.tuesday')}</th>
              <th className="text-center">{t('calendar.wednesday')}</th>
              <th className="text-center">{t('calendar.thursday')}</th>
              <th className="text-center">{t('calendar.friday')}</th>
              <th className="text-center">{t('calendar.saturday')}</th>
              <th className="text-center text-danger">{t('calendar.sunday')}</th>
            </tr>
          </thead>
          <tbody>
            {Array.from({ length: calendarData.rows }).map((_, rowIdx) => (
              <tr key={rowIdx}>
                {Array.from({ length: 7 }).map((_, colIdx) => {
                  const cellIndex = rowIdx * 7 + colIdx
                  const { offset, daysInMonth } = calendarData

                  if (cellIndex < offset || cellIndex >= offset + daysInMonth) {
                    return <td key={colIdx} className="empty bg-light"></td>
                  }

                  const dayNum = cellIndex - offset + 1
                  const date = new Date(year, month, dayNum)
                  const dateStr = toDateString(date)
                  const isSunday = date.getDay() === 0
                  const isToday = dateStr === todayString
                  const isPast = dateStr < todayString
                  const hasEvents = eventsByDate.has(dateStr)
                  const dayEvents = eventsByDate.get(dateStr) || []
                  const isHighlighted = highlightedDates?.has(dateStr)
                  const isExpanded = expandedDays.has(dateStr)
                  const visibleEvents = isExpanded ? dayEvents : dayEvents.slice(0, 2)
                  const hiddenCount = dayEvents.length - 2

                  return (
                    <td
                      key={colIdx}
                      className={`calendar-day position-relative ${hasEvents ? 'has-booking' : ''
                        } ${isSunday ? 'sunday' : ''} ${isHighlighted ? highlightedClass : ''
                        } ${isToday ? 'bg-primary-subtle border-primary' : ''} ${isPast && !isToday ? 'bg-light text-muted' : ''
                        }`}
                      style={{ minHeight: '80px', verticalAlign: 'top' }}
                    >
                      <div className="d-flex justify-content-between align-items-start">
                        <span
                          className={`date-label fw-bold ${isSunday ? 'text-danger' : ''} ${isToday ? 'badge bg-primary text-white rounded-circle p-2' : ''
                            }`}
                        >
                          {dayNum}
                        </span>
                        {/* Show event count badge if no custom day actions */}
                        {hasEvents && !renderDayContent && !renderDayActions && (
                          <span className="badge bg-success rounded-pill">
                            {dayEvents.length}
                          </span>
                        )}
                      </div>

                      {/* Custom day actions with event count badge to the left */}
                      {renderDayActions && (
                        <div
                          style={{
                            position: 'absolute',
                            top: '4px',
                            right: '4px',
                          }}
                          className="d-flex align-items-center gap-1"
                        >
                          {hasEvents && (
                            <span className="badge bg-success rounded-pill" style={{ fontSize: '0.7rem' }}>
                              {dayEvents.length}
                            </span>
                          )}
                          {renderDayActions(date, dateStr, isPast)}
                        </div>
                      )}

                      {/* Custom day content or default event display */}
                      {renderDayContent ? (
                        renderDayContent(date, dateStr, dayEvents)
                      ) : (
                        hasEvents && (
                          <div className="mt-2">
                            {visibleEvents.map((event, idx) => (
                              <div
                                key={idx}
                                className={`appt-entry small ${event.badgeClass || 'bg-success-subtle'} rounded px-1 mb-1`}
                                title={event.title || event.label}
                              >
                                <small className="text-truncate d-block">
                                  {new Date(event.dateTime).toLocaleTimeString(locale, {
                                    hour: '2-digit',
                                    minute: '2-digit',
                                  })}
                                  {' - '}
                                  {event.label}
                                </small>
                              </div>
                            ))}
                            {hiddenCount > 0 && !isExpanded && (
                              <button
                                type="button"
                                className="btn btn-link btn-sm p-0 text-muted"
                                onClick={() => toggleDayExpanded(dateStr)}
                                style={{ fontSize: '0.75rem' }}
                              >
                                +{hiddenCount} {t('myPage.more')}
                              </button>
                            )}
                            {isExpanded && dayEvents.length > 2 && (
                              <button
                                type="button"
                                className="btn btn-link btn-sm p-0 text-muted"
                                onClick={() => toggleDayExpanded(dateStr)}
                                style={{ fontSize: '0.75rem' }}
                              >
                                {t('calendar.showLess')}
                              </button>
                            )}
                          </div>
                        )
                      )}
                    </td>
                  )
                })}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
