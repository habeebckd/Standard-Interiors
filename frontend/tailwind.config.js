/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ['./src/**/*.{html,ts}'],
  theme: {
    extend: {
      colors: {
        brand: {
          gold: '#b58a35',
          charcoal: '#22252d',
          cream: '#f6f0e3',
          paper: '#fcfbf7',
          ink: '#171717'
        }
      },
      boxShadow: {
        premium: '0 12px 30px rgba(17, 24, 39, 0.12)'
      }
    }
  },
  plugins: []
};
