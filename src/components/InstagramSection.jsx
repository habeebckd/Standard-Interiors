import React, { memo } from 'react';

const InstagramSection = memo(({
  instagramSectionRef,
  instagramPreviewRowRef,
  phoneScreenSrc,
  setPhoneScreenSrc,
  phoneReel,
  trailReels,
  visibleReels
}) => {
  return (
    <section id="instagram" className="section instagram-section" ref={instagramSectionRef}>
      <div className="instagram-head">
        <h2>Our Works &amp; Reels</h2>
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

      <div className="instagram-tablet-image" aria-hidden="true">
        <img
          src="/instagramPag.png"
          alt="Standard Interiors Instagram tablet preview"
          loading="lazy"
        />
      </div>

      <div className="instagram-mobile-image" aria-hidden="true">
        <span className="instagram-mobile-sensor" />
        <img
          src="/instagramPage(mobile).png"
          alt="Standard Interiors Instagram mobile preview"
          loading="lazy"
        />
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
              <img src={item.image} alt={`${item.title} - Standard Interiors Kerala`} loading="lazy" />
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
