import { useState, useEffect, useCallback } from 'react'

/**
 * FontResizer Component
 * Provides accessibility feature to adjust font size on the page
 * Features:
 * - Increase/decrease font size buttons
 * - Reset to default size
 * - Persists preference in localStorage
 * - Applies to elements with .font-resizable-area class
 * 
 * Usage: Place this component in the navbar or header
 */
export default function FontResizer() {
  // Default font size in pixels
  const DEFAULT_SIZE = 16
  const MIN_SIZE = 12
  const MAX_SIZE = 24
  const STEP = 2

  // State for current font size
  const [fontSize, setFontSize] = useState(() => {
    // Load saved preference from localStorage
    const saved = localStorage.getItem('fontSize')
    return saved ? parseInt(saved, 10) : DEFAULT_SIZE
  })

  /**
   * Apply font size to all resizable areas
   */
  const applyFontSize = useCallback((size) => {
    const elements = document.querySelectorAll('.font-resizable-area')
    elements.forEach(el => {
      el.style.fontSize = `${size}px`
    })
  }, [])

  /**
   * Apply font size on mount and when it changes
   */
  useEffect(() => {
    applyFontSize(fontSize)
    localStorage.setItem('fontSize', fontSize.toString())
  }, [fontSize, applyFontSize])

  /**
   * Increase font size
   */
  const increaseFontSize = () => {
    setFontSize(prev => Math.min(prev + STEP, MAX_SIZE))
  }

  /**
   * Decrease font size
   */
  const decreaseFontSize = () => {
    setFontSize(prev => Math.max(prev - STEP, MIN_SIZE))
  }

  /**
   * Reset to default font size
   */
  const resetFontSize = () => {
    setFontSize(DEFAULT_SIZE)
  }

  return (
    <div className="font-resizer d-flex align-items-center gap-1">
      {/* Decrease font size button */}
      <button
        type="button"
        className="btn btn-sm btn-outline-secondary"
        onClick={decreaseFontSize}
        disabled={fontSize <= MIN_SIZE}
        title="Mindre tekst"
        aria-label="Reduser skriftstørrelse"
      >
        <i className="bi bi-dash"></i>
        <span className="visually-hidden">Mindre tekst</span>
      </button>

      {/* Current size indicator */}
      <span className="badge bg-secondary" title="Gjeldende skriftstørrelse">
        {fontSize}px
      </span>

      {/* Increase font size button */}
      <button
        type="button"
        className="btn btn-sm btn-outline-secondary"
        onClick={increaseFontSize}
        disabled={fontSize >= MAX_SIZE}
        title="Større tekst"
        aria-label="Øk skriftstørrelse"
      >
        <i className="bi bi-plus"></i>
        <span className="visually-hidden">Større tekst</span>
      </button>

      {/* Reset button */}
      <button
        type="button"
        className="btn btn-sm btn-outline-secondary"
        onClick={resetFontSize}
        disabled={fontSize === DEFAULT_SIZE}
        title="Tilbakestill tekststørrelse"
        aria-label="Tilbakestill skriftstørrelse"
      >
        <i className="bi bi-arrow-counterclockwise"></i>
        <span className="visually-hidden">Tilbakestill</span>
      </button>
    </div>
  )
}
