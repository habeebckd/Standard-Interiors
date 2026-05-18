import { useMemo, useCallback } from 'react';

export const useRevealVariants = () => {
  const REVEAL_EASE = [0.22, 1, 0.36, 1];
  const REVEAL_DURATION = 0.58;

  return useMemo(
    () => ({
      hidden: { opacity: 0, y: 18 },
      visible: {
        opacity: 1,
        y: 0,
        transition: { duration: REVEAL_DURATION, ease: REVEAL_EASE }
      }
    }),
    []
  );
};

export const useRevealInViewProps = (reduceMotion) => {
  const revealVariants = useRevealVariants();

  return useCallback(
    (delay = 0) => {
      if (reduceMotion) return {};
      return {
        variants: revealVariants,
        initial: 'hidden',
        whileInView: 'visible',
        viewport: { once: true, amount: 0.18 },
        transition: { delay }
      };
    },
    [reduceMotion, revealVariants]
  );
};

export const useFadeVariants = () => {
  const FADE_DURATION = 0.4;
  const REVEAL_EASE = [0.22, 1, 0.36, 1];

  return useMemo(
    () => ({
      hidden: { opacity: 0 },
      visible: {
        opacity: 1,
        transition: { duration: FADE_DURATION, ease: REVEAL_EASE }
      }
    }),
    []
  );
};
