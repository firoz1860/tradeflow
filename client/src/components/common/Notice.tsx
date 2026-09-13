export function Notice({ message, kind = 'info' }: { message?: string; kind?: 'info' | 'error' | 'success' }) {
  return message ? <div className={`notice ${kind}`}>{message}</div> : null
}
