import React, { useEffect, useRef, useState, lazy, Suspense } from 'react';
import { AnimatePresence, motion, useReducedMotion } from 'framer-motion';
import { gsap } from 'gsap';
import { ScrollTrigger } from 'gsap/ScrollTrigger';

const logo = '/standard interiors Logo.png';
const PremiumSplash = lazy(() => import('./components/PremiumSplash'));
import p1 from '../assets/images/project-1.jpeg';
import p2 from '../assets/images/project-2.jpeg';
import p3 from '../assets/images/project-3.jpeg';
import p4 from '../assets/images/project-4.jpeg';
import p5 from '../assets/images/project-5.jpeg';
import p6 from '../assets/images/project-6.jpeg';
import p7 from '../assets/images/project-7.jpeg';
import p8 from '../assets/images/project-8.jpeg';
import w1 from '../workPhotos/WhatsApp Image 2026-03-25 at 3.02.18 PM.jpeg';
import w2 from '../workPhotos/WhatsApp Image 2026-03-25 at 3.02.19 PM.jpeg';
import w3 from '../workPhotos/WhatsApp Image 2026-03-25 at 3.02.21 PM.jpeg';
import w4 from '../workPhotos/WhatsApp Image 2026-03-25 at 3.02.22 PM.jpeg';
import w5 from '../workPhotos/WhatsApp Image 2026-03-25 at 3.02.25 PM.jpeg';
import w6 from '../workPhotos/WhatsApp Image 2026-03-25 at 3.02.26 PM.jpeg';
import w7 from '../workPhotos/WhatsApp Image 2026-03-25 at 3.02.27 PM.jpeg';
import w8 from '../workPhotos/WhatsApp Image 2026-03-25 at 3.02.28 PM.jpeg';


// Animation constants
const REVEAL_EASE = [0.22, 1, 0.36, 1];
const REVEAL_DURATION = 0.58;
const FADE_DURATION = 0.4;
const SPLASH_DURATION = 2200;
const splashLogoSrc = logo;

const revealVariants = {
  hidden: { opacity: 0, y: 18 },
  visible: {
    opacity: 1,
    y: 0,
    transition: { duration: REVEAL_DURATION, ease: REVEAL_EASE }
  }
};

const fadeVariants = {
  hidden: { opacity: 0 },
  visible: {
    opacity: 1,
    transition: { duration: FADE_DURATION, ease: REVEAL_EASE }
  }
};

function SplashLoadFallback({ onDone = null, reduceMotion = false }) {
  const handleDismiss = () => {
    if (onDone) onDone();
  };

  return (
    <div className="premium-splash" aria-hidden="true">
      <div className="splash-scene">
        <span className="splash-orb splash-orb--left" />
        <span className="splash-orb splash-orb--right" />
        <span className="splash-backlight" />

        <div className="splash-logo-wrap">
          <span className="splash-logo-pulse" />
          <img src={splashLogoSrc} alt="" className="splash-logo" />
          <span className="splash-logo-reflection" />
        </div>

        <span
          className={`splash-dismiss-trigger splash-dismiss-trigger--fallback${reduceMotion ? ' is-reduced' : ''}`}
          onAnimationEnd={handleDismiss}
          aria-hidden="true"
        />
      </div>
    </div>
  );
}

class SplashBoundary extends React.Component {
  constructor(props) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError() {
    return { hasError: true };
  }

  componentDidCatch(error) {
    if (this.props.onError) this.props.onError(error);
  }

  render() {
    if (this.state.hasError) {
      return this.props.fallback || null;
    }
    return this.props.children;
  }
}

// Data constants
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

import Hero from './components/Hero';
import ServicesSection from './components/ServicesSectionRedesigned';
import Portfolio from './components/Portfolio';
import InstagramSection from './components/InstagramSection';
import ContactSection from './components/ContactSection';

const instagramItems = [
  { image: w1, title: 'Premium Lounge Setup' },
  { image: w2, title: 'Statement Ceiling Concept' },
  { image: w3, title: 'Warm Ceiling Finish' },
  { image: w4, title: 'Living Room Ambience' },
  { image: w5, title: 'Curtain & Lighting Pairing' },
  { image: w6, title: 'Bedroom Transformation' },
  { image: w7, title: 'Wood Panel Detailing' },
  { image: w8, title: 'Minimal Geometry Build' }
];

const partnersHighlights = [
  '100+ Happy Clients',
  'Premium Branded Materials',
  'On-Time Project Delivery',
  'Skilled & Dedicated Team'
];

function App() {
  const [showSplash, setShowSplash] = useState(true);
  const [scrolled, setScrolled] = useState(false);
  const [expanded, setExpanded] = useState(false);
  const [menuOpen, setMenuOpen] = useState(false);
  const [lightboxItem, setLightboxItem] = useState(null);
  const [formSubmitting, setFormSubmitting] = useState(false);
  const [formMessage, setFormMessage] = useState(null);
  const [phoneScreenSrc, setPhoneScreenSrc] = useState('/WhatsApp Image 2026-04-21 at 3.33.11 PM.jpeg');
  const reduceMotion = useReducedMotion();
  const [lowPerf, setLowPerf] = useState(false);
  const lastScrollYRef = useRef(0);
  const servicesRef = useRef(null);
  const servicesBgRef = useRef(null);
  const servicesGridRef = useRef(null);
  const servicesRealRef = useRef(null);
  const servicesWireRef = useRef(null);
  const instagramSectionRef = useRef(null);
  const instagramPreviewRowRef = useRef(null);

  useEffect(() => {
    const splashTimer = window.setTimeout(() => {
      setShowSplash(false);
    }, reduceMotion ? 900 : SPLASH_DURATION);

    return () => window.clearTimeout(splashTimer);
  }, [reduceMotion]);

  // Prefetch the PremiumSplash chunk early to avoid loading delay on first render
  useEffect(() => {
    let cancelled = false;
    if (!cancelled) {
      import('./components/PremiumSplash').catch(() => {});
    }
    return () => { cancelled = true; };
  }, []);

  // detect low-performance devices (simple heuristic)
  useEffect(() => {
    try {
      const mem = navigator.deviceMemory || 4;
      const hc = navigator.hardwareConcurrency || 4;
      const isMobile = /Mobi|Android/i.test(navigator.userAgent || '') || window.innerWidth < 900;
      const low = mem <= 2 || hc <= 2 || isMobile;
      setLowPerf(!!low);
    } catch (e) {
      setLowPerf(false);
    }
  }, []);

  useEffect(() => {
    document.body.classList.toggle('splash-active', showSplash);

    return () => {
      document.body.classList.remove('splash-active');
    };
  }, [showSplash]);

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
    const SCROLL_THRESHOLD = 3;
    const NAVBAR_SCROLL_POINT = 90;

    const onScroll = () => {
      const y = window.scrollY;

      if (y < NAVBAR_SCROLL_POINT) {
        setExpanded(false);
        setScrolled(false);
      } else {
        setScrolled(true);

        if (Math.abs(y - lastScrollYRef.current) > SCROLL_THRESHOLD) {
          setExpanded(false);
        }
      }

      lastScrollYRef.current = y;
    };

    lastScrollYRef.current = window.scrollY;
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

    const updateCarouselPosition = () => {
      const row = instagramPreviewRowRef.current;
      if (!row) return;

      const cards = gsap.utils.toArray('.instagram-preview-card', row);
      const rowBounds = row.getBoundingClientRect();
      const rowCenter = rowBounds.left + rowBounds.width * 0.5;
      
      // Calculate closest card to center and assign positions
      let closestCard = null;
      let closestDistance = Infinity;
      const cardPositions = [];

      cards.forEach((card, idx) => {
        const bounds = card.getBoundingClientRect();
        const cardCenter = bounds.left + bounds.width * 0.5;
        const distance = Math.abs(cardCenter - rowCenter);
        
        cardPositions.push({
          card,
          index: idx,
          distance,
          center: cardCenter
        });

        if (distance < closestDistance) {
          closestDistance = distance;
          closestCard = card;
        }
      });

      // Sort by distance from center
      cardPositions.sort((a, b) => a.distance - b.distance);

      // Remove all position classes
      cards.forEach(card => {
        card.classList.remove(
          'is-center', 'is-left-1', 'is-left-2', 'is-right-1', 'is-right-2'
        );
      });

      // Assign new positions based on sorted order
      cardPositions.forEach((pos, sortedIdx) => {
        if (sortedIdx === 0) {
          pos.card.classList.add('is-center');
        } else if (pos.center < rowCenter) {
          // Left side cards
          if (sortedIdx === 1) pos.card.classList.add('is-left-1');
          else if (sortedIdx === 2) pos.card.classList.add('is-left-2');
        } else {
          // Right side cards
          if (sortedIdx === 1) pos.card.classList.add('is-right-1');
          else if (sortedIdx === 2) pos.card.classList.add('is-right-2');
        }
      });
    };

    const updateRowMotionBlur = (velocity) => {
      const row = instagramPreviewRowRef.current;
      if (!row) return;

      const magnitude = Math.abs(velocity);
      const blur = magnitude < 28 ? 0 : Math.min(magnitude / 3200, 0.8);
      row.style.setProperty('--row-motion-blur', `${blur.toFixed(3)}px`);
    };

    mm.add('(min-width: 769px)', () => {
      const cards = gsap.utils.toArray('.service-card', servicesGridRef.current);

      gsap.set(cards, { y: 42, opacity: 0, scale: 0.95 });

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
        instagramPreviewRowRef.current,
        { xPercent: -9 },
        {
          xPercent: 12,
          ease: 'none',
          scrollTrigger: {
            trigger: instagramSectionRef.current,
            start: 'top bottom',
            end: 'bottom top',
            scrub: 1.45,
            onUpdate: (self) => {
              updateCarouselPosition();
              updateRowMotionBlur(self.getVelocity());
            },
            onRefresh: () => updateCarouselPosition()
          }
        }
      );

      gsap.delayedCall(0.08, updateCarouselPosition);
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

    const formData = new FormData(event.currentTarget);
    const name = formData.get('name') || '';
    const phone = formData.get('phone') || '';
    const service = formData.get('service') || 'Not specified';
    const message = formData.get('message') || '';

    const whatsappText = `Hello Standard Interiors,%0A%0AMy name is ${encodeURIComponent(name)}.%0AMy contact number is ${encodeURIComponent(phone)}.%0AService needed: ${encodeURIComponent(service)}.%0A%0AProject details:%0A${encodeURIComponent(message)}`;
    const whatsappUrl = `https://wa.me/919947015742?text=${whatsappText}`;

    setTimeout(() => {
      window.open(whatsappUrl, '_blank', 'noopener');
      setFormSubmitting(false);
      setFormMessage({ type: 'success', text: 'Redirecting to WhatsApp...' });
      event.currentTarget.reset();
      setTimeout(() => setFormMessage(null), 3000);
    }, 300);
  };

  const navClass = `navbar ${scrolled ? 'is-short' : ''} ${expanded ? 'is-open' : ''}`;
  const phoneReel = { image: w6, title: 'Bedroom Transformation' };
  const trailReels = [instagramItems[1], instagramItems[2], instagramItems[3]];
  const visibleReels = [instagramItems[1], instagramItems[2], instagramItems[3], instagramItems[4], instagramItems[5]];

  return (
    <div className="app">
      <AnimatePresence mode="wait">
        {showSplash && (
          <motion.div
            key="splash-shell"
            className="premium-splash-shell"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0, transition: { duration: 0.34, ease: REVEAL_EASE } }}
          >
            <SplashBoundary
              fallback={(
                <SplashLoadFallback
                  reduceMotion={reduceMotion}
                  onDone={() => setShowSplash(false)}
                />
              )}
            >
              <Suspense fallback={(
                <SplashLoadFallback
                  reduceMotion={reduceMotion}
                  onDone={() => setShowSplash(false)}
                />
              )}>
                <PremiumSplash
                  reduceMotion={reduceMotion}
                  lowPerf={lowPerf}
                  onFinish={() => setShowSplash(false)}
                />
              </Suspense>
            </SplashBoundary>
          </motion.div>
        )}
      </AnimatePresence>

      <motion.div
        className="site-content"
        initial={false}
        animate={showSplash ? { opacity: 0, y: 18, scale: 0.988, filter: 'blur(10px)' } : { opacity: 1, y: 0, scale: 1, filter: 'blur(0px)' }}
        transition={{ duration: showSplash ? 0.2 : 0.85, ease: REVEAL_EASE }}
      >
      <header className="header-wrap">
        <div className={navClass}>
          <a href="#home" className="brand">
            <img src={logo} alt="Standard Interiors logo" />
            <div>
              <p>STANDARD</p>
              <span className="brand-title">Interiors</span>
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
              transition={{ duration: 0.2, ease: REVEAL_EASE }}
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
        <Hero reduceMotion={reduceMotion} />

        <ServicesSection
          servicesRef={servicesRef}
          servicesBgRef={servicesBgRef}
          servicesGridRef={servicesGridRef}
          servicesRealRef={servicesRealRef}
          servicesWireRef={servicesWireRef}
        />

        <Portfolio
          portfolioItems={portfolioItems}
          onSelectItem={setLightboxItem}
          reduceMotion={reduceMotion}
        />

        <InstagramSection
          instagramSectionRef={instagramSectionRef}
          instagramPreviewRowRef={instagramPreviewRowRef}
          phoneScreenSrc={phoneScreenSrc}
          setPhoneScreenSrc={setPhoneScreenSrc}
          phoneReel={phoneReel}
          trailReels={trailReels}
          visibleReels={visibleReels}
        />

        <ContactSection
          onSubmitForm={submitForm}
          formSubmitting={formSubmitting}
          formMessage={formMessage}
          services={services}
          reduceMotion={reduceMotion}
        />

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
              <span>Edavannappara, Malappuram, Kerala</span>
              <a href="tel:+919539120466">+91 9539120466</a>
              <a href="https://wa.me/919947015742" target="_blank" rel="noreferrer">wa.me/919947015742</a>
              <a href="https://www.instagram.com/standard__interiors/" target="_blank" rel="noreferrer">@standard__interiors</a>
            </div>
          </div>
        </div>

        <div className="footer-bottom">© 2026 Standard Interiors. All rights reserved.</div>
      </motion.footer>

      <a className="float-whatsapp" href="https://wa.me/919947015742" target="_blank" rel="noreferrer" aria-label="Chat on WhatsApp with Standard Interiors">
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
              transition={reduceMotion ? { duration: 0.05 } : { duration: 0.24, ease: REVEAL_EASE }}
            >
              <button type="button" onClick={() => setLightboxItem(null)}>X</button>
              <img src={lightboxItem.image} alt={lightboxItem.title} />
              <h4>{lightboxItem.title}</h4>
              <p>{lightboxItem.text}</p>
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>
      </motion.div>
    </div>
  );
}

export default App;
