import DatePicker from 'react-datepicker'
import 'react-datepicker/dist/react-datepicker.css'

interface CalendarProps {
    startDate: Date | undefined
    endDate: Date | undefined
    onChange: (dates: [Date | undefined, Date | undefined]) => void
    className?: string
}

export function Calendar({ startDate, endDate, onChange, className }: CalendarProps) {
    return (
        <DatePicker
            startDate={startDate}
            endDate={endDate}
            onChange={(dates) => onChange([dates[0] ?? undefined, dates[1] ?? undefined])}
            className={className}
            selectsRange
            inline
        />
    )
}