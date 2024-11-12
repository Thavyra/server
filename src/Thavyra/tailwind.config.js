/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["Areas/*/Views/**/*.cshtml",
  ],
  theme: {
    colors: {
      transparent: 'transparent',
      current: 'currentColor',

      'dark-950': '#030712',
      'dark-900': '#111827',
      'dark-800': '#1f2937',
      'dark-750': '#2b3440',
      'dark-700': '#374151',
      
      'bright': {
        DEFAULT: '#d1d5db',
      },
      
      'light': {
        DEFAULT: '#9ca3af',
      },

      'primary': {
        DEFAULT: '#374151',
        hover: '#475569',
        focus: '#475569'
      },

      'link': {
        DEFAULT: '#0284c7',
        hover: '#0369a1'
      },
      
      'discord': {
        DEFAULT: '#5865F2',
        hover: '#3C45A5'
      },
      
      'github': {
        DEFAULT: '#fafafa',
        hover: '#9ca3af',
      },

      'positive': '#16a34a',
      'negative': '#dc2626',
      'neutral': '#ca8a04'
    },
    extend: {},
  },
  plugins: [],
}

