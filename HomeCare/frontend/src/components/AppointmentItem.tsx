import { useTranslation } from 'react-i18next'

type AppointmentItemProps = {
  time: string
  title: string
  date?: string
  CaregiverName?: string | null
  address?: string | null
  phone?: string | null
  tasks?: string[]
  showTodayBadge?: boolean
  locale?: string
}

export default function AppointmentItem({
  time,
  title,
  date,
  CaregiverName,
  address,
  phone,
  tasks,
  showTodayBadge = false,
  locale = 'nb-NO',
}: AppointmentItemProps) {
  const { t } = useTranslation()

  const formatTime = (dateTime: string) =>
    new Date(dateTime).toLocaleTimeString(locale, { hour: '2-digit', minute: '2-digit' })

  const formatDate = (dateTime: string) =>
    new Date(dateTime).toLocaleDateString(locale, {
      weekday: 'long',
      day: 'numeric',
      month: 'long',
    })

  return (
    <li className="list-group-item px-0">
      <div className="d-flex justify-content-between align-items-start">
        <div>
          {date && (
            <div className="fw-semibold">{formatDate(date)}</div>
          )}
          <strong className="text-primary">{formatTime(time)}</strong>
          <div className="fw-semibold">{title}</div>
          {CaregiverName && (
            <small className="text-muted d-block">
              <i className="bi bi-person me-1"></i>{CaregiverName}
            </small>
          )}
          {address && (
            <small className="text-muted d-block">
              <i className="bi bi-geo-alt me-1"></i>{address}
            </small>
          )}
          {phone && (
            <small className="text-muted d-block">
              <i className="bi bi-telephone me-1"></i>{phone}
            </small>
          )}
          {tasks && tasks.length > 0 && (
            <span className="badge bg-secondary mt-1">{tasks.join(', ')}</span>
          )}
        </div>
        {showTodayBadge && (
          <span className="badge bg-success">{t('common.today')}</span>
        )}
      </div>
    </li>
  )
}
