import React, { useEffect, useRef, useState } from 'react';
import { AnimatePresence, motion, useReducedMotion } from 'framer-motion';
import { gsap } from 'gsap';
import { ScrollTrigger } from 'gsap/ScrollTrigger';

import logo from '../assets/logo/standard-interiors-logo.png';
import p1 from '../assets/images/project-1.jpeg';
import p2 from '../assets/images/project-2.jpeg';
import p3 from '../assets/images/project-3.jpeg';
import p4 from '../assets/images/project-4.jpeg';
import p5 from '../assets/images/project-5.jpeg';
import p6 from '../assets/images/project-6.jpeg';
import p7 from '../assets/images/project-7.jpeg';
import p8 from '../assets/images/project-8.jpeg';

const portfolioItems = [
  { image: p2, title: 'Living Room Cove Lighting', text: 'Soft perimeter glow with budget-conscious detailing.' },
  { image: p3, title: 'Wood-Panel Ceiling Feature', text: 'Warm wood tones with modern gypsum framing.' },
  { image: p4, title: 'Minimal Ceiling Geometry', text: 'Simple lines and clean finishing for modern homes.' },
  { image: p5, title: 'Curtain-Led Lounge Finish', text: 'Balanced curtain styling and ambient ceiling details.' },
  { image: p6, title: 'Modern Bedroom Lighting', text: 'Compact room with bright, floating ceiling effect.' },
  { image: p7, title: 'Statement Ceiling Panel', text: 'Lighting-led centerpiece design with crisp details.' },
  { image: p8, title: 'Warm Wood Living Space', text: 'Soft neutral tones and practical interior layout.' },
  { image: p1, title: 'Signature Gypsum Pattern', text: 'Layered gypsum and indirect light in a premium finish.' }
];

const services = [
  {
    title: 'Bride & Groom Room Setup',
    includes: ['Ceiling', 'Wiring', 'Putty', 'Lighting', 'Painting', 'Curtains', 'Furniture (Optional)']
  },
  {
    title: 'Complete Room Interior',
    includes: ['Ceiling', 'Wiring', 'Putty', 'Lighting', 'Painting', 'Curtains', 'Furniture (Optional)']
  },
  {
    title: 'Shop Interior',
    includes: ['Ceiling', 'Wiring', 'Putty', 'Lighting', 'Painting', 'Curtains', 'Furniture (Optional)']
  },
  {
    title: 'Ceiling Work Only',
    includes: ['Ceiling Design & Installation']
  },
  {
    title: 'Customize as Per Your Needs',
    includes: [
      'All services can be customized based on customer requirements',
      'Choose any combination of services'
    ]
  }
];

const instagramItems = [
  { image: p1, title: 'Warm Ceiling Finish', kind: 'reel' },
  { image: p2, title: 'Living Room Ambience', kind: 'post' },
  { image: p3, title: 'Wood Panel Detailing', kind: 'reel' },
  { image: p4, title: 'Minimal Geometry Build', kind: 'post' },
  { image: p5, title: 'Curtain & Lighting Pairing', kind: 'reel' },
  { image: p6, title: 'Bedroom Transformation', kind: 'post' },
  { image: p7, title: 'Statement Ceiling Concept', kind: 'reel' },
  { image: p8, title: 'Premium Lounge Setup', kind: 'post' }
];

const partnersHighlights = [
  '100+ Happy Clients',
  'Premium Branded Materials',
  'On-Time Project Delivery',
  'Skilled & Dedicated Team'
];

const revealEase = [0.22, 1, 0.36, 1];

const revealVariants = {
  hidden: { opacity: 0, y: 18 },
  visible: {
    opacity: 1,
    y: 0,
    transition: { duration: 0.58, ease: revealEase }
  }
};

const fadeVariants = {
  hidden: { opacity: 0 },
  visible: {
    opacity: 1,
    transition: { duration: 0.4, ease: revealEase }
  }
};

function App() {
  const [scrolled, setScrolled] = useState(false);
  const [expanded, setExpanded] = useState(false);
  const [menuOpen, setMenuOpen] = useState(false);
  const [lightboxItem, setLightboxItem] = useState(null);
  const [formSubmitting, setFormSubmitting] = useState(false);
  const [formMessage, setFormMessage] = useState(null);
  const reduceMotion = useReducedMotion();
  const lastScrollYRef = useRef(0);
  const servicesRef = useRef(null);
  const servicesBgRef = useRef(null);
  const servicesContentRef = useRef(null);
  const servicesWireRef = useRef(null);
  const servicesRealRef = useRef(null);
  const servicesGlowRef = useRef(null);
  const servicesGridRef = useRef(null);

  useEffect(() => {
    const onKeyDown = (event) => {
      if (event.key === 'Escape') {
        setLightboxItem(null);
        setMenuOpen(false);
        setExpanded(false);
      }
    };

    window.addEventListener('keydown', onKeyDown);
    return () => window.removeEventListener('keydown', onKeyDown);
  }, []);

  useEffect(() => {
    let ticking = false;

    const onScroll = () => {
      if (ticking) return;
      ticking = true;

      window.requestAnimationFrame(() => {
        const y = window.scrollY;
        const delta = y - lastScrollYRef.current;

        if (y < 90) {
          setExpanded(false);
          setScrolled(false);
        } else if (delta > 3) {
          setScrolled(true);
        } else if (delta < -3) {
          setExpanded(false);
          setScrolled(false);
        }

        lastScrollYRef.current = y;

        ticking = false;
      });
    };

    lastScrollYRef.current = window.scrollY;
    onScroll();
    window.addEventListener('scroll', onScroll, { passive: true });
    return () => window.removeEventListener('scroll', onScroll);
  }, []);

  useEffect(() => {
    const onResize = () => {
      if (window.innerWidth > 1024) {
        setMenuOpen(false);
      }
    };

    window.addEventListener('resize', onResize);
    return () => window.removeEventListener('resize', onResize);
  }, []);

  useEffect(() => {
    if (reduceMotion) return undefined;

    gsap.registerPlugin(ScrollTrigger);
    const mm = gsap.matchMedia();

    mm.add('(min-width: 769px)', () => {
      const cards = gsap.utils.toArray('.service-card', servicesGridRef.current);

      gsap.set(servicesRealRef.current, { scaleX: 0.001, transformOrigin: 'left center' });
      gsap.set(cards, { y: 42, opacity: 0, scale: 0.95 });

      gsap.timeline({
        defaults: { ease: 'none' },
        scrollTrigger: {
          trigger: servicesRef.current,
          start: 'top 78%',
          end: 'bottom 32%',
          scrub: 1.8
        }
      })
        .fromTo(servicesWireRef.current, { opacity: 0.86 }, { opacity: 0.12 }, 0)
        .fromTo(servicesRealRef.current, { scaleX: 0.001 }, { scaleX: 1 }, 0)
        .fromTo(servicesGlowRef.current, { xPercent: -110, opacity: 0 }, { xPercent: 112, opacity: 0.3 }, 0)
        .to(servicesGlowRef.current, { opacity: 0 }, 0.88);

      gsap.to(cards, {
        y: 0,
        opacity: 1,
        scale: 1,
        duration: 1,
        ease: 'power3.out',
        stagger: 0.15,
        scrollTrigger: {
          trigger: servicesGridRef.current,
          start: 'top 82%',
          toggleActions: 'play none none reverse'
        }
      });

      gsap.fromTo(
        servicesBgRef.current,
        { yPercent: -1 },
        {
          yPercent: 5,
          ease: 'none',
          scrollTrigger: {
            trigger: servicesRef.current,
            start: 'top bottom',
            end: 'bottom top',
            scrub: 1.9
          }
        }
      );

      gsap.fromTo(
        servicesWireRef.current,
        { yPercent: -2 },
        {
          yPercent: 8,
          ease: 'none',
          scrollTrigger: {
            trigger: servicesRef.current,
            start: 'top bottom',
            end: 'bottom top',
            scrub: 1.9
          }
        }
      );

      gsap.fromTo(
        servicesContentRef.current,
        { yPercent: 2 },
        {
          yPercent: -10,
          ease: 'none',
          scrollTrigger: {
            trigger: servicesRef.current,
            start: 'top bottom',
            end: 'bottom top',
            scrub: 1.7
          }
        }
      );
    });

    mm.add('(max-width: 768px)', () => {
      const cards = gsap.utils.toArray('.service-card', servicesGridRef.current);

      gsap.set(servicesRealRef.current, { scaleX: 0.001, transformOrigin: 'left center' });
      gsap.set(cards, { y: 24, opacity: 0, scale: 0.97 });

      gsap.timeline({
        defaults: { ease: 'none' },
        scrollTrigger: {
          trigger: servicesRef.current,
          start: 'top 86%',
          end: 'bottom 42%',
          scrub: 1.5
        }
      })
        .fromTo(servicesWireRef.current, { opacity: 0.82 }, { opacity: 0.2 }, 0)
        .fromTo(servicesRealRef.current, { scaleX: 0.001 }, { scaleX: 1 }, 0);

      gsap.to(cards, {
        y: 0,
        opacity: 1,
        scale: 1,
        duration: 0.7,
        ease: 'power3.out',
        stagger: 0.1,
        scrollTrigger: {
          trigger: servicesGridRef.current,
          start: 'top 90%',
          toggleActions: 'play none none reverse'
        }
      });
    });

    return () => mm.revert();
  }, [reduceMotion]);

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

  const submitForm = (event) => {
    event.preventDefault();
    setFormSubmitting(true);
    setFormMessage(null);

    const form = new FormData(event.currentTarget);
    const name = form.get('name') || '';
    const phone = form.get('phone') || '';
    const service = form.get('service') || 'Not specified';
    const message = form.get('message') || '';
    const text = `Hello Standard Interiors,%0A%0AMy name is ${encodeURIComponent(name)}.%0AMy contact number is ${encodeURIComponent(phone)}.%0AService needed: ${encodeURIComponent(service)}.%0A%0AProject details:%0A${encodeURIComponent(message)}`;

    setTimeout(() => {
      window.open(`https://wa.me/919947015742?text=${text}`, '_blank', 'noopener');
      setFormSubmitting(false);
      setFormMessage({ type: 'success', text: 'Redirecting to WhatsApp...' });
      event.currentTarget.reset();

      setTimeout(() => setFormMessage(null), 3000);
    }, 300);
  };

  const navClass = `navbar ${scrolled ? 'is-short' : ''} ${expanded ? 'is-open' : ''}`;

  return (
    <div className="app">
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
            <a href="#home" onClick={() => setExpanded(false)}>Home</a>
            <a href="#services" onClick={() => setExpanded(false)}>Services</a>
            <a href="#portfolio" onClick={() => setExpanded(false)}>Portfolio</a>
            <a href="#contact" onClick={() => setExpanded(false)}>Contact</a>
          </nav>

          <a href="#contact" className="cta">Request for Work</a>

          <button className="dots" type="button" onClick={() => setExpanded((v) => !v)} aria-label="Toggle menu">
            <span />
            <span />
            <span />
          </button>

          <button className="mobile-menu-btn" onClick={() => setMenuOpen((v) => !v)} type="button" aria-label="Toggle mobile menu">
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
              transition={{ duration: 0.2, ease: revealEase }}
            >
              <a href="#home" onClick={() => setMenuOpen(false)}>Home</a>
              <a href="#services" onClick={() => setMenuOpen(false)}>Services</a>
              <a href="#portfolio" onClick={() => setMenuOpen(false)}>Portfolio</a>
              <a href="#contact" onClick={() => setMenuOpen(false)}>Contact</a>
            </motion.div>
          )}
        </AnimatePresence>
      </header>

      <main>
        <motion.section id="home" className="hero" {...revealInViewProps()}>
          <h2>
            Build Interiors
            <span>That Speak for You</span>
          </h2>
          <p>
            From personal homes to business spaces, Standard Interiors crafts beautiful,
            budget-friendly interior solutions that bring your vision to life.
          </p>
          <div className="hero-actions">
            <a href="#contact" className="btn-dark">Get a Free Quote</a>
            <a href="#services" className="btn-outline">Explore Services</a>
          </div>
        </motion.section>

        <section id="services" className="section services-section" ref={servicesRef}>
          <div
            className="blueprint-bg"
            aria-hidden="true"
            ref={servicesBgRef}
          >
            <img src={p8} alt="Modern interior background" className="services-bg-image" />
            <div className="services-wire-overlay" ref={servicesWireRef}>
              <img src={p8} alt="Interior wireframe layer" className="services-wire-image" />
              <div className="services-wire-grid" />
            </div>
            <div className="services-real-reveal" ref={servicesRealRef}>
              <img src={p8} alt="Interior final layer" className="services-bg-image" />
            </div>
            <div className="services-reveal-glow" ref={servicesGlowRef} />
          </div>

          <div className="services-content" ref={servicesContentRef}>
            <h3>Services Section</h3>
            <p className="section-sub">Clean, simple cards with included works for each service type.</p>
            <div className="services-grid" ref={servicesGridRef}>
              {services.map((service) => (
                <article className="service-card" key={service.title}>
                  <h4>{service.title}</h4>
                  <p>Includes:</p>
                  <ul>
                    {service.includes.map((item) => (
                      <li key={item}>{item}</li>
                    ))}
                  </ul>
                </article>
              ))}
            </div>
          </div>
        </section>

        <motion.section id="portfolio" className="section" {...revealInViewProps()}>
          <h3>Portfolio</h3>
          <p className="section-sub">Recent interior highlights built around warmth, lighting, and clean ceiling details.</p>
          <div className="portfolio-grid">
            {portfolioItems.map((item, idx) => (
              <motion.button
                key={item.title}
                className="portfolio-card"
                onClick={() => setLightboxItem(item)}
                type="button"
                {...revealInViewProps(idx * 0.03)}
              >
                <img src={item.image} alt={item.title} loading="lazy" />
                <span>
                  <strong>{item.title}</strong>
                  <small>{item.text}</small>
                </span>
              </motion.button>
            ))}
          </div>
        </motion.section>

        <motion.section id="instagram" className="section instagram-section" {...revealInViewProps()}>
          <div className="instagram-head">
            <h3>Our Works &amp; Reels</h3>
            <p className="section-sub">
              Explore our latest interior designs, projects, and transformations on Instagram.
            </p>
          </div>

          <div className="instagram-profile-card">
            <div className="instagram-meta">
              <span className="instagram-icon" aria-hidden="true">
                <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <rect x="3" y="3" width="18" height="18" rx="5" stroke="currentColor" strokeWidth="1.9" />
                  <circle cx="12" cy="12" r="4.1" stroke="currentColor" strokeWidth="1.9" />
                  <circle cx="17.2" cy="6.9" r="1.1" fill="currentColor" />
                </svg>
              </span>
              <div>
                <strong>standard__interiors</strong>
                <p>Interior projects, reels, and before/after transformations.</p>
              </div>
            </div>
            <a
              href="https://www.instagram.com/standard__interiors?igsh=Z2xuaHEyNDhldmQx"
              target="_blank"
              rel="noreferrer"
              className="instagram-follow-btn"
            >
              Follow Us
            </a>
          </div>

          <div className="instagram-grid">
            {instagramItems.map((item, idx) => (
              <motion.a
                key={item.title}
                href="https://www.instagram.com/standard__interiors?igsh=Z2xuaHEyNDhldmQx"
                target="_blank"
                rel="noreferrer"
                className="instagram-card"
                initial={reduceMotion ? false : { opacity: 0, y: 24 }}
                whileInView={{ opacity: 1, y: 0 }}
                viewport={{ once: true, amount: 0.25 }}
                transition={reduceMotion ? { duration: 0 } : { duration: 0.5, ease: revealEase, delay: idx * 0.08 }}
              >
                <img src={item.image} alt={item.title} loading="lazy" />
                <span className="instagram-card-overlay">
                  <strong>{item.title}</strong>
                  {item.kind === 'reel' && <em className="reel-play">▶</em>}
                </span>
              </motion.a>
            ))}
          </div>

          <a
            href="https://www.instagram.com/standard__interiors?igsh=Z2xuaHEyNDhldmQx"
            target="_blank"
            rel="noreferrer"
            className="instagram-more-btn"
          >
            View More on Instagram
          </a>
        </motion.section>

        <motion.section id="contact" className="section contact-section" {...revealInViewProps()}>
          <motion.div className="contact-left" {...revealInViewProps(0.02)}>
            <h3>Get in Touch</h3>
            <p>Phone: +91 9539120466</p>
            <p>WhatsApp: wa.me/919947015742</p>
            <p>Instagram: @standard__interiors</p>
            <p>Areas:</p>
            <p>EDAVANNAPPARA | AREEKODE | KONDOTTY | VAZHAKKAD | MUKKAM | KIZHISSERI</p>
            <a href="https://wa.me/919947015742" target="_blank" rel="noreferrer" className="btn-green">WhatsApp Us</a>
          </motion.div>
          <motion.form className="contact-form" onSubmit={submitForm} {...revealInViewProps(0.08)}>
            <input name="name" placeholder="Name" required disabled={formSubmitting} />
            <input name="phone" placeholder="Phone / WhatsApp" required disabled={formSubmitting} />
            <select name="service" required defaultValue="" disabled={formSubmitting}>
              <option value="" disabled>Select service</option>
              {services.map((service) => (
                <option key={service.title} value={service.title}>{service.title}</option>
              ))}
            </select>
            <textarea name="message" placeholder="Tell us your requirements" rows="5" required disabled={formSubmitting} />
            <button type="submit" className="btn-dark" disabled={formSubmitting}>
              {formSubmitting ? 'Sending...' : 'Send Message'}
            </button>
            {formMessage && (
              <motion.div
                className={`form-message form-message-${formMessage.type}`}
                initial={reduceMotion ? false : { opacity: 0, y: -10 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, y: -10 }}
                transition={{ duration: 0.3 }}
              >
                {formMessage.text}
              </motion.div>
            )}
          </motion.form>
        </motion.section>

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
      </main>

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

      <AnimatePresence>
        {lightboxItem && (
          <motion.div
            className="lightbox"
            onClick={() => setLightboxItem(null)}
            initial="hidden"
            animate="visible"
            exit="hidden"
            variants={fadeVariants}
          >
            <motion.div
              className="lightbox-inner"
              onClick={(e) => e.stopPropagation()}
              initial={reduceMotion ? false : { opacity: 0, y: 12, scale: 0.98 }}
              animate={{ opacity: 1, y: 0, scale: 1 }}
              exit={reduceMotion ? { opacity: 0 } : { opacity: 0, y: 10, scale: 0.98 }}
              transition={reduceMotion ? { duration: 0.05 } : { duration: 0.24, ease: revealEase }}
            >
              <button type="button" onClick={() => setLightboxItem(null)}>X</button>
              <img src={lightboxItem.image} alt={lightboxItem.title} />
              <h4>{lightboxItem.title}</h4>
              <p>{lightboxItem.text}</p>
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
}

export default App;
