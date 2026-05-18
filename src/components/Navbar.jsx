import React, { memo } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import logo from '../../assets/logo/standard-interiors-logo.png';

const REVEAL_EASE = [0.22, 1, 0.36, 1];

const Navbar = memo(({ scrolled, expanded, menuOpen, onExpandToggle, onMenuToggle, onNavClick, reduceMotion }) => {
  const navClass = `navbar ${scrolled ? 'is-short' : ''} ${expanded ? 'is-open' : ''}`;

  return (
    <header className="header-wrap">
      <div className={navClass}>
        <a href="#home" className="brand">
          <img src={logo} alt="Standard Interiors logo" />
          <div>
            <p>STANDARD</p>
            <h1>Interiors</h1>
          </div>
        </a>

        <nav className="desktop-nav">
          <a href="#home" onClick={onNavClick}>Home</a>
          <a href="#services" onClick={onNavClick}>Services</a>
          <a href="#portfolio" onClick={onNavClick}>Portfolio</a>
          <a href="#contact" onClick={onNavClick}>Contact</a>
        </nav>

        <a href="#contact" className="cta">Request for Work</a>

        <button className="dots" type="button" onClick={onExpandToggle} aria-label="Toggle menu">
          <span />
          <span />
          <span />
        </button>

        <button className="mobile-menu-btn" onClick={onMenuToggle} type="button" aria-label="Toggle mobile menu">
          {menuOpen ? 'X' : '≡'}
        </button>
      </div>

      <AnimatePresence>
        {menuOpen && (
          <motion.div
            className="mobile-menu"
            initial={reduceMotion ? false : { opacity: 0, y: -10 }}
            animate={reduceMotion ? { opacity: 1, y: 0 } : { opacity: 1, y: 0 }}
            exit={reduceMotion ? { opacity: 0 } : { opacity: 0, y: -8 }}
            transition={{ duration: 0.2, ease: REVEAL_EASE }}
          >
            <a href="#home" onClick={onNavClick}>Home</a>
            <a href="#services" onClick={onNavClick}>Services</a>
            <a href="#portfolio" onClick={onNavClick}>Portfolio</a>
            <a href="#contact" onClick={onNavClick}>Contact</a>
          </motion.div>
        )}
      </AnimatePresence>
    </header>
  );
});

Navbar.displayName = 'Navbar';

export default Navbar;
