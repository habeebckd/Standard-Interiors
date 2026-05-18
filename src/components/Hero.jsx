import React, { memo } from 'react';
import { motion } from 'framer-motion';

const REVEAL_EASE = [0.22, 1, 0.36, 1];
const REVEAL_DURATION = 0.58;

const Hero = memo(({ reduceMotion }) => {
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
  );
});

Hero.displayName = 'Hero';

export default Hero;
