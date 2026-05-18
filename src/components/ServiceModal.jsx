import React, { memo } from 'react';
import { motion, AnimatePresence } from 'framer-motion';

const REVEAL_EASE = [0.22, 1, 0.36, 1];

const ServiceModal = memo(({ service, isOpen, onClose, onBookNow, reduceMotion }) => {
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
        duration: 0.35, 
        ease: REVEAL_EASE 
      }
    },
    exit: { 
      opacity: 0, 
      scale: 0.95,
      y: 20,
      transition: { 
        duration: 0.2 
      }
    }
  };

  return (
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
            <div className="modal-crystal">
              {/* Close Button */}
              <motion.button
                className="modal-close-crystal"
                onClick={onClose}
                whileHover={{ scale: 1.1 }}
                whileTap={{ scale: 0.95 }}
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
                  transition={{ delay: 0.1, duration: 0.3 }}
                >
                  <div
                    className="modal-image-wrapper-crystal"
                  >
                    <img
                      src={service.image}
                      alt={service.title}
                      className="modal-image-crystal"
                    />
                  </div>
                  
                  {/* Book Now Button under Image */}
                  <motion.button
                    className="modal-book-btn-crystal"
                    onClick={() => {
                      onBookNow();
                      onClose();
                    }}
                    whileHover={{ y: -4 }}
                    whileTap={{ scale: 0.95 }}
                    initial={{ opacity: 0, y: 20 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ delay: 0.2, duration: 0.3 }}
                  >
                    Book Now
                  </motion.button>
                </motion.div>

                {/* Text Content */}
                <motion.div
                  className="modal-text-crystal"
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: 0.15, duration: 0.3 }}
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
                      {service.includes.map((item, idx) => (
                        <motion.li
                          key={item}
                          initial={{ opacity: 0, x: -10 }}
                          animate={{ opacity: 1, x: 0 }}
                          transition={{ delay: 0.2 + idx * 0.05, duration: 0.2 }}
                          className="modal-list-item-crystal"
                        >
                          <span className="modal-check-crystal">✓</span>
                          {item}
                        </motion.li>
                      ))}
                    </ul>
                  </div>

                  {/* Features Section */}
                  <div className="modal-features-crystal">
                    <h3 className="modal-section-title-crystal">Key Features</h3>
                    <div className="modal-features-grid-crystal">
                      {service.features.map((feature, idx) => (
                        <motion.div
                          key={feature}
                          className="modal-feature-badge-crystal"
                          initial={{ opacity: 0, scale: 0.9 }}
                          animate={{ opacity: 1, scale: 1 }}
                          transition={{ delay: 0.25 + idx * 0.05, duration: 0.2 }}
                        >
                          {feature}
                        </motion.div>
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
});

ServiceModal.displayName = 'ServiceModal';

export default ServiceModal;
