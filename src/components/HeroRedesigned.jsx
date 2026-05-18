import React, { memo } from 'react';
import { motion } from 'framer-motion';
import logo from '../../assets/logo/standard-interiors-logo.png';
import heroImg from '../../assets/images/project-1.jpeg'; // use project-1 or similar as the hero image

const REVEAL_EASE = [0.22, 1, 0.36, 1];
const REVEAL_DURATION = 0.58;

const HeroRedesigned = memo(({ reduceMotion }) => {
  const revealVariants = {
    hidden: { opacity: 0, y: 18 },
    visible: { opacity: 1, y: 0, transition: { duration: REVEAL_DURATION, ease: REVEAL_EASE } }
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
      <style dangerouslySetInnerHTML={{__html: `
        .hero-v2-container {
          position: relative;
          width: 100%;
          min-height: 85vh;
          background-color: #f6f3f0;
          border-radius: 24px;
          display: flex;
          overflow: hidden;
          margin-top: 20px;
          box-shadow: 0 10px 40px rgba(0,0,0,0.05);
        }

        .hero-v2-left {
          flex: 1.2;
          padding: 60px 40px 60px 80px;
          display: flex;
          flex-direction: column;
          justify-content: center;
          position: relative;
          z-index: 2;
        }

        .hero-v2-header-row {
          position: absolute;
          top: 40px;
          left: 80px;
          right: 40px;
          display: flex;
          justify-content: space-between;
          align-items: center;
          width: calc(100% + 400px); /* Extend over the right image roughly */
          max-width: 90vw;
          z-index: 10;
        }

        .hero-v2-logo-wrap {
          display: flex;
          align-items: center;
          gap: 15px;
        }

        .hero-v2-logo {
          width: 65px;
        }

        .hero-v2-logo-text {
          font-weight: 700;
          font-size: 16px;
          line-height: 1.2;
          color: #1a1a1a;
          letter-spacing: 1px;
        }

        .hero-v2-socials {
          display: flex;
          gap: 15px;
          margin-right: 50px;
        }

        .hero-v2-social-btn {
          width: 45px;
          height: 45px;
          border-radius: 50%;
          background: white;
          color: #1a1a1a;
          display: flex;
          align-items: center;
          justify-content: center;
          text-decoration: none;
          box-shadow: 0 4px 15px rgba(0,0,0,0.08);
          transition: transform 0.3s;
        }

        .hero-v2-social-btn:hover {
          transform: translateY(-3px);
        }

        .hero-v2-subhead {
          font-size: 12px;
          font-weight: 700;
          color: #7b2c45;
          letter-spacing: 2px;
          text-transform: uppercase;
          margin-bottom: 25px;
          margin-top: 80px;
        }

        .hero-v2-heading {
          font-size: 58px;
          line-height: 1.15;
          color: #1a1a1a;
          font-family: var(--font-serif, serif);
          margin-bottom: 25px;
          font-weight: 400;
        }

        .hero-v2-heading span {
          color: #be8e54;
        }

        .hero-v2-desc {
          color: #555;
          font-size: 18px;
          line-height: 1.6;
          max-width: 85%;
          margin-bottom: 45px;
        }

        .hero-v2-cta-card {
          background: #3b1c2b;
          border-radius: 16px;
          padding: 24px 30px;
          display: flex;
          align-items: center;
          justify-content: space-between;
          color: white;
          max-width: 95%;
          margin-bottom: 60px;
        }

        .hero-v2-cta-left {
          display: flex;
          align-items: center;
          gap: 20px;
        }

        .hero-v2-cta-icon {
          width: 50px;
          height: 50px;
          background: rgba(255,255,255,0.1);
          border-radius: 50%;
          display: flex;
          align-items: center;
          justify-content: center;
        }

        .hero-v2-cta-text {
          font-size: 16px;
          font-weight: 500;
          line-height: 1.4;
          max-width: 220px;
        }

        .hero-v2-cta-btn {
          background: white;
          color: #3b1c2b;
          padding: 14px 24px;
          border-radius: 30px;
          font-size: 14px;
          font-weight: 600;
          text-decoration: none;
          display: inline-flex;
          align-items: center;
          gap: 8px;
          transition: background 0.3s;
        }

        .hero-v2-cta-btn:hover {
          background: #f0f0f0;
        }

        .hero-v2-features {
          display: flex;
          gap: 40px;
          border-top: 1px solid rgba(0,0,0,0.06);
          padding-top: 30px;
          margin-top: auto;
        }

        .hero-v2-feature {
          display: flex;
          align-items: center;
          gap: 15px;
        }

        .hero-v2-feature-icon {
          width: 48px;
          height: 48px;
          background: white;
          border-radius: 50%;
          display: flex;
          align-items: center;
          justify-content: center;
          color: #7b2c45;
          box-shadow: 0 4px 15px rgba(0,0,0,0.05);
        }

        .hero-v2-feature-text strong {
          display: block;
          font-size: 14px;
          color: #1a1a1a;
          margin-bottom: 3px;
        }

        .hero-v2-feature-text span {
          display: block;
          font-size: 12px;
          color: #666;
        }

        .hero-v2-right {
          flex: 1;
          position: relative;
          clip-path: ellipse(100% 100% at 100% 50%);
        }

        .hero-v2-right-img {
          width: 100%;
          height: 100%;
          object-fit: cover;
          position: absolute;
          inset: 0;
          border-radius: 300px 0 0 300px;
        }

        @media (max-width: 1024px) {
          .hero-v2-container { flex-direction: column; }
          .hero-v2-left { padding: 40px 20px; }
          .hero-v2-header-row { position: static; width: 100%; margin-bottom: 40px; }
          .hero-v2-socials { margin-right: 0; }
          .hero-v2-subhead { margin-top: 0; }
          .hero-v2-heading { font-size: 42px; }
          .hero-v2-cta-card { flex-direction: column; gap: 20px; text-align: center; }
          .hero-v2-cta-left { flex-direction: column; text-align: center; }
          .hero-v2-features { flex-direction: column; gap: 20px; }
          .hero-v2-right { min-height: 400px; clip-path: none; }
          .hero-v2-right-img { border-radius: 0; }
        }
      `}} />

      <motion.section id="home" className="hero-v2-container" {...revealInViewProps()}>
        <div className="hero-v2-header-row">
          <div className="hero-v2-logo-wrap">
            <img src={logo} alt="Logo" className="hero-v2-logo" />
            <div className="hero-v2-logo-text">STANDARD<br/>INTERIORS</div>
          </div>
          <div className="hero-v2-socials">
            <a href="https://www.instagram.com/standard__interiors/" className="hero-v2-social-btn" aria-label="Instagram">
              <svg width="20" height="20" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><rect x="2" y="2" width="20" height="20" rx="5" ry="5"></rect><path d="M16 11.37A4 4 0 1 1 12.63 8 4 4 0 0 1 16 11.37z"></path><line x1="17.5" y1="6.5" x2="17.51" y2="6.5"></line></svg>
            </a>
            <a href="https://wa.me/919947015742" className="hero-v2-social-btn" aria-label="WhatsApp">
              <svg width="22" height="22" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" viewBox="0 0 24 24"><path d="M21 11.5a8.38 8.38 0 0 1-.9 3.8 8.5 8.5 0 0 1-7.6 4.7 8.38 8.38 0 0 1-3.8-.9L3 21l1.9-5.7a8.38 8.38 0 0 1-.9-3.8 8.5 8.5 0 0 1 4.7-7.6 8.38 8.38 0 0 1 3.8-.9h.5a8.48 8.48 0 0 1 8 8v.5z"></path></svg>
            </a>
            <a href="tel:+919539120466" className="hero-v2-social-btn" aria-label="Phone">
              <svg width="20" height="20" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z"></path></svg>
            </a>
          </div>
        </div>

        <div className="hero-v2-left">
          <div className="hero-v2-subhead">INTERIOR SOLUTIONS FOR EVERYONE</div>
          <h1 className="hero-v2-heading">Beautiful Interiors,<br/>Thoughtfully <span>Designed</span></h1>
          <p className="hero-v2-desc">From personal spaces to complete room and shop interiors — we bring your vision to life.</p>
          
          <div className="hero-v2-cta-card">
            <div className="hero-v2-cta-left">
              <div className="hero-v2-cta-icon">
                <svg width="28" height="28" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" viewBox="0 0 24 24"><path d="M21 11.5a8.38 8.38 0 0 1-.9 3.8 8.5 8.5 0 0 1-7.6 4.7 8.38 8.38 0 0 1-3.8-.9L3 21l1.9-5.7a8.38 8.38 0 0 1-.9-3.8 8.5 8.5 0 0 1 4.7-7.6 8.38 8.38 0 0 1 3.8-.9h.5a8.48 8.48 0 0 1 8 8v.5z"></path></svg>
              </div>
              <div className="hero-v2-cta-text">Need a clean interior finish at a budget-friendly price?</div>
            </div>
            <a href="https://wa.me/919947015742" className="hero-v2-cta-btn">
              Start on WhatsApp
              <svg width="16" height="16" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><polyline points="9 18 15 12 9 6"></polyline></svg>
            </a>
          </div>

          <div className="hero-v2-features">
            <div className="hero-v2-feature">
              <div className="hero-v2-feature-icon">
                <svg width="22" height="22" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24"><path d="M20 9H4v10a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9Z"></path><path d="M4 9l8-6 8 6"></path></svg>
              </div>
              <div className="hero-v2-feature-text">
                <strong>Custom Designs</strong>
                <span>Tailored to your style and space</span>
              </div>
            </div>
            <div className="hero-v2-feature">
              <div className="hero-v2-feature-icon">
                <svg width="22" height="22" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24"><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"></path><polyline points="9 12 11 14 15 10"></polyline></svg>
              </div>
              <div className="hero-v2-feature-text">
                <strong>Quality Assurance</strong>
                <span>Premium materials and finishes</span>
              </div>
            </div>
            <div className="hero-v2-feature">
              <div className="hero-v2-feature-icon">
                <svg width="22" height="22" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24"><rect x="2" y="5" width="20" height="14" rx="2"></rect><line x1="2" y1="10" x2="22" y2="10"></line></svg>
              </div>
              <div className="hero-v2-feature-text">
                <strong>Budget Friendly</strong>
                <span>Elegant interiors that fit your budget</span>
              </div>
            </div>
          </div>
        </div>

        <div className="hero-v2-right">
          <img src={heroImg} alt="Interior Design" className="hero-v2-right-img" />
        </div>
      </motion.section>
    </>
  );
});

HeroRedesigned.displayName = 'HeroRedesigned';

export default HeroRedesigned;