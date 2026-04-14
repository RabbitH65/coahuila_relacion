import Encabezado from './Encabezado'
import Footer from './Footer'

export default function Layout({ children, view = 'capture', onNavigate }) {
  return (
    <div className="app-shell">
      <Encabezado view={view} onNavigate={onNavigate} />
      <main className="app-main">{children}</main>
      <Footer />
    </div>
  )
}
