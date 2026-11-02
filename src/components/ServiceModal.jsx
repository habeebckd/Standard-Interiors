import React, { memo, useEffect } from 'react';
import { createPortal } from 'react-dom';
import { motion, AnimatePresence } from 'framer-motion';

const REVEAL_EASE = [0.22, 1, 0.36, 1];

const ServiceModal = memo(({ service, isOpen, onClose, onBookNow, reduceMotion }) => {
  useEffect(() => {
    if (!isOpen) return undefined;

    const previousOverflow = document.body.style.overflow;
    document.body.style.overflow = 'hidden';

    return () => {
      document.body.style.overflow = previousOverflow;
    };
  }, [isOpen]);

  const backdropVariants = {
    hidden: { opacity: 0 },
    visible: { opacity: 1 },
    exit: { opacity: 0 }
  };

  const modalVariants = {
    hidden: { 
      opacity: 0, 
      scale: 0.95,
      y: 20
    },
    visible: { 
      opacity: 1, 
      scale: 1,
      y: 0,
      transition: { 
        duration: 0.22, 
        ease: REVEAL_EASE 
      }
    },
    exit: { 
      opacity: 0, 
      scale: 0.95,
      y: 20,
      transition: { 
        duration: 0.16 
      }
    }
  };

  const modalNode = (
    <AnimatePresence>
      {isOpen && service && (
        <>
          {/* Backdrop */}
          <motion.div
            className="modal-backdrop-crystal"
            variants={backdropVariants}
            initial="hidden"
            animate="visible"
            exit="exit"
            onClick={onClose}
            transition={{ duration: 0.3 }}
          />

          {/* Modal */}
            <motion.div
              className="modal-overlay-crystal"
              initial="hidden"
              animate="visible"
              exit="exit"
              variants={modalVariants}
            >
              <div className="modal-crystal" style={{ transform: 'translateZ(0)' }}>
              {/* Close Button */}
              <motion.button
                className="modal-close-crystal"
                onClick={onClose}
                whileHover={reduceMotion ? undefined : { scale: 1.08 }}
                whileTap={reduceMotion ? undefined : { scale: 0.96 }}
              >
                ✕
              </motion.button>

              {/* Modal Content */}
              <div className="modal-content-crystal">
                {/* Image Section with Button */}
                  <motion.div
                    className="modal-image-section-crystal"
                    initial={{ opacity: 0, y: 20 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ delay: 0.08, duration: 0.22 }}
                  >
                  <div
                    className="modal-image-wrapper-crystal"
                  >
                    <img
                      src={service.image}
                      alt={service.title}
                      className="modal-image-crystal"
                      loading="eager"
                      decoding="async"
                    />
                  </div>
                  
                  {/* Book Now Button under Image */}
                  <motion.button
                    className="modal-book-btn-crystal"
                    onClick={() => {
                      onBookNow();
                      onClose();
                    }}
                    whileHover={reduceMotion ? undefined : { y: -2 }}
                    whileTap={reduceMotion ? undefined : { scale: 0.98 }}
                    initial={{ opacity: 0, y: 20 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ delay: 0.12, duration: 0.22 }}
                  >
                    Book Now
                  </motion.button>
                </motion.div>

                {/* Text Content */}
                <motion.div
                  className="modal-text-crystal"
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.1, duration: 0.22 }}
                >
                  {/* Header */}
                  <div className="modal-header-crystal">
                    <h2 className="modal-title-crystal">{service.title}</h2>
                    <p className="modal-subtitle-crystal">{service.subtitle}</p>
                  </div>

                  {/* Description */}
                  <p className="modal-description-crystal">
                    {service.description}
                  </p>

                  {/* Includes Section */}
                  <div className="modal-includes-crystal">
                    <h3 className="modal-section-title-crystal">What's Included</h3>
                    <ul className="modal-list-crystal">
                      {service.includes.map((item) => (
                        <li key={item} className="modal-list-item-crystal">
                          <span className="modal-check-crystal">✓</span>
                          {item}
                        </li>
                      ))}
                    </ul>
                  </div>

                  {/* Features Section */}
                  <div className="modal-features-crystal">
                    <h3 className="modal-section-title-crystal">Key Features</h3>
                    <div className="modal-features-grid-crystal">
                      {service.features.map((feature) => (
                        <div key={feature} className="modal-feature-badge-crystal">
                          {feature}
                        </div>
                      ))}
                    </div>
                  </div>
                </motion.div>
              </div>
            </div>
          </motion.div>
        </>
      )}
    </AnimatePresence>
  );

  if (typeof document === 'undefined') {
    return modalNode;
  }

  return createPortal(modalNode, document.body);
});

ServiceModal.displayName = 'ServiceModal';

export default ServiceModal;
