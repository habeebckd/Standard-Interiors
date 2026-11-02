import React, { memo } from 'react';
import { motion } from 'framer-motion';
import { AnimatePresence } from 'framer-motion';

const FADE_DURATION = 0.4;
const REVEAL_EASE = [0.22, 1, 0.36, 1];

const fadeVariants = {
  hidden: { opacity: 0 },
  visible: {
    opacity: 1,
    transition: { duration: FADE_DURATION, ease: REVEAL_EASE }
  }
};

const Lightbox = memo(({ item, onClose, reduceMotion }) => {
  if (!item) return null;

  return (
    <AnimatePresence>
      {item && (
        <motion.div
          className="lightbox"
          onClick={onClose}
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
            <button type="button" onClick={onClose}>X</button>
            <img src={item.image} alt={item.title} />
            <h4>{item.title}</h4>
            <p>{item.text}</p>
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>
  );
});

Lightbox.displayName = 'Lightbox';

export default Lightbox;
