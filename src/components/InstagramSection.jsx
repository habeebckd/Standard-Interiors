import React, { memo, useRef, useEffect, useCallback, useMemo } from 'react';
import { gsap } from 'gsap';
import { ScrollTrigger } from 'gsap/ScrollTrigger';

gsap.registerPlugin(ScrollTrigger);

const InstagramSection = memo(({ instagramItems, reduceMotion }) => {
  const instagramSectionRef = useRef(null);
  const instagramPreviewRowRef = useRef(null);

  const phoneReel = useMemo(() => ({ image: instagramItems[5], title: 'Bedroom Transformation' }), [instagramItems]);
  const trailReels = useMemo(() => [instagramItems[1], instagramItems[2], instagramItems[3]], [instagramItems]);
  const visibleReels = useMemo(() => [instagramItems[1], instagramItems[2], instagramItems[3], instagramItems[4], instagramItems[5]], [instagramItems]);

  const [phoneScreenSrc, setPhoneScreenSrc] = React.useState('/WhatsApp Image 2026-04-21 at 3.33.11 PM.jpeg');

  const updateCarouselPosition = useCallback(() => {
    const row = instagramPreviewRowRef.current;
    if (!row) return;

    const cards = gsap.utils.toArray('.instagram-preview-card', row);
    const rowBounds = row.getBoundingClientRect();
    const rowCenter = rowBounds.left + rowBounds.width * 0.5;
    
    let closestDistance = Infinity;
    const cardPositions = [];

    cards.forEach((card, idx) => {
      const bounds = card.getBoundingClientRect();
      const cardCenter = bounds.left + bounds.width * 0.5;
      const distance = Math.abs(cardCenter - rowCenter);
      
      cardPositions.push({ card, index: idx, distance, center: cardCenter });
      closestDistance = Math.min(closestDistance, distance);
    });

    cardPositions.sort((a, b) => a.distance - b.distance);

    cards.forEach(card => {
      card.classList.remove('is-center', 'is-left-1', 'is-left-2', 'is-right-1', 'is-right-2');
    });

    cardPositions.forEach((pos, sortedIdx) => {
      if (sortedIdx === 0) {
        pos.card.classList.add('is-center');
      } else if (pos.center < rowCenter) {
        if (sortedIdx === 1) pos.card.classList.add('is-left-1');
        else if (sortedIdx === 2) pos.card.classList.add('is-left-2');
      } else {
        if (sortedIdx === 1) pos.card.classList.add('is-right-1');
        else if (sortedIdx === 2) pos.card.classList.add('is-right-2');
      }
    });
  }, []);

  const updateRowMotionBlur = useCallback((velocity) => {
    const row = instagramPreviewRowRef.current;
    if (!row) return;
    const magnitude = Math.abs(velocity);
    const blur = magnitude < 28 ? 0 : Math.min(magnitude / 3200, 0.8);
    row.style.setProperty('--row-motion-blur', `${blur.toFixed(3)}px`);
  }, []);

  useEffect(() => {
    if (reduceMotion) return;

    const mm = gsap.matchMedia();

    mm.add('(min-width: 769px)', () => {
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

    return () => mm.revert();
  }, [reduceMotion, updateCarouselPosition, updateRowMotionBlur]);

  return (
    <section id="instagram" className="section instagram-section" ref={instagramSectionRef}>
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

      <div className="instagram-immersive-stage" aria-live="polite">
        <div className="instagram-phone-zone">
          <div className="phone-stand" aria-hidden="true">
            <span className="stand-head" />
            <span className="stand-arm" />
            <span className="stand-joint" />
            <span className="stand-base" />
          </div>

          <a
            href="https://www.instagram.com/standard__interiors?igsh=Z2xuaHEyNDhldmQx"
            target="_blank"
            rel="noreferrer"
            className="instagram-phone-mockup"
          >
            <img
              src={phoneScreenSrc}
              alt="Standard Interiors Instagram profile"
              loading="lazy"
              className="instagram-phone-screen-image"
              onError={() => setPhoneScreenSrc(phoneReel.image)}
            />
          </a>

          <span className="flow-arrow flow-arrow-a" aria-hidden="true" />
          <span className="flow-arrow flow-arrow-b" aria-hidden="true" />

          <div className="floating-reel-trail" aria-hidden="true">
            {trailReels.map((item, idx) => (
              <span
                key={`${item.title}-trail`}
                className={`floating-trail-card trail-${idx + 1}`}
              >
                <img src={item.image} alt="" loading="lazy" />
              </span>
            ))}
          </div>
        </div>

        <div className="instagram-preview-row" ref={instagramPreviewRowRef}>
          {visibleReels.map((item) => (
            <a
              key={`${item.title}-preview`}
              href="https://www.instagram.com/standard__interiors?igsh=Z2xuaHEyNDhldmQx"
              target="_blank"
              rel="noreferrer"
              className="instagram-preview-card"
            >
              <img src={item.image} alt={item.title} loading="lazy" />
              <span className="instagram-preview-overlay">
                <strong>{item.title}</strong>
              </span>
            </a>
          ))}
        </div>
      </div>

      <a
        href="https://www.instagram.com/standard__interiors?igsh=Z2xuaHEyNDhldmQx"
        target="_blank"
        rel="noreferrer"
        className="instagram-more-btn"
      >
        View More on Instagram
      </a>
    </section>
  );
});

InstagramSection.displayName = 'InstagramSection';

export default InstagramSection;
