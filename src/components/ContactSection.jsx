import React, { memo, useMemo } from 'react';
import { motion, AnimatePresence } from 'framer-motion';

const FADE_DURATION = 0.4;
const REVEAL_EASE = [0.22, 1, 0.36, 1];
const REVEAL_DURATION = 0.58;

const fadeVariants = {
  hidden: { opacity: 0 },
  visible: {
    opacity: 1,
    transition: { duration: FADE_DURATION, ease: REVEAL_EASE }
  }
};

const ContactSection = memo(({ onSubmitForm, formSubmitting, formMessage, services, reduceMotion }) => {
  const servicesList = useMemo(() => services || [], [services]);

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
    <motion.section id="contact" className="section contact-section" {...revealInViewProps()}>
      <motion.div className="contact-left" {...revealInViewProps(0.02)}>
        <h3>Get in Touch</h3>
        <p>Phone: +91 9539120466</p>
        <p>WhatsApp: wa.me/919947015742</p>
        <p>Instagram: @standard__interiors</p>
        <p>Areas:</p>
        <p>EDAVANNAPPARA | AREEKODE | KONDOTTY | VAZHAKKAD | MUKKAM | KIZHISSERI</p>
        <a href="https://wa.me/919947015742" target="_blank" rel="noreferrer" className="btn-green">WhatsApp Us</a>
      </motion.div>
      <motion.form className="contact-form" onSubmit={onSubmitForm} {...revealInViewProps(0.08)}>
        <input name="name" placeholder="Name" required disabled={formSubmitting} />
        <input name="phone" placeholder="Phone / WhatsApp" required disabled={formSubmitting} />
        <select name="service" required defaultValue="" disabled={formSubmitting}>
          <option value="" disabled>Select service</option>
          {servicesList.map((service) => (
            <option key={service.title} value={service.title}>{service.title}</option>
          ))}
        </select>
        <textarea name="message" placeholder="Tell us your requirements" rows="5" required disabled={formSubmitting} />
        <button type="submit" className="btn-dark" disabled={formSubmitting}>
          {formSubmitting ? 'Sending...' : 'Send Message'}
        </button>
        {formMessage && (
          <motion.div
            className={`form-message form-message-${formMessage.type}`}
            initial={reduceMotion ? false : { opacity: 0, y: -10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
            transition={{ duration: 0.3 }}
          >
            {formMessage.text}
          </motion.div>
        )}
      </motion.form>
    </motion.section>
  );
});

ContactSection.displayName = 'ContactSection';

export default ContactSection;