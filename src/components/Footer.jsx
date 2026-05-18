import React, { memo } from 'react';
import { motion } from 'framer-motion';

const REVEAL_EASE = [0.22, 1, 0.36, 1];
const REVEAL_DURATION = 0.58;

const Footer = memo(({ logo, partnersHighlights, reduceMotion }) => {
  const revealVariants = {
    hidden: { opacity: 0, y: 18 },
    visible: {
      opacity: 1,
      y: 0,
      transition: { duration: REVEAL_DURATION, ease: REVEAL_EASE }
    }
  };

  const revealInViewProps = (delay = 0) => {
    if (reduceMotion) return {};
    return {
      variants: revealVariants,
      initial: 'hidden',
      whileInView: 'visible',
      viewport: { once: true, amount: 0.18 },
      transition: { delay }
    };
  };

  return (
    <>
      <motion.section className="partners-band" {...revealInViewProps()}>
        <div className="partners-marquee" aria-label="Trusted partners">
          <div className="partners-track">
            {partnersHighlights.map((item) => (
              <span key={`main-${item}`}>{item}</span>
            ))}
          </div>
          <div className="partners-track" aria-hidden="true">
            {partnersHighlights.map((item) => (
              <span key={`clone-${item}`}>{item}</span>
            ))}
          </div>
        </div>
      </motion.section>

      <motion.footer className="site-footer" {...revealInViewProps()}>
        <div className="footer-top-row">
          <div>
            <img src={logo} alt="Standard Interiors logo" className="footer-logo" />
            <p className="footer-about">
              Interior solutions built for everyone. From personal spaces to complete room and shop interiors.
            </p>
            <div className="footer-socials">
              <a href="https://www.instagram.com/standard__interiors/" target="_blank" rel="noreferrer" aria-label="Instagram">IG</a>
              <a href="https://wa.me/919947015742" target="_blank" rel="noreferrer" aria-label="WhatsApp">WA</a>
              <a href="tel:+919539120466" aria-label="Phone">PH</a>
            </div>
          </div>

          <aside className="footer-cta-panel">
            <p>Need a clean interior finish at a budget-friendly price?</p>
            <a href="https://wa.me/919947015742" target="_blank" rel="noreferrer">Start on WhatsApp</a>
          </aside>
        </div>

        <div className="footer-grid">
          <div>
            <h4>Quick Links</h4>
            <div className="footer-links">
              <a href="#home">Home</a>
              <a href="#services">Services</a>
              <a href="#portfolio">Portfolio</a>
              <a href="#contact">Contact</a>
            </div>
          </div>

          <div>
            <h4>Our Services</h4>
            <div className="footer-links">
              <span>Bride &amp; Groom Setup</span>
              <span>Complete Room Interior</span>
              <span>Shop Interior</span>
              <span>Ceiling Work</span>
              <span>Custom Interior Plans</span>
            </div>
          </div>

          <div>
            <h4>Contact Info</h4>
            <div className="footer-links">
              <span>Kerala, India</span>
              <a href="tel:+919539120466">+91 9539120466</a>
              <a href="https://wa.me/919947015742" target="_blank" rel="noreferrer">wa.me/919947015742</a>
              <a href="https://www.instagram.com/standard__interiors/" target="_blank" rel="noreferrer">@standard__interiors</a>
            </div>
          </div>
        </div>

        <div className="footer-bottom">© 2026 Standard Interiors. All rights reserved.</div>
      </motion.footer>

      <a className="float-whatsapp" href="https://wa.me/919947015742" target="_blank" rel="noreferrer" aria-label="WhatsApp">
        WA
      </a>
    </>
  );
});

Footer.displayName = 'Footer';

export default Footer;
