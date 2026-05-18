import React, { memo, useMemo } from 'react';

const ServicesSection = memo(({ services, servicesRef, servicesBgRef, servicesGridRef, servicesRealRef, servicesWireRef }) => {
  const servicesList = useMemo(() => services, [services]);

  return (
    <section id="services" className="section services-section" ref={servicesRef}>
      <div
        className="blueprint-bg"
        aria-hidden="true"
        ref={servicesBgRef}
      >
        <img src="/modern-interior-services.jpg" alt="Modern minimalist interior background" className="services-bg-image" />
        <div className="services-overlay-dark" />
      </div>

      <div className="services-content">
        <h3>Services Section</h3>
        <p className="section-sub">Clean, simple cards with included works for each service type.</p>
        <div className="services-grid" ref={servicesGridRef}>
          {servicesList.map((service) => (
            <article className="service-card" key={service.title}>
              <h4>{service.title}</h4>
              <p>Includes:</p>
              <ul>
                {service.includes.map((item) => (
                  <li key={item}>{item}</li>
                ))}
              </ul>
              <a 
                href="https://wa.me/919947015742?text=Hi%20Standard%20Interiors%2C%20I%20am%20interested%20in%20your%20services."
                target="_blank"
                rel="noreferrer"
                className="service-card-cta"
              >
                Book Now
              </a>
            </article>
          ))}
        </div>
      </div>
    </section>
  );
});

ServicesSection.displayName = 'ServicesSection';

export default ServicesSection;
