import React, { memo, useMemo, useState } from 'react';
import { motion } from 'framer-motion';
import { SERVICES_ENHANCED } from '../data/servicesData';
import ServiceModal from './ServiceModal';

const ICON_MAP = {
  1: (
    <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="service-icon-svg">
      <path d="M12 10a4 4 0 1 0 0-8 4 4 0 0 0 0 8z" />
      <path d="M12 10c-3.3 0-6 2.7-6 6v1c0 2.2 3.8 4 6 4s6-1.8 6-4v-1c0-3.3-2.7-6-6-6z" />
      <path d="M7 16v-1c0-2-1-3-3-4" />
      <path d="M17 16v-1c0-2 1-3 3-4" />
    </svg>
  ),
  2: (
    <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="service-icon-svg">
      <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z" />
      <polyline points="9 22 9 12 15 12 15 22" />
    </svg>
  ),
  3: (
    <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="service-icon-svg">
      <path d="M3 9h18v10a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V9Z" />
      <path d="M3 9l2.45-4.9A2 2 0 0 1 7.24 3h9.52a2 2 0 0 1 1.8 1.1L21 9" />
      <path d="M12 3v6" />
    </svg>
  ),
  4: (
    <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="service-icon-svg">
      <circle cx="12" cy="12" r="3" />
      <path d="M12 9V2" />
      <path d="M12 15v7" />
      <path d="M9 12H2" />
      <path d="M15 12h7" />
      <path d="M14 9.5l4-4" />
      <path d="M10 14.5l-4 4" />
      <path d="M10 9.5l-4-4" />
      <path d="M14 14.5l4 4" />
    </svg>
  ),
  5: (
    <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="service-icon-svg">
      <path d="M16 4h2a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h2" />
      <rect x="8" y="2" width="8" height="4" rx="1" ry="1" />
      <path d="M12 11h4" />
      <path d="M12 16h4" />
      <path d="M8 11h.01" />
      <path d="M8 16h.01" />
    </svg>
  ),
  6: (
    <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="service-icon-svg">
      <path d="M21 11.5a8.38 8.38 0 0 1-.9 3.8 8.5 8.5 0 0 1-7.6 4.7 8.38 8.38 0 0 1-3.8-.9L3 21l1.9-5.7a8.38 8.38 0 0 1-.9-3.8 8.5 8.5 0 0 1 4.7-7.6 8.38 8.38 0 0 1 3.8-.9h.5a8.48 8.48 0 0 1 8 8v.5z" />
    </svg>
  )
};

const ServiceCard = memo(({ service, index, onClick }) => {
  // Use a semantic button for accessibility and keyboard support
  return (
    <motion.button
      type="button"
      aria-haspopup="dialog"
      aria-label={`Open details for ${service.title}`}
      custom={index}
      initial={{ opacity: 0, y: 25 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true, amount: 0.15 }}
      transition={{
        duration: 0.4,
        ease: [0.22, 1, 0.36, 1],
        delay: index * 0.08
      }}
      className="service-card-design service-card"
      onClick={() => onClick(service)}
    >
      {/* Icon Circle */}
      <motion.div
        className="service-icon-glow"
        whileHover={{ scale: 1.06 }}
        transition={{ duration: 0.22 }}
      >
        <span className="service-icon">{ICON_MAP[service.id]}</span>
      </motion.div>

      {/* Card Content */}
      <div className="service-card-content">
        <h3 className="service-card-title">{service.title}</h3>
        <p className="service-card-desc">{service.description}</p>
      </div>
    </motion.button>
  );
});

ServiceCard.displayName = 'ServiceCard';

const ServicesSection = memo(({ services = SERVICES_ENHANCED, servicesRef, servicesBgRef, servicesGridRef, servicesRealRef, servicesWireRef }) => {
  // Accept an optional `services` prop for easier reuse and testing
  const servicesData = useMemo(() => services, [services]);
  
  const [selectedService, setSelectedService] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);

  const handleCardClick = (service) => {
    setSelectedService(service);
    setIsModalOpen(true);
  };

  const handleModalBookNow = () => {
    if (!selectedService) return;
    const text = `Hi Standard Interiors, I'm interested in the ${selectedService.title} service and would like to book it.`;
    window.open(`https://wa.me/919947015742?text=${encodeURIComponent(text)}`, '_blank', 'noopener');
  };

  return (
    <motion.section
      id="services"
      ref={servicesRef}
      className="services-dark-section"
      initial={{ opacity: 0 }}
      whileInView={{ opacity: 1 }}
      viewport={{ once: true }}
      transition={{ duration: 0.5 }}
    >
      {/* Section Header */}
      <div className="services-header-dark">
        <motion.p
          className="services-label"
          initial={{ opacity: 0 }}
          whileInView={{ opacity: 1 }}
          viewport={{ once: true }}
          transition={{ duration: 0.4 }}
        >
          OUR SERVICES
        </motion.p>

        <motion.h2
          className="services-title-main"
          initial={{ opacity: 0, y: 20 }}
          whileInView={{ opacity: 1, y: 0 }}
          viewport={{ once: true }}
          transition={{ duration: 0.4, ease: [0.22, 1, 0.36, 1] }}
        >
          We Design Spaces That Inspire
        </motion.h2>

        {/* Decorative divider */}
        <div className="services-divider-line">
          <span className="divider-dot"></span>
        </div>
      </div>

      {/* Background placeholder for compatibility with App GSAP refs */}
      <div className="blueprint-bg" aria-hidden="true" ref={servicesBgRef} />

      {/* Decorative wire + real layers (placeholders for GSAP animations) */}
      <div className="services-wire" aria-hidden="true" ref={servicesWireRef} />
      <div className="services-real" aria-hidden="true" ref={servicesRealRef} />

      {/* Services Grid */}
      <div className="services-grid-dark" ref={servicesGridRef}>
        {servicesData.map((service, index) => (
          <ServiceCard
            key={service.id}
            service={service}
            index={index}
            onClick={handleCardClick}
          />
        ))}
      </div>

      <ServiceModal
        service={selectedService}
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onBookNow={handleModalBookNow}
      />

      {/* Action bar removed per request */}
    </motion.section>
  );
});

ServicesSection.displayName = 'ServicesSection';

export default ServicesSection;

