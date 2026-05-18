import React, { memo, useMemo } from 'react';
import { motion } from 'framer-motion';

const REVEAL_EASE = [0.22, 1, 0.36, 1];
const REVEAL_DURATION = 0.58;

const Portfolio = memo(({ portfolioItems, onSelectItem, reduceMotion }) => {
  const items = useMemo(() => portfolioItems, [portfolioItems]);

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
    <motion.section id="portfolio" className="section" {...revealInViewProps()}>
      <h3>Portfolio</h3>
      <p className="section-sub">Recent interior highlights built around warmth, lighting, and clean ceiling details.</p>
      <div className="portfolio-grid">
        {items.map((item, idx) => (
          <motion.button
            key={item.title}
            className="portfolio-card"
            onClick={() => onSelectItem(item)}
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
  );
});

Portfolio.displayName = 'Portfolio';

export default Portfolio;
