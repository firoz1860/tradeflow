export function RiskAlerts({ alerts }: { alerts: string[] }) {
  return <section className="risk-alerts">{alerts.map((alert, index) => <div key={`${alert}-${index}`} className="notice error">Risk check: {alert}</div>)}</section>
}
