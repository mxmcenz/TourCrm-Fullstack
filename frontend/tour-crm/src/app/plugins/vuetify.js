import '@/styles/tokens.css'
import 'vuetify/styles'
import {createVuetify} from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import { aliases, mdi } from 'vuetify/iconsets/mdi'

import '@mdi/font/css/materialdesignicons.css'

const getVar = (name, fallback) => {
    if (typeof window === 'undefined') return fallback
    const v = getComputedStyle(document.documentElement).getPropertyValue(name)
    return (v && v.trim()) || fallback
}
const tourTheme = {
    dark: false,
    colors: {
        background: getVar('--color-baby-powder', '#F2F3ED'),
        surface:    getVar('--brand-paper',      '#FFFFFF'),
        primary:    getVar('--brand-primary',    '#8B926D'),
        secondary:  getVar('--color-pear',       '#CEDB95'),

        info:       getVar('--color-gray',       '#808080'),
        success:    '#22c55e',
        warning:    '#f59e0b',
        error:      '#ef4444',

        'on-background': getVar('--brand-ink',   '#000000'),
        'on-surface':    getVar('--brand-ink',   '#000000'),
    },

    variables: {
        'border-color': getVar('--color-silver', '#CCCCCC'),
    },
}

export default createVuetify({
    components,
    directives,
    icons: {
        defaultSet: 'mdi',
        aliases,
        sets: { mdi },
    },
    theme: {
        defaultTheme: 'tourTheme',
        themes: {tourTheme},
    },

    defaults: {
        VAppBar: {
            color: 'surface',
            flat: true,
            elevation: 0,
        },

        VNavigationDrawer: {
            color: 'background',
            elevation: 0,
            border: 0,
            width: 240,
        },

        VTable: { density: 'comfortable' },
        VTextField: { color: 'primary', variant: 'outlined', density: 'comfortable' },
        VSelect:    { color: 'primary', variant: 'outlined', density: 'comfortable' },
        VCard:      { color: 'surface' },
        VBtn: {rounded: 'xl', height: 36, class: 'text-none'},
        VMain: {class: 'bg-background'},
    },
})