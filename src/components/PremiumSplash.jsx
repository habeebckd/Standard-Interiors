import React, { useEffect } from 'react';

const logoSrc = encodeURI('/standard interiors Logo.png');

export default function PremiumSplash({ onFinish = null, reduceMotion = false }) {
  const handleDismiss = () => {
    if (onFinish) onFinish();
  };

  return (
    <div className="premium-splash" aria-hidden="true">
      <div className="splash-scene">
        <span className="splash-orb splash-orb--left" />
        <span className="splash-orb splash-orb--right" />
        <span className="splash-backlight" />

        <div className="splash-logo-wrap">
          <span className="splash-logo-pulse" />
          <img src={logoSrc} alt="Standard Interiors" className="splash-logo" />
          <span className="splash-logo-reflection" />
        </div>

        <span
          className={`splash-dismiss-trigger${reduceMotion ? ' is-reduced' : ''}`}
          onAnimationEnd={handleDismiss}
          aria-hidden="true"
        />
      </div>
    </div>
  );
}